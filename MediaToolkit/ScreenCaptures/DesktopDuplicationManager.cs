﻿using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

using System.IO;

using System.Threading;

using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
//using SharpDX.Direct2D1;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

using GDI = System.Drawing;
using Direct2D = SharpDX.Direct2D1;
using MediaToolkit.Utils;
using System.Runtime.InteropServices;
using SharpDX.Mathematics.Interop;
using SharpDX.MediaFoundation;
using MediaToolkit.Logging;
using MediaToolkit.SharedTypes;
using System.Windows.Forms;

namespace MediaToolkit.ScreenCaptures
{


    class DesktopDuplicationManager
    {
        private static TraceSource logger = TraceManager.GetTrace("MediaToolkit.ScreenCaptures");

        private Device device = null;


        public GDI.Rectangle AllScreensRect { get; private set; }

        public Texture2D compositionTexture = null;

        private List<_DesktopDuplicator> deskDupls = new List<_DesktopDuplicator>();

        public bool CaptureMouse { get; set; }

        private bool initialized = false;

        public long AdapterId { get; private set; }

        public void Init()
        {
            logger.Debug("DesktopDuplicationManager::Init(...)");

            CaptureMouse = true;
            this.AllScreensRect = SystemInformation.VirtualScreen;

            //var rawRect = new RawRectangle
            //{
            //    Left = AllScreensRect.Left,
            //    Right = AllScreensRect.Right,
            //    Top = AllScreensRect.Top,
            //    Bottom = AllScreensRect.Bottom,
            //};

           // SetupRegions(rawRect, )

            SharpDX.DXGI.Factory1 dxgiFactory = null;
            Adapter1 adapter = null;
            Output output = null;
            try
            {
                dxgiFactory = new SharpDX.DXGI.Factory1();

                logger.Info(MediaFoundation.DxTool.LogDxAdapters(dxgiFactory.Adapters1));

                if (adapter == null)
                {// первым идет адаптер с которому подключен primary монитор
                    adapter = dxgiFactory.GetAdapter1(0);
                }

                AdapterId = adapter.Description.Luid;

                //logger.Info("Screen source info: " + adapter.Description.Description + " " + output.Description.DeviceName);

                var deviceCreationFlags =
                    //DeviceCreationFlags.Debug |
                    DeviceCreationFlags.VideoSupport |
                    DeviceCreationFlags.BgraSupport;


                device = new Device(adapter, deviceCreationFlags);
                using (var multiThread = device.QueryInterface<SharpDX.Direct3D11.Multithread>())
                {
                    multiThread.SetMultithreadProtected(true);
                }

                if (deskDupls != null)
                {
                    //...
                }


                deskDupls = new List<_DesktopDuplicator>();

                foreach (var _output in adapter.Outputs)
                {
                    var descr = _output.Description;

                    var desktopBounds = descr.DesktopBounds;

                    var desktopRect = new GDI.Rectangle
                    {
                        X = desktopBounds.Left,
                        Y = desktopBounds.Top,
                        Width = desktopBounds.Right - desktopBounds.Left,
                        Height = desktopBounds.Bottom - desktopBounds.Top,
                    };

                    _DesktopDuplicator deskDupl = new _DesktopDuplicator(device);
                    deskDupl.Init(_output, desktopRect);
                    deskDupl.CaptureMouse = this.CaptureMouse;

                    deskDupls.Add(deskDupl);

                    //var rect = GDI.Rectangle.Intersect(desktopRect, SrcRect);
                    //if (rect.Width > 0 && rect.Height > 0)
                    //{
                    //    logger.Info("Screen source info: " + adapter.Description.Description + " " + descr.DeviceName);

                    //    DesktopDuplicator deskDupl = new DesktopDuplicator(device);

                    //    deskDupl.Init(_output, SrcRect);
                    //    deskDupl.CaptureMouse = this.CaptureMouse;

                    //    deskDupls.Add(deskDupl);
                    //}
                    //else
                    //{
                    //    logger.Debug("No common area: " + descr.DeviceName + " " + SrcRect.ToString());
                    //    continue;
                    //}

                    _output.Dispose();
                }

            }
            finally
            {
                if (adapter != null)
                {
                    adapter.Dispose();
                    adapter = null;
                }

                if (output != null)
                {
                    output.Dispose();
                    output = null;
                }

                if (dxgiFactory != null)
                {
                    dxgiFactory.Dispose();
                    dxgiFactory = null;
                }
            }

            compositionTexture = new Texture2D(device,
               new Texture2DDescription
               {
                   CpuAccessFlags = CpuAccessFlags.None,
                   BindFlags = BindFlags.ShaderResource,
                   Format = Format.B8G8R8A8_UNorm,

                   Width = AllScreensRect.Width,
                   Height = AllScreensRect.Height,
                   MipLevels = 1,
                   ArraySize = 1,
                   SampleDescription = { Count = 1, Quality = 0 },
                   Usage = ResourceUsage.Default,
               });


            initialized = true;

        }




