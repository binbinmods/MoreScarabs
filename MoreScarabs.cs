using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using static Obeliskial_Essentials.Essentials;
using System;
using static MoreScarabs.CustomFunctions;
using System.Text.RegularExpressions;
using System.Reflection;

// Make sure your namespace is the same everywhere
namespace MoreScarabs
{

    [HarmonyPatch] //DO NOT REMOVE/CHANGE

    public class MoreScarabPatches
    {
        public static int i = 1;
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AtOManager), nameof(AtOManager.BeginAdventure))]
        public static void BeginAdventurePostfix()
        {
            PLog("More Scarab Test");
            // i++;
        }

        public static void CreateNPCLocal(NPCData _npcData,
            string effectTarget = "",
            int _position = -1,
            bool generateFromReload = false,
            string internalId = "",
            CardData _cardActive = null) 
        {
            // PLog("Testing Reflection version before code");
            // MethodInfo methodInfo = typeof(MatchManager).GetMethod("CreateNPC", BindingFlags.NonPublic | BindingFlags.Instance);
            // var parameters = new object[] { _npcData, effectTarget,_position,generateFromReload,internalId,_cardActive};            
            // methodInfo.Invoke(null,parameters);
            // PLog("Testing Reflection version after code");

            // Alternative
            PLog("Testing Traverse Create NPC - START");
            object[] arguments = [_npcData, effectTarget,_position,generateFromReload,internalId,_cardActive];
            MatchManager matchManager = MatchManager.Instance;
            Traverse.Create(matchManager).Method("CreateNPC").GetValue(arguments);
            PLog("Testing Traverse Create NPC - END");

        }

        // [HarmonyReversePatch]
        // [HarmonyPatch(typeof(MatchManager), "CreateNPC")]
        // public static void CreateNPCReversePatch(NPCData _npcData,
        //     string effectTarget = "",
        //     int _position = -1,
        //     bool generateFromReload = false,
        //     string internalId = "",
        //     CardData _cardActive = null) 
        // {
        //     //This is intentionally a stub
        //     PLog("Executing CreateNPCReversePatch");
        // }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(MatchManager), nameof(MatchManager.EndTurn))]
        public static void EndTurnPrefix(MatchManager __instance, bool forceIt = false)
        {
            
            PLog("End Turn Prefix for Jades - START");
            PLog("Jade START");

            MatchManager matchManager = __instance;
            

            if (matchManager == null || matchManager.MatchIsOver)
                return;

            // PLog("Match Isn't over");
             
            if (Traverse.Create(matchManager).Field("gameStatus").GetValue<string>() == nameof (matchManager.EndTurn) && !forceIt)
                return;
            
            // PLog("GameStatus isn't EndTurn");

            string scarabSpawned = Traverse.Create(matchManager).Field("scarabSpawned").GetValue<string>();
            CombatData combatData = Traverse.Create(matchManager).Field("combatData").GetValue<CombatData>();
            int currentRound = Traverse.Create(matchManager).Field("currentRound").GetValue<int>();
            NPC[] TeamNPC = Traverse.Create(matchManager).Field("TeamNPC").GetValue<NPC[]>();

            // PLog("Loaded Traverses");

            if (currentRound == 0 || scarabSpawned != "" || (UnityEngine.Object)combatData == (UnityEngine.Object)null || (UnityEngine.Object)Globals.Instance.GetNodeData(AtOManager.Instance.currentMapNode) == (UnityEngine.Object)null)
            {
                if (currentRound ==0 ) PLog("Round = 0");
                if (scarabSpawned != "" ) PLog("Scarab Spawned: " + scarabSpawned);
                if ((UnityEngine.Object)combatData == (UnityEngine.Object)null ) PLog("null combat");
                if ((UnityEngine.Object)Globals.Instance.GetNodeData(AtOManager.Instance.currentMapNode) == (UnityEngine.Object)null ) PLog("null mapNode");
                return;
            }
                

            bool guaranteedSpawn = true; //Plugin.GuaranteedSpawn.Value;
            bool onlyJade = true; //Plugin.OnlySpawnJades.Value
            int percentToSwawn = 7; //Plugin.PercentChanceToSpawn.Value;

            PLog("Guaranteed Spawn " + guaranteedSpawn);
            PLog("Guaranteed Jade " + onlyJade);
            int spawnChance = guaranteedSpawn ? 100 : percentToSwawn;

            int scarabType = onlyJade ? 2 : matchManager.GetRandomIntRange(0, 4);
            
            // if scarabType==2 then Jade, if 1 then Gold, if 0 then Crystal. Else scourge
            scarabSpawned = scarabType == 2 ? "jadescarab" : scarabType == 1 ? "goldenscarab" : scarabType == 0 ? "crystalscarab" : "scourgescarab";

            bool isValidCombat = false;
            switch (Globals.Instance.GetNodeData(AtOManager.Instance.currentMapNode).NodeCombatTier)
            {
                case Enums.CombatTier.T2:
                    if (GameManager.Instance.IsObeliskChallenge())
                    {
                        isValidCombat = true;
                        break;
                    }
                    break;
                case Enums.CombatTier.T3:
                    isValidCombat = true;
                    break;
                case Enums.CombatTier.T4:
                    isValidCombat = true;
                    break;
                case Enums.CombatTier.T5:
                    isValidCombat = true;
                    break;
                case Enums.CombatTier.T6:
                    if (GameManager.Instance.IsObeliskChallenge())
                    {
                        isValidCombat = true;
                        break;
                    }
                    break;
                case Enums.CombatTier.T7:
                    if (GameManager.Instance.IsObeliskChallenge())
                    {
                        isValidCombat = true;
                        break;
                    }
                    break;
            }
            PLog("Is it a valid combat: " + isValidCombat);
            if (isValidCombat)
            {
                int i = matchManager.GetNPCAvailablePosition();
                if (i > -1)
                {
                    int randomIntRange = matchManager.GetRandomIntRange(0, 100);
                    if (randomIntRange < spawnChance)
                    {
                        // scarabType = 0;
                        string str = "";
                        if (!GameManager.Instance.IsObeliskChallenge())
                        {
                            if (AtOManager.Instance.GetTownTier() == 2)
                                str = AtOManager.Instance.GetNgPlus() <= 0 ? "_b" : "_plus_b";
                            else if (AtOManager.Instance.GetNgPlus() > 0)
                                str = "_plus";
                        }
                        else if (AtOManager.Instance.GetObeliskMadness() > 8)
                            str = combatData.CombatTier == Enums.CombatTier.T6 || combatData.CombatTier == Enums.CombatTier.T7 ? "_plus_b" : "_plus";
                        else if (combatData.CombatTier == Enums.CombatTier.T6 || combatData.CombatTier == Enums.CombatTier.T7)
                            str = "_b";
                        // scarabSpawned = scarabType != 0 ? (scarabType != 1 ? (scarabType != 2 ? "scourgescarab" : "jadescarab") : "goldenscarab") : "crystalscarab";
                        NPCData npc = Globals.Instance.GetNPC(scarabSpawned + str);
                        if ((UnityEngine.Object)npc == (UnityEngine.Object)null)
                        {
                            PLog("scarabData Null for scarab => " + scarabSpawned + str);
                        }
                        else
                        {
                            CreateNPCLocal(npc,_position:i);
                            // CreateNPCReversePatch(npc, _position: i);
                            Globals.Instance.WaitForSeconds(0.5f);
                            if (scarabType != 3)
                                TeamNPC[i].SetAura((Character)null, Globals.Instance.GetAuraCurseData("luckyscarab"), 1);
                            TeamNPC[i].SetAura((Character)null, Globals.Instance.GetAuraCurseData("sight"), 3);
                            if (scarabType == 1)
                            {
                                if (AtOManager.Instance.GetNgPlus() == 0)
                                {
                                    if (AtOManager.Instance.GetTownTier() == 1)
                                        TeamNPC[i].SetAura((Character)null, Globals.Instance.GetAuraCurseData("zeal"), 5);
                                    else
                                        TeamNPC[i].SetAura((Character)null, Globals.Instance.GetAuraCurseData("zeal"), 7);
                                }
                                else if (AtOManager.Instance.GetTownTier() == 1)
                                    TeamNPC[i].SetAura((Character)null, Globals.Instance.GetAuraCurseData("zeal"), 6);
                                else
                                    TeamNPC[i].SetAura((Character)null, Globals.Instance.GetAuraCurseData("zeal"), 7);
                            }
                            else if (scarabType == 2)
                            {
                                if (AtOManager.Instance.GetNgPlus() == 0)
                                {
                                    if (AtOManager.Instance.GetTownTier() == 1)
                                    {
                                        TeamNPC[i].SetAura((Character)null, Globals.Instance.GetAuraCurseData("evasion"), 5);
                                        TeamNPC[i].SetAura((Character)null, Globals.Instance.GetAuraCurseData("buffer"), 5);
                                    }
                                    else
                                    {
                                        TeamNPC[i].SetAura((Character)null, Globals.Instance.GetAuraCurseData("evasion"), 7);
                                        TeamNPC[i].SetAura((Character)null, Globals.Instance.GetAuraCurseData("buffer"), 7);
                                    }
                                }
                                else if (AtOManager.Instance.GetTownTier() == 1)
                                {
                                    TeamNPC[i].SetAura((Character)null, Globals.Instance.GetAuraCurseData("evasion"), 6);
                                    TeamNPC[i].SetAura((Character)null, Globals.Instance.GetAuraCurseData("buffer"), 6);
                                }
                                else
                                {
                                    TeamNPC[i].SetAura((Character)null, Globals.Instance.GetAuraCurseData("evasion"), 7);
                                    TeamNPC[i].SetAura((Character)null, Globals.Instance.GetAuraCurseData("buffer"), 7);
                                }
                            }
                            GameManager.Instance.PlayLibraryAudio("glitter", 0.25f);
                            Globals.Instance.WaitForSeconds(0.5f);
                        }
                    }
                }
            }

            // PLog("Attempting to set traverses");

            Traverse.Create(matchManager).Field("scarabSpawned").SetValue(scarabSpawned);
            Traverse.Create(matchManager).Field("combatData").SetValue(combatData);
            Traverse.Create(matchManager).Field("teamNPC").SetValue(TeamNPC);
            // PLog("traverses set");
            PLog("End Turn Prefix for Jades - END");
        }
    }
}
