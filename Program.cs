using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpikeForPres
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter File name: ");
            string fname = Console.ReadLine();
            Console.WriteLine("Enter the size: ");
            int size = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Color?");
            string col = Console.ReadLine();
            if (col == "y")
            {
                ASCIIConvert con = new ASCIIConvert(fname, size, true);
            }
            else
            {
                ASCIIConvert con = new ASCIIConvert(fname, size);
            }
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
