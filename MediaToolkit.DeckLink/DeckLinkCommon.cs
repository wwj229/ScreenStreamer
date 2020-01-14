﻿using DeckLinkAPI;
using NLog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediaToolkit.DeckLink
{
    public class DeckLinkTools
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static bool GetDeviceByIndex(int inputIndex, out IDeckLink deckLink)
        {
            logger.Trace("GetDeviceByIndex(...) " + inputIndex);

            bool Success = false;

            deckLink = null;
            IDeckLinkIterator deckLinkIterator = null;
            try
            {
                deckLinkIterator = new CDeckLinkIterator();

                int index = 0;
                do
                {
                    if (deckLink != null)
                    {
                        Marshal.ReleaseComObject(deckLink);
                        deckLink = null;
                    }

                    deckLinkIterator.Next(out deckLink);
                    if (index == inputIndex)
                    {
                        Success = true;
                        break;
                    }

                    index++;
                }
                while (deckLink != null);

            }
            catch (Exception ex)
            {
                logger.Error(ex);

            }
            finally
            {
                if (deckLinkIterator != null)
                {
                    Marshal.ReleaseComObject(deckLinkIterator);
                    deckLinkIterator = null;
                }

            }

            return Success;
        }

        public static bool GetDeviceByName(string deviceName, out IDeckLink deckLink)
        {
            logger.Trace("GetDeviceByName(...) " + deviceName);

            bool Success = false;

            deckLink = null;
            IDeckLinkIterator deckLinkIterator = null;
            try
            {
                deckLinkIterator = new CDeckLinkIterator();

                int index = 0;
                do
                {
                    if (deckLink != null)
                    {
                        Marshal.ReleaseComObject(deckLink);
                        deckLink = null;
                    }

                    deckLinkIterator.Next(out deckLink);
                    if (deckLink != null)
                    {
                        deckLink.GetDisplayName( out string name);
                        if(deviceName == name)
                        {
                            break;
                        }
                    }
                    index++;
                }
                while (deckLink != null);

            }
            catch (Exception ex)
            {
                logger.Error(ex);

            }
            finally
            {
                if (deckLinkIterator != null)
                {
                    Marshal.ReleaseComObject(deckLinkIterator);
                    deckLinkIterator = null;
                }

            }

            return Success;
        }


        public static string LogDisplayMode(IDeckLinkDisplayMode deckLinkDisplayMode)
        {
            string log = "";

            if (deckLinkDisplayMode != null)
            {
                var displayMode = deckLinkDisplayMode.GetDisplayMode();
                int width = deckLinkDisplayMode.GetWidth();
                int height = deckLinkDisplayMode.GetHeight();
                deckLinkDisplayMode.GetName(out string name);
                var fieldDominance = deckLinkDisplayMode.GetFieldDominance();

                deckLinkDisplayMode.GetFrameRate(out long frameDuration, out long timeScale);
                var fps = ((double)timeScale / frameDuration);
                log = displayMode + " " + width + "x" + height + " " + fps.ToString("0.00") + "fps " + fieldDominance;

            }

            return log;
        }

        public static int GetPixelFormatFourCC(_BMDPixelFormat pixFormat)
        {
            int fourCC = 0;
            if (BMDPixelFormatsToFourCCDict.ContainsKey(pixFormat))
            {
                fourCC = BMDPixelFormatsToFourCCDict[pixFormat];
            }

            return fourCC;
        }

        private static readonly Dictionary<_BMDPixelFormat, int> BMDPixelFormatsToFourCCDict = new Dictionary<_BMDPixelFormat, int>
        {
             //https://docs.microsoft.com/en-us/windows/win32/medfound/video-subtype-guids
             { _BMDPixelFormat.bmdFormat8BitYUV, 0x59565955 /*"UYVY" */},
             { _BMDPixelFormat.bmdFormat8BitBGRA, 0x00000015 /*MFVideoFormat_ARGB32*/},

             //{ _BMDPixelFormat.bmdFormat10BitYUV, 0x30313256 /*"v210"*/ }, //не поддерживается EVR, VideoProcessorMFT конвертит только софтверно!
             // остальные форматы не проверялись....
        };


        public static List<DeckLinkDeviceDescription> GetDeckLinkInputDevices()
        {
            List<DeckLinkDeviceDescription> devices = new List<DeckLinkDeviceDescription>();
            IDeckLinkIterator deckLinkIterator = null;
            try
            {
                deckLinkIterator = new CDeckLinkIterator();

                int index = 0;
                IDeckLink deckLink = null;
                do
                {
                    if (deckLink != null)
                    {
                        Marshal.ReleaseComObject(deckLink);
                        deckLink = null;
                    }

                    deckLinkIterator.Next(out deckLink);

                    if (deckLink == null)
                    {
                        break;
                    }

                    deckLink.GetDisplayName(out string deviceName);

                    try
                    {
                        var deckLinkInput = (IDeckLinkInput)deckLink;
                        var deckLinkStatus = (IDeckLinkStatus)deckLink;

                        bool available = false;
                        deckLinkStatus.GetFlag(_BMDDeckLinkStatusID.bmdDeckLinkStatusVideoInputSignalLocked, out int videoInputSignalLockedFlag);
                        available = (videoInputSignalLockedFlag != 0);

                        var displayModeIds = GetDisplayDescriptions(deckLinkInput);

                        DeckLinkDeviceDescription deviceDescription = new DeckLinkDeviceDescription
                        {
                            DeviceIndex = index,
                            DeviceName = deviceName,
                            Available = available,
                            DisplayModeIds = displayModeIds,
                        };

                        devices.Add(deviceDescription);


                        //Marshal.ReleaseComObject(deckLinkInput);
                        //Marshal.ReleaseComObject(deckLinkStatus);

                    }
                    catch (InvalidCastException)
                    {

                    }

                    index++;

                }
                while (deckLink != null);

            }
            catch (Exception ex)
            {
                if (deckLinkIterator == null)
                {
                    throw new Exception("This application requires the DeckLink drivers installed.\n" +
                        "Please install the Blackmagic DeckLink drivers to use the features of this application");
                }

                throw;
            }

            return devices;
        }


        public static List<DeckLinkDisplayModeDescription> GetDisplayDescriptions(IDeckLinkInput deckLinkInput,
            _BMDVideoConnection connection = _BMDVideoConnection.bmdVideoConnectionHDMI,
            _BMDSupportedVideoModeFlags videoModeFlags = _BMDSupportedVideoModeFlags.bmdSupportedVideoModeDefault)
        {

            Dictionary<_BMDPixelFormat, string> pixelFormats = new Dictionary<_BMDPixelFormat, string>
            {
                { _BMDPixelFormat.bmdFormat8BitYUV, "UYVY"},
                { _BMDPixelFormat.bmdFormat8BitBGRA, "RGBA32"}
            };

            List<DeckLinkDisplayModeDescription> displayDescriptions = new List<DeckLinkDisplayModeDescription>();

            IDeckLinkDisplayModeIterator iterator = null;
            try
            {
                deckLinkInput.GetDisplayModeIterator(out iterator);

                while (true)
                {
                    IDeckLinkDisplayMode displayMode = null;
                    try
                    {
                        iterator.Next(out displayMode);
                        if (displayMode == null)
                        {
                            break;
                        }

                        var displayModeId = displayMode.GetDisplayMode();

                        displayMode.GetName(out string displayName);
                        displayMode.GetFrameRate(out long frameDuration, out long timeScale);
                        var fps = (double)timeScale / frameDuration;

                        int width = displayMode.GetWidth();
                        int height = displayMode.GetHeight();
                        var resolution = width + "x" + height;

                        var displayModeFlags = displayMode.GetFlags();
                        var fieldDominance = displayMode.GetFieldDominance();
  
                        // var log = string.Join(", " , displayName, resolution, bdmDisplayMode, displayModeFlags, frameDuration, timeScale, fieldDominance);

                        foreach (var pixFmt in pixelFormats.Keys)
                        {
                            deckLinkInput.DoesSupportVideoMode(connection, displayModeId, pixFmt, videoModeFlags, out int supported);
                            if (supported != 0)
                            {
                                displayDescriptions.Add(new DeckLinkDisplayModeDescription
                                {
                                    ModeId = (long)displayModeId,
                                    Width = width,
                                    Height = height,
                                    Fps = fps,
                                    PixFmt = (long)pixFmt,
                                    Description = displayName + " (" + resolution + " " + fps.ToString("0.##") + " fps " + pixelFormats[pixFmt] + ")",
                                });
                            }
                            else
                            {
                                //Console.WriteLine("Display mode not supported: "+ displayModeId + " " + pixFmt);
                            }

                        }

                    }
                    finally
                    {
                        if (displayMode != null)
                        {
                            Marshal.ReleaseComObject(displayMode);
                            displayMode = null;
                        }
                    }

                }

            }
            finally
            {
                if (iterator != null)
                {
                    Marshal.ReleaseComObject(iterator);
                    iterator = null;
                }
            }

            return displayDescriptions;

        }
    }



    class MemoryAllocator : IDeckLinkMemoryAllocator
    {
        private volatile int count = 0;
        private IntPtr allocatedBuffer = IntPtr.Zero;
        private uint bufferSize = 0;
     
        public void AllocateBuffer(uint size, out IntPtr buffer)
        {// тестовый буффер на один кадр

            //if(count> 10)
            //{
            //    buffer = IntPtr.Zero;
            //    return;
            //}

            if (allocatedBuffer == IntPtr.Zero)
            {
                allocatedBuffer = Marshal.AllocHGlobal((int)size);
                bufferSize = size;
            }

            if (bufferSize < size)
            {
                if (allocatedBuffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(allocatedBuffer);
                    allocatedBuffer = IntPtr.Zero;
                }

                allocatedBuffer = Marshal.AllocHGlobal((int)size);
                bufferSize = size;
            }

            buffer = allocatedBuffer;//Marshal.AllocHGlobal((int)bufferSize);
          
            //count++;

            //Console.WriteLine("AllocateBuffer(...) " + size + " "+ buffer + " "+ count);
        }

        public void Commit()
        {

            Console.WriteLine("Commit()");

        }

        public void Decommit()
        {
            Console.WriteLine("Decommit()");
            Dispose();
        }

        public void ReleaseBuffer(IntPtr buffer)
        {

            //count--;
            //Console.WriteLine("ReleaseBuffer(...) " + buffer + " " + count);

            //if (buffer != IntPtr.Zero)
            //{
            //    Marshal.FreeHGlobal(buffer);
            //}

        }

        public void Dispose()
        {
            if (allocatedBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(allocatedBuffer);
                allocatedBuffer = IntPtr.Zero;
            }
        }
    }


    public class DeckLinkDeviceDescription
    {
        public int DeviceIndex { get; set; } = -1;
        public string DeviceName { get; set; } = "";
        public bool Available { get; set; } = false;

        public List<DeckLinkDisplayModeDescription> DisplayModeIds { get; set; } = null;

        public override string ToString()
        {
            return DeviceName + " " + (Available ? "(Available)" : "(Not Available)");
        }
    }

    public class DeckLinkDisplayModeDescription
    {
        public long ModeId { get; set; } = (long)_BMDDisplayMode.bmdModeUnknown;

        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;
        public double Fps { get; set; } = 0;
        public long PixFmt { get; set; } = (long)_BMDPixelFormat.bmdFormatUnspecified;

        public string Description { get; set; } = "";


    }


}
