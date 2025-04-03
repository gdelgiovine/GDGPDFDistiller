using iText.Html2pdf.Resolver.Font;
using iText.Kernel.Pdf;
using iText.Html2pdf;
using iText.Layout.Font;
using iText.Layout;
using iText.Html2pdf.Attach.Impl.Layout;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using iText.Kernel.Geom;


public class CustomPageTagWorker : iText.Html2pdf.Attach.Impl.Tags.DivTagWorker
{
    public const int PageDivProperty = -10;
    public CustomPageTagWorker(iText.StyledXmlParser.Node.IElementNode element, iText.Html2pdf.Attach.ProcessorContext context) : base(element, context)
    {
    }

    public override void ProcessEnd(iText.StyledXmlParser.Node.IElementNode element, iText.Html2pdf.Attach.ProcessorContext context)
    {
        base.ProcessEnd(element, context);
        iText.Layout.IPropertyContainer elementResult = GetElementResult();
        if (elementResult != null &&
            !String.IsNullOrEmpty(element.GetAttribute(iText.Html2pdf.Html.AttributeConstants.CLASS)) &&
            element.GetAttribute(iText.Html2pdf.Html.AttributeConstants.CLASS).StartsWith("frpage"))
        {
            elementResult.SetProperty(PageDivProperty, element.GetAttribute(iText.Html2pdf.Html.AttributeConstants.CLASS));
        }
    }
}
public class CustomTagWorkerFactory : iText.Html2pdf.Attach.Impl.DefaultTagWorkerFactory
{
    public override iText.Html2pdf.Attach.ITagWorker GetCustomTagWorker(iText.StyledXmlParser.Node.IElementNode tag, iText.Html2pdf.Attach.ProcessorContext context)
    {
        if (iText.Html2pdf.Html.TagConstants.DIV.Equals(tag.Name().ToLower()))
        {
            return new CustomPageTagWorker(tag, context);
        }
        return base.GetCustomTagWorker(tag, context);
    }
}

public class HtmlPageInfo
{
    public string Html { get; set; } = string.Empty;    
    public float Width { get; set; }
    public float Height { get; set; }

    public float MarginTop { get; set; }
    public float MarginRight { get; set; }
    public float MarginBottom { get; set; }
    public float MarginLeft { get; set; }

    public PageOrientation Orientation => Width >= Height ? PageOrientation.Landscape : PageOrientation.Portrait;
}
public enum PageOrientation
{
    Portrait,
    Landscape
}


public class PageSizeHelper
{
    public static PageSize? GetPageSizeEnumForDimensions(float width, float height)
    {
        var pageSizes = new Dictionary<PageSizeEnum, PageSize>
        {
            { PageSizeEnum.A0, PageSize.A0 },
            { PageSizeEnum.A1, PageSize.A1 },
            { PageSizeEnum.A2, PageSize.A2 },
            { PageSizeEnum.A3, PageSize.A3 },
            { PageSizeEnum.A4, PageSize.A4 },
            { PageSizeEnum.A5, PageSize.A5 },
            { PageSizeEnum.A6, PageSize.A6 },
            { PageSizeEnum.A7, PageSize.A7 },
            { PageSizeEnum.A8, PageSize.A8 },
            { PageSizeEnum.A9, PageSize.A9 },
            { PageSizeEnum.A10, PageSize.A10 },
            { PageSizeEnum.LETTER, PageSize.LETTER },
            { PageSizeEnum.LEGAL, PageSize.LEGAL },
            { PageSizeEnum.TABLOID, PageSize.TABLOID },
            { PageSizeEnum.EXECUTIVE, PageSize.EXECUTIVE  },
            { PageSizeEnum.LEDGER , PageSize.LEDGER  },
            { PageSizeEnum.DEFAULT, PageSize.DEFAULT  }
        };

        // Ordina le dimensioni della pagina in base all'area (larghezza * altezza) in ordine crescente
        var sortedPageSizes = pageSizes.OrderBy(ps => ps.Value.GetWidth() * ps.Value.GetHeight());

        foreach (var pageSize in sortedPageSizes)
        {
            if ((pageSize.Value.GetWidth() >= width && pageSize.Value.GetHeight() >= height) ||
                (pageSize.Value.GetWidth() >= height && pageSize.Value.GetHeight() >= width))
            {
                return pageSize.Value ;
            }
        }

        return PageSize .DEFAULT ; // Nessuna dimensione predefinita può contenere il documento
    }
}

