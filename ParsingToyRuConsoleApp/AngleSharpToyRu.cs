using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ParsingToyRuConsoleApp
{
    internal class AngleSharpToyRu
    {
        static string siteLink = "https://www.toy.ru";
        static HtmlParser htmlParser = new HtmlParser();
        public static string cityCode = "61000001000";// 77000000000 - мск 61000001000 - ростов на дону

        public static Product GetInfoProduct(IElement porductShortCard)
        {
            IElement productFullCard = GetElementsFromPageByQuerySelector(porductShortCard.QuerySelector("a.product-name").GetAttribute("href"), "div[id=wrapper]").First();
            Product productResult = new Product();
            List<Thread> threads = new List<Thread>();
            threads.Add(new Thread(() => GetPriceProduct(porductShortCard,ref productResult)));
            threads.Add(new Thread(() => GetProductAvailability(productFullCard, ref productResult)));
            threads.Add(new Thread(() => GetProductBreadcrumb(productFullCard, ref productResult)));
            threads.Add(new Thread(() => GetProductName(productFullCard,ref productResult)));
            threads.Add(new Thread(() => GetProductRegion(productFullCard,ref productResult)));
            threads.Add(new Thread(() => GetProductImages(productFullCard,ref productResult)));
            foreach (Thread thread in threads)
            {
                thread.Start();
            }
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
            threads.Clear();
            return productResult;
        }
        private static void GetPriceProduct(IElement productElement, ref Product product)
        {
            string priceMain = productElement.QuerySelector("span.price").TextContent;
            IElement? priceDiscountElement = productElement.QuerySelector("span.price-discount");
            if (priceDiscountElement != null)
                product.Price = priceDiscountElement.TextContent.Trim();
            else
                product.Price = priceMain;
            product.LastPrice = priceMain;
        }
        private static void GetProductAvailability(IElement productElement, ref Product product)
        {
            IElement? availabilityElement = productElement.QuerySelector("div.detail-block span.ok");
            if (availabilityElement != null)
                product.Availability = "В НАЛИЧИИ";
            else
                product.Availability = "НЕТ НАЛИЧИИ";
        }
        private static void GetProductName(IElement productElement, ref Product product)
        {
            IElement? nameElement = productElement.QuerySelector("h1.detail-name");
            if (nameElement != null)
                product.Name = nameElement.GetAttribute("content");
            else
                product.Name = "ОШИБКА ПОЛУЧЕНИЯ";
        }
        private static void GetProductBreadcrumb(IElement productElement, ref Product product)
        {
            IElement? breadcrumbElement = productElement.QuerySelector("nav.breadcrumb");
            if (breadcrumbElement != null)
            {
                IHtmlCollection<IElement> breadcrumbItems = breadcrumbElement.GetElementsByClassName("breadcrumb-item");
                for (int i = 0; i < breadcrumbItems.Count()-1; i++)
                {
                    product.Breadcrumb += breadcrumbItems[i].TextContent;
                    if (i != breadcrumbItems.Count() - 2)
                        product.Breadcrumb += ">";
                }
            }
            else
                product.Breadcrumb = "ОШИБКА ПОЛУЧЕНИЯ";
        }
        private static void GetProductRegion(IElement productElement, ref Product product)
        {
            IElement? regionElement = productElement.QuerySelector("div.select-city-link a");
            if (regionElement != null)
                product.Region = regionElement.TextContent.Trim();
            else
                product.Region = "ОШИБКА ПОЛУЧЕНИЯ";
        }
        private static void GetProductImages(IElement productElement, ref Product product)
        {
            IHtmlCollection<IElement> imageElements = productElement.QuerySelectorAll("div.card-slider-for a");
            if (imageElements.Count() > 0)
            {
                foreach (IElement imageElement in imageElements)
                {
                    product.ImagesLinks += imageElement.GetAttribute("href") + "     ";
                }
            }
            else
                product.ImagesLinks = "ОШИБКА ПОЛУЧЕНИЯ";
        }

        public static List<Product> GetBoyTransportProducts()
        {
            List<Product> products = new List<Product>();
            string link = "https://www.toy.ru/catalog/boy_transport/";
            int pageNumber = 1;
            bool stop = false;
            while (!stop)
            {
                IHtmlCollection<IElement> productsLink = GetElementsFromPageByQuerySelector(link + pageNumber, "div.product-card");// a.product-name
                foreach (IElement item in productsLink)
                {
                    Product product = GetInfoProduct(item);
                    product.WriteLine();
                    products.Add(product);
                }
                IHtmlCollection<IElement> pagesLink = GetElementsFromPageByQuerySelector(link + pageNumber, "a.page-link");
                foreach (IElement item in pagesLink)
                {
                    stop = true;
                    if(item.Text() == "След.")
                        stop= false;
                }
                pageNumber++;
            }
            return products;
        }
        public static IHtmlCollection<IElement> GetElementsFromPageByQuerySelector(string url, string selectors, bool isXMLHttpRequest = false)
        {
            IHtmlDocument documentPage = GetPageDocument(url, isXMLHttpRequest);
            return documentPage.QuerySelectorAll(selectors);
        }
        public static IHtmlCollection<IElement> GetElementsFromPageByClass(string url, string className, bool isXMLHttpRequest = false)
        {
            IHtmlDocument documentRazdel = GetPageDocument(url, isXMLHttpRequest); 
            return documentRazdel.GetElementsByClassName(className);
        }
        public static IHtmlDocument GetPageDocument(string url, bool isXMLHttpRequest)
        {
            string pageHtml = GetPageHtml(url, isXMLHttpRequest);
            IHtmlDocument resultDocumentNew = htmlParser.ParseDocument(pageHtml);
            return resultDocumentNew;
        }
        public static string GetPageHtml(string link, bool isXMLHttpRequest) 
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetValidUrl(siteLink, link));
                request.Method = "GET";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 YaBrowser/22.11.5.715 Yowser/2.5 Safari/537.36";

                Uri target = new Uri(siteLink);
                request.CookieContainer= new CookieContainer();
                request.CookieContainer.Add(new Cookie("BITRIX_SM_city", cityCode) { Domain = target.Host });

                if (isXMLHttpRequest)
                {
                    request.Headers.Add("x-requested-with", "XMLHttpRequest");
                }
                
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("Windows-1251"));
                string result = streamReader.ReadToEnd();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(link + "\t" + isXMLHttpRequest + "\n");
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public static string GetValidUrl(string urlSite, string url)
        {
            if (url.IndexOf(urlSite) == -1)
            {
                return urlSite + url;
            }
            return url;
        }
    }
}
