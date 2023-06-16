using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using AngleSharp.Dom;
using System.Text;

namespace ParsingToyRuConsoleApp
{
    internal class Program
    {
        
        static void Main(string[] args)
        {


            //using (CsvToyRu ds = new CsvToyRu())
            //{

            //    //    for (int i = 0; i < 1000000; i++)
            //    //    {
            //    Product product = new Product();
            //    product.Name = "dsf";
            //    product.LastPrice = "121";
            //    product.ImagesLinks = string.Join("     ", new List<string> { "dfs", "dfs", "2312" });
            //    ds.AddRecord(product);
            //    //    }
            //}
            //Console.WriteLine(AngleSharpToyRu.GetPageHtml("https://www.toy.ru/catalog/boy_transport/", false));

            List<Product> productsMoskow = AngleSharpToyRu.GetBoyTransportProducts();
            using (CsvToyRu ds = new CsvToyRu())
            {
                foreach (var product in productsMoskow)
                {
                    ds.AddRecord(product);
                }
            }
            //    Console.WriteLine("Для Москвы товаров: "+linksProductsMoskow.Count());
            //    foreach (string item in linksProductsMoskow)
            //    {

            //    }
            Console.WriteLine("End");

        }
    }
}