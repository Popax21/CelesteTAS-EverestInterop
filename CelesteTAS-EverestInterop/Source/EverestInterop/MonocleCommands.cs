using System.Linq;
using Celeste;
using Monocle;
using TAS.Utils;

namespace TAS.EverestInterop;

public static class MonocleCommands {
    [Command("clrsav", "clears save data on debug file (CelesteTAS)")]
    private static void CmdClearSave() {
        SaveData.TryDelete(-1);
        SaveData.Start(new SaveData {Name = "debug"}, -1);
        // Pretend that we've beaten Prologue.
        LevelSetStats stats = SaveData.Instance.GetLevelSetStatsFor("Celeste");
        stats.UnlockedAreas = 1;
        stats.AreasIncludingCeleste[0].Modes[0].Completed = true;
    }

    [Command("hearts",
        "sets the amount of obtained hearts for the specified level set to a given number (default all hearts and current level set) (support mini heart door via CelesteTAS)")]
    private static void CmdHearts(int amount = int.MaxValue, string levelSet = null) {
        SaveData saveData = SaveData.Instance;
        if (saveData == null) {
            return;
        }

        if (string.IsNullOrEmpty(levelSet)) {
            const string miniHeartDoorFullName = "Celeste.Mod.CollabUtils2.Entities.MiniHeartDoor";

            if (Engine.Scene.Entities.FirstOrDefault(e => e.GetType().FullName == miniHeartDoorFullName) is { } miniHeartDoor) {
                levelSet = miniHeartDoor.GetFieldValue<string>("levelSet");
            } else {
                levelSet = saveData.GetLevelSet();
            }
        }

        int num = 0;
        foreach (AreaStats areaStats in saveData.Areas_Safe.Where(stats => stats.LevelSet == levelSet)) {
            for (int i = 0; i < areaStats.Modes.Length; i++) {
                if (AreaData.Get(areaStats.ID).Mode is not { } mode || mode.Length <= i || mode[i]?.MapData == null)
                    continue;

                AreaModeStats areaModeStats = areaStats.Modes[i];
                if (num < amount) {
                    areaModeStats.HeartGem = true;
                    num++;
                } else {
                    areaModeStats.HeartGem = false;
                }
            }
        }
    }
}