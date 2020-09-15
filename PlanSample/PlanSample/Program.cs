using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace PlanSample
{
    class Program
    {
        static void Main(string[] args)
        {
            //var v = new Sample1();
            //v.Do();

            PlanSample app = new PlanSample();
            app.InitializeComponent();
            app.Run();

        }
    }

}
