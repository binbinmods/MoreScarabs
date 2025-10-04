using HarmonyLib;
using System;

namespace MoreScarabs
{
    [HarmonyPatch] // DO NOT REMOVE/CHANGE - This tells your plugin that this is part of the mod
    public class AntiCheat
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamManager), "SetWeeklyScore")]
        public static bool SetWeeklyScorePrefix(ref SteamManager __instance,
                    int score,
                    int week,
                    string nick,
                    string nickgroup,
                    bool singleplayer = true)
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamManager), "SetWeeklyScoreLeaderboard")]
        public static bool SetWeeklyScoreLeaderboardPrefix(ref SteamManager __instance, int score,
            int week,
            string nick,
            string nickgroup,
            bool singleplayer = true)
        {
            return false;
        }

    }
}