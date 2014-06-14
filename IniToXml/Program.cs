using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;

namespace IniToXml
{
    class Program
    {
        static void Main(string[] args)
        {
            var before = DateTime.Now.Ticks;

            var iniPath = Path.Combine(Environment.CurrentDirectory, "sample.ini");
            var xmlPath = Path.Combine(Environment.CurrentDirectory, "sample.xml");

#if true
            var iniFile = IniUtility.GetSections(iniPath)
                            .Select(s => new Section
                            {
                                Name = s,
                                Items = IniUtility.GetKeys(iniPath, s)
                                    .Select(k => new Item
                                    {
                                        Key = k,
                                        Value = IniUtility.GetValue(iniPath, s, k)
                                    })
                                    .ToList()
                            })
                            .ToList();
            var serializer = new XmlSerializer(iniFile.GetType());
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);
            using (var sw = new StreamWriter(xmlPath, false, Encoding.UTF8))
            {
                serializer.Serialize(sw, iniFile, ns);
            }
#endif

#if false
            var sections = new List<Section>();
            var tSections = IniUtility.GetSections(iniPath);
            foreach (var section in tSections)
            {
                var keys = IniUtility.GetKeys(iniPath, section);
                var items = new List<Item>();
                foreach (var key in keys)
                {
                    var value = IniUtility.GetValue(iniPath, section, key);
                    items.Add(new Item { Key = key, Value = value });
                }
                sections.Add(new Section { Name = section, Items = items });
            }
            var iniFile = new Root { Name = Path.GetFileName(iniPath), Sections = sections };
            Serialize(xmlPath, iniFile);
#endif

#if false
            var sections = new List<Section>();
            var tSections = IniUtility.GetSections(iniPath);
            Parallel.ForEach(tSections, section =>
            {
                var keys = IniUtility.GetKeys(iniPath, section);
                var items = new List<Item>();
                Parallel.ForEach(keys, key =>
                {
                    var value = IniUtility.GetValue(iniPath, section, key);
                    items.Add(new Item { Key = key, Value = value });
                });
                sections.Add(new Section { Name = section, Items = items });
            });
            var iniFile = new Root { Name = Path.GetFileName(iniPath), Sections = sections };
            Serialize(xmlPath, iniFile);
#endif

#if false
            var iniFile = IniUtility.GetSections(iniPath)
                            .Select(s =>
                                new
                                {
                                    Section = s,
                                    Items = IniUtility.GetKeys(iniPath, s)
                                        .Select(k =>
                                            new
                                            {
                                                Key = k,
                                                Value = IniUtility.GetValue(iniPath, s, k)
                                            })
                                });
#endif

            var after = DateTime.Now.Ticks;
            Console.WriteLine(new TimeSpan(after - before).TotalMilliseconds);

            Console.ReadKey();
        }

        static void Serialize(string xmlPath, Root iniFile)
        {
            var serializer = new XmlSerializer(typeof(Root));
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using (var sw = new StreamWriter(xmlPath, false, Encoding.UTF8))
            {
                serializer.Serialize(sw, iniFile, ns);
            }
        }
    }

    public class Root
    {
        public string Name { get; set; }
        public List<Section> Sections { get; set; }
    }

    public class Section
    {
        public string Name { get; set; }
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