public enum PageSizeEnum
{
    A0,
    A1,
    A2,
    A3,
    A4,
    A5,
    A6,
    A7,
    A8,
    A9,
    A10,
    LETTER,
    LEGAL,
    TABLOID,
    EXECUTIVE,
    FOLIO,
    LEDGER,
    DEFAULT 
}

public class ConvertHTMLToPDF
{
    //public int PageDivProperty  = -10;
    public static float PixelsToPoints(float pixels, int ppi = 96)
    {
        return pixels / ppi * 72;
    }
    public static PageSize GetPageSizeFromHtmlPageInfo(HtmlPageInfo pageInfo,int ppi=96 )
    {
        
        // Calcola la larghezza e l'altezza effettive della pagina sottraendo i margini
        float width = PixelsToPoints(pageInfo.Width,ppi) - PixelsToPoints(pageInfo.MarginLeft,ppi) - PixelsToPoints(pageInfo.MarginRight,ppi);
        float height = PixelsToPoints(pageInfo.Height, ppi) - PixelsToPoints(pageInfo.MarginTop,ppi) - PixelsToPoints(pageInfo.MarginBottom,ppi);

        // Crea un oggetto PageSize con le dimensioni calcolate
        PageSize pageSize = new PageSize(width, height);

        return pageSize;
    }
    public static List<HtmlPageInfo> SplitHTMLPages(string htmlContent, string selectNodesCondition = "//div[contains(@class, 'page')]")
    {
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(htmlContent);

        var pages = new List<HtmlPageInfo>();

        // Trova i div con class="page"
        var pageDivs = doc.DocumentNode.SelectNodes(selectNodesCondition);

        if (pageDivs != null && pageDivs.Count > 0)
        {
            // Estrai gli elementi di style o CSS dalla prima pagina
            var firstPage = pageDivs[0];
            var styleNodes = firstPage.SelectNodes("//style");
            var cssLinks = firstPage.SelectNodes("//link[@rel='stylesheet']");

            foreach (var div in pageDivs)
            {
                var pageHtml = div.OuterHtml;

                // Aggiungi gli elementi di style o CSS alla pagina corrente se non è la prima
                if (div != firstPage)
                {
                    if (styleNodes != null)
                    {
                        foreach (var styleNode in styleNodes)
                        {
                            pageHtml = styleNode.OuterHtml + pageHtml;
                        }
                    }

                    if (cssLinks != null)
                    {
                        foreach (var cssLink in cssLinks)
                        {
                            pageHtml = cssLink.OuterHtml + pageHtml;
                        }
                    }
                }

                var pageInfo = GetHtmlPageInfo(pageHtml);
                pages.Add(pageInfo);
            }
        }

        return pages;
    }


