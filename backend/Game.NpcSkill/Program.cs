using System;
using Game.NpcSkill.Dialog;

Console.WriteLine("NpcSkill demo starting...");
var dataPath = Path.Combine(AppContext.BaseDirectory, "Data", "NPCs");
var runner = new DialogRunner(dataPath);
var list = runner.ListNpcs();
Console.WriteLine($"Found NPCs: {string.Join(", ", list)}");

var snapshot = new PlayerSnapshot();
snapshot["player.name"] = "Alice";
snapshot["player.level"] = 4;
snapshot["player.stats.hp"] = 12;

var (session, opening) = runner.StartSession("old_sage", "alice", snapshot);
Console.WriteLine("--- Opening ---\n");
Console.WriteLine(opening);

var (replyFormatted, raw) = runner.SendMessage(session.Id, "hello, can you tell me about the relic?", snapshot);
Console.WriteLine("\n--- NPC Reply ---\n");
Console.WriteLine(replyFormatted);

Console.WriteLine("\nDemo complete.");
