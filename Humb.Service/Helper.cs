using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace Humb.Service
{
    public static class Helper
    {
        public static string SanitizeInput(string input)
        {
            const string removeChars = "?&^$#@!()+-,:;<>’\'-_*";
            // specify capacity of StringBuilder to avoid resizing
            StringBuilder sb = new StringBuilder(input.Length);
            foreach (char x in input.Where(c => !removeChars.Contains(c)))
            {
                sb.Append(x);
            }
            return sb.ToString();
        }

        public static string SanitizeImageName(string input)
        {
            const string removeChars = "?&^$#!()+-,:;<>’\'-*";
            // specify capacity of StringBuilder to avoid resizing
            StringBuilder sb = new StringBuilder(input.Length);
            foreach (char x in input.Where(c => !removeChars.Contains(c)))
            {
                sb.Append(x);
            }
            return sb.ToString();
        }

        public static bool IsEmailValid(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static int[] ConvertStringToIntArray(string arrayString)
        {
            if (!string.IsNullOrEmpty(arrayString))
            {
                return arrayString.Split('_').Select(n => Convert.ToInt32(n)).ToArray();
            }
            else
            {
                return new int[0];
            }
        }
        public static int GetCommonCharacters(string searchString, string otherString)
        {
            return otherString.ToLower().Intersect(searchString.ToLower()).Count();
        }

        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input

            MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);

            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static string GenerateRandomPassword()
        {
            Random rd = new Random();

            const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
            char[] chars = new char[6];

            for (int i = 0; i < 6; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }
            return new string(chars);
        }
    }
}