    public static HtmlPageInfo GetHtmlPageInfo(string htmlContent, string selectSingleNodesCondition = "//div[contains(@class, 'page')]")
    {
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(htmlContent);

        var pageDiv = doc.DocumentNode.SelectSingleNode(selectSingleNodesCondition);
        if (pageDiv == null)
            throw new Exception("Nessuna pagina trovata.");

        var style = pageDiv.GetAttributeValue("style", "");

        float GetValue(string name)
        {
            var match = Regex.Match(style, $@"{name}\s*:\s*(\d+(\.\d+)?)px", RegexOptions.IgnoreCase);
            return match.Success ? float.Parse(match.Groups[1].Value) : 0;
        }


        var info = new HtmlPageInfo
        {
            Html = htmlContent,
            Width = GetValue("width")/10,
            Height = GetValue("height")/100,
            MarginTop = GetValue("margin-top"),
            MarginRight = GetValue("margin-right"),
            MarginBottom = GetValue("margin-bottom"),
            MarginLeft = GetValue("margin-left"),
        };

        // Fallback per margini shorthand (es. margin: 10px 20px 10px 20px;)
        var marginMatch = Regex.Match(style, @"margin\s*:\s*(\d+(\.\d+)?)px\s+(\d+(\.\d+)?)px\s+(\d+(\.\d+)?)px\s+(\d+(\.\d+)?)px", RegexOptions.IgnoreCase);
        if (marginMatch.Success)
        {
            info.MarginTop = (int)Math.Ceiling(float.Parse(marginMatch.Groups[1].Value));
            info.MarginRight = (int)Math.Ceiling(float.Parse(marginMatch.Groups[3].Value));
            info.MarginBottom = (int)Math.Ceiling(float.Parse(marginMatch.Groups[5].Value));
            info.MarginLeft = (int)Math.Ceiling(float.Parse(marginMatch.Groups[7].Value));
        }

        return info;
    }
    public static byte[] GeneratePdfFromHtml(string reportHtml, iText.Kernel.Geom.PageSize pageSize, int PageDivProperty=-10)
    {

        if (string.IsNullOrEmpty(reportHtml))
        {
            return null;
        }


        //using (var workStream = new MemoryStream())
        //{
        //    using (var pdfWriter = new iText.Kernel.Pdf.PdfWriter(workStream))
        //    {
        //        FontProvider fontProvider = new DefaultFontProvider(true, true, true);
        //        var converterProperties = new ConverterProperties();
        //        converterProperties.SetTagWorkerFactory(new CustomTagWorkerFactory());
        //        converterProperties.SetFontProvider(fontProvider);
        //        var pdfDocument = new PdfDocument(pdfWriter);
        //        pdfDocument.SetDefaultPageSize(Utils.GetPageSizeFromMilimeters(pages[0].PaperWidth, pages[0].PaperHeight));
        //        var elements = HtmlConverter.ConvertToElements(reportHtml, converterProperties);
        //        var document = new Document(pdfDocument);
        //        document.SetMargins(pages[0].TopMargin, pages[0].RightMargin, pages[0].BottomMargin, pages[0].LeftMargin);
        //        int elementIndex = 0;
        //        int pageIndex = 0;
        //        foreach (IElement element in elements)
        //        {
        //            if (element.HasProperty(Utils.PageDivProperty) && elementIndex > 0)
        //            {
        //                pageIndex++;
        //                pdfDocument.SetDefaultPageSize(Utils.GetPageSizeFromMilimeters(pages[pageIndex].PaperWidth, pages[pageIndex].PaperHeight));
        //                document.SetMargins(pages[pageIndex].TopMargin, pages[pageIndex].RightMargin, pages[pageIndex].BottomMargin, pages[pageIndex].LeftMargin);
        //                document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
        //            }
        //            document.Add((IBlockElement)element);
        //            elementIndex++;
        //        }
        //        document.Close();
        //        return workStream.ToArray();
        //    }
        //}

        using (var workStream = new MemoryStream())

        {
            using (var pdfWriter = new iText.Kernel.Pdf.PdfWriter(workStream))
            {
                //iText.Layout.Font.FontProvider fontProvider = new iText.Html2pdf.Resolver.Font.DefaultFontProvider(true, true, true);

                iText.Layout.Font.FontProvider fontProvider = new DefaultFontProvider(true, true, false);
                var converterProperties = new iText.Html2pdf.ConverterProperties();
                converterProperties.SetTagWorkerFactory(new CustomTagWorkerFactory());
                converterProperties.SetFontProvider(fontProvider);

                iText.Kernel.Pdf.PdfDocument pdfDocument = new iText.Kernel.Pdf.PdfDocument(pdfWriter);
                pdfDocument.SetDefaultPageSize(pageSize);

                var elements = iText.Html2pdf.HtmlConverter.ConvertToElements(reportHtml, converterProperties);
                var document = new iText.Layout.Document(pdfDocument, pageSize);
                document.SetMargins(0, 0, 0, 0);
                int elementIndex = 0;
                foreach (iText.Layout.Element.IElement element in elements)
                {
                    if (element.HasProperty(PageDivProperty) && elementIndex > 0)
                    {
                        document.Add(new iText.Layout.Element.AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
                    }
                    document.Add((iText.Layout.Element.IBlockElement)element);
                    elementIndex++;
                }

                document.Close();
                return workStream.ToArray();
            }
        }
    }


}
