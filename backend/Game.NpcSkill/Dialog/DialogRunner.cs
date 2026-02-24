using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Game.NpcSkill.Dialog
{
    public class PlayerSnapshot : Dictionary<string, object>
    {
        public T Get<T>(string path, T fallback = default!)
        {
            if (TryGetValue(path, out var v) && v is T t)
                return t;
            return fallback!;
        }
    }

    public class DialogSession
    {
        public string Id { get; } = Guid.NewGuid().ToString("n");
        public string NpcId { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
        public List<string> Transcript { get; } = new();
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
    }

    public class DialogRunner
    {
        private readonly string _dataPath;
        private readonly ConcurrentDictionary<string, DialogSession> _sessions = new();
        private string _agentGuidelines = string.Empty;
        private string _lore = string.Empty;

        public DialogRunner(string dataPath)
        {
            _dataPath = dataPath ?? throw new ArgumentNullException(nameof(dataPath));
            LoadAuxFiles();
        }

        private void LoadAuxFiles()
        {
            try
            {
                var baseDir = Path.GetDirectoryName(_dataPath) ?? _dataPath;
                var guidPath = Path.Combine(baseDir, "Data", "AgentGuidelines.md");
                var lorePath = Path.Combine(baseDir, "Data", "Lore.md");
                if (File.Exists(guidPath)) _agentGuidelines = File.ReadAllText(guidPath);
                if (File.Exists(lorePath)) _lore = File.ReadAllText(lorePath);
            }
            catch
            {
                // swallow errors for robustness in demos
            }
        }

        public void ReloadData()
        {
            LoadAuxFiles();
            // future: reload NPC cache if implemented
        }

        public IReadOnlyList<string> ListNpcs()
        {
            if (!Directory.Exists(_dataPath)) return Array.Empty<string>();
            return Directory.GetFiles(_dataPath, "*.md").Select(Path.GetFileNameWithoutExtension).ToList();
        }

        private string LoadNpcMarkdown(string npcId)
        {
            var f = Path.Combine(_dataPath, npcId + ".md");
            if (!File.Exists(f)) throw new FileNotFoundException("NPC not found", f);
            return File.ReadAllText(f);
        }

        private (string name, string screen, string[] examples, string constraints) ParseNpc(string md)
        {
            // Very lightweight heuristic parser for headings
            string name = Regex.Match(md, @"^#\s*(.+)$", RegexOptions.Multiline).Groups[1].Value.Trim();
            string screen = "";
            var examples = new List<string>();
            string constraints = "";

            var lines = md.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string section = null;
            var sb = new System.Text.StringBuilder();
            foreach (var raw in lines)
            {
                var line = raw.Trim();
                var h = Regex.Match(line, @"^##\s*(.+)$");
                if (h.Success)
                {
                    // flush
                    if (section == "ExampleLines")
                    {
                        // split by quotes or line
                        var text = sb.ToString();
                        foreach (Match m in Regex.Matches(text, "\"([^\"]+)\""))
                        {
                            examples.Add(m.Groups[1].Value);
                        }
                    }
                    if (section == "Screen") screen = sb.ToString().Trim();
                    if (section == "Constraints") constraints = sb.ToString().Trim();

                    section = h.Groups[1].Value.Trim();
                    sb.Clear();
                }
                else
                {
                    sb.AppendLine(line);
                }
            }
            // final flush
            if (section == "ExampleLines")
            {
                var text = sb.ToString();
                foreach (Match m in Regex.Matches(text, "\"([^\"]+)\""))
                {
                    examples.Add(m.Groups[1].Value);
                }
            }
            if (section == "Screen") screen = sb.ToString().Trim();
            if (section == "Constraints") constraints = sb.ToString().Trim();

            return (name ?? "Unknown", screen ?? string.Empty, examples.ToArray(), constraints ?? string.Empty);
        }

        private string SubstitutePlaceholders(string template, PlayerSnapshot snapshot)
        {
            if (string.IsNullOrEmpty(template)) return template;
            return Regex.Replace(template, "\\{([^}]+)\\}", m =>
            {
                var key = m.Groups[1].Value.Trim();
                // support player.name or player.level simple paths; snapshot stores flattened keys like player.name
                if (snapshot != null && snapshot.TryGetValue(key, out var val)) return val?.ToString() ?? "<unknown>";
                return "<unknown>";
            });
        }

        public (DialogSession session, string formatted) StartSession(string npcId, string ownerId, PlayerSnapshot snapshot)
        {
            var md = LoadNpcMarkdown(npcId);
            var parsed = ParseNpc(md);

            var session = new DialogSession { NpcId = npcId, OwnerId = ownerId };
            _sessions[session.Id] = session;

            // choose an opening line: prefer example lines, else a default
            string opening;
            if (parsed.examples.Length > 0)
            {
                opening = parsed.examples[0]; // deterministic for demo
            }
            else
            {
                opening = $"{parsed.name} greets you.";
            }

            // include agent guidelines and lore in the rendering context (not yet a language model prompt)
            var context = _agentGuidelines + "\n\n" + _lore;

            var body = SubstitutePlaceholders(opening, snapshot);
            var formatted = RenderScreen(parsed.name, parsed.screen, body);

            session.Transcript.Add($"NPC: {body}");
            session.LastActive = DateTime.UtcNow;
            return (session, formatted);
        }

        public (string formatted, string rawReply) SendMessage(string sessionId, string playerMessage, PlayerSnapshot snapshot)
        {
            if (!_sessions.TryGetValue(sessionId, out var session)) throw new ArgumentException("session not found");
            var md = LoadNpcMarkdown(session.NpcId);
            var parsed = ParseNpc(md);

            // naive reply: pick first example and append a comment
            string reply;
            if (parsed.examples.Length > 0)
            {
                reply = parsed.examples[0];
            }
            else
            {
                reply = $"{parsed.name} ponders and replies.";
            }

            // simple variation based on playerMessage
            reply = reply + "\n\n" + "(responding to: " + Truncate(playerMessage, 200) + ")";

            // include guidelines (future: use for LLM prompt)
            var context = _agentGuidelines + "\n\n" + _lore;

            var body = SubstitutePlaceholders(reply, snapshot);
            var formatted = RenderScreen(parsed.name, parsed.screen, body);

            session.Transcript.Add($"PLAYER: {playerMessage}");
            session.Transcript.Add($"NPC: {body}");
            session.LastActive = DateTime.UtcNow;
            return (formatted, body);
        }

        private string Truncate(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return s;
            if (s.Length <= max) return s;
            return s.Substring(0, max - 3) + "...";
        }

        private string RenderScreen(string npcName, string screenTemplate, string body)
        {
            if (string.IsNullOrEmpty(screenTemplate))
            {
                return $"**{npcName}**\n\n{body}";
            }
            // substitute {npc.name} and {body}
            var outp = screenTemplate.Replace("{npc.name}", npcName).Replace("{body}", body);
            return outp;
        }

    }
}
