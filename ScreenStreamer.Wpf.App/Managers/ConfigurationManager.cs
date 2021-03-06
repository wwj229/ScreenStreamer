﻿using System;
using System.IO;
using Newtonsoft.Json;
using NLog;
using ScreenStreamer.Wpf.Helpers;
using ScreenStreamer.Wpf.Models;

namespace ScreenStreamer.Wpf.Managers
{
    public class ConfigManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static string configName = "Configuration.json";

        public static readonly string CommonAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        public static readonly string ConfigPath = Path.Combine(CommonAppDataPath, "Polywall\\ScreenStreamer.Wpf.App");
      
        // private static string configPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

        private static string configFileFullName = System.IO.Path.Combine(ConfigPath, configName);

        public static AppModel LoadConfigurations()
        {
            logger.Debug("ConfigurationManager::LoadConfigurations()");

            AppModel config = null;
            try
            {
                if (File.Exists(configFileFullName))
                {
                    JsonSerializer serializer = new JsonSerializer
                    {
                        ObjectCreationHandling = ObjectCreationHandling.Replace,
                    };

                    using (StreamReader streamReader = new StreamReader(configFileFullName))
                    {
                        using (JsonReader jsonReader = new JsonTextReader(streamReader))
                        {
                            config = serializer.Deserialize<AppModel>(jsonReader);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            if(config == null)
            {
                logger.Info("Create default configuration...");
                config = AppModel.Default;
            }

            return config;
        }

        public static void Save()
        {
            logger.Debug("ConfigurationManager::Save()");

            try
            {
                if (!Directory.Exists(ConfigPath))
                {
                    Directory.CreateDirectory(ConfigPath);
                }

                var config = ServiceLocator.GetInstance<AppModel>();

				if(config != null)
				{
					JsonSerializer serializer = new JsonSerializer
					{
						Formatting = Formatting.Indented,
					};

					using (StreamWriter streamWriter = new StreamWriter(configFileFullName))
					{
						using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
						{
							serializer.Serialize(jsonWriter, config);
						}
					}
				}         
            }
            catch(Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}