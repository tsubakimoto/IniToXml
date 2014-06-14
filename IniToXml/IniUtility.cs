using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IniToXml
{
    public static class IniUtility
    {
        [DllImport("KERNEL32.DLL")]
        private static extern uint
            GetPrivateProfileString(string lpAppName,
                string lpKeyName, string lpDefault,
                StringBuilder lpReturnedString, uint nSize,
                string lpFileName);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringA")]
        private static extern uint
            GetPrivateProfileStringByByteArray(string lpAppName,
                string lpKeyName, string lpDefault,
                byte[] lpReturnedString, uint nSize,
                string lpFileName);

        public static IList<string> GetSections(string iniPath)
        {
            var arr = new byte[1024];
            var size = GetPrivateProfileStringByByteArray(
                null, null, "default", arr, (uint)arr.Length, iniPath);
            var result = Encoding.Default.GetString(arr, 0, (int)size - 1);
            return result.Split('\0');
        }

        public static IList<string> GetKeys(string iniPath, string section)
        {
            var arr = new byte[1024];
            var size = GetPrivateProfileStringByByteArray(
                section, null, "default", arr, (uint)arr.Length, iniPath);
            var result = Encoding.Default.GetString(arr, 0, (int)size - 1);
            return result.Split('\0');
        }

        public static string GetValue(string iniPath, string section, string key)
        {
            var sb = new StringBuilder(1024);
            GetPrivateProfileString(
                section, key, "default", sb, (uint)sb.Capacity, iniPath);
            return sb.ToString();
        }
    }
}
