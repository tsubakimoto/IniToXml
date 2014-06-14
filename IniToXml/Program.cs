using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IniToXml
{
    public class IniFile
    {
        public string Name { get; set; }
        public List<IniSection> Sections { get; set; }
    }

    public class IniSection
    {
        public string Name { get; set; }
        public List<IniItem> Items { get; set; }
    }

    public class IniItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    class Program
    {
        [DllImport("KERNEL32.DLL")]
        public static extern uint
            GetPrivateProfileString(string lpAppName,
                string lpKeyName, string lpDefault,
                StringBuilder lpReturnedString, uint nSize,
                string lpFileName);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringA")]
        public static extern uint
            GetPrivateProfileStringByByteArray(string lpAppName,
                string lpKeyName, string lpDefault,
                byte[] lpReturnedString, uint nSize,
                string lpFileName);

        static void Main(string[] args)
        {
            var iniPath = Path.Combine(Environment.CurrentDirectory, "sample.ini");
            var xmlPath = Path.Combine(Environment.CurrentDirectory, "sample.xml");

            var tSections = GetSections(iniPath);
            var sections = new List<IniSection>();
            foreach (var section in tSections)
            {
                var keys = GetKeys(iniPath, section);
                var items = new List<IniItem>();
                foreach (var key in keys)
                {
                    var value = GetValue(iniPath, section, key);
                    items.Add(new IniItem { Key = key, Value = value });
                }
                sections.Add(new IniSection { Name = section, Items = items });
            }
            var iniFile = new IniFile { Name = Path.GetFileName(iniPath), Sections = sections };
            Serialize(xmlPath, iniFile);

#if false
            var iniFile = GetSections(iniPath)
                            .Select(s =>
                                new
                                {
                                    Section = s,
                                    Items = GetKeys(iniPath, s)
                                        .Select(k =>
                                            new
                                            {
                                                Key = k,
                                                Value = GetValue(iniPath, s, k)
                                            })
                                })
                            ;

            var serializer = new XmlSerializer(iniFile.GetType());
            using (var sw = new StreamWriter(xmlPath, false, Encoding.UTF8))
            {
                serializer.Serialize(sw, iniFile);
            }
#endif

            Console.ReadKey();
        }

        static IList<string> GetSections(string iniPath)
        {
            var arr = new byte[1024];
            var size = GetPrivateProfileStringByByteArray(
                null, null, "default", arr, (uint)arr.Length, iniPath);
            var result = Encoding.Default.GetString(arr, 0, (int)size - 1);
            return result.Split('\0');
        }

        static IList<string> GetKeys(string iniPath, string section)
        {
            var arr = new byte[1024];
            var size = GetPrivateProfileStringByByteArray(
                section, null, "default", arr, (uint)arr.Length, iniPath);
            var result = Encoding.Default.GetString(arr, 0, (int)size - 1);
            return result.Split('\0');
        }

        static string GetValue(string iniPath, string section, string key)
        {
            var sb = new StringBuilder(1024);
            GetPrivateProfileString(
                section, key, "default", sb, (uint)sb.Capacity, iniPath);
            return sb.ToString();
        }

        static void Serialize(string xmlPath, IniFile iniFile)
        {
            var serializer = new XmlSerializer(typeof(IniFile));
            using (var sw = new StreamWriter(xmlPath, false, Encoding.UTF8))
            {
                serializer.Serialize(sw, iniFile);
            }
        }
    }
}
