using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeDom_MSBuild
{
    public static class Builder
    {
        public static bool Build(string targetname, List<BuildItem> items, List<string> references, string buildlocation = null)
        {
            #region get build location
            var path = buildlocation;
            //Get a temp directory to act as build location(if none specified)
            if (string.IsNullOrEmpty(buildlocation))
                path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            #endregion

            var olddir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(path);
            try
            {
                #region Deserialziation
                //Deserialize our build sources
                foreach (var item in items)
                {
                    var fullpath = Path.Combine(path, item.FileName);
                    var fs = new FileStream(fullpath, FileMode.Create, FileAccess.Write);
                    CopyStream(item.Content, fs);
                    fs.Close();
                }
                #endregion

                var pcoll = new ProjectCollection();
                //pcoll.DefaultToolsVersion = "4.0";
                var project = new Project(pcoll);

                #region Add default properties
                var pgroup = project.Xml.CreatePropertyGroupElement();
                project.Xml.InsertAfterChild(pgroup, project.Xml.LastChild);
                pgroup.SetProperty("AssemblyName", targetname);
                pgroup.SetProperty("RootNamespace", targetname);
                pgroup.SetProperty("TargetFrameworkVersion", "v4.0");
                pgroup.SetProperty("OutputType", "Library");
                //pgroup.AddProperty("DefaultTarget", "Build");
                //pgroup.AddProperty("SchemaVersion", "2.0");
                pgroup.SetProperty("IntermediateOutputPath", "obj");
                //pgroup.SetProperty("DebugType", "pdbonly");
                //pgroup.SetProperty("Optimize", "true");
                pgroup.SetProperty("OutputPath", @"..\Debug\");
                pgroup.SetProperty("WarningLevel", "4");
                #endregion

                #region Add Imports
                var refgroup = project.Xml.CreateItemGroupElement();
                project.Xml.InsertAfterChild(refgroup, project.Xml.LastChild);
                foreach (var reference in references)
                {
                    refgroup.AddItem("Reference", reference);
                }
                #endregion

                #region Add Build Items
                var buildgroup = project.Xml.CreateItemGroupElement();
                project.Xml.InsertAfterChild(buildgroup, project.Xml.LastChild);
                var properties = new Dictionary<string, string>();
                foreach (var item in items)
                {
                    properties.Clear();
                    var ext = Path.GetExtension(item.FileName);
                    switch (ext)
                    {
                        case ".cs":
                            //properties.Add("SubType", "Code");
                            buildgroup.AddItem("Compile", item.FileName);
                            break;
                        default:
                            throw new ArgumentException("BuildItems contains invalid filetype, currently only (*.cs and *.xaml) are supported");
                    }
                }
                #endregion

                project.Xml.AddImport(@"$(MSBuildToolsPath)\Microsoft.CSharp.targets");
                ILogger logger = new FileLogger()
                {
                    Verbosity = LoggerVerbosity.Normal,
                    Parameters = "logfile=" + Path.Combine(path, "buildlog.txt")
                };
                project.Build(logger);

            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                Directory.SetCurrentDirectory(olddir);
            }
            return true;
        }
        private static void CopyStream(Stream source, Stream target)
        {
            var buffer = new byte[1024];
            const int size = 1024;
            var count = 0;
            count = source.Read(buffer, 0, size);
            while (count > 0)
            {
                target.Write(buffer, 0, count);
                count = source.Read(buffer, 0, size);
            }
            target.Flush();
        }
    }

    public class BuildItem
    {
        public string FileName { get; set; }
        public Stream Content { get; set; }
    }
}
