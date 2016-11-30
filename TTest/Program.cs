using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestParse(@"E:\WorkProjects\TKT\TKT 0.1.6\发布\例子\打飞机\战场.tkt");
            //Console.ReadKey();
        }
        /*
        static TKTClassModel TestParse(string file)
        {
            SchemaScanner scanner = new SchemaScanner();
            SchemaParser parser = new SchemaParser();

            string code = File.ReadAllText(file);
            var tokens = scanner.Scan(code);
            Console.WriteLine("Token数量:"+tokens.Count);
            TKTClassModel tktclass = parser.Parse(tokens);
            Console.WriteLine("tktclass信息:\n" + tktclass.ToString());
            return tktclass;
        }*/
    }
}
