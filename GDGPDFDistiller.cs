using System;

namespace GDGPrinter
{
    static class GDGPrinterModule
    {

        public static void Main()
        {

            var AppConfigManager = new AppConfigManager();
            var Ex = new ExecutionResult();
            var WindowsPrinterManager = new PrinterManager();
            var PreProcessor = new PrintPreProcessor();
            var PrintProcessor = new PrintProcessor();
            AppConfig AppConfig;
            var LogFile = new LogFile();
            var BootLogFile = new LogFile();


            // Dim Impersonation As New GDGPrinter.IdentityImpersonation

            // If IdentityImpersonation.ImpersonateValidUser("gabriele", "", "pantarei") = False Then

            // End
            // End If


            Ex.Reset();

            BootLogFile.LogFileName = @"C:\CONFIG\GDGPrinterConfig.log";
            AppConfigManager.ConfigFileName = @"C:\CONFIG\GDGPrinterConfig.XML";
            // SOSTITUIRE CON PARAMETRO DI REGISTRY

            Ex = AppConfigManager.LoadConfig();
            if (Ex.Failed)
            {
                string msg = "LOADCONFIG ERROR: " + Ex.ResultMessage;
                BootLogFile.WriteLogEntry(LogEntryType.Error, msg);
                // MsgBox(msg)
                Environment.Exit(0);
            }
            else
            {
                string msg = "LOADCONFIG. " + AppConfigManager.ConfigFileName;
                BootLogFile.WriteLogEntry(LogEntryType.Success, msg);
            }

            AppConfig = AppConfigManager.AppConfig;
            LogFile.LogFileName = AppConfig.GeneralSettings.LogPath + AppConfig.GeneralSettings.LogFileName;
            PreProcessor.AppConfig = AppConfig;
            PrintProcessor.AppConfig = AppConfig;
            PrintProcessor.LogFile = LogFile;

            LogFile.WriteLogEntry(LogEntryType.Success, string.Format("PREPROCESS CALL{0}", 0));


            string pdfext = ".PDF";
            string Fase = "0";
            try
            {
                // PP.PreProcess("New Printer")
                PrintRequest PRequest;
                PRequest = PreProcessor.PreProcess();
                LogFile.WriteLogEntry(LogEntryType.Error, string.Format("PREPROCESS CALL {0} {1}", PRequest.PrintOutFile, PRequest.PDL));
                // If PRequest.ExecutionResult.Success = True Then

                switch (PRequest.PDL ?? "")
                {
                    case "PCL":
                        {
                            Fase = "1";
                            PrintProcessor.ConvertPCLtoPDF(AppConfig.GeneralSettings.GhostPCLPath, PRequest.PrintOutFile, PRequest.PrintOutFile + pdfext);
                            break;
                        }

                    case "PDF":
                        {
                            Fase = "2";
                            System.IO.File.Copy(PRequest.PrintOutFile, PRequest.PrintOutFile + pdfext, true);
                            break;
                        }
                    // PrintProcessor.ConvertPCLtoPDF(AppConfig.GeneralSettings.GhostPCLPath, PRequest.PrintOutFile, PRequest.PrintOutFile + pdfext)

                    case "PS":
                        {
                            Fase = "3";
                            PrintProcessor.ConvertPStoPDF(AppConfig.GeneralSettings.GhostPSPath, PRequest.PrintOutFile, PRequest.PrintOutFile + pdfext);
                            break;
                        }

                    default:
                        {
                            Fase = "10";
                            PrintProcessor.ConvertPStoPDF(AppConfig.GeneralSettings.GhostPSPath, PRequest.PrintOutFile, PRequest.PrintOutFile + pdfext);
                            break;
                        }
                }
            }
            // End If

            catch (Exception ex1)
            {
                LogFile.WriteLogEntry(LogEntryType.Error, string.Format("PREPROCESS CALL Fase {0} ERROR {1}", Fase, ex1.Message));
            }
            LogFile.WriteLogEntry(LogEntryType.Success, string.Format("PREPROCESS OUT {0}", 0));


            Environment.Exit(0);

        }




    }
}