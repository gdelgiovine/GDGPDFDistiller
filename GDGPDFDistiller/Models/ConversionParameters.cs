using System.ComponentModel.DataAnnotations;

namespace GDGPDFDistiller.Models
{
    public class ConversionParams
    {
        [Range(72, 2400)]
        public int Resolution { get; set; } = 300;
        [Required]
        public string PageSize { get; set; } = "A4";
        public string ConvertedFileName { get; set; } = "";
        public string ConvertedFileDestinationFolder { get; set; } = "";
        public bool ArchiveSourceFile { get; set; } = false;
        public string ArchivedSourceFileDestinationFolder { get; set; } = "";
    }
}