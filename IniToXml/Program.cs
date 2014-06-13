using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IniToXml
{
#if false
    class IniFile
    {
        public string Name { get; private set; }
        public IList<IniSection> Sections { get; private set; }
        public IniFile(string name, IList<IniSection> sections)
        {
            this.Name = name;
            this.Sections = sections;
        }
    }

    class IniSection
    {
        public string Name { get; private set; }
        public IList<IniItem> Items { get; private set; }
        public IniSection(string name, IList<IniItem> items)
        {
            this.Name = name;
            this.Items = items;
        }
    }

    class IniItem
    {
        public string Key { get; private set; }
        public string Value { get; private set; }
        public IniItem(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
#endif

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
    }
}
