﻿using MediaToolkit.MediaFoundation;

using MediaToolkit.NativeAPIs;
using MediaToolkit.UI;
using NLog;
using SharpDX.Direct3D11;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using GDI = System.Drawing;

namespace WebCamTest
{
    class VideoCaptureSource
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        public VideoCaptureSource() { }

        private Device device = null;

        public Texture2D SharedTexture { get; private set; }

        public event Action BufferUpdated;
        private void OnBufferUpdated()
        {
            BufferUpdated?.Invoke();
        }

        private Texture2D texture = null;
        private SourceReader sourceReader = null;
        private MediaSource mediaSource = null;

        private MfVideoProcessor processor = null;

        public void Setup(int deviceIndex = 0)
        {
            logger.Debug("VideoCaptureSource::Setup()");

            Activate[] activates = null;
            using (var attributes = new MediaAttributes())
            {
                MediaFactory.CreateAttributes(attributes, 1);
                attributes.Set(CaptureDeviceAttributeKeys.SourceType, CaptureDeviceAttributeKeys.SourceTypeVideoCapture.Guid);

                activates = MediaFactory.EnumDeviceSources(attributes);

            }

            if (activates == null || activates.Length == 0)
            {
                logger.Error("SourceTypeVideoCapture not found");
                Console.ReadKey();
            }

            foreach (var activate in activates)
            {
                Console.WriteLine("---------------------------------------------");
                var friendlyName = activate.Get(CaptureDeviceAttributeKeys.FriendlyName);
                var isHwSource = activate.Get(CaptureDeviceAttributeKeys.SourceTypeVidcapHwSource);
                //var maxBuffers = activate.Get(CaptureDeviceAttributeKeys.SourceTypeVidcapMaxBuffers);
                var symbolicLink = activate.Get(CaptureDeviceAttributeKeys.SourceTypeVidcapSymbolicLink);

                logger.Info("FriendlyName " + friendlyName + "\r\n" +
                    "isHwSource " + isHwSource + "\r\n" +
                    //"maxBuffers " + maxBuffers + 
                    "symbolicLink " + symbolicLink);
            }


            var currentActivator = activates[deviceIndex];

            mediaSource = currentActivator.ActivateObject<MediaSource>();
            
            foreach (var a in activates)
            {
                a.Dispose();
            }

            using (var mediaAttributes = new MediaAttributes(IntPtr.Zero))
            {
                MediaFactory.CreateAttributes(mediaAttributes, 2);
                mediaAttributes.Set(SourceReaderAttributeKeys.EnableVideoProcessing, 1);


                //var devMan = new DXGIDeviceManager();
                //devMan.ResetDevice(device);

                //mediaAttributes.Set(SourceReaderAttributeKeys.D3DManager, devMan);


                //MediaFactory.CreateSourceReaderFromMediaSource(mediaSource, mediaAttributes, sourceReader);

                sourceReader = new SourceReader(mediaSource, mediaAttributes);
            }

            Console.WriteLine("------------------CurrentMediaType-------------------");
            var mediaType = sourceReader.GetCurrentMediaType(SourceReaderIndex.FirstVideoStream);
            Console.WriteLine(MfTool.LogMediaType(mediaType));

            var frameSize = MfTool.GetFrameSize(mediaType);
            var subtype = mediaType.Get(MediaTypeAttributeKeys.Subtype);


            mediaType?.Dispose();

            //Device device = null;
            int adapterIndex = 0;
            using (var dxgiFactory = new SharpDX.DXGI.Factory1())
            {
                var adapter = dxgiFactory.Adapters1[adapterIndex];

                device = new Device(adapter,
                                    //DeviceCreationFlags.Debug |
                                    DeviceCreationFlags.VideoSupport |
                                    DeviceCreationFlags.BgraSupport);

                using (var multiThread = device.QueryInterface<SharpDX.Direct3D11.Multithread>())
                {
                    multiThread.SetMultithreadProtected(true);
                }
            }


            SharedTexture = new Texture2D(device,
                 new Texture2DDescription
                 {

                     CpuAccessFlags = CpuAccessFlags.None,
                     BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                     Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                     Width = frameSize.Width,
                     Height = frameSize.Height,

                     MipLevels = 1,
                     ArraySize = 1,
                     SampleDescription = { Count = 1, Quality = 0 },
                     Usage = ResourceUsage.Default,
                         //OptionFlags = ResourceOptionFlags.GdiCompatible//ResourceOptionFlags.None,
                         OptionFlags = ResourceOptionFlags.Shared,

                 });

            texture = new Texture2D(device,
                    new Texture2DDescription
                    {
                        CpuAccessFlags = CpuAccessFlags.Read,
                        BindFlags = BindFlags.None,
                        Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                        Width = frameSize.Width,
                        Height = frameSize.Height,
                        MipLevels = 1,
                        ArraySize = 1,
                        SampleDescription = { Count = 1, Quality = 0 },
                        Usage = ResourceUsage.Staging,
                        OptionFlags = ResourceOptionFlags.None,
                    });


            processor = new MfVideoProcessor(null);
            var inProcArgs = new MfVideoArgs
            {
                Width = frameSize.Width,
                Height = frameSize.Height,
                // Format = VideoFormatGuids.Rgb24,
                Format = subtype,//VideoFormatGuids.NV12,
            };


            var outProcArgs = new MfVideoArgs
            {
                Width = frameSize.Width,
                Height = frameSize.Height,
                Format = VideoFormatGuids.Argb32,
                //Format = VideoFormatGuids.Rgb32,//VideoFormatGuids.Argb32,
            };

            processor.Setup(inProcArgs, outProcArgs);


            //processor.SetMirror(VideoProcessorMirror.MirrorHorizontal);
            processor.SetMirror(VideoProcessorMirror.MirrorVertical);


        }

