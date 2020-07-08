using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KS
{
    public static class Security
    {
        private static int _UniqueNum = -99999999;
        private static string _MasterKey => KSGlobal.App.MasterKey;
        private static string _KeyValidate => "@";

        #region Encrypt/Descrypt
        public static string Encrypt(string message)
        {
            return Encrypt(message, _MasterKey);
        }

        public static string Decrypt(string token)
        {
            return Decrypt(token, _MasterKey);
        }

        public static string Encrypt(string message, string key)
        {
            message = Encrypt1(message, MyKey(key));
            FormatTokenValidation(ref message);
            return message;
        }

        public static string Decrypt(string token, string key)
        {
            if (!IsFormatValidToken(ref token))
                throw new FormatException("Token is not currect format");

            return Decrypt1(token, MyKey(key));
        }
        #endregion

        #region Hashs
        /// <summary>
        /// Create a simple value of hash (MD5)
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string Hash(params object[] values)
        {
            return MD5(Dynamic.ConvertToString(values));
        }
        public static string Hash(FileInfo file)
        {
            if (!file.Exists)
                throw new KSException(null, Enums.MessageCode.FindFiles_0);

            using (var stream = File.OpenRead(file.FullName))
            {
                var md5 = new MD5CryptoServiceProvider();
                var hashBytes = md5.ComputeHash(stream);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                    sb.Append(hashBytes[i].ToString("X2"));
                return sb.ToString();
            }
        }
        /// <summary>
        /// Create a token (SHA512)
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string Token(params object[] values)
        {
            return SHA512(Dynamic.ConvertToString(values));
        }
        #endregion

        /// <summary>
        /// Create unique id to values
        /// </summary>
        /// <param name="values">Values</param>
        /// <returns>Id is represented per hexa</returns>
        public static string Id(params object[] values)
        {
            if (values is null) return "1BB000"; // null

            var input = String.Join(Dynamic.ConvertToString(values), values);

            input = input.Replace(Environment.NewLine, "").Replace(@"\n", "").Replace(@"\t", "");

            var id = 0;
            foreach (char c in input) id += c;

            return String.Format("{0:X}", id);
        }
        public static string Id2(string mask, params object[] values)
        {
            if (values is null) return "1BB000"; // null

            var input = String.Join("", values);
            input = input.Replace(Environment.NewLine, "").Replace(@"\n", "").Replace(@"\t", "");

            var id = 0;
            foreach (char c in input) id += c;

            return String.Format("{" + mask + ":X}", id);
        }
        public static int IdNumber()
        {
            return IdNumber(DateTime.Now.ToString("yyyyMMddHHmmss") + RandomChars(5, true));
        }

        public static int IdNumber(params object[] values)
        {
            var input = String.Join("", values);
            input = input.Replace(Environment.NewLine, "").Replace(@"\n", "").Replace(@"\t", "");

            var id = 0;
            foreach (char c in input) id += c;

            return id;
        }

        public static string RandomChars(int size, bool special = false)
        {
            var chars = System.String.Empty;
            var hash = Dynamic.RemoveDuplicateChars(MD5(_UniqueNum++ + DateTime.Now.ToString()));
            if (special)
                chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!£$%^&*(){}-_+=[]:;@~#";
            else
                chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            foreach (var c in hash)
            {
                var l = c.ToString();
                chars = chars.Replace(l.ToLower(), null).Replace(l, l.ToLower());
            }


            var random = new Random();
            var result = new string(Enumerable
                            .Repeat(chars + hash, size)
                            .Select(s => s[random.Next(s.Length)])
                            .ToArray());
            return result;
        }

        #region Validation
        private static string FormatTokenValidation(ref string token)
        {
            int sum = 0, key = 0;

            foreach (var c in token.ToCharArray())
                sum += (int)c;

            var sumstr = sum.ToString();
            for (int i = 0; i < sumstr.Length; i++)
                key += int.Parse(sumstr[i].ToString());

            var hex = key.ToString("X");
            token = $"{_KeyValidate}{token}{hex}{hex.Length}";

            return hex;
        }

        /// <summary>
        /// Valid it is a token and remove the key
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>Valid or invalid</returns>
        private static bool IsFormatValidToken(ref string token)
        {
            if (!token.StartsWith(_KeyValidate))
                return false;

            var keyQtty = int.Parse(token.Substring(token.Length - 1));
            var keyVerify = token.Substring(token.Length - keyQtty - 1, keyQtty);

            var fooToken = token.Substring(token.IndexOf(_KeyValidate) + 1);
            fooToken = fooToken.Substring(0, fooToken.Length - keyVerify.Length - 1);
            var bar = (string)fooToken.Clone();
            if (FormatTokenValidation(ref bar) == keyVerify)
            {
                token = fooToken;
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Crypto version 1
        private static string Encrypt1(object input, string key)
        {
            if (input == null)
                return null;

            key = key ?? _MasterKey;
            var byte24 = MD5(key).Substring(0, 24);
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input.ToString());
            var tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(byte24);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            var cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        private static string Decrypt1(string input, string key)
        {
            key = key ?? _MasterKey;
            var byte24 = MD5(key).Substring(0, 24);
            byte[] inputArray = Convert.FromBase64String(input.ToString());
            var tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(byte24);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            var cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        #endregion

        #region Hashs
        private static string MD5(string input)
        {

            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private static string SHA512(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }
        #endregion

        #region Others
        private static string MyKey(string key)
        {
            return MD5($"{key}{_MasterKey}{key}");
        }
        #endregion


    }
}
