using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Formats.Asn1;

namespace ParsingToyRuConsoleApp
{
    internal class CsvToyRu : IDisposable
    {
        CsvWriter WriterCsv;

        public CsvToyRu()
        {
            var configCsv = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                ShouldQuote = args => args.Row.Index == 1
            };
            var fs = new StreamWriter("toys.csv", true);
            WriterCsv = new CsvWriter(fs, configCsv);
        }
        public void AddRecord(Product productRecord)
        {
            WriterCsv.NextRecord();
            WriterCsv.WriteRecord<Product>(productRecord);
        }

        public void Dispose()
        {
            WriterCsv.Dispose();
        }
    }
}
