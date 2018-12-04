using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeDom_MSBuild
{
    public class Program
    {
        static void Main(string[] args)
        {
            var tb = new TSBuildTest();
            tb.TestMethod1();

            Console.ReadLine();
        }
    }
}
