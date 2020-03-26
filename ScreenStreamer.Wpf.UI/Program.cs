﻿using NLog;
using ScreenStreamer.Wpf.Common.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScreenStreamer.Wpf.UI
{
    public static class AppConsts
    {

        public const string ApplicationId = ApplicationName + "_" + "E7B28EAF-A330-4467-98F8-F3BCA7613268";

        public const string ApplicationName = "ScreenStreamer";

    }

    public class Program
    {

        private static Logger logger = null;


        [STAThread]
        public static int Main(string[] args)
        {
            int exitCode = 0;

            logger = LogManager.GetCurrentClassLogger();

            logger.Info("========== START ============");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;


            bool createdNew = false;
            Mutex mutex = null;
            try
            {
                mutex = new Mutex(true, AppConsts.ApplicationId, out createdNew);
                if (!createdNew)
                {
                    logger.Info("Another instance is already running...");
                    //return -1;
                    //...
                }

                if (args != null && args.Length > 0)
                {//...


                }

                var application = new App();
                application.InitializeComponent();

                logger.Info("============ RUN ===============");
                application.Run();

            }
            catch(Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                if (mutex != null)
                {
                    if (createdNew)
                    {
                        mutex.ReleaseMutex();
                    }
                    mutex.Dispose();
                }

                //...
                //ConfigurationManager.Save();

                logger.Info("========== THE END ============");
            }

           
            return exitCode;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = null;

            var obj = e.ExceptionObject;
            if (obj != null)
            {
                ex = obj as Exception;
                logger.Fatal(ex);

            }
            if (ex != null)
            {
                logger.Fatal(ex);
            }
            else
            {
                logger.Fatal("FATAL ERROR!!!");
            }
        }
    }
}