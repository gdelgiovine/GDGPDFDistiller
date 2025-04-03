using iText.Kernel.Geom;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace GDGPDFDistiller.Models
{
    public enum PDFSettings
    {
        screen,
        ebook,
        printer,
        prepress
    }

    public enum PDFCompatibilityLevel:int
    {
        V13=13,
        V14=14,
        V15=15,
        V16=16,
        V17 = 17

    }

  
    [Flags]
    public enum PDFPermissions
    {
        All=-1,                          // Tutti i permessi    
        None = 0,
        Print = 4,                       // Bit 3 - Stampa a bassa qualità
        Modify = 8,                      // Bit 4 - Modifica il contenuto
        Copy = 16,                       // Bit 5 - Copia testo/immagini
        Annotate = 32,                   // Bit 6 - Aggiungi/modifica annotazioni
        HighQualityPrint = 256,          // Bit 9 - Stampa in alta qualità
        FillInForms = 512,               // Bit 10 - Compila moduli interattivi
        AssembleDocument = 1024,         // Bit 11 - Assemblaggio documenti (estrai pagine)
        Accessibility = 2048             // Bit 12 - Accesso con tecnologie assistive
    }

    /// <summary>
    /// Specifica i metodi di ricampionamento delle immagini a colori utilizzati da Ghostscript.
    /// </summary>
    public enum ColorImageDownsampleType
    {
        /// <summary>
        /// Metodo di ricampionamento che seleziona un singolo pixel rappresentativo per blocco.
        /// </summary>
        Subsample,

        /// <summary>
        /// Metodo di ricampionamento che calcola la media dei colori dei pixel in ciascun blocco.
        /// </summary>
        Average,

        /// <summary>
        /// Metodo di ricampionamento che utilizza l'interpolazione bicubica per determinare i valori dei pixel.
        /// </summary>
        Bicubic
    }

    public enum GraphicsAlphaBits : int
    {
        _1Bit = 1,
        _2Bits = 2,
        _4Bits = 4
    }


public enum PaperSize
    {
        a0,
        a1,
        a2,
        a3,
        a4,
        a5,
        a6,
        a7,
        a8,
        a9,
        a10,
        b0,
        b1,
        b2,
        b3,
        b4,
        b5,
        b6,
        letter,       // 8.5 x 11 inches
        legal,        // 8.5 x 14 inches
        tabloid,      // 11 x 17 inches
        statement,
        executive,
        folio,
        ledger,
        _11x17,
        note,
        postcard,
        doublepostcard,
        monarch,
        commercial10,
        custom // Usato per dimensioni definite da -g o altre opzioni
    }


    //public class ConversionParams
    //{
    //    [Range(72, 2400)]
    //    public int Resolution { get; set; } = 300;
    //    [Required]
    //    public string PageSize { get; set; } = "A4";
    //    public PaperSize PaperSize { get; set; } = PaperSize.a4; 
    //    public string ConvertedFileName { get; set; } = "";
    //    public string ConvertedFileDestinationFolder { get; set; } = "";
    //    public bool ArchiveSourceFile { get; set; } = false;
    //    public string ArchivedSourceFileDestinationFolder { get; set; } = "";
    //    public decimal sPCLPageSize { get; set; } = 0;
    //    public int dFirstPage { get; set; } = 0;
    //    public int dLastPage { get; set; } = 0;
    //    public PDFSettings dPDFSettings { get; set; } = PDFSettings.screen;
    //    public PDFCompatibilityLevel dPDFCompatibilityLevel { get; set; } = PDFCompatibilityLevel.V15;
    //    public PDFPermissions dPDFPermissions { get; set; } = PDFPermissions.All ;
    //    public string? sOwnerPassword { get; set; } = "";
    //    public string? sUserPassword { get; set; } = "";
    //    public Boolean bEncrypt { get; set; } = false;
    //    public Boolean dSubsetFonts { get; set; } = false;
    //    public Boolean dEmbedAllFonts { get; set; } = true;
        
    //    public ColorImageDownsampleType dColorImageDownsampleType { get; set; } =  ColorImageDownsampleType.Bicubic ;
    //    public int dColorImageResolution { get; set; } = 300;
      
    //    public bool dFIXEDMEDIA { get; set; } = false;
    //    public bool dDetectDuplicateImages { get; set; } = true;   
    //    public bool dPrinted { get; set; } = false;
    //    public int dInterpolateControl { get; set; } = 0;
    //    public GraphicsAlphaBits dGraphicsAlphaBits { get; set; } = GraphicsAlphaBits._1Bit ;
    //    public string sPageList { get; set; } = ""; 
    //    public bool dFitPage { get; set; } = false;
    //    public int dGridFitTT { get; set; } = 2;    
    //}



public class ConversionParams
    {
        [Range(72, 2400)]
        public int Resolution { get; set; } = 300;

        [JsonConverter(typeof(StringEnumConverter))]
        public PaperSize PaperSize { get; set; } = PaperSize.a4;

        public string ConvertedFileName { get; set; } = "";

        public string ConvertedFileDestinationFolder { get; set; } = "";

        public bool ArchiveSourceFile { get; set; } = false;

        public string ArchivedSourceFileDestinationFolder { get; set; } = "";

        public decimal sPCLPageSize { get; set; } = 0;

        public int dFirstPage { get; set; } = 1;

        public int dLastPage { get; set; } = 1;

        [JsonConverter(typeof(StringEnumConverter))]
        public PDFSettings dPDFSettings { get; set; } = PDFSettings.printer ;

        [JsonConverter(typeof(StringEnumConverter))]
        public PDFCompatibilityLevel dPDFCompatibilityLevel { get; set; } = PDFCompatibilityLevel.V13;

        [JsonConverter(typeof(StringEnumConverter))]
        public PDFPermissions dPDFPermissions { get; set; } = PDFPermissions.All;

        public string? sOwnerPassword { get; set; }

        public string? sUserPassword { get; set; }

        public bool bEncrypt { get; set; } = false;

        public bool dSubsetFonts { get; set; } = false;

        public bool dEmbedAllFonts { get; set; } = false;

        [JsonConverter(typeof(StringEnumConverter))]
        public ColorImageDownsampleType dColorImageDownsampleType { get; set; } = ColorImageDownsampleType.Bicubic;

        public int dColorImageResolution { get; set; } = 300;

        public bool dFIXEDMEDIA { get; set; } = false;

        public bool dDetectDuplicateImages { get; set; } = false;

        public bool dPrinted { get; set; } = false;

        public int dInterpolateControl { get; set; } = 0;

        [JsonConverter(typeof(StringEnumConverter))]
        public GraphicsAlphaBits dGraphicsAlphaBits { get; set; } = GraphicsAlphaBits._1Bit ;

        public string sPageList { get; set; } = "";

        public bool dFitPage { get; set; } = false;

        public int dGridFitTT { get; set; } = 0;
    }


}