        private object syncLock = new object();

        public ErrorCode UpdateBuffer()
        {
            ErrorCode Result = ErrorCode.Ok;

            try
            {
                if (!initialized)
                {
                   // Init();
                }

                bool lockTaken = false;
                try
                {
                    Monitor.TryEnter(syncLock, 5, ref lockTaken);

                    if (lockTaken)
                    {
                        foreach (var dupl in deskDupls)
                        {
                            var errorCode = dupl.TryGetScreenTexture(out Texture2D screenTexture);

                            if (errorCode != ErrorCode.Ok)
                            {
                                //CloseDx();

                                //Thread.Sleep(100);
                                return errorCode;

                            }

                            var duplRect = dupl.duplRect;

                            ResourceRegion srcRegion = new ResourceRegion
                            {
                                Left = duplRect.Left,
                                Top = duplRect.Top,
                                Right = duplRect.Right,
                                Bottom = duplRect.Bottom,
                                Back = 1,
                            };

                            var destRect = dupl.drawRect;

                            device.ImmediateContext.CopySubresourceRegion(screenTexture, 0, srcRegion, compositionTexture, 0, destRect.Left, destRect.Top);

                        }
                    }
                    else
                    {
                        logger.Debug("lockTaken == false");
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        Monitor.Exit(syncLock);
                    }
                }

            }
            catch (SharpDXException ex)
            {
                logger.Error(ex);
                // Process error...

               // CloseDx();

                //throw;
                Thread.Sleep(100);

            }

            return Result;
        }


        public void Close()
        {
            if (deskDupls != null)
            {
                foreach (var dupl in deskDupls)
                {
                    dupl?.Close();
                }
                deskDupls = null;
            }

            if (device != null && !device.IsDisposed)
            {
                device.Dispose();
                device = null;
            }

            if (compositionTexture != null && !compositionTexture.IsDisposed)
            {
                compositionTexture.Dispose();
                compositionTexture = null;
            }

            initialized = false;

        }



        class _DesktopDuplicator
        {
            private readonly Device device = null;

            internal _DesktopDuplicator(Device device)
            {
                this.device = device;
            }

            private OutputDuplication deskDupl = null;
            private Texture2D duplTexture = null;

            internal Texture2D screenTexture = null;

            private SharpDX.Direct2D1.RenderTarget screenTarget = null;

            internal bool CaptureMouse = true;
            private CursorInfo cursorInfo = new CursorInfo();

            internal Rectangle drawRect = Rectangle.Empty;
            internal Rectangle duplRect = Rectangle.Empty;

