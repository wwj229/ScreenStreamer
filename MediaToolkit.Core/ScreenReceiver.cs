﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using MediaToolkit;
using MediaToolkit.Common;
using MediaToolkit.MediaFoundation;
using MediaToolkit.RTP;
using MediaToolkit.Utils;
using NLog;
using SharpDX.Direct3D11;
using SharpDX.MediaFoundation;

namespace MediaToolkit
{
    public class ScreenReceiver
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        private Device device = null;

        public Texture2D sharedTexture { get; private set; }

        private MfH264Decoder decoder = null;
        private MfVideoProcessor processor = null;


        private H264Session h264Session = null;
        private RtpTcpReceiver rtpReceiver = null;
        
        //private RtpReceiver rtpReceiver = null;

        public void Setup(VideoEncodingParams inputPars, VideoEncodingParams outputPars, NetworkStreamingParams networkPars)
        {
            var inputArgs = new MfVideoArgs
            {
                Width = inputPars.Width,
                Height = inputPars.Height,
                FrameRate = inputPars.FrameRate,
            };

            var outputArgs = new MfVideoArgs
            {

                Width = outputPars.Width,
                Height = outputPars.Height,

                FrameRate = outputPars.FrameRate,
            };


            int adapterIndex = 0;
            var dxgiFactory = new SharpDX.DXGI.Factory1();
            var adapter = dxgiFactory.Adapters1[adapterIndex];


            device = new Device(adapter,
                                //DeviceCreationFlags.Debug |
                                DeviceCreationFlags.VideoSupport |
                                DeviceCreationFlags.BgraSupport);

            using (var multiThread = device.QueryInterface<SharpDX.Direct3D11.Multithread>())
            {
                multiThread.SetMultithreadProtected(true);
            }

            sharedTexture = new Texture2D(device,
            new Texture2DDescription
            {

                CpuAccessFlags = CpuAccessFlags.None,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                Width = outputArgs.Width,//640,//texture.Description.Width,
                Height = outputArgs.Height, //480,//texture.Description.Height,

                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Default,
                //OptionFlags = ResourceOptionFlags.GdiCompatible//ResourceOptionFlags.None,
                OptionFlags = ResourceOptionFlags.Shared,

            });

            //ImageProvider = new D3DImageProvider(dispatcher);


            decoder = new MfH264Decoder(device);

            decoder.Setup(inputArgs);


            var decoderType = decoder.OutputMediaType;
            var decFormat = decoderType.Get(MediaTypeAttributeKeys.Subtype);
            var decFrameSize = MfTool.GetFrameSize(decoderType);


            processor = new MfVideoProcessor(device);
            var inProcArgs = new MfVideoArgs
            {
                Width = decFrameSize.Width,
                Height = decFrameSize.Height,
                Format = decFormat,
            };



            var outProcArgs = new MfVideoArgs
            {
                Width = outputArgs.Width,
                Height = outputArgs.Height,
                Format = VideoFormatGuids.Argb32,
            };

            processor.Setup(inProcArgs, outProcArgs);


            h264Session = new H264Session();
            rtpReceiver = new RtpTcpReceiver(null);
            rtpReceiver.Open(networkPars.LocalAddr, networkPars.LocalPort);
            rtpReceiver.RtpPacketReceived += RtpReceiver_RtpPacketReceived;


            receiverStats = new ReceiverStats();

        }

        public void Play()
        {
            Statistic.RegisterCounter(receiverStats);

            //ImageProvider.Start(sharedTexture);

            decoder.Start();
            processor.Start();
            rtpReceiver.Start();
        }

        private Stopwatch sw = new Stopwatch();
        private void RtpReceiver_RtpPacketReceived(RtpPacket packet)
        {

            var time = MediaTimer.GetRelativeTime();

            byte[] rtpPayload = packet.Payload.ToArray();

            //totalPayloadReceivedSize += rtpPayload.Length;
            // logger.Debug("totalPayloadReceivedSize " + totalPayloadReceivedSize);

            var nal = h264Session.Depacketize(packet);
            if (nal != null)
            {
                sw.Restart();

                Decode(nal);

                receiverStats.Update(time, nal.Length, sw.ElapsedMilliseconds);

            }

        }


