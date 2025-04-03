using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using GDGPDFDistiller.Services;
using GDGPDFDistiller.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations; // Per IFormFile

namespace GDGPDFDistiller.Controllers;
/// <summary>
/// Controller per la gestione delle operazioni sui PDF.
/// </summary>
[ApiController]
[Route("api/conversion")]
public class ConversionController : ControllerBase
{
    private readonly ConversionService _ConversionService;
    private readonly IConfiguration _configuration;

    public ConversionController(ConversionService conversionService, IConfiguration configuration)
    {
        _ConversionService = conversionService;
        _configuration = configuration;
    }
    private GhostscriptSettings GetGhostscriptSettings()
    {
        var settings = new GhostscriptSettings();
        _configuration.GetSection("GhostscriptSettings").Bind(settings);
        return settings;
    }
    [HttpPost("ps-json")]
    [SwaggerOperation(Summary = "Convert PS to PDF", Description = "Converts a PostScript file to a PDF file.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the converted PDF file.", typeof(FileResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input parameters.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error converting file.")]

    public async Task<IActionResult> ConvertPsJson([FromBody] ConversionRequest request)
    {
        if (string.IsNullOrEmpty(request.PsFileBase64) || request.Parameters == null)
        {
            return BadRequest("Both a PostScript file and parameters are required.");
        }

        // Decodifica il file da Base64
        byte[] sourceFileBytes;
        try
        {
            sourceFileBytes = Convert.FromBase64String(request.PsFileBase64);
        }
        catch (FormatException)
        {
            return BadRequest("Invalid Base64 encoding for the PS file.");
        }

        var tempPath = Path.GetTempPath();
        string GUID = $"{Guid.NewGuid()}";
        var sourceFilePath = Path.Combine(tempPath, $"{GUID}.ps");
        var outputPdfPath = Path.Combine(tempPath, $"{GUID}.pdf");

        //string gs = @"C:\\GhostScript\\gs10.04.0\\bin";
        //string gspcl = @"C:\GhostScript\ghostpcl-10.04.0-win64";
        var ghostscriptSettings = GetGhostscriptSettings();
        ConversionService cs = new GDGPDFDistiller.Services.ConversionService(ghostscriptSettings.GhostscriptPath ,ghostscriptSettings .GhostPCLPath );

        await System.IO.File.WriteAllBytesAsync(sourceFilePath, sourceFileBytes);


        try
        {
            var pdfPath = await cs.ConvertPSAsync(sourceFilePath, outputPdfPath, request.Parameters);
            var pdfBytes = await System.IO.File.ReadAllBytesAsync(pdfPath);
            string convertedFileName= GUID;  
            if (request.Parameters.ConvertedFileName != null)
            {
                convertedFileName = request.Parameters.ConvertedFileName;
            }
            return File(pdfBytes, "application/pdf", convertedFileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error converting file: {ex.Message}");
        }
        finally
        {
            System.IO.File.Delete(sourceFilePath);
        }
    }
    [HttpPost("pcl-json")]
    [SwaggerOperation(Summary = "Convert PS to PDF", Description = "Converts a PostScript file to a PDF file.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the converted PDF file.", typeof(FileResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input parameters.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error converting file.")]

    public async Task<IActionResult> ConvertPclJson([FromBody] ConversionRequest request)
    {
        if (string.IsNullOrEmpty(request.PsFileBase64) || request.Parameters == null)
        {
            return BadRequest("Both a PCL file and parameters are required.");
        }

        // Decodifica il file da Base64
        byte[] sourceFileBytes;
        try
        {
            sourceFileBytes = Convert.FromBase64String(request.PsFileBase64);
        }
        catch (FormatException)
        {
            return BadRequest("Invalid Base64 encoding for the PCL file.");
        }

        var tempPath = Path.GetTempPath();
        string GUID = $"{Guid.NewGuid()}";
        var sourceFilePath = Path.Combine(tempPath, $"{GUID}.pcl");
        var outputPdfPath = Path.Combine(tempPath, $"{GUID}.pdf");
        var ghostscriptSettings = GetGhostscriptSettings();
        ConversionService cs = new GDGPDFDistiller.Services.ConversionService(ghostscriptSettings.GhostscriptPath, ghostscriptSettings.GhostPCLPath);

        await System.IO.File.WriteAllBytesAsync(sourceFilePath, sourceFileBytes);


        try
        {
            var pdfPath = await cs.ConvertPSAsync(sourceFilePath, outputPdfPath, request.Parameters);
            var pdfBytes = await System.IO.File.ReadAllBytesAsync(pdfPath);
            string convertedFileName = GUID;
            if (request.Parameters.ConvertedFileName != null)
            {
                convertedFileName = request.Parameters.ConvertedFileName;
            }
            return File(pdfBytes, "application/pdf", convertedFileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error converting file: {ex.Message}");
        }
        finally
        {
            System.IO.File.Delete(sourceFilePath);
        }
    }

    [HttpPost("ps")]
      public async Task<IActionResult> ConvertPs(IFormFile psFile, IFormFile jsonParams)
      {
        if (psFile == null || jsonParams == null)
        {
            return BadRequest("Both a PostScript file and JSON parameters are required.");
        }
        var ghostscriptSettings = GetGhostscriptSettings();
        ConversionService cs = new GDGPDFDistiller.Services.ConversionService(ghostscriptSettings.GhostscriptPath, ghostscriptSettings.GhostPCLPath);

      
        var tempPath = Path.GetTempPath();
        var psFilePath = Path.Combine(tempPath, $"{Guid.NewGuid()}.ps");
        var jsonFilePath = Path.Combine(tempPath, $"{Guid.NewGuid()}.json");
        var outputPdfPath = Path.Combine(tempPath, $"{Guid.NewGuid()}.pdf");

        await using (var psStream = new FileStream(psFilePath, FileMode.Create))
        {
            await psFile.CopyToAsync(psStream);
        }

        await using (var jsonStream = new FileStream(jsonFilePath, FileMode.Create))
        {
            await jsonParams.CopyToAsync(jsonStream);
        }

        var jsonContent = await System.IO.File.ReadAllTextAsync(jsonFilePath);
        var parameters = JsonConvert.DeserializeObject<ConversionParams>(jsonContent) ?? new ConversionParams();

        try
        {
            var pdfPath = await cs.ConvertPSAsync(psFilePath, outputPdfPath, parameters);
            var pdfBytes = await System.IO.File.ReadAllBytesAsync(pdfPath);
            return File(pdfBytes, "application/pdf", "converted.pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error converting file: {ex.Message}");
        }
        finally
        {
            System.IO.File.Delete(psFilePath);
            System.IO.File.Delete(jsonFilePath);
        }
    }

    [HttpPost("pcl")]
    public async Task<IActionResult> ConvertPcl( IFormFile pclFile, IFormFile jsonParams)
    {
        if (pclFile == null || jsonParams == null)
        {
            return BadRequest("Both a PCL file and JSON parameters are required.");
        }

        var tempPath = Path.GetTempPath();
        var pclFilePath = Path.Combine(tempPath, $"{Guid.NewGuid()}.pcl");
        var jsonFilePath = Path.Combine(tempPath, $"{Guid.NewGuid()}.json");
        var outputPdfPath = Path.Combine(tempPath, $"{Guid.NewGuid()}.pdf");
        var ghostscriptSettings = GetGhostscriptSettings();
        ConversionService cs = new GDGPDFDistiller.Services.ConversionService(ghostscriptSettings.GhostscriptPath, ghostscriptSettings.GhostPCLPath);


        await using (var psStream = new FileStream(pclFilePath, FileMode.Create))
        {
            await pclFile.CopyToAsync(psStream);
        }

        await using (var jsonStream = new FileStream(jsonFilePath, FileMode.Create))
        {
            await jsonParams.CopyToAsync(jsonStream);
        }

        var jsonContent = await System.IO.File.ReadAllTextAsync(jsonFilePath);
        var parameters = JsonConvert.DeserializeObject<ConversionParams>(jsonContent) ?? new ConversionParams();

        try
        {
            var pdfPath = await cs.ConvertPCLAsync(pclFilePath, outputPdfPath, parameters);
            var pdfBytes = await System.IO.File.ReadAllBytesAsync(pdfPath);
            return File(pdfBytes, "application/pdf", "converted.pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error converting file: {ex.Message}");
        }
        finally
        {
            System.IO.File.Delete(pclFilePath);
            System.IO.File.Delete(jsonFilePath);
            
        }
    }

    [HttpGet("default-params")]
    [SwaggerOperation(Summary = "Get Default Conversion Parameters", Description = "Returns a default instance of ConversionParams.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the default conversion parameters.", typeof(ConversionParams))]
    public IActionResult GetDefaultConversionParams()
    {
        var defaultParams = new ConversionParams
        {
            //Resolution = 300,
            //PageSize = "A4",
            //PaperSize = PaperSize.a4 ,
            //ConvertedFileName = "converted.pdf",
            //ConvertedFileDestinationFolder = "/default/path",
            //ArchiveSourceFile = false,
            //ArchivedSourceFileDestinationFolder = "/archive/path",
            //sPCLPageSize = 0,
            //dFirstPage = 1,
            //dLastPage = 1,
            //dPDFSettings = PDFSettings.printer,
            //dPDFCompatibilityLevel = PDFCompatibilityLevel.V16,
            //dPDFPermissions = PDFPermissions.All ,
            //sOwnerPassword = null,
            //sUserPassword = null,
            //bEncrypt = false,
            //dSubsetFonts = false,
            //dEmbedAllFonts = false,
            //dColorImageDownsampleType = ColorImageDownsampleType.Bicubic ,
            //dColorImageResolution = 300,
            //dFIXEDMEDIA = false,
            //dDetectDuplicateImages = false,
            //dPrinted = false,
            //dInterpolateControl = 0,
            //dGraphicsAlphaBits = GraphicsAlphaBits._1Bit,
            //sPageList = "",
            //dFitPage = false,
            //dGridFitTT = 0
        };

        var json = JsonConvert.SerializeObject(defaultParams, new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new Newtonsoft.Json.Converters.StringEnumConverter() },
            Formatting = Formatting.Indented
        });

        return Ok(json);
    }

}
