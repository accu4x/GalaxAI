using System;
using System.Security.Cryptography;
using System.Text;

namespace Game.Api.Services
{
    public static class NameGenerator
    {
        private static readonly string[] Roots = new[] { "Kestrel","Meridian","Noval","Helion","Arcum","Kepler","Orion","Gemini","Aster","Solin" };

        public static string GenerateName(string coordHash)
        {
            // deterministic name from hash
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(coordHash));
            var idx = BitConverter.ToUInt32(bytes, 0) % (uint)Roots.Length;
            var suffix = (BitConverter.ToUInt16(bytes, 4) % 100).ToString();
            return Roots[idx] + "-" + suffix;
        }
    }
}