            public void Init(Output output, GDI.Rectangle srcRect)
            {
                logger.Debug("DesktopDuplicator::Init(...) " + srcRect.ToString());

                try
                {
                    var descr = output.Description;
                    RawRectangle screenRect = descr.DesktopBounds;
                    int width = screenRect.Right - screenRect.Left;
                    int height = screenRect.Bottom - screenRect.Top;

                    SetupRegions(screenRect, srcRect);


                    if(descr.DeviceName == "\\\\.\\DISPLAY1")
                    {
                        drawRect = new Rectangle
                        {
                            X = 1920,
                            Y = 0, 
                            Width = width,
                            Height = height,
                        };
                    }
                    else if (descr.DeviceName == "\\\\.\\DISPLAY2")
                    {
                        drawRect = new Rectangle
                        {
                            X = 0,
                            Y = 0,
                            Width = width,
                            Height = height,
                        };
                    }

                    screenTexture = new Texture2D(device,
                      new Texture2DDescription
                      {
                          CpuAccessFlags = CpuAccessFlags.None,
                          BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                          Format = Format.B8G8R8A8_UNorm,
                          Width = width,
                          Height = height,
                          MipLevels = 1,
                          ArraySize = 1,
                          SampleDescription = { Count = 1, Quality = 0 },
                          Usage = ResourceUsage.Default,

                          OptionFlags = ResourceOptionFlags.Shared,

                      });

                    using (SharpDX.Direct2D1.Factory1 factory2D1 = new SharpDX.Direct2D1.Factory1(SharpDX.Direct2D1.FactoryType.MultiThreaded))
                    {
                        using (var surf = screenTexture.QueryInterface<Surface>())
                        {
                            var pixelFormat = new Direct2D.PixelFormat(Format.B8G8R8A8_UNorm, Direct2D.AlphaMode.Premultiplied);
                            var renderTargetProps = new Direct2D.RenderTargetProperties(pixelFormat);
                            screenTarget = new Direct2D.RenderTarget(factory2D1, surf, renderTargetProps);
                        }

                    }

                    using (var output1 = output.QueryInterface<Output1>())
                    {
                        // Duplicate the output
                        deskDupl = output1.DuplicateOutput(device);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    Close();

                    throw;
                }

                deviceReady = true;

            }

            private void SetupRegions(RawRectangle screenRect, GDI.Rectangle srcRect)
            {


                int left, right, top, bottom;

                if (srcRect.X < screenRect.Left)
                {// за левой границей экрана
                    left = screenRect.Left;
                }
                else
                {
                    left = srcRect.X;
                }

                if (srcRect.Right > screenRect.Right)
                { // за правой границей
                    right = screenRect.Right;
                }
                else
                {
                    right = srcRect.Right;
                }

                if (srcRect.Y < screenRect.Top)
                {// за верхней границей
                    top = screenRect.Top;
                }
                else
                {
                    top = srcRect.Y;
                }

                if (srcRect.Bottom > screenRect.Bottom)
                {// за нижней границей
                    bottom = screenRect.Bottom;
                }
                else
                {
                    bottom = srcRect.Bottom;
                }

                //// в координатах сцены
                //drawRect = new Rectangle
                //{
                //    X = left - srcRect.X,
                //    Y = top - srcRect.Y,
                //    Width = right - left,
                //    Height = bottom - top,
                //};


                // в координатах сцены
                drawRect = new Rectangle
                {
                    X = left - srcRect.X,
                    Y = top - srcRect.Y,
                    Width = right - left,
                    Height = bottom - top,
                };

                // в координатах захвата экрана
                var duplLeft = left - screenRect.Left;
                var duplRight = right - screenRect.Left;
                var duplTop = top - screenRect.Top;
                var duplBottom = bottom - screenRect.Top;

                duplRect = new Rectangle
                {
                    X = duplLeft,
                    Y = duplTop,
                    Width = duplRight - duplLeft,
                    Height = duplBottom - duplTop,
                };


                logger.Debug("duplRect=" + duplRect.ToString() + " drawRect=" + drawRect.ToString());

            }

            private bool deviceReady = false;
            public ErrorCode TryGetScreenTexture(out Texture2D texture, int timeout = 0)
            {
                texture = null;

                ErrorCode Result = ErrorCode.Unexpected;
                if (!deviceReady)
                {
                    return ErrorCode.NotReady;
                }

                try
                {

                    SharpDX.DXGI.Resource desktopResource = null;
                    try
                    {
                        var acquireResult = deskDupl.TryAcquireNextFrame(timeout, out OutputDuplicateFrameInformation frameInfo, out desktopResource);

                        if (acquireResult.Failure)
                        {
                            if (acquireResult == SharpDX.DXGI.ResultCode.WaitTimeout)
                            {
                                return ErrorCode.Ok;
                            }
                            else
                            {
                                logger.Debug("result.Code " + acquireResult.Code);
                                acquireResult.CheckError();
                            }
                        }

                        if (duplTexture != null)
                        {
                            duplTexture.Dispose();
                            duplTexture = null;
                        }

                        duplTexture = desktopResource.QueryInterface<Texture2D>();

                        #region Update regions info

                        //var frameParams = GetFrameParams(frameInfo);
                        /*
                            var moveRects = frameParams.MoveRects;
                            foreach (OutputDuplicateMoveRectangle moveRect in moveRects)
                            {
                                //...
                                var srcPoint = moveRect.SourcePoint;
                                GDI.Point point = new GDI.Point(srcPoint.X, srcPoint.Y);

                                var destRect = moveRect.DestinationRect;
                                int x = destRect.Left;
                                int y = destRect.Top;
                                int width = destRect.Right - destRect.Left;
                                int height = destRect.Bottom - destRect.Top;
                                GDI.Rectangle rect = new GDI.Rectangle(x, y, width, height);

                                logger.Debug("srcPoint " + point.ToString() + " destRect " + rect.ToString());
                            }
                            var dirtyRects = frameParams.DirtyRects;
                            foreach (RawRectangle dirtyRect in dirtyRects)
                            {
                                int x = dirtyRect.Left;
                                int y = dirtyRect.Top;
                                int width = dirtyRect.Right - dirtyRect.Left;
                                int height = dirtyRect.Bottom - dirtyRect.Top;
                                GDI.Rectangle rect = new GDI.Rectangle(x, y, width, height);
                            }
                        */

                        #endregion

                        UpdateMouseInfo(frameInfo);

                        device.ImmediateContext.CopyResource(duplTexture, screenTexture);
                        device.ImmediateContext.Flush();

                        if (cursorInfo != null && cursorInfo.Visible && CaptureMouse)
                        {
                            screenTarget.BeginDraw();
                            //screenTarget.Clear(Color.Green);

                            DrawCursor(screenTarget, cursorInfo);

                            screenTarget.EndDraw();
                        }
                        texture = screenTexture;

                        //texture = duplTexture;


                        Result = 0;
                    }
                    finally
                    {
                        if (desktopResource != null)
                        {
                            deskDupl.ReleaseFrame();

                            desktopResource.Dispose();
                            desktopResource = null;
                        }
                    }
                }
                catch (SharpDXException ex)
                {
                    if (ex.ResultCode == SharpDX.DXGI.ResultCode.WaitTimeout)
                    {
                        return 0;
                    }
                    else
                    {
                        if (ex.ResultCode == SharpDX.DXGI.ResultCode.AccessLost ||
                            ex.ResultCode == SharpDX.DXGI.ResultCode.AccessDenied ||
                            ex.ResultCode == SharpDX.DXGI.ResultCode.DeviceReset ||
                            ex.ResultCode == SharpDX.DXGI.ResultCode.DeviceRemoved ||
                            ex.HResult == (int)NativeAPIs.HResult.E_ACCESSDENIED)
                        {

                            logger.Warn(ex.Descriptor.ToString());

                            Result = ErrorCode.AccessDenied;


                        }
                        else
                        {
                            logger.Error(ex);
                            Result = ErrorCode.Unexpected;
                        }




                    }

                }

                return Result;
            }


            class FramePars
            {
                public OutputDuplicateFrameInformation FrameInfo;
                public OutputDuplicateMoveRectangle[] MoveRects = new OutputDuplicateMoveRectangle[0];
                public RawRectangle[] DirtyRects = new RawRectangle[0];

            }

            private FramePars GetFrameParams(OutputDuplicateFrameInformation frameInfo)
            {
                FramePars frameParams = new FramePars();

                OutputDuplicateMoveRectangle[] moveRects = new OutputDuplicateMoveRectangle[0];
                RawRectangle[] dirtyRects = new RawRectangle[0];

                if (frameInfo.TotalMetadataBufferSize > 0) //if (frameInfo.AccumulatedFrames > 0)
                {
                    var bufferSizeBytes = frameInfo.TotalMetadataBufferSize;

                    int moveRectSizeBytes = GetMoveRects(bufferSizeBytes, out moveRects);
                    bufferSizeBytes -= moveRectSizeBytes;


                    int dirtyRectSizeBytes = GetDirtyRects(bufferSizeBytes, out dirtyRects);
                    bufferSizeBytes -= dirtyRectSizeBytes;

                    frameParams.DirtyRects = dirtyRects;
                    frameParams.MoveRects = moveRects;
                    frameParams.FrameInfo = frameInfo;

                }

                return frameParams;
            }

            private int GetDirtyRects(int bufferSizeBytes, out RawRectangle[] dirtyRects)
            {

                int dirtyRectsBufferSizeRequired = 0;
                dirtyRects = new RawRectangle[0];
                if (bufferSizeBytes <= 0)
                {
                    return dirtyRectsBufferSizeRequired;

                }

                var rawRectItemSize = Marshal.SizeOf(typeof(RawRectangle));
                var maxRawRectItemCount = bufferSizeBytes / rawRectItemSize;

                var dirtyRectsBuffer = new RawRectangle[maxRawRectItemCount];

                try
                {
                    deskDupl.GetFrameDirtyRects(bufferSizeBytes, dirtyRectsBuffer, out dirtyRectsBufferSizeRequired);
                }
                catch (SharpDXException ex)
                {
                    if (ex.ResultCode == SharpDX.DXGI.ResultCode.MoreData)
                    {
                        logger.Error(ex);

                        // TODO:
                        //...
                    }

                    throw;
                }

                var dirtyCount = dirtyRectsBufferSizeRequired / rawRectItemSize;
                if (dirtyCount > 0)
                {
                    if (dirtyCount > maxRawRectItemCount)
                    {
                        dirtyRects = dirtyRectsBuffer.Take(dirtyCount).ToArray();
                    }
                    else
                    {
                        dirtyRects = dirtyRectsBuffer;
                    }
                }

                return dirtyRectsBufferSizeRequired;
            }

            private int GetMoveRects(int bufferSizeBytes, out OutputDuplicateMoveRectangle[] moveRects)
            {
                int moveRectsBufferSizeRequired = 0;

                moveRects = new OutputDuplicateMoveRectangle[0];
                if (bufferSizeBytes <= 0)
                {
                    return moveRectsBufferSizeRequired;
                }

                var moveItemSize = Marshal.SizeOf(typeof(OutputDuplicateMoveRectangle));
                var maxMoveRectItemCount = bufferSizeBytes / moveItemSize;

                if (maxMoveRectItemCount > 0)
                {
                    var moveRectBuf = new OutputDuplicateMoveRectangle[maxMoveRectItemCount];

                    try
                    {
                        deskDupl.GetFrameMoveRects(bufferSizeBytes, moveRectBuf, out moveRectsBufferSizeRequired);
                    }
                    catch (SharpDXException ex)
                    {
                        if (ex.ResultCode == SharpDX.DXGI.ResultCode.MoreData)
                        {
                            logger.Error(ex);

                            // TODO:
                            //maxMoveRectItemCount = moveRectsBufferSizeRequired / moveItemSize;
                            //moveRectBuf = new OutputDuplicateMoveRectangle[maxMoveRectItemCount];
                            // continue...
                        }

                        throw;
                    }


                    var moveCount = moveRectsBufferSizeRequired / moveItemSize;

                    if (moveCount > 0)
                    {
                        if (maxMoveRectItemCount > moveCount)
                        {
                            moveRects = moveRectBuf.Take(moveCount).ToArray();
                        }
                        else
                        {
                            moveRects = moveRectBuf;
                        }
                    }
                }

                return moveRectsBufferSizeRequired;
            }


            private void UpdateMouseInfo(OutputDuplicateFrameInformation frameInfo)
            {
                if (frameInfo.LastMouseUpdateTime > 0)
                {// A non-zero mouse update timestamp indicates that there is a mouse position update and optionally a shape change

                    var pointerPosition = frameInfo.PointerPosition;

                    cursorInfo.LastTimeStamp = frameInfo.LastMouseUpdateTime;
                    cursorInfo.Position = pointerPosition.Position;
                    cursorInfo.Visible = pointerPosition.Visible;

                    // logger.Debug("mouseInfo " + mouseInfo.Position.X + " " +  mouseInfo.Position.Y);

                    if (frameInfo.PointerShapeBufferSize > 0)
                    {

                        var bufSize = frameInfo.PointerShapeBufferSize;

                        if (bufSize > cursorInfo.BufferSize)
                        {
                            var ptr = cursorInfo.PtrShapeBuffer;
                            if (ptr != IntPtr.Zero)
                            {
                                Marshal.FreeHGlobal(ptr);
                            }
                            cursorInfo.BufferSize = bufSize;
                            cursorInfo.PtrShapeBuffer = Marshal.AllocHGlobal(cursorInfo.BufferSize);

                        }

                        try
                        {

                            deskDupl.GetFramePointerShape(cursorInfo.BufferSize, cursorInfo.PtrShapeBuffer,
                                out int pointerShapeBufferSizeRequired,
                                out OutputDuplicatePointerShapeInformation pointerShapeInfo);

                            cursorInfo.ShapeInfo = pointerShapeInfo;

                            //logger.Debug("pointerShapeInfo " + pointerShapeInfo.Type);
                        }
                        catch (SharpDXException ex)
                        {

                            if (ex.ResultCode == SharpDX.DXGI.ResultCode.MoreData)
                            {
                                // TODO:
                                //...
                                logger.Error(ex);

                            }

                            throw;
                        }

                    }
                    else
                    {// No new shape

                    }
                }

            }


            #region CursorInfo

            private unsafe void DrawCursor(SharpDX.Direct2D1.RenderTarget renderTarger, CursorInfo cursor)
            {

                var position = cursor.Position;

                var shapeBuff = cursor.PtrShapeBuffer;
                var shapeInfo = cursor.ShapeInfo;

                int width = shapeInfo.Width;
                int height = shapeInfo.Height;
                int pitch = shapeInfo.Pitch;

                int left = position.X;
                int top = position.Y;
                int right = position.X + width;
                int bottom = position.Y + height;

                //logger.Debug(left + " " + top + " " + right + " " + bottom);

                if (cursor.ShapeInfo.Type == (int)ShapeType.DXGI_OUTDUPL_POINTER_SHAPE_TYPE_COLOR)
                {

                    var data = new DataPointer(shapeBuff, height * pitch);
                    var prop = new Direct2D.BitmapProperties(new Direct2D.PixelFormat(Format.B8G8R8A8_UNorm, Direct2D.AlphaMode.Premultiplied));
                    var size = new Size2(width, height);
                    var cursorBits = new Direct2D.Bitmap(renderTarger, size, data, pitch, prop);
                    try
                    {

                        var cursorRect = new RawRectangleF(left, top, right, bottom);

                        renderTarger.DrawBitmap(cursorBits, cursorRect, 1.0f, Direct2D.BitmapInterpolationMode.Linear);

                    }
                    finally
                    {
                        cursorBits?.Dispose();
                    }
                }
                else if (cursor.ShapeInfo.Type == (int)ShapeType.DXGI_OUTDUPL_POINTER_SHAPE_TYPE_MONOCHROME)
                {
                    height = height / 2;

                    left = position.X;
                    top = position.Y;
                    right = position.X + width;
                    bottom = position.Y + height;
                    pitch = width * 4;

                    Texture2D desktopRegionTex = null;
                    try
                    {
                        desktopRegionTex = new Texture2D(device,
                        new Texture2DDescription
                        {
                            CpuAccessFlags = CpuAccessFlags.Read,
                            BindFlags = BindFlags.None,
                            Format = Format.B8G8R8A8_UNorm,
                            Width = width,
                            Height = height,
                            MipLevels = 1,
                            ArraySize = 1,
                            SampleDescription = { Count = 1, Quality = 0 },
                            Usage = ResourceUsage.Staging,
                            OptionFlags = ResourceOptionFlags.None,
                        });

                        var region = new ResourceRegion(left, top, 0, right, bottom, 1);
                        var immediateContext = device.ImmediateContext;
                        immediateContext.CopySubresourceRegion(screenTexture, 0, region, desktopRegionTex, 0);

                        var dataBox = immediateContext.MapSubresource(desktopRegionTex, 0, MapMode.Read, MapFlags.None);
                        try
                        {
                            var desktopBuffer = new byte[width * height * 4];
                            Marshal.Copy(dataBox.DataPointer, desktopBuffer, 0, desktopBuffer.Length);

                            var shapeBufferLenght = width * height * 4;
                            var shapeBuffer = new byte[shapeBufferLenght];

                            var maskBufferLenght = width * height / 8;
                            var andMaskBuffer = new byte[maskBufferLenght];
                            Marshal.Copy(shapeBuff, andMaskBuffer, 0, andMaskBuffer.Length);

                            var xorMaskBuffer = new byte[maskBufferLenght];
                            Marshal.Copy(shapeBuff + andMaskBuffer.Length, xorMaskBuffer, 0, xorMaskBuffer.Length);

                            for (var row = 0; row < height; ++row)
                            {
                                byte mask = 0x80;

                                for (var col = 0; col < width; ++col)
                                {
                                    var maskIndex = row * width / 8 + col / 8;

                                    var andMask = ((andMaskBuffer[maskIndex] & mask) == mask) ? 0xFF : 0;
                                    var xorMask = ((xorMaskBuffer[maskIndex] & mask) == mask) ? 0xFF : 0;

                                    int pos = row * width * 4 + col * 4;
                                    for (int i = 0; i < 3; i++)
                                    {// RGB
                                        shapeBuffer[pos] = (byte)((desktopBuffer[pos] & andMask) ^ xorMask);
                                        pos++;
                                    }
                                    // Alpha
                                    shapeBuffer[pos] = (byte)((desktopBuffer[pos] & 0xFF) ^ 0);

                                    if (mask == 0x01)
                                    {
                                        mask = 0x80;
                                    }
                                    else
                                    {
                                        mask = (byte)(mask >> 1);
                                    }
                                }
                            }


                            Direct2D.Bitmap cursorBits = null;
                            try
                            {
                                fixed (byte* ptr = shapeBuffer)
                                {
                                    var data = new DataPointer(ptr, height * pitch);
                                    var prop = new Direct2D.BitmapProperties(new Direct2D.PixelFormat(Format.B8G8R8A8_UNorm, Direct2D.AlphaMode.Premultiplied));
                                    var size = new Size2(width, height);
                                    cursorBits = new Direct2D.Bitmap(renderTarger, size, data, pitch, prop);
                                };

                                var shapeRect = new RawRectangleF(left, top, right, bottom);

                                renderTarger.DrawBitmap(cursorBits, shapeRect, 1.0f, Direct2D.BitmapInterpolationMode.Linear);
                            }
                            finally
                            {
                                cursorBits?.Dispose();
                            }

                        }
                        finally
                        {
                            immediateContext.UnmapSubresource(desktopRegionTex, 0);
                        }

                    }
                    finally
                    {
                        desktopRegionTex?.Dispose();
                    }
                }
                else if (cursor.ShapeInfo.Type == (int)ShapeType.DXGI_OUTDUPL_POINTER_SHAPE_TYPE_COLOR)
                {
                    logger.Warn("Not supported cursor type " + ShapeType.DXGI_OUTDUPL_POINTER_SHAPE_TYPE_COLOR);
                }
            }

            class CursorInfo
            {
                // public byte[] ShapeBuffer = null;

                public IntPtr PtrShapeBuffer = IntPtr.Zero;
                public int BufferSize = 0;

                public OutputDuplicatePointerShapeInformation ShapeInfo;
                public RawPoint Position = new RawPoint();
                public bool Visible = false;

                public int WhoUpdatedPositionLast = 0;
                public long LastTimeStamp = 0;

                internal GDI.Bitmap GetBitmap(GDI.Bitmap desktop)
                {
                    GDI.Bitmap bmpCursor = null;

                    int x = Position.X;
                    int y = Position.Y;

                    int width = ShapeInfo.Width;
                    int height = ShapeInfo.Height;
                    int pitch = ShapeInfo.Pitch;

                    int givenLeft = Position.X;
                    int givenTop = Position.Y;


                    int screenWidth = desktop.Width;
                    int screenHeigth = desktop.Height;
                    if (x + width > screenWidth)
                    {
                        width = screenWidth - x;
                    }

                    if (y + height > screenHeigth)
                    {
                        height = screenHeigth - y;
                    }

                    var rect = new GDI.Rectangle(x, y, width, height);

                    if (ShapeInfo.Type == (int)ShapeType.DXGI_OUTDUPL_POINTER_SHAPE_TYPE_COLOR)
                    {
                        bmpCursor = new GDI.Bitmap(width, height, pitch, GDI.Imaging.PixelFormat.Format32bppArgb, PtrShapeBuffer);

                    }
                    else if (ShapeInfo.Type == (int)ShapeType.DXGI_OUTDUPL_POINTER_SHAPE_TYPE_MONOCHROME)
                    {
                        height = height / 2;

                        var desktopBuffer = new byte[width * height * 4];

                        {
                            var data = desktop.LockBits(rect, GDI.Imaging.ImageLockMode.ReadOnly, desktop.PixelFormat);
                            var offset = 0;
                            for (var row = 0; row < height; row++)
                            {
                                Marshal.Copy(data.Scan0, desktopBuffer, offset, width * 4);
                                offset += width * 4;
                                data.Scan0 = IntPtr.Add(data.Scan0, data.Stride);
                            }

                            desktop.UnlockBits(data);
                        }

                        var shapeBufferLenght = width * height * 4;
                        var shapeBuffer = new byte[shapeBufferLenght];

                        var maskBufferLenght = width * height / 8;
                        var andMaskBuffer = new byte[maskBufferLenght];
                        Marshal.Copy(PtrShapeBuffer, andMaskBuffer, 0, andMaskBuffer.Length);

                        var xorMaskBuffer = new byte[maskBufferLenght];
                        Marshal.Copy(PtrShapeBuffer + andMaskBuffer.Length, xorMaskBuffer, 0, xorMaskBuffer.Length);

                        for (var row = 0; row < height; ++row)
                        {
                            byte mask = 0x80;

                            for (var col = 0; col < width; ++col)
                            {
                                var maskIndex = row * width / 8 + col / 8;

                                var andMask = ((andMaskBuffer[maskIndex] & mask) == mask) ? 0xFF : 0;
                                var xorMask = ((xorMaskBuffer[maskIndex] & mask) == mask) ? 0xFF : 0;

                                int pos = row * width * 4 + col * 4;
                                for (int i = 0; i < 3; i++)
                                {// RGB
                                    shapeBuffer[pos] = (byte)((desktopBuffer[pos] & andMask) ^ xorMask);
                                    pos++;
                                }
                                // Alpha
                                shapeBuffer[pos] = (byte)((desktopBuffer[pos] & 0xFF) ^ 0);

                                if (mask == 0x01)
                                {
                                    mask = 0x80;
                                }
                                else
                                {
                                    mask = (byte)(mask >> 1);
                                }
                            }
                        }

                        bmpCursor = new GDI.Bitmap(width, height, GDI.Imaging.PixelFormat.Format32bppArgb);
                        {
                            var data = bmpCursor.LockBits(new GDI.Rectangle(0, 0, width, height), GDI.Imaging.ImageLockMode.ReadWrite, bmpCursor.PixelFormat);
                            var offset = 0;
                            for (var row = 0; row < height; row++)
                            {
                                Marshal.Copy(shapeBuffer, offset, data.Scan0, width * 4);
                                offset += width * 4;
                                data.Scan0 = IntPtr.Add(data.Scan0, data.Stride);
                            }
                            bmpCursor.UnlockBits(data);
                        }

                    }
                    else if (ShapeInfo.Type == (int)ShapeType.DXGI_OUTDUPL_POINTER_SHAPE_TYPE_MASKED_COLOR)
                    {
                        //...
                        logger.Warn("Not supported");
                    }

                    return bmpCursor;
                }

                public void Dispose()
                {
                    if (PtrShapeBuffer != null)
                    {
                        Marshal.FreeHGlobal(PtrShapeBuffer);
                        PtrShapeBuffer = IntPtr.Zero;
                    }
                }

            }
            #endregion

            public void Close()
            {
                deviceReady = false;

                if (screenTexture != null && !screenTexture.IsDisposed)
                {
                    screenTexture.Dispose();
                    screenTexture = null;
                }

                if (screenTarget != null && !screenTarget.IsDisposed)
                {
                    screenTarget.Dispose();
                    screenTarget = null;
                }

                if (duplTexture != null && !duplTexture.IsDisposed)
                {
                    duplTexture.Dispose();
                    duplTexture = null;
                }

                if (deskDupl != null && !deskDupl.IsDisposed)
                {
                    deskDupl.Dispose();
                    deskDupl = null;
                }

                if (cursorInfo != null)
                {
                    cursorInfo.Dispose();
                    cursorInfo = null;
                }

            }

        }

    }




}
