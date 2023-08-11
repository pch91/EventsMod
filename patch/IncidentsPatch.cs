using Assets.Scripts;
using Assets.Scripts.Networking;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;
using static WorldManager;

namespace EventsMod.patch
{
    [HarmonyPatch(typeof(WorldManager))]
    [HarmonyPatch("SetWorldEnvironments")]
    public class SetWorldEnvironmentsPatch
    {
        static List<string> incidentsName = new List<string>();

        public SetWorldEnvironmentsPatch()
        {
            if (!String.IsNullOrEmpty(StaticAttributes.incMetName))
            {
                addMethodNames();
            }
        }

        [UsedImplicitly]
        [HarmonyPrefix]
        public static void prefixPatch(ref TerrainFeatureSettings __instance)
        {
            try
            {
                if (bool.Parse(EventsMod.fmainconfig["EnabledMod"]?.ToString()))
                {
                    EventsMod.log("SetWorldEnvironmentsPatch:: prefixPatch --> carregando Incidents", EventsMod.Logs.DEBUG);
                    SetWorldEnvironmentsPatch incidentsPatch = new SetWorldEnvironmentsPatch();
                    EventsMod.log("SetWorldEnvironmentsPatch:: prefixPatch --> get incidentsName size " + incidentsName.Count(), EventsMod.Logs.DEBUG);
                    IEnumerable<TerrainFeatureIncident> lt = incidentsPatch.addIncidents(incidentsName);
                    EventsMod.log("SetWorldEnvironmentsPatch:: prefixPatch --> carregando " + lt.Count() + " Incidents", EventsMod.Logs.INFO);
                    foreach (TerrainFeatureIncident incident in lt)
                    {
                        if (!TileSystem.Instance.CurrentIncidentTypes.ContainsKey(incident.Type))
                        {
                            TileSystem.Instance.CurrentIncidentTypes.Add(incident.Type, incident);
                            TileSystem.WorldContainsIncidents = true;
                            EventsMod.log("SetWorldEnvironmentsPatch:: TakePlantDrinkPatch --> adicionado Incident " + incident.Type, EventsMod.Logs.DEBUG);
                        }
                    }
                    EventsMod.log("SetWorldEnvironmentsPatch:: prefixPatch --> carregado", EventsMod.Logs.INFO);
                }
            }catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private IEnumerable<TerrainFeatureIncident> addIncidents(List<string> incidentsName)
        {
            EventsMod.log("SetWorldEnvironmentsPatch:: addIncidents --> carregando Incidents", EventsMod.Logs.DEBUG);
            List<TerrainFeatureIncident> response = new List<TerrainFeatureIncident>();
            foreach (string incident in incidentsName)
            {
                EventsMod.log("SetWorldEnvironmentsPatch:: addIncidents --> Incident " + incident, EventsMod.Logs.DEBUG);
                TerrainFeatureIncident terrainFeatureIncident = new TerrainFeatureIncident();

                try
                {
                    response.Add((TerrainFeatureIncident)GetType().GetMethod(incident, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, new object[] { terrainFeatureIncident }));
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
                EventsMod.log("SetWorldEnvironmentsPatch:: addIncidents --> adicionado " + incident, EventsMod.Logs.DEBUG);
            }
            if (!String.IsNullOrEmpty(StaticAttributes.confcinc))
            {
                response.AddRange(confiIncidents());
            }
            return response;
        }

        // -------------------------------incidents-------------------------------------------------------

        private IEnumerable<TerrainFeatureIncident> confiIncidents()
        {
            EventsMod.log("SetWorldEnvironmentsPatch:: confiIncidents --> adicionando config incidents ", EventsMod.Logs.DEBUG);
            List<TerrainFeatureIncident> response = new List<TerrainFeatureIncident>();
            List<String> configEvent = StaticAttributes.confcinc.Split('#').ToList();
            foreach (var item in configEvent)
            {
                TerrainFeatureIncident terrainFeatureIncident = new TerrainFeatureIncident();
                List<String> values = item.Split('|').ToList();
                EventsMod.log("SetWorldEnvironmentsPatch:: confiIncidents --> adicionando value: " + item, EventsMod.Logs.DEBUG);
                EventsMod.log("SetWorldEnvironmentsPatch:: confiIncidents --> adicionando type: " + values[0], EventsMod.Logs.DEBUG);
                terrainFeatureIncident.Type = Int32.Parse(values[0].Trim());
                terrainFeatureIncident.MaxPerTile = Int32.Parse(values[1].Trim());
                terrainFeatureIncident.SpawnChance = Int32.Parse(values[2].Trim());
                terrainFeatureIncident.IsRepeating = Boolean.Parse(values[3].Trim());
                terrainFeatureIncident.MaxDelay = Int32.Parse(values[4].Trim());
                terrainFeatureIncident.MinDelay = Int32.Parse(values[5].Trim());
                terrainFeatureIncident.CanLaunchOutsideTile = Boolean.Parse(values[6].Trim());
                terrainFeatureIncident.Serialize = Boolean.Parse(values[7].Trim());
                terrainFeatureIncident.RequiresHumanInTile = Boolean.Parse(values[8].Trim());
                terrainFeatureIncident.RunOnTileEnter = Boolean.Parse(values[9].Trim());
                terrainFeatureIncident.ContainStructures = Boolean.Parse(values[10].Trim());
                response.Add(terrainFeatureIncident);
                EventsMod.log("SetWorldEnvironmentsPatch:: confiIncidents --> adicionado", EventsMod.Logs.DEBUG);
            }
            return response;
        }

        private TerrainFeatureIncident testIncident(TerrainFeatureIncident terrainFeatureIncident)
        {
            terrainFeatureIncident = AddIncident(97, 1, 2000, true, 5, 0, true, false, false, false, true,
                terrainFeatureIncident, System.Reflection.MethodBase.GetCurrentMethod().Name);
            return terrainFeatureIncident;
        }


        private TerrainFeatureIncident AddIncident(int type, int maxPerTile, int spawnChance, bool isRepeating,
            int maxDelay, int minDelay, bool canLaunchOutsideTile, bool serialize, bool requiresHumanInTile,
            bool runOnTileEnter, bool containStructures, TerrainFeatureIncident terrainFeatureIncident, String methodname = null)
        {

            terrainFeatureIncident.Type = type;
            terrainFeatureIncident.MaxPerTile = maxPerTile;
            terrainFeatureIncident.SpawnChance = spawnChance;
            terrainFeatureIncident.IsRepeating = isRepeating;
            terrainFeatureIncident.MaxDelay = maxDelay;
            terrainFeatureIncident.MinDelay = minDelay;
            terrainFeatureIncident.CanLaunchOutsideTile = canLaunchOutsideTile;
            terrainFeatureIncident.Serialize = serialize;
            terrainFeatureIncident.RequiresHumanInTile = requiresHumanInTile;
            terrainFeatureIncident.RunOnTileEnter = runOnTileEnter;
            terrainFeatureIncident.ContainStructures = containStructures;

            if (methodname != null && !String.IsNullOrEmpty(StaticAttributes.chanincparam))
            {
                List<String> configEvent = StaticAttributes.chanincparam.Split('#').ToList();

                foreach (var item in configEvent)
                {
                    List<String> values = item.Split('|').ToList();

                    //nameMethod | Param | value
                    if (methodname.Equals(values[0]))
                    {
                        EventsMod.log("SetWorldEnvironmentsPatch:: AddIncident --> Alterando  Incidents " + values[0] + " -> " + values[1], EventsMod.Logs.INFO);

                        terrainFeatureIncident.GetType().GetField(values[1]).SetValue(terrainFeatureIncident, values[2]);
                    }
                }
            }

            EventsMod.log("SetWorldEnvironmentsPatch:: AddIncident --> carregou Incidents " + type, EventsMod.Logs.INFO);

            return terrainFeatureIncident;
        }
        private void addMethodNames()
        {
            EventsMod.log("SetWorldEnvironmentsPatch:: addMethodNames --> adicionando incidents names", EventsMod.Logs.DEBUG);
            List<String> configEvent = StaticAttributes.incMetName.Split('#').ToList();
            foreach (var item in configEvent)
            {
                EventsMod.log("SetWorldEnvironmentsPatch:: addMethodNames --> adicionando " + item, EventsMod.Logs.DEBUG);
                incidentsName.Add(item);
                EventsMod.log("SetWorldEnvironmentsPatch:: addMethodNames --> adicionado", EventsMod.Logs.DEBUG);
            }
        }


    }
    
    [HarmonyPatch(typeof(Incident))]
    public class IncidentsLog3
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Incident), "GetIncident")]
        public static void AddIncidentLog3(DirectoryInfo folder, object[] __args)
        {
            if (folder.FullName.Contains("EventsModGameData")) {
                EventsMod.log("IncidentsLog3:: AddIncidentLog3 --> Load Incident: " + __args[0].ToString() , EventsMod.Logs.DEBUG);
            }
        }
    }

    [HarmonyPatch(typeof(ModData))]
    public class ChangeGameDatafolder
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModData), "GameDataFolder")]
        public static void ChangeGameDatafolderPath(ref DirectoryInfo __result)
        {
            if (bool.Parse(EventsMod.fmainconfig["EnabledMod"]?.ToString()))
            {
                if (__result.FullName.Contains("\\Stationeers\\mods\\EventsMod\\GameData"))
                {
                    EventsMod.log("ChangeGameDatafolder:: ChangeGameDatafolderPath --> Original Path: " + __result.FullName, EventsMod.Logs.DEBUG);
                    __result = new DirectoryInfo(__result.FullName.Replace("GameData", "EventsModGameData"));
                }
            }
        }
    }
}