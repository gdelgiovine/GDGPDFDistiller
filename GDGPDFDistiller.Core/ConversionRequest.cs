using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace GDGPDFDistiller.Models
{
    [SwaggerSchema(Description = "Richiesta di conversione contenente il file PostScript in Base64 e i parametri di conversione.")]
    public class ConversionRequest
    {
        [Required]
        [SwaggerSchema(Description = "Il file PostScript codificato in Base64.")]
        public string PsFileBase64 { get; set; } = "";

        [Required]
        [SwaggerSchema(Description = "I parametri di conversione.")]
        public ConversionParams Parameters { get; set; } = new ConversionParams();
    }
}
