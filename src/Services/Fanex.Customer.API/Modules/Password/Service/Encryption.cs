using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic;

namespace Fanex.Customer.API.Modules.Password.Service
{
    public interface IEncryption
    {
        string PearsonEncode(string key);

        string CfsEnCode(string codeStr);

        string RandomString(int length);

        string RandomString(int length, bool noZeroO);

        string ComputeHash(string hashAlgorithm, string plainText, string salt = null);
    }

    public class Encryption : IEncryption
    {
        public string PearsonEncode(string key)
        {
            if (string.IsNullOrEmpty(key) || key.Length == 1) return key;

            var h = 0L;
            var c = key.Length;
            var keys = key.ToCharArray();

            for (var i = 0; i < c; i++)
            {
                h += keys[i].GetHashCode();
            }

            return h.ToString();
        }

        public string CfsEnCode(string codeStr)
        {
            var num = 30;
            var num2 = num - codeStr.Length;
            if (num2 > 1)
            {
                for (var m = 1; m <= num2; m++)
                {
                    codeStr += Strings.Chr(0x15);
                }
            }
            var num3 = 1.0;
            for (var i = 1; i <= num; i++)
            {
                var num5 = num + Strings.Asc(Strings.Mid(codeStr, i, 1)) * i;
                num3 *= num5;
            }
            codeStr = num3.ToString("G15");
            var str = "";
            for (var j = 1; j <= codeStr.Length; j++)
            {
                str += CfsCode(Strings.Mid(codeStr, j, 3));
            }
            var str2 = "";
            for (var k = 20; k <= str.Length - 0x12; k += 2)
            {
                str2 += Strings.Mid(str, k, 1);
            }
            return str2;
        }

        public string RandomString(int length)
        {
            return RandomString(length, false);
        }

        public string RandomString(int length, bool noZeroO)
        {
            string chars;
            if (noZeroO)
                chars = "qwertyuipasdfghjklzxcvbnm123456789";
            else
                chars = "qwertyuiopasdfghjklzxcvbnm0123456789";
            var rndStr = string.Empty;
            var rnd = new Random(DateTime.Now.Second + DateTime.Now.Millisecond);

            for (var i = 0; i < length; ++i)
            {
                rndStr += chars[rnd.Next() % chars.Length];
            }
            return rndStr;
        }

        public string ComputeHash(string hashAlgorithm, string plainText, string salt = null)
        {
            if (salt != null)
            {
                plainText += salt;
            }

            // Convert plain text into a byte array
            byte[] data = Encoding.UTF8.GetBytes(plainText);

            // Calculate the hash
            // Compute hash value of our plain text with appended salt.
            byte[] encrypt = GetHashProvider(hashAlgorithm).ComputeHash(data);

            return ToHexString(encrypt);
        }

        private static HashAlgorithm GetHashProvider(string hashAlgorithm)
        {
            // Hashing algorithm

            hashAlgorithm = hashAlgorithm.ToUpper();
            HashAlgorithm hash = hashAlgorithm switch
            {
                "SHA1" => SHA1.Create(),
                "SHA256" => SHA256.Create(),
                "SHA384" => SHA384.Create(),
                "SHA512" => SHA512.Create(),
                _ => MD5.Create(),
            };
            return hash;
        }

        private static string ToHexString(byte[] value)
        {
            int length = value.Length;
            char[] hex = new char[length * 2];

            int j = 0;
            for (int i = 0; i < length; i++)
            {
                string s = LowerHexBytes[value[i]];
                hex[j++] = s[0];
                hex[j++] = s[1];
            }

            return new string(hex);
        }

        private static string[] LowerHexBytes
        {
            get
            {
                return [
                    "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0a", "0b", "0c", "0d", "0e", "0f",
                "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1a", "1b", "1c", "1d", "1e", "1f",
                "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2a", "2b", "2c", "2d", "2e", "2f",
                "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3a", "3b", "3c", "3d", "3e", "3f",
                "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4a", "4b", "4c", "4d", "4e", "4f",
                "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5a", "5b", "5c", "5d", "5e", "5f",
                "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6a", "6b", "6c", "6d", "6e", "6f",
                "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7a", "7b", "7c", "7d", "7e", "7f",
                "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8a", "8b", "8c", "8d", "8e", "8f",
                "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9a", "9b", "9c", "9d", "9e", "9f",
                "a0", "a1", "a2", "a3", "a4", "a5", "a6", "a7", "a8", "a9", "aa", "ab", "ac", "ad", "ae", "af",
                "b0", "b1", "b2", "b3", "b4", "b5", "b6", "b7", "b8", "b9", "ba", "bb", "bc", "bd", "be", "bf",
                "c0", "c1", "c2", "c3", "c4", "c5", "c6", "c7", "c8", "c9", "ca", "cb", "cc", "cd", "ce", "cf",
                "d0", "d1", "d2", "d3", "d4", "d5", "d6", "d7", "d8", "d9", "da", "db", "dc", "dd", "de", "df",
                "e0", "e1", "e2", "e3", "e4", "e5", "e6", "e7", "e8", "e9", "ea", "eb", "ec", "ed", "ee", "ef",
                "f0", "f1", "f2", "f3", "f4", "f5", "f6", "f7", "f8", "f9", "fa", "fb", "fc", "fd", "fe", "ff" ];
            }
        }

        private static string CfsCode(string nWord)
        {
            var number = "";
            for (var i = 1; i <= nWord.Length; i++)
            {
                number += Strings.Asc(Strings.Mid(nWord, i, 1));
            }
            return Conversion.Hex(number);
        }
    }
}