        public void Start()
        {
            logger.Debug("VideoCaptureSource::Start()");
            running = true;

            Task.Run(() => 
            {
                processor.Start();

                int sampleCount = 0;

                try
                {

                    while (running)
                    {
                        int actualIndex = 0;
                        SourceReaderFlags flags = SourceReaderFlags.None;
                        long timestamp = 0;
                        var sample = sourceReader.ReadSample(SourceReaderIndex.FirstVideoStream, SourceReaderControlFlags.None, out actualIndex, out flags, out timestamp);

                        try
                        {
                            //Console.WriteLine("#" + sampleCount + " Timestamp " + timestamp + " Flags " + flags);

                            if (sample != null)
                            {
                                //Console.WriteLine("SampleTime " + sample.SampleTime + " SampleDuration " + sample.SampleDuration + " SampleFlags " + sample.SampleFlags);
                                Sample outputSample = null;
                                try
                                {
                                    var res = processor.ProcessSample(sample, out outputSample);

                                    if (res)
                                    {
                                        //Console.WriteLine("outputSample!=null" + (outputSample != null));

                                        var mediaBuffer = outputSample.ConvertToContiguousBuffer();
                                        var ptr = mediaBuffer.Lock(out int cbMaxLengthRef, out int cbCurrentLengthRef);

                                        //var width = outProcArgs.Width;
                                        //var height = outProcArgs.Height;

                                        var dataBox = device.ImmediateContext.MapSubresource(texture, 0, MapMode.Read, MapFlags.None);

                                        Kernel32.CopyMemory(dataBox.DataPointer, ptr, (uint)cbCurrentLengthRef);

                                        device.ImmediateContext.UnmapSubresource(texture, 0);


                                        device.ImmediateContext.CopyResource(texture, SharedTexture);
                                        device.ImmediateContext.Flush();

                                        OnBufferUpdated();

                                        //GDI.Bitmap bmp = new GDI.Bitmap(width, height, GDI.Imaging.PixelFormat.Format32bppArgb);

                                        //DxTool.TextureToBitmap(texture, bmp);

                                        ////var bmpData = bmp.LockBits(new GDI.Rectangle(0, 0, width, height), GDI.Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat);
                                        ////uint size = (uint)(bmpData.Stride * height);
                                        ////Kernel32.CopyMemory(bmpData.Scan0, ptr, size);
                                        ////bmp.UnlockBits(bmpData);

                                        ////var fileName = @"d:\BMP\" + "#" + sampleCount + "_" + timestamp + ".bmp";
                                        ////bmp.Save(fileName, GDI.Imaging.ImageFormat.Bmp);

                                        //bmp.Dispose();

                                        mediaBuffer.Unlock();
                                        mediaBuffer?.Dispose();

                                    }

                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex);
                                }
                                finally
                                {
                                    if (outputSample != null)
                                    {
                                        outputSample.Dispose();
                                    }
                                }
                            }
                        }
                        finally
                        {
                            sample?.Dispose();
                        }

                        sampleCount++;

                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
                finally
                {
                    if (processor != null)
                    {
                        processor.Stop();
                    }
                
                }

            });

        }

        private volatile bool running = false;
        public void Stop()
        {
            logger.Debug("VideoCaptureSource::Stop()");

            running = false;
        }

        public void Close()
        {
            logger.Debug("VideoCaptureSource::Close()");

            if (mediaSource != null)
            {
                //mediaSource?.Shutdown();

                mediaSource.Dispose();
                mediaSource = null;
            }

            if (sourceReader != null)
            {
                sourceReader.Dispose();
                sourceReader = null;
            }

            if (device != null)
            {
                device.Dispose();
                device = null;
            }

            if (SharedTexture != null)
            {
                SharedTexture.Dispose();
                SharedTexture = null;
            }

            if(texture != null)
            {
                texture.Dispose();
                texture = null;
            }

            if (processor != null)
            {
                processor.Close();
                processor = null;
            }
        }


        private static void LogSourceTypes(SourceReader sourceReader)
        {
            int streamIndex = 0;
            while (true)
            {
                bool invalidStreamNumber = false;

                int _streamIndex = -1;

                for (int mediaIndex = 0; ; mediaIndex++)
                {
                    try
                    {
                        var nativeMediaType = sourceReader.GetNativeMediaType(streamIndex, mediaIndex);

                        if (_streamIndex != streamIndex)
                        {
                            _streamIndex = streamIndex;
                            Console.WriteLine("====================== StreamIndex#" + streamIndex + "=====================");
                        }

                        Console.WriteLine(MfTool.LogMediaType(nativeMediaType));
                        nativeMediaType?.Dispose();

                    }
                    catch (SharpDX.SharpDXException ex)
                    {
                        if (ex.ResultCode == SharpDX.MediaFoundation.ResultCode.NoMoreTypes)
                        {
                            Console.WriteLine("");
                            break;
                        }
                        else if (ex.ResultCode == SharpDX.MediaFoundation.ResultCode.InvalidStreamNumber)
                        {
                            invalidStreamNumber = true;
                            break;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                if (invalidStreamNumber)
                {
                    break;
                }

                streamIndex++;
            }
        }

    }

}
