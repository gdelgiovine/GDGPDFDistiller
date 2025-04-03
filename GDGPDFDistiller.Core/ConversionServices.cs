using System.Diagnostics;
using System.Text;
using GDGPDFDistiller.Models;
using Microsoft.AspNetCore.Components.Forms;
using static iText.IO.Image.Jpeg2000ImageData;


namespace GDGPDFDistiller.Services
{
    public class ConversionService
    {
        private string _ghostscriptPath = ""; // Assicurati che Ghostscript sia nel PATH
        private string _ghostspclPath = ""; // Assicurati che Ghostscript sia nel PATH
        public string GSArguments { get; set; } = "";
        public string GSCommand { get; set; } = "";
        public string? GSConversionErrors { get; set; } = null;
        public ConversionService(string ghostScriptPath,string ghostPCLPath)
        {

            _ghostscriptPath = ghostScriptPath; //@"C:\\GhostScript\\gs10.04.0\\bin";
            _ghostspclPath = ghostPCLPath; // @"C:\GhostScript\ghostpcl-10.04.0-win64";
            
        }
        public ConversionService()
        {
        }

        public string GhostScriptPath
        {
            get
            {
                return _ghostscriptPath;
            }
            set
            {
                _ghostscriptPath = value;
            }
        }
        public string GhostPCLPath
        {
            get
            {
                return _ghostspclPath;
            }
            set
            {
                _ghostspclPath = value;
            }
        }


      


        public string SetGSArgumentsFromParameters(ConversionParams conversionParams)
        {

            StringBuilder arguments = new StringBuilder();
            arguments.Append($"-dNOPAUSE -dBATCH -sDEVICE=pdfwrite ");

            foreach (var property in conversionParams.GetType().GetProperties())
            {
                if (property.GetValue(conversionParams) != null)
                {
                    if (property.Name == nameof(conversionParams.Resolution))
                    {
                        arguments.Append($"-r{conversionParams.Resolution} ");
                    }
                    
                    else if (property.Name == nameof(conversionParams.PaperSize))
                    {
                        arguments.Append($"-sPAPERSIZE={conversionParams.PaperSize.ToString().ToLower()} ");
                    }
                    else if (property.Name == nameof(conversionParams.ConvertedFileName))
                    { }
                    else if (property.Name == nameof(conversionParams.ConvertedFileDestinationFolder))
                    { }
                    else if (property.Name == nameof(conversionParams.ArchiveSourceFile))
                    { }
                    else if (property.Name == nameof(conversionParams.ArchivedSourceFileDestinationFolder))
                    { }
                    else if (property.Name == nameof(conversionParams.sPCLPageSize))
                    { }
                    else if (property.Name == nameof(conversionParams.dFirstPage))
                    { 
                        if (conversionParams.dFirstPage > 0)
                        {
                            arguments.Append($"-dFirstPage={conversionParams.dFirstPage.ToString().ToLower()} ");
                        }   
                    }
                    else if (property.Name == nameof(conversionParams.dLastPage))
                    { 
                        if (conversionParams.dLastPage > 0)
                        {
                            arguments.Append($"-dLastPage={conversionParams.dLastPage.ToString().ToLower()} ");
                        }
                    }
                    else if (property.Name == nameof(conversionParams.dPDFSettings))
                    { 
                        arguments.Append($"-dPDFSettings=/{conversionParams.dPDFSettings.ToString().ToLower()} ");  
                    }
                    else if (property.Name == nameof(conversionParams.dPDFCompatibilityLevel))
                    {
                        string level = "1.3";  
                        switch (conversionParams.dPDFCompatibilityLevel)
                        {
                            case PDFCompatibilityLevel.V13:
                                level = "1.3";  
                                break;
                            case PDFCompatibilityLevel.V14:
                                level = "1.4";  
                                break;
                            case PDFCompatibilityLevel.V15:
                                level = "1.5";
                                break;
                            case PDFCompatibilityLevel.V16:
                                level = "1.6";
                                break;
                            default:
                                level = "1.3";
                                break;
                        }
                        arguments.Append($"-dPDFCompatibilityLevel={level} ");  
                    }
                    else if (property.Name == nameof(conversionParams.dPDFPermissions))
                    {
                        arguments.Append($"-dPDFPermissions=/{conversionParams.dPDFPermissions} ");
                    }
                    else if (property.Name == nameof(conversionParams.sOwnerPassword))
                    {
                        if (!string.IsNullOrEmpty(conversionParams.sOwnerPassword))
                        {
                            arguments.Append($"-sOwnerPassword={conversionParams.sOwnerPassword} ");
                        }   
                    }
                    else if (property.Name == nameof(conversionParams.sUserPassword))
                    { 
                        if(!string.IsNullOrEmpty(conversionParams.sUserPassword))
                        {
                            arguments.Append($"-sUserPassword={conversionParams.sUserPassword} ");
                        }   
                    }
                    else if (property.Name == nameof(conversionParams.bEncrypt))
                    {
                        if (conversionParams.bEncrypt)
                        {
                            arguments.Append($"-sEncrypt ");
                        }   
                    }
                    else if (property.Name == nameof(conversionParams.dSubsetFonts))
                    { }
                    else if (property.Name == nameof(conversionParams.dEmbedAllFonts))
                    { 
                            arguments.Append($"-dEmbedAllFonts={conversionParams.dEmbedAllFonts.ToString().ToLower()} ");
                    }
                    
                    else if (property.Name == nameof(conversionParams.dColorImageDownsampleType))
                    { }
                    else if (property.Name == nameof(conversionParams.dColorImageResolution))
                    { }
                    else if (property.Name == nameof(conversionParams.dFIXEDMEDIA))
                    { }
                    else if (property.Name == nameof(conversionParams.dDetectDuplicateImages))
                    {
                        arguments.Append($"-dDetectDuplicateImages={conversionParams.dDetectDuplicateImages.ToString().ToLower()} ");   
                    }
                    else if (property.Name == nameof(conversionParams.dPrinted))
                    { }
                    else if (property.Name == nameof(conversionParams.dInterpolateControl))
                    { 
                        arguments.Append($"-dInterpolateControl={conversionParams.dInterpolateControl.ToString().ToLower()} ");
                    }
                }
            }
           
            return arguments.ToString();
        }


