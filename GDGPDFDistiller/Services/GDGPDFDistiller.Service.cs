using System.Diagnostics;
using GDGPDFDistiller.Models;


namespace  GDGPDFDistiller.Services
{
    public class ConversionService
    {
        private string _ghostscriptPath = "gs"; // Assicurati che Ghostscript sia nel PATH
        private string _ghostspclPath = "gs"; // Assicurati che Ghostscript sia nel PATH

        public ConversionService() 
        {

            _ghostscriptPath = @"C:\\GhostScript\\gs10.04.0\\bin";
            _ghostspclPath = @"C:\GhostScript\ghostpcl-10.04.0-win64";
        }
        public async Task<string> ConvertPSAsync(string inputPsFile, string outputPdfFile, ConversionParams parameters)
        {
            if (!_ghostscriptPath.EndsWith(@"\"))
            {
                _ghostscriptPath += @"\";
            }

            var processStartInfo = new ProcessStartInfo
            {
                FileName = $@"{_ghostscriptPath}gswin64.exe",
                Arguments = $"-dNOPAUSE -dBATCH -sDEVICE=pdfwrite " +
                            $"-r{parameters.Resolution} " +
                            $"-sPAPERSIZE={parameters.PageSize.ToLower()} " +
                            $"-sOutputFile=\"{outputPdfFile}\" \"{inputPsFile}\"",
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
                throw new Exception($"Ghostscript error: {await process.StandardError.ReadToEndAsync()}");
            }
                
            return outputPdfFile;
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
                            $"-r{parameters.Resolution} " +
                            $"-sPAPERSIZE={parameters.PageSize.ToLower()} " +
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
                throw new Exception($"GhostPCL error: {await process.StandardError.ReadToEndAsync()}");
            }

            return outputPdfFile;
        }

    }

}