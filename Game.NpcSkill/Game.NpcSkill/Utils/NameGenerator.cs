using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Game.NpcSkill.Utils
{
    public static class NameGenerator
    {
        // Simple deterministic name generator based on a seed (bytes). It prefers curated roots
        // and falls back to combining a root with a numeric suffix for uniqueness.

        public static string GenerateName(ReadOnlySpan<byte> seed, string rootsFilePath)
        {
            var roots = File.ReadAllLines(rootsFilePath)
                        .Where(l => !string.IsNullOrWhiteSpace(l))
                        .Select(l => l.Trim())
                        .ToArray();

            // Use SHA256 to get a deterministic number from seed
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(seed.ToArray());

            // pick an index from the hash
            var idx = BitConverter.ToUInt32(hash, 0) % (uint)roots.Length;
            var baseName = roots[idx];

            // compute a numeric suffix from another part of the hash
            var suffix = (BitConverter.ToUInt32(hash, 4) % 100).ToString();

            return baseName + "-" + suffix;
        }

        // Convenience overloads
        public static string GenerateNameFromCoords(int x, int y, int z, string rootsFilePath, string salt = "")
        {
            var input = $"{x}:{y}:{z}:{salt}";
            var seed = System.Text.Encoding.UTF8.GetBytes(input);
            return GenerateName(seed, rootsFilePath);
        }
    }
}
