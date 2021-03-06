﻿using MediaToolkit.Core;
using MediaToolkit.Logging;
using NAudio.CoreAudioApi;
using NAudio.Wave;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediaToolkit
{

    public class AudioSource
    {

        private static TraceSource logger = TraceManager.GetTrace("MediaToolkit");

        private const long ReftimesPerSec = 10000000;
        private const long ReftimesPerMillisec = 10000;

        public AudioSource(){ }

        private MMDevice captureDevice = null;
        private AudioClient audioClient;

        private byte[] recordBuffer;
        private int bytesPerFrame;
        private WaveFormat waveFormat;
        private int audioBufferMillisecondsLength;

        private bool isUsingEventSync;
        private AutoResetEvent frameEventWaitHandle;
        private Task captureTask;

        public event Action<byte[]> DataAvailable;
        //public event Action<byte[], int> DataAvailable;
        public event Action CaptureStarted;
        public event Action<object> CaptureStopped;

        private volatile CaptureState captureState = CaptureState.Closed;
        public CaptureState State => captureState;
        public int ErrorCode { get; private set; } = 0;

        public AudioClientShareMode ShareMode { get; private set; }
        public WaveFormat WaveFormat
        {
            get
            {
                // for convenience, return a WAVEFORMATEX, instead of the real
                // WAVEFORMATEXTENSIBLE being used
                return waveFormat.AsStandardWaveFormat();
            }
            //set { waveFormat = value; }
        }

		//public void Setup(string DeviceId, bool useEventSync = false, int audioBufferMillisecondsLength = 100, bool exclusiveMode = false)
		public void Setup(string deviceId, object captureProperties = null)
        {
            logger.Debug("AudioSourceEx::Setup(...) " + deviceId );

            if (captureState != CaptureState.Closed)
            {
                throw new InvalidOperationException("Invalid audio capture state " + captureState);
            }

			WasapiCaptureProperties wasapiCaptureProperties = captureProperties as WasapiCaptureProperties ?? new WasapiCaptureProperties();

			using (var deviceEnum = new MMDeviceEnumerator())
            {
                var mmDevices = deviceEnum.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);

                for(int i= 0; i< mmDevices.Count; i++)
                {
                    var d = mmDevices[i];
                    if(d.ID == deviceId)
                    {
                        captureDevice = d;
                        continue;
                    }
                    d.Dispose();
                }
            }

            if (captureDevice == null)
            {
                throw new Exception("MMDevice not found...");
            }

            this.isUsingEventSync = wasapiCaptureProperties.EventSyncMode;
            this.audioBufferMillisecondsLength = wasapiCaptureProperties.BufferMilliseconds;

            this.audioClient = captureDevice.AudioClient;
            this.ShareMode = wasapiCaptureProperties.ExclusiveMode? AudioClientShareMode.Exclusive : AudioClientShareMode.Shared;

            this.waveFormat = audioClient.MixFormat;

            long requestedDuration = ReftimesPerMillisec * audioBufferMillisecondsLength;

            if (!audioClient.IsFormatSupported(ShareMode, waveFormat))
            {
                throw new ArgumentException("Unsupported Wave Format");
            }

            try
            {
                var streamFlags = AudioClientStreamFlags.None;
                if (captureDevice.DataFlow != DataFlow.Capture)
                {
                    streamFlags = AudioClientStreamFlags.Loopback;
                }

                // If using EventSync, setup is specific with shareMode
                if (isUsingEventSync)
                {
                    var flags = AudioClientStreamFlags.EventCallback | streamFlags;

                    // Init Shared or Exclusive
                    if (ShareMode == AudioClientShareMode.Shared)
                    {
                        // With EventCallBack and Shared, both latencies must be set to 0
                        audioClient.Initialize(ShareMode, flags, requestedDuration, 0, waveFormat, Guid.Empty);
                    }
                    else
                    {
                        // With EventCallBack and Exclusive, both latencies must equals
                        audioClient.Initialize(ShareMode, flags, requestedDuration, requestedDuration, waveFormat, Guid.Empty);
                    }

                    // Create the Wait Event Handle
                    frameEventWaitHandle = new AutoResetEvent(false);
                    audioClient.SetEventHandle(frameEventWaitHandle.SafeWaitHandle.DangerousGetHandle());
                }
                else
                {
                    // Normal setup for both sharedMode
                    audioClient.Initialize(ShareMode, streamFlags, requestedDuration, 0, waveFormat, Guid.Empty);
                }

                int bufferFrameCount = audioClient.BufferSize;
                bytesPerFrame = waveFormat.Channels * waveFormat.BitsPerSample / 8;
                recordBuffer = new byte[bufferFrameCount * bytesPerFrame];

                captureState = CaptureState.Initialized;

            }
            catch(Exception ex)
            {
                logger.Error(ex);

                CleanUp();

                throw;
            }

        }



        public void Start()
        {
            logger.Debug("AudioSource::Start()");

            if (!(captureState == CaptureState.Stopped || captureState == CaptureState.Initialized))
            {
                throw new InvalidOperationException("Previous recording still in progress");
            }

            captureState = CaptureState.Starting;

            captureTask = Task.Run(() => 
            {
                try
                {
                    logger.Info("Capture thread started...");
                    captureState = CaptureState.Capturing;

                    CaptureStarted?.Invoke();

                    DoCapture();

                }
                catch (Exception ex)
                {
                    logger.Error(ex);

                    this.ErrorCode = 100500;
                }
                finally
                {
                    logger.Info("Capture thread stopped...");

                    captureState = CaptureState.Stopped;
                    CaptureStopped?.Invoke(null);

                }

            });

        }

        private void DoCapture()
        {
            try
            {
                //Debug.WriteLine(String.Format("Client buffer frame count: {0}", client.BufferSize));
                int bufferFrameCount = audioClient.BufferSize;

                // Calculate the actual duration of the allocated buffer.
                long actualDuration = (long)((double)ReftimesPerSec *
                                 bufferFrameCount / waveFormat.SampleRate);

                int sleepMilliseconds = (int)(actualDuration / ReftimesPerMillisec / 2);
                int waitMilliseconds = (int)(3 * actualDuration / ReftimesPerMillisec);

                var capture = audioClient.AudioCaptureClient;
                audioClient.Start();

                // avoid race condition where we stop immediately after starting
                if (captureState == CaptureState.Starting)
                {
                    captureState = CaptureState.Capturing;
                }
                while (captureState == CaptureState.Capturing)
                {
                    bool readBuffer = true;
                    if (isUsingEventSync)
                    {
                        readBuffer = frameEventWaitHandle.WaitOne(waitMilliseconds, false);
                    }
                    else
                    {
                        Thread.Sleep(sleepMilliseconds);
                    }

                    if (captureState != CaptureState.Capturing)
                    {
                        break;
                    }

                    // If still recording and notification is ok
                    if (readBuffer)
                    {
                        ReadNextPacket(capture);
                    }
                }
            }
            finally
            {
                //...
            }
        }


        private void ReadNextPacket(AudioCaptureClient capture)
        {
            int packetSize = capture.GetNextPacketSize();
            int recordBufferOffset = 0;
            //Debug.WriteLine(string.Format("packet size: {0} samples", packetSize / 4));

            while (packetSize != 0)
            {
                IntPtr buffer = capture.GetBuffer(out int framesAvailable, out AudioClientBufferFlags flags);

                int bytesAvailable = framesAvailable * bytesPerFrame;

                // apparently it is sometimes possible to read more frames than we were expecting?
                // fix suggested by Michael Feld:
                int spaceRemaining = Math.Max(0, recordBuffer.Length - recordBufferOffset);
                if (spaceRemaining < bytesAvailable && recordBufferOffset > 0)
                {
                    OnDataAvailable(recordBuffer, recordBufferOffset);
                    //DataAvailable?.Invoke(this, new WaveInEventArgs(recordBuffer, recordBufferOffset));
                    recordBufferOffset = 0;
                }

                // if not silence...
                if ((flags & AudioClientBufferFlags.Silent) != AudioClientBufferFlags.Silent)
                {
                    Marshal.Copy(buffer, recordBuffer, recordBufferOffset, bytesAvailable);
                }
                else
                {
                    Array.Clear(recordBuffer, recordBufferOffset, bytesAvailable);
                }
                recordBufferOffset += bytesAvailable;
                capture.ReleaseBuffer(framesAvailable);
                packetSize = capture.GetNextPacketSize();
            }

            OnDataAvailable(recordBuffer, recordBufferOffset);
           // DataAvailable?.Invoke(this, new WaveInEventArgs(recordBuffer, recordBufferOffset));
        }

        private void OnDataAvailable(byte[]buffer, int bytesRecorded)
        {
            if (bytesRecorded > 0)
            {
                byte[] data = new byte[bytesRecorded];
                Array.Copy(buffer, data, data.Length);

                DataAvailable?.Invoke(data);

                //DataAvailable?.Invoke(recordBuffer, recordBufferOffset);
            }
           
        }

        public void Stop()
        {
            logger.Debug("AudioSource::Stop()");

            if (captureState != CaptureState.Stopped)
            {
                captureState = CaptureState.Stopping;
            }

        }


        public void Close(bool force = false)
        {
            logger.Debug("AudioSource::Close(...) " + force);

            Stop();

            if (!force)
            {
                if (captureTask != null)
                {
                    if (captureTask.Status == TaskStatus.Running)
                    {
                        bool waitResult = false;
                        do
                        {
                            waitResult = captureTask.Wait(1000);
                            if (!waitResult)
                            {
                                logger.Warn("ScreenSource::Close() " + waitResult);
                            }
                        } while (!waitResult);

                    }
                }
            }

            CleanUp();

            captureState = CaptureState.Closed;
        }

        private void CleanUp()
        {
            logger.Debug("AudioSource::CleanUp()");

            if (captureDevice != null)
            {
                captureDevice.Dispose();
                captureDevice = null;
            }

            if (audioClient != null)
            {
                audioClient.Dispose();
                audioClient = null;
            }

            if (frameEventWaitHandle != null)
            {
                frameEventWaitHandle.Dispose();
                frameEventWaitHandle = null;
            }
        }
    }



    public class AudioTool
    {
        private static TraceSource logger = TraceManager.GetTrace("MediaToolkit");

        public static List<AudioCaptureDevice> GetAudioCaptureDevices()
        {
            List<AudioCaptureDevice> captureDevices = new List<AudioCaptureDevice>();

            var mmdevices = GetMMDevices();

            foreach (var d in mmdevices)
            {
                AudioCaptureDevice captureDevice = null;
                var client = d.AudioClient;
                if (client != null)
                {
                    var mixFormat = client.MixFormat;
                    if (mixFormat != null)
                    {

                        captureDevice = new AudioCaptureDevice
                        {
                            DeviceId = d.ID,
                            Name = d.FriendlyName,

                            BitsPerSample = mixFormat.BitsPerSample,
                            SampleRate = mixFormat.SampleRate,
                            Channels = mixFormat.Channels,
                            Description = $"{mixFormat.BitsPerSample} bit PCM: {mixFormat.SampleRate / 1000}kHz {mixFormat.Channels} channels",

                            //Properties = prop,
                        };

                        captureDevices.Add(captureDevice);

                    }
                }

                d?.Dispose();
            }
            mmdevices.Clear();


            return captureDevices;
        }

        private static List<MMDevice> GetMMDevices()
        {
            List<MMDevice> mmdevices = new List<MMDevice>();

            try
            {
                using (var deviceEnum = new MMDeviceEnumerator())
                {

                    var defaultCaptureId = "";
                    try
                    {
                        if (deviceEnum.HasDefaultAudioEndpoint(DataFlow.Capture, Role.Console))
                        {
                            var captureDevice = deviceEnum.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
                            if (captureDevice != null)
                            {

                                defaultCaptureId = captureDevice.ID;
                                mmdevices.Add(captureDevice);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Warn(ex);
                    }

                    var defaultRenderId = "";
                    try
                    {
                        if (deviceEnum.HasDefaultAudioEndpoint(DataFlow.Render, Role.Console))
                        {
                            var renderDevice = deviceEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
                            if (renderDevice != null)
                            {
                                defaultRenderId = renderDevice.ID;
                                mmdevices.Add(renderDevice);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Warn(ex);
                    }

                    try
                    {

                        var allDevices = deviceEnum.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
                        foreach (var d in allDevices)
                        {
                            if (d.ID == defaultRenderId || d.ID == defaultCaptureId)
                            {
                                continue;
                            }
                            mmdevices.Add(d);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Warn(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return mmdevices;
        }
    }

}
