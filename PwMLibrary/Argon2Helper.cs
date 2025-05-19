using Konscious.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PwM_Library.Argon2Helper;

namespace PwM_Library
{
    public static class Argon2Helper
    {
        public static class Argon2
        {
            public static Argon2id s_argon2;

            public static byte[] Argon2HashPassword(string password)
            {
                s_argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
                {
                    Salt = Encoding.UTF8.GetBytes(password.Substring(2, 10)),
                    DegreeOfParallelism = 2,
                    Iterations = 40,
                    MemorySize = 4096
                };
                return s_argon2.GetBytes(32);
            }
        }
    }
}
