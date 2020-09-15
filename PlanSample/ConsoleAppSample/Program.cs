using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace ConsoleAppSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new ConsoleAppSample(args);
            app.InitializeComponent();
            app.Run();
        }
    }
}
