using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PwM_Library
{
    public static class PasswordService
    {
        public static class PasswordGenerator
        {
            private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVXYZ";
            private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
            private const string Digits = "0123456789";
            private const string Symbols = "`~!@#$%^&*()-_=+[]{}\\|;:'\\,<.>/?";

            private static string GeneratePassword(int length = 12)
            {
                if (length < 12)
                {
                    throw new ApplicationException("Passwords must have a minimum length of 12");
                }

                StringBuilder sb = new StringBuilder(Lowercase + Uppercase + Digits + Symbols);
                var rnd = new Random();

                //At least 1 uppercase, 1 symbol, 1 digit (shuffled later)
                var pwChars = new List<char>
                {
                    Lowercase[rnd.Next(Lowercase.Length)],
                    Uppercase[rnd.Next(Uppercase.Length)],
                    Digits[rnd.Next(Digits.Length)],
                    Symbols[rnd.Next(Symbols.Length)],
                };

                for (int i = pwChars.Count; i < length; i++)
                {
                    pwChars.Add(sb[rnd.Next(sb.Length)]);
                }

                string password = new string(pwChars.OrderBy(x => rnd.Next()).ToArray());
                return password;
            }
        }

        public static bool ValidatePassword(string password)
        {

            /// At least 1 lowercase
            /// At least 1 uppercase
            /// At least 1 digit
            /// At least 1 symbol
            /// Length between 12 and 500
            const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()\-_=+\[\]{}\\|;:'"",<.>/?`~]).{12,500}$";

            return (!string.IsNullOrEmpty(password) && !password.Contains(" ") && Regex.IsMatch(password, PasswordPattern) && SpecialCharCheck(password));
        }

        public static SecureString GetHiddenConsoleInput()
        {
            var pwd = new SecureString();
            while (true)
            {
                var i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }

                if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length <= 0) continue;
                    pwd.RemoveAt(pwd.Length - 1);
                    Console.Write("\b \b");
                }
                else if (i.KeyChar != '\u0000')
                {
                    pwd.AppendChar(i.KeyChar);
                    Console.Write("*");
                }
            }
            return pwd;
        }

        private static bool SpecialCharCheck(string input)
        {
            return input.IndexOfAny(@"\|!#$%&/()=?»«@£§€{}.-;'<>_,".ToCharArray()) > -1;
        }

        public static string ConvertSecureStringToString(this SecureString data)
        {
            return new System.Net.NetworkCredential(string.Empty, data).Password;
        }

        public static SecureString StringToSecureString(string data)
        {
            var secureString = new SecureString();
            foreach (var c in data)
            {
                secureString.AppendChar(c);
            }
            return secureString;
        }
    }
}
