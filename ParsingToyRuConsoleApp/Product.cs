using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingToyRuConsoleApp
{
    public class Product
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string LastPrice { get; set; }
        public string ImagesLinks { get; set; }
        public string Availability { get; set; }
        public string Breadcrumb { get; set; }
        public string Region { get; set; }
        
        public void WriteLine()
        {
            Console.WriteLine($"{Name} {Price} {LastPrice} {Availability}");
        }
    }
}
