using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Banban.Tools
{
    /// <summary>
    /// 替换.param文件里本地化L字符
    /// </summary>
    class Program
    {
        const string dirTag = "/d";
        static void Main(string[] args)
        {
            var program = new Program();
            //--------------------

            if (args.Any())
            {
                if (args[0].Contains(dirTag))
                {
                    try
                    {
                        var dir = args[0].Remove(0, dirTag.Length);
                        program.ReplaceLString("Auto.param");

                        Console.WriteLine("Replace successed");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

            }
            Console.WriteLine("请输入 '/d' 开头的路径");
            //--------------------
            Console.ReadLine();
        }

        private void ReplaceLString(string dir)
        {
            var paths = Directory.GetFiles(dir, "*.param", SearchOption.AllDirectories);
            var encoding = Encoding.Default;
            foreach (string path in paths)
            {
                try
                {
                    var text = File.ReadAllLines(path, encoding);
                    text = this.Replace(text);
                    File.WriteAllLines(path, text, encoding);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Path: {0}, is failed to replace; Error: {1}", path, ex.ToString());
                }
            }
        }

        private string[] Replace(string[] inputs)
        {
            foreach (var item in inputs)
            {
                var line = Regex.Replace(item, @"L(.*)", new MatchEvaluator(RefineCodeTagForL), RegexOptions.Singleline);
                stringLines.Add(line);
            }
            return stringLines.ToArray();
        }
        private List<string> stringLines = new List<string>();

        string RefineCodeTagForL(Match m)
        {
            string x = m.ToString();

            x = Regex.Replace(x, "L\\(", "");
            x = Regex.Replace(x, "\\)", "");

            return x.Trim();
        }
    }

}
