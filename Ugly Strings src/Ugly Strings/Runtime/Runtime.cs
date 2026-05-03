using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ugly_Strings.Runtimeshit
{
    internal class Runtime
    {
        public static string ReverseUglyStrings(string input, int key)
        {
            byte[] compressed = Convert.FromBase64String(input);
            byte[] data;

            using (var ms = new MemoryStream(compressed))
            using (var gz = new GZipStream(ms, CompressionMode.Decompress))
            using (var output = new MemoryStream())
            {
                gz.CopyTo(output);
                data = output.ToArray();
            }

            byte[] result = new byte[data.Length / 2];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (byte)(data[i * 2] ^ key);
            }

            return Encoding.UTF8.GetString(result);
        }
    }
}
