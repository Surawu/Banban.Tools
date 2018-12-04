using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeDom_MSBuild
{
    public class TSBuildTest
    {
        public void TestMethod1()
        {
            var fs1 = new FileStream(@"D:\VSProject\Temp\TSBuildTest\ClassLib.cs", FileMode.Open, FileAccess.Read);
            var references = new List<string>
             {
              "Microsoft.CSharp",
              "PresentationFramework",
              "PresentationCore",
              "System.Activities",
              "System.Activities.Presentation",
              "System",
              "System.Core",
              "System.Data",
              "System.Data.DataSetExtensions",
              "System.Xml",
              "System.Xaml",
              "System.Xml.Linq",
              "WindowsBase"
             };
            var items = new List<BuildItem>
            {
                new BuildItem { FileName = "ClassLib.cs", Content = fs1 }
            };
            Builder.Build("fubar", items, references, AppDomain.CurrentDomain.BaseDirectory);
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fubar.dll")) == false)
            {
                Console.WriteLine("Failed to build the dll");
            }

            var nofubar = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.ManifestModule.Name == "fubar.dll").FirstOrDefault();

            if (nofubar == null)
            {
                Console.WriteLine(@"fubar.dll Contaminated the AppDomain! :(");
            }
        }
    }
}
