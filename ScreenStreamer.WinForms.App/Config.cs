﻿using MediaToolkit.Core;
using ScreenStreamer.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ScreenStreamer.WinForms.App
{
    public static class AppConsts
    {
        public const string ConfigFileName = "Config.xml";

        public const string ApplicationId = ApplicationName + "_" + "75BF96FE-C169-41C3-A4C6-C9D74846CA34";

        public const string ApplicationName = "ScreenStreamer";

    }

    public class Config
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static readonly string ApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static readonly string CurrentDirectory = Directory.GetCurrentDirectory();// new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).DirectoryName;


        public static readonly string ConfigPath = Path.Combine(ApplicationDataPath, "Polywall\\ScreenStreamer");
        public static readonly string ConfigFullName = Path.Combine(ConfigPath, AppConsts.ConfigFileName);

        public StreamSession Session { get; set; } = new StreamSession();

        [XmlElement(typeof(XmlRect))]
        public Rectangle SelectAreaRectangle { get; set; } = new Rectangle(0, 0, 640, 480);

        public ScreenCaptureProperties ScreenCaptureProperties { get; set; } = new ScreenCaptureProperties();
        public WasapiCaptureProperties WasapiCaptureProps { get; set; } = new WasapiCaptureProperties();

        private static Config data;
        public static Config Data
        {
            get
            {
                if (data == null)
                {
                    data = new Config();
                }
                return data;
            }
        }


        public static bool TempMode { get; private set; }

        public static void Initialize(bool tempMode = false)
        {
            logger.Debug("Config::Initialize()");

            TempMode = tempMode;
            //Validate ...
            //...

            if (!TempMode)
            {
                Config.Read();
            }
            else
            {
                data = Config.Default();
            }


        }

        public static void Read()
        {

            logger.Debug("Config::Read()");

            ValidatePathAndFiles();


            bool success = false;
            try
            {// читаем конфиг
                if (File.Exists(Config.ConfigFullName))
                {
                    bool result = TryReadConfig(Config.ConfigFullName, out data);
                    if (result)
                    {
                        if (data != null)
                        {
                            success = data.Validate();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            if (!success)
            {
                data = Config.Default();
            }
        }

        public void Save()
        {
            logger.Debug("Config::Save()");

            try
            {
                ValidatePathAndFiles();

                Config.Save(Config.ConfigFullName, this);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }


        }

        public static void Shutdown()
        {
            logger.Debug("Config::Shutdown()");

            if (!TempMode)
            {
                Data.Save();
            }

        }


        private static void ValidatePathAndFiles()
        {
            if (!Directory.Exists(ConfigPath))
            {// если папка с конфигом не найдена создаем с правами "FullControl"
                DirectorySecurity security = GetEveryoneDirectorySecurity();

                Directory.CreateDirectory(Config.ConfigPath, security);
            }
        }

        private static DirectorySecurity GetEveryoneDirectorySecurity()
        {
            SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

            DirectorySecurity security = new DirectorySecurity();

            security.AddAccessRule(new FileSystemAccessRule(everyone,
            FileSystemRights.FullControl,
            InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
            PropagationFlags.None,
            AccessControlType.Allow));

            return security;
        }

        private bool Validate()
        { //...
            return true;
        }

        private const int ReadCountMax = 3;
        public static T TryReadConfigPart<T>(string filename) where T : class, new()
        {
            T config = null;
            if (File.Exists(filename) == true)
            {
                int readCount = 0;
                while (readCount < ReadCountMax)
                {
                    try
                    {
                        using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            XmlSerializer xs = new XmlSerializer(typeof(T));
                            config = (T)xs.Deserialize(fs);

                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }

                    readCount++;
                }
            }

            if (config == null)
            {
                config = new T();
            }
            return config;
        }

        public static bool TryReadConfig<T>(string filename, out T config) where T : class, new()
        {
            bool Result = false;
            config = null;
            if (File.Exists(filename))
            {
                int readCount = 0;
                while (readCount < ReadCountMax)
                {
                    try
                    {
                        using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            XmlSerializer xs = new XmlSerializer(typeof(T));
                            config = (T)xs.Deserialize(fs);
                            Result = true;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }

                    readCount++;
                }
            }

            return Result;
        }


        private static void Save(string filename, object conf)
        {
            Debug.Assert(conf != null, "conf != null");
            Debug.Assert(!string.IsNullOrEmpty(filename), "!string.IsNullOrEmpty(filename)");

            if (string.IsNullOrEmpty(filename))
            {
                return;
            }

            string tmp = filename + ".tmp";
            bool success = false;
            try
            {
                SaveXml(tmp, conf);

                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                File.Move(tmp, filename);

                success = true;

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                //if (success)
                {
                    try
                    {
                        if (File.Exists(tmp))
                        {
                            File.Delete(tmp);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
            }

        }
        private static void SaveXml(string filename, object conf)
        {
            using (Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(conf.GetType());
                serializer.Serialize(stream, conf);
            }
        }

        public static Config Default()
        {
            var config = new Config();

            int port = -1;

            var freeTcpPorts = MediaToolkit.Utils.NetworkHelper.GetFreePortRange(System.Net.Sockets.ProtocolType.Tcp, 1, 808);
            if (freeTcpPorts != null && freeTcpPorts.Count() > 0)
            {
                port = freeTcpPorts.FirstOrDefault();
            }

            var session = new StreamSession
            {
                StreamName = Environment.MachineName,
                NetworkIpAddress = "0.0.0.0",
                MutlicastAddress = "239.0.0.1",
                CommunicationPort = port,
                IsMulticast = false,
                TransportMode = TransportMode.Tcp,



            };

            var videoEncoderSettings = new VideoEncoderSettings
            {
                Width = 1920,
                Height = 1080,
                Encoder = VideoEncoderMode.H264,
                Profile = H264Profile.Main,
                BitrateMode = BitrateControlMode.CBR,
                Bitrate = 2500,
                MaxBitrate = 5000,
                FrameRate = 30,
                LowLatency = true,

            };



            var videoSettings = new VideoStreamSettings
            {
                Enabled = true,
                Id = "video_" + Guid.NewGuid().ToString(),
                NetworkSettings = new NetworkSettings(),
                CaptureDevice = null,
                EncoderSettings = videoEncoderSettings,
                StreamFlags = VideoStreamFlags.UseEncoderResoulutionFromSource,

                //ScreenCaptureProperties = captureProperties,

            };

            var audioEncoderSettings = new AudioEncoderSettings
            {
                SampleRate = 8000,
                Channels = 1,
                Encoding = "PCMU",

            };

            var audioSettings = new AudioStreamSettings
            {
                Enabled = false,
                Id = "audio_" + Guid.NewGuid().ToString(),
                NetworkSettings = new NetworkSettings(),
                CaptureDevice = new AudioCaptureDevice(),
                EncoderSettings = audioEncoderSettings,
            };

            var screenCaptureProperties = new ScreenCaptureProperties
            {
                CaptureMouse = true,
                AspectRatio = true,
                CaptureType = VideoCaptureType.DXGIDeskDupl,
                UseHardware = true,
                Fps = 30,
                ShowDebugInfo = false,
            };

            config.SelectAreaRectangle = new Rectangle(0, 0, 640, 480);
            config.ScreenCaptureProperties = screenCaptureProperties;

            session.AudioSettings = audioSettings;
            session.VideoSettings = videoSettings;

            config.Session = session;

            return config;

        }
    }


}
