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
            List<Product> productsMoskow = AngleSharpToyRu.GetBoyTransportProducts();
            using (CsvToyRu ds = new CsvToyRu())
            {
                foreach (var product in productsMoskow)
                {
                    ds.AddRecord(product);
                }
            }
            Console.WriteLine("End");
        }
    }
}