using System.Text.RegularExpressions;

namespace Game.NpcSkill
{
    public class Npc
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string RawMarkdown { get; set; } = "";
        public Dictionary<string,string> Sections { get; set; } = new();
    }

    public class NpcService
    {
        private readonly string _dataPath;
        private readonly Dictionary<string, Npc> _cache = new();

        public NpcService(string dataPath)
        {
            _dataPath = dataPath;
            if (!Directory.Exists(_dataPath)) Directory.CreateDirectory(_dataPath);
            LoadLocalMdFiles();
        }

        private static readonly Regex HeadingRegex = new("^#{1,6}\\s*(.+)$", RegexOptions.Multiline);

        private void LoadLocalMdFiles()
        {
            foreach(var f in Directory.GetFiles(_dataPath, "*.md"))
            {
                try
                {
                    var md = File.ReadAllText(f);
                    var id = Path.GetFileNameWithoutExtension(f);
                    var npc = new Npc { Id = id, Name = id, RawMarkdown = md };
                    npc.Sections = ParseSections(md);
                    if (npc.Sections.TryGetValue("name", out var n)) npc.Name = n.Trim();
                    _cache[id] = npc;
                }
                catch { /* ignore malformed files */ }
            }
        }

        private Dictionary<string,string> ParseSections(string md)
        {
            var sections = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
            var matches = HeadingRegex.Matches(md);
            if (matches.Count == 0)
            {
                sections["content"] = md;
                return sections;
            }

            var indices = new List<(int Index, string Title)>();
            foreach(Match m in matches)
            {
                indices.Add((m.Index, m.Groups[1].Value.Trim()));
            }
            indices.Add((md.Length, "__END__"));

            for(int i=0;i<indices.Count-1;i++)
            {
                var start = indices[i].Index + matches[i].Length;
                var len = indices[i+1].Index - start;
                var title = indices[i].Title.Trim();
                var content = md.Substring(start, len).Trim();
                sections[title] = content;
            }
            return sections;
        }

        public IEnumerable<string> ListNpcIds() => _cache.Keys;
        public bool TryGet(string id, out Npc npc) => _cache.TryGetValue(id, out npc);
        public string GetRawMarkdown(string id) => _cache.TryGetValue(id, out var n) ? n.RawMarkdown : string.Empty;
    }
}
