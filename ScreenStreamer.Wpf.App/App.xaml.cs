﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NLog;
using ScreenStreamer.Wpf.Helpers;
using ScreenStreamer.Wpf.Managers;


namespace ScreenStreamer.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
            logger.Debug("OnStartup(...) " + string.Join(" ", e.Args));

            var args = e.Args;

            var config = ConfigManager.LoadConfigurations();
            //TODO: validate...

            if (!config.Validate())
            {
                //...
                // Reset config...

            }


            ServiceLocator.RegisterInstance(config);

           // ServiceLocator.RegisterSingleton(GalaSoft.MvvmLight.Messaging.Messenger.Default); //х.з зачем это...

            var dialogService = new Services.DialogService();
			ServiceLocator.RegisterInstance<Interfaces.IDialogService>(dialogService);


            var mainViewModel = new ViewModels.Dialogs.MainViewModel(config);

            Views.MainWindow mainWindow = new Views.MainWindow(mainViewModel);

            dialogService.Register(mainViewModel, mainWindow);

            dialogService.Show(mainViewModel);


            base.OnStartup(e);

        }

        protected override void OnExit(ExitEventArgs e)
        {

            logger.Debug("OnExit(...) " + e.ApplicationExitCode);

			ConfigManager.Save();

			//ConfigurationManager.Save();
			base.OnExit(e);
        }
    }
}