        int sampleCount = 0;
        private static object syncRoot = new object();
        private void Decode(byte[] nal)
        {
            try
            {
                var encodedSample = MediaFactory.CreateSample();
                try
                {
                    using (MediaBuffer mb = MediaFactory.CreateMemoryBuffer(nal.Length))
                    {
                        var dest = mb.Lock(out int cbMaxLength, out int cbCurrentLength);
                        //logger.Debug(sampleCount + " Marshal.Copy(...) " + nal.Length);
                        Marshal.Copy(nal, 0, dest, nal.Length);

                        mb.CurrentLength = nal.Length;
                        mb.Unlock();

                        encodedSample.AddBuffer(mb);
                    }

                    var res = decoder.ProcessSample(encodedSample, out Sample decodedSample);

                    if (res)
                    {
                        if (decodedSample != null)
                        {
                            res = processor.ProcessSample(decodedSample, out Sample rgbSample);

                            if (res)
                            {
                                if (rgbSample != null)
                                {
                                    var buffer = rgbSample.ConvertToContiguousBuffer();
                                    using (var dxgiBuffer = buffer.QueryInterface<DXGIBuffer>())
                                    {
                                        var uuid = SharpDX.Utilities.GetGuidFromType(typeof(Texture2D));
                                        dxgiBuffer.GetResource(uuid, out IntPtr intPtr);
                                        using (Texture2D rgbTexture = new Texture2D(intPtr))
                                        {
                                            device.ImmediateContext.CopyResource(rgbTexture, sharedTexture);
                                            device.ImmediateContext.Flush();
                                        };

                                        OnUpdateBuffer();

                                        //ImageProvider?.Update();
                                    }

                                    buffer?.Dispose();
                                }
                            }

                            rgbSample?.Dispose();

                        }
                    }

                    decodedSample?.Dispose();
                }
                finally
                {
                    encodedSample?.Dispose();
                }

                sampleCount++;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

        }

        public event Action UpdateBuffer;

        private void OnUpdateBuffer()
        {
            UpdateBuffer?.Invoke();
        }

        public void Stop()
        {
            if (rtpReceiver != null)
            {
                rtpReceiver.RtpPacketReceived -= RtpReceiver_RtpPacketReceived;
                rtpReceiver.Close();
            }

            if (decoder != null)
            {
                decoder.Close();
                decoder = null;
            }
            if (processor != null)
            {
                processor.Close();
                processor = null;
            }

            if (sharedTexture != null)
            {
                sharedTexture.Dispose();
                sharedTexture = null;
            }
            if (device != null)
            {
                device.Dispose();
                device = null;
            }

            Statistic.UnregisterCounter(receiverStats);
        }


        private ReceiverStats receiverStats = new ReceiverStats();

        class ReceiverStats : StatCounter
        {
            public uint buffersCount = 0;

            public long totalBytesReceived = 0;
            public double receiveBytesPerSec1 = 0;
            public double receiveBytesPerSec2 = 0;
            public double receiveBytesPerSec3 = 0;
            public double lastTimestamp = 0;

            public long avgEncodingTime = 0;

            public double totalTime = 0;

            public void Update(double timestamp, int bytesReceived, long encTime)
            {
                if (lastTimestamp > 0)
                {
                    var time = timestamp - lastTimestamp;
                    if (time > 0)
                    {
                        var bytesPerSec = bytesReceived / time;

                        receiveBytesPerSec1 = (bytesPerSec * 0.05 + receiveBytesPerSec1 * (1 - 0.05));

                        receiveBytesPerSec2 = (receiveBytesPerSec1 * 0.05 + receiveBytesPerSec2 * (1 - 0.05));

                        receiveBytesPerSec3 = (receiveBytesPerSec2 * 0.05 + receiveBytesPerSec3 * (1 - 0.05));


                        avgEncodingTime = (long)(encTime * 0.1 + avgEncodingTime * (1 - 0.1));

                        totalTime += (timestamp - lastTimestamp);

                        //if (totalTime > 0)
                        //{
                        //    receiveBytesPerSec3 = totalBytesReceived / totalTime;
                        //}

                    }
                }

                totalBytesReceived += bytesReceived;

                lastTimestamp = timestamp;
                buffersCount++;
            }

            public override string GetReport()
            {
                StringBuilder sb = new StringBuilder();

                //var mbytesPerSec = sendBytesPerSec / (1024.0 * 1024);
                //var mbytes = totalBytesSend / (1024.0 * 1024);


                TimeSpan time = TimeSpan.FromSeconds(totalTime);
                sb.AppendLine(time.ToString(@"hh\:mm\:ss\.fff"));

                sb.AppendLine(buffersCount + " Buffers");

                //sb.AppendLine(StringHelper.SizeSuffix((long)sendBytesPerSec1) + "/s");
                //sb.AppendLine(StringHelper.SizeSuffix((long)sendBytesPerSec2) + "/s");
                sb.AppendLine(StringHelper.SizeSuffix((long)receiveBytesPerSec3) + "/s");
                sb.AppendLine(StringHelper.SizeSuffix(totalBytesReceived));


                sb.AppendLine(avgEncodingTime + " ms");

                //sb.AppendLine(mbytes.ToString("0.0") + " MBytes");
                //sb.AppendLine(mbytesPerSec.ToString("0.000") + " MByte/s");

                return sb.ToString();
            }

            public override void Reset()
            {
                buffersCount = 0;

                totalBytesReceived = 0;
                receiveBytesPerSec1 = 0;

                lastTimestamp = 0;
            }
        }
    }
}