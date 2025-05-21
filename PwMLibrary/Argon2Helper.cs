using Konscious.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwM_Library
{
    public static class Argon2Helper
    {
        public static byte[] DeriveKey(string password, byte[] salt, string context, int length = 32)
        {
            var input = Encoding.UTF8.GetBytes(password + context);
            using var argon2 = new Argon2id(input)
            {
                Salt = salt,
                DegreeOfParallelism = 2,
                Iterations = 40,
                MemorySize = 4096
            };
            return argon2.GetBytes(length);
        }
    }
}
