﻿using FFmpegLib;
using MediaToolkit;
using MediaToolkit.Core;
using MediaToolkit.RTP;
using MediaToolkit.Utils;
using NAudio.CoreAudioApi;
using NAudio.Gui;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStreamer
{

    public class AudioStreamer
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();


        public AudioStreamer(AudioSource source)
        {
            this.audioSource = source;
        }

        private AudioSource audioSource = null;

        private BufferedWaveProvider bufferedWaveProvider = null;

        private SampleChannel sampleChannel = null;

        private AudioEncoder audioResampler = null;

        public int ClientsCount
        {
            get
            {
                int count = 0;
                if (RtpSender != null)
                {
                    count = RtpSender.ClientsCount;
                }

                return count;
            }
        }

        public IRtpSender RtpSender { get; private set; }

        private PCMUSession session = null;

        public bool IsStreaming { get; private set; }
        private int sampleByteSize = 0;

        private WaveformPainter[] wavePainters = null;

        //public void SetWaveformPainter(WaveformPainter painter)
        //{
        //    this.waveformPainter = painter;

        //}

        public void SetWaveformPainter(WaveformPainter[] wavePainters)
        {
            this.wavePainters = wavePainters;

        }


        public void Setup(AudioEncoderSettings encoderSettings, NetworkSettings networkSettings)
        {
            logger.Debug("AudioLoopbackSource::Start(...) ");

            //FileStream fs = new FileStream("d:\\test_audio_4", FileMode.Create, FileAccess.ReadWrite);

            try
            {

                // var capture = audioSource.Capture;
                var waveFormat = audioSource.WaveFormat;
                bufferedWaveProvider = new BufferedWaveProvider(waveFormat);
                bufferedWaveProvider.DiscardOnBufferOverflow = true;

                sampleChannel = new SampleChannel(bufferedWaveProvider);
                sampleChannel.PreVolumeMeter += SampleChannel_PreVolumeMeter;

                audioResampler = new AudioEncoder();

                var captureParams = new AudioEncoderSettings
                {
                    SampleRate = waveFormat.SampleRate,
                    Channels = waveFormat.Channels,

                };


                audioResampler.Open(captureParams, encoderSettings);

                session = new PCMUSession();

                if (networkSettings.TransportMode == TransportMode.Tcp)
                {
                    RtpSender = new RtpTcpSender(session);
                }
                else if (networkSettings.TransportMode == TransportMode.Udp)
                {
                    RtpSender = new RtpUdpSender(session);
                }
                else
                {
                    throw new FormatException("NotSupportedFormat " + networkSettings.TransportMode);
                }


                audioSource.DataAvailable += AudioSource_DataAvailable;
                RtpSender.Setup(networkSettings);
                networkSettings.SSRC = session.SSRC;



                RtpSender.Start();



                IsStreaming = true;
                OnStateChanged();
            }
            catch (Exception ex)
            {
                logger.Error(ex);

                Close();
            }

        }

        public void Start()
        {

        }

        private uint rtpTimestamp = 0;
        private Stopwatch sw = new Stopwatch();
        private void AudioSource_DataAvailable(byte[] data)
        {
            if (closing)
            {
                return;
            }

            if (data.Length > 0)
            {

                bufferedWaveProvider.AddSamples(data, 0, data.Length);

                var audioBuffer = new float[data.Length];

                sampleChannel.Read(audioBuffer, 0, data.Length);

                byte[] dest = null;
                audioResampler.Resample2(data, out dest);
                if (dest != null && dest.Length > 0)
                {
                    //Debug.WriteLine("dest.Length " + dest.Length);

                    rtpTimestamp += (uint)(sw.ElapsedMilliseconds * 8.0);
                    sw.Restart();

                    double ralativeTime = MediaTimer.GetRelativeTime();
                    //uint rtpTime = (uint)(ralativeTime * 8000);

                    // streamer.Push(dest, ralativeTime);

                    RtpSender.Push(dest, ralativeTime);

                    //fs.Write(dest, 0, dest.Length);
                    //fs.Write(a.Buffer, 0, a.BytesRecorded);
                }
            }
        }

        private void SampleChannel_PreVolumeMeter(object sender, StreamVolumeEventArgs e)
        {
            if (wavePainters == null)
            {
                return;
            }

            var samples = e.MaxSampleValues;

            for (int i = 0; i < samples.Length; i++)
            {
                var s = samples[i];

                if (i < wavePainters.Length)
                {
                    var painter = wavePainters[i];

                    painter?.AddMax(s);
                }
            }
        }

        public event Action StateChanged;

        private void OnStateChanged()
        {
            StateChanged?.Invoke();
        }

        private void Capture_RecordingStopped(object sender, StoppedEventArgs e)
        {
            logger.Debug("AudioLoopbackSource::Capture_RecordingStopped(...)");

            Close();

            IsStreaming = false;
            OnStateChanged();
        }

        public void Close()
        {
            logger.Debug("AudioLoopbackSource::Close()");
            closing = true;

            if (RtpSender != null)
            {
                RtpSender.Close();
                RtpSender = null;
            }

            if (audioSource != null)
            {
                audioSource.DataAvailable -= AudioSource_DataAvailable;
            }

            if (audioResampler != null)
            {
                audioResampler.Close();
                audioResampler = null;
            }

        }

        private bool closing = false;



    }

}