        public string ConvertPS(string inputPsFile, string outputPdfFile, ConversionParams parameters)
        {
            if (!_ghostscriptPath.EndsWith(@"\"))
            {
                _ghostscriptPath += @"\";
            }

            var processStartInfo = new ProcessStartInfo
            {
                FileName = $@"{_ghostscriptPath}gswin64c.exe",
                Arguments = SetGSArgumentsFromParameters(parameters) +
                            $"-sOutputFile=\"{outputPdfFile}\" \"{inputPsFile}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            GSArguments = processStartInfo.Arguments;
            GSCommand = processStartInfo.FileName;
            GSConversionErrors = null;
            using var process = new Process { StartInfo = processStartInfo };
            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                
                GSConversionErrors = $"{process.StandardOutput.ReadToEnd()}\n{process.StandardError.ReadToEnd()}";
                //string error = $"GhostScript error: {GSConversionErrors}";
                //throw new Exception(error);

            }

            return outputPdfFile;
        }

        public string ConvertPCL(string inputPCLFile, string outputPdfFile, ConversionParams parameters)
        {
            if (!_ghostspclPath .EndsWith(@"\"))
            {
                _ghostspclPath += @"\";
            }

            var processStartInfo = new ProcessStartInfo
            {
                FileName = $@"{_ghostspclPath}gpcl6win64.exe ",
                Arguments = SetGSArgumentsFromParameters(parameters) +
                            $"-sOutputFile=\"{outputPdfFile}\" \"{inputPCLFile}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            GSArguments = processStartInfo.Arguments;
            GSCommand = processStartInfo.FileName;
            GSConversionErrors = null;
            using var process = new Process { StartInfo = processStartInfo };
            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {

                GSConversionErrors = $"{process.StandardOutput.ReadToEnd()}\n{process.StandardError.ReadToEnd()}";
                //string error = $"GhostScript error: {GSConversionErrors}";
                //throw new Exception(error);

            }

            return outputPdfFile;
        }

        public async Task<string> ConvertPSAsync(string inputPsFile, string outputPdfFile, ConversionParams parameters)
        {
            if (!_ghostscriptPath.EndsWith(@"\"))
            {
                _ghostscriptPath += @"\";
            }

            var processStartInfo = new ProcessStartInfo
            {
                FileName = $@"{_ghostscriptPath}gswin64c.exe",
                Arguments = $"-dNOPAUSE -dBATCH -sDEVICE=pdfwrite " +
                            SetGSArgumentsFromParameters(parameters) +
                            $"-sOutputFile=\"{outputPdfFile}\" \"{inputPsFile}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            GSArguments = processStartInfo.Arguments;
            GSCommand = processStartInfo.FileName;
            GSConversionErrors = null;
            using var process = new Process { StartInfo = processStartInfo };
            process.Start();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                //string error = $"GhostScript error: {await process.StandardError.ReadToEndAsync()}";
                string error = $"GhostScript error: {process.StandardError.ReadToEnd()}";
                throw new Exception(error);
                
            }

            return outputPdfFile;
        }


        public string SetGSPCLParameter(string parameterName, ConversionParams conversionParams, string parameterValuePrefix="/")
        {

            if (conversionParams .GetType().GetProperty(parameterName) == null)
            {
                return "";
            }
            string parameterValue = "";
            parameterValue = conversionParams.GetType().GetProperty(parameterName).GetValue(conversionParams).ToString();
            return $"-{parameterName}={parameterValuePrefix}{parameterValue} ";
        }   
        public async Task<string> ConvertPCLAsync(string inputPclFile, string outputPdfFile, ConversionParams parameters)
        {
            if (!_ghostspclPath.EndsWith(@"\"))
            {
                _ghostspclPath += @"\";
            }
            var processStartInfo = new ProcessStartInfo
            {
                FileName = $@"{_ghostspclPath}gpcl6win64.exe",
                Arguments = $"-dNOPAUSE -dBATCH -sDEVICE=pdfwrite " +
                              SetGSArgumentsFromParameters(parameters) +
                            $"-sOutputFile=\"{outputPdfFile}\" \"{inputPclFile}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processStartInfo };
            process.Start();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                GSConversionErrors = $"{await process.StandardOutput.ReadToEndAsync()}\n{await process.StandardError.ReadToEndAsync()}";
            }

            return outputPdfFile;
        }

    }
   

}