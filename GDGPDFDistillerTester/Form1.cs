using GDGPDFDistiller.Services;
using iText.Kernel.Geom;

namespace GDGPDFDistillerTester
{
    public partial class Form1 : Form
    {
        GDGPDFDistiller.Models.ConversionParams conversionParams = new GDGPDFDistiller.Models.ConversionParams();
        GDGPDFDistiller.Services.ConversionService conversionService;
        public Form1()
        {
            InitializeComponent();
            this.propertyGrid1 .SelectedObject= conversionParams;
           

            string gs = @"C:\\GhostScript\\gs10.04.0\\bin";
            string gspcl = @"C:\GhostScript\ghostpcl-10.04.0-win64";
            conversionService = new GDGPDFDistiller.Services.ConversionService(gs, gspcl);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string filePath = @"C:\TEST\FASTREPORT.HTML";
            string html = File.ReadAllText(filePath);
            var pagine = ConvertHTMLToPDF.SplitHTMLPages(html);


            PageSize pageSize = ConvertHTMLToPDF.GetPageSizeFromHtmlPageInfo(pagine[0]);

            pageSize = PageSizeHelper.GetPageSizeEnumForDimensions(ConvertHTMLToPDF.PixelsToPoints(pagine[0].Width), ConvertHTMLToPDF.PixelsToPoints(pagine[0].Height));


            Byte[] b = ConvertHTMLToPDF.GeneratePdfFromHtml(pagine[1].Html, pageSize);
            Byte[] b1 = ConvertHTMLToPDF.GeneratePdfFromHtml(pagine[1].Html, iText.Kernel.Geom.PageSize.A4);

            File.WriteAllBytes(@"C:\TEST\FASTREPORT.PDF", b);
            File.WriteAllBytes(@"C:\TEST\FASTREPORTA4.PDF", b1);
        }

        private  void button2_Click(object sender, EventArgs e)
        {

          
            conversionService.ConvertPS(@"C:\TEST\TEST.PS", @"C:\TEST\TESTPS.PDF", conversionParams);

            this.propertyGrid2.SelectedObject = conversionService;
            if (conversionService.GSConversionErrors != null)
            {
                MessageBox.Show(conversionService.GSConversionErrors);
            }

        }

        private   void button3_Click(object sender, EventArgs e)
        {
          
        
           conversionService.ConvertPCL(@"C:\TEST\TEST.PCL", @"C:\TEST\TESTPCL.PDF", conversionParams);
            this.propertyGrid2.SelectedObject = conversionService;
            if (conversionService.GSConversionErrors != null)
            {
                MessageBox.Show(conversionService.GSConversionErrors);
            }
        }

        
    }
}
