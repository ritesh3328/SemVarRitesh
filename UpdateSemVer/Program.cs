using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;

namespace UpdateVersionTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var info = Info();

            if ((args == null) || (args.Length <= 0))
            {
                Usage(info, "");
                Environment.ExitCode = 1;
                return;
            }

            var filename = args[0];
            if (!File.Exists(filename))
            {
                Usage(info, string.Format("Can't find file: {0}", filename));
                Environment.ExitCode = 2;
                return;
            }

            var buf = File.ReadAllText(filename);

            XDocument xDoc = null;

            try
            {
                xDoc = XDocument.Parse(buf);
            }
            catch
            {
                Usage(info, "Unable to load XML");
                return;
            }

            var version = string.Empty;

            var node = (XElement)xDoc.XPathSelectElement(".//FileVersion");

            if (node != null)
            {
                var v = node.Value;
                if (!string.IsNullOrWhiteSpace(v))
                {
                    if (string.IsNullOrEmpty(version))
                    {
                        var parts = v.Split(new char[] { '.' });
                        var build = parts[parts.Length - 1];
                        if (!int.TryParse(build, out int bn)) bn = 0;
                        bn++;
                        parts[parts.Length - 1] = bn.ToString();
                        version = string.Join('.', parts);
                    }
                }

                node.Value = version;
            }

            if (string.IsNullOrWhiteSpace(version))
            {
                Usage(info, string.Format("Project '{0}' does not contain '<FileVersion></FileVersion>' attribute", filename));
                Environment.ExitCode = 4;
                return;
            }
            else
            {
                xDoc.Save(filename);
                Console.WriteLine("{0} updated to {1}", filename, version);
            }
        }

        static void Usage(Dictionary<string, string> info, string message = "")
        {
            Console.WriteLine("{0} {1} {2} {3}\n", DictionaryValue(info, "AssemblyDescriptionAttribute"), DictionaryValue(info, "AssemblyCompanyAttribute"), DictionaryValue(info, "AssemblyCopyrightAttribute"), DictionaryValue(info, "AssemblyFileVersionAttribute"));

            Console.WriteLine("Usage: UpdateVersionTool projectToVersion.csproj");
            Console.WriteLine("Given the version attributes of 'AssemblyVersion' and 'FileVersion' in a `.csproj` file");
            Console.WriteLine("And a version number in the form of 'major.minor.release.build' will increment the build number by one");
            if (!string.IsNullOrWhiteSpace(message)) Console.WriteLine("\n{0}", message);
        }

        static Dictionary<string, string> Info()
        {
            var d = new Dictionary<string, string>();

            var assembly = typeof(Program).Assembly;
            foreach (var attribute in assembly.GetCustomAttributesData())
            {
                if (!attribute.TryParse(out string value))
                {
                    value = string.Empty;
                }

                if (!string.IsNullOrWhiteSpace(value)) d.Add(attribute.AttributeType.Name, value);
            }

            return d;
        }

        static string DictionaryValue(Dictionary<string, string> info, string key)
        {
            if (info.ContainsKey(key)) return info[key];
            else return string.Empty;
        }

    }
}
