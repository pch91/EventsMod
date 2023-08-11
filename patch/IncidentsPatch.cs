using Assets.Scripts;
using Assets.Scripts.Networking;
using Assets.Scripts.Objects.Pipes;
using BepInEx.Configuration;
using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static WorldManager;

namespace EventsMod.patch
{
    [HarmonyPatch(typeof(WorldManager))]
    [HarmonyPatch("SetWorldEnvironments")]
    public class SetWorldEnvironmentsPatch
    {
        static List<string> incidentsName = new List<string>();
        String chanincparam = (EventsMod.fconfigEvents["chanincparam"] as ConfigEntry<string>).Value;

        public SetWorldEnvironmentsPatch()
        {
            addMethodNames();
        }

        [UsedImplicitly]
        [HarmonyPrefix]
        public static void prefixPatch(ref TerrainFeatureSettings __instance)
        {
            if (bool.Parse(EventsMod.fmainconfig["EnabledMod"]?.ToString())) {
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
            //__instance.IncidentFeatures.AddRange(incidentsPatch.addIncidents(incidentsName));
        }

        private IEnumerable<TerrainFeatureIncident> addIncidents(List<string> incidentsName)
        {
            EventsMod.log("SetWorldEnvironmentsPatch:: addIncidents --> carregando Incidents", EventsMod.Logs.DEBUG);
            List<TerrainFeatureIncident> response = new List<TerrainFeatureIncident>();

            response.AddRange(confiIncidents());

            foreach (string incident in incidentsName)
            {
                EventsMod.log("SetWorldEnvironmentsPatch:: addIncidents --> Incident "+ incident, EventsMod.Logs.DEBUG);
                TerrainFeatureIncident terrainFeatureIncident = new TerrainFeatureIncident();

                try
                {
                    response.Add((TerrainFeatureIncident)GetType().GetMethod(incident, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, new object[] { terrainFeatureIncident }));
                }catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
                EventsMod.log("SetWorldEnvironmentsPatch:: addIncidents --> adicionado " + incident, EventsMod.Logs.DEBUG);
            }
            return response;
        }

        // -------------------------------incidents-------------------------------------------------------
        private List<TerrainFeatureIncident> confiIncidents()
        {
            EventsMod.log("SetWorldEnvironmentsPatch:: confiIncidents --> adicionando config incidents ", EventsMod.Logs.DEBUG);
            List<TerrainFeatureIncident> response = new List<TerrainFeatureIncident>();
            String confcinc = (EventsMod.fconfigEvents["confcinc"] as ConfigEntry<string>).Value;
            if (!String.IsNullOrEmpty(confcinc)) {
                List<String> configEvent = confcinc.Split('#').ToList();
                foreach (var item in configEvent)
                {
                    TerrainFeatureIncident terrainFeatureIncident = new TerrainFeatureIncident();
                    List<String> values = item.Split('|').ToList();
                    EventsMod.log("SetWorldEnvironmentsPatch:: confiIncidents --> adicionando type: " + values[0], EventsMod.Logs.DEBUG);
                    terrainFeatureIncident.Type = int.Parse(values[0]);
                    terrainFeatureIncident.MaxPerTile = int.Parse(values[1]);
                    terrainFeatureIncident.SpawnChance = int.Parse(values[2]);
                    terrainFeatureIncident.IsRepeating = bool.Parse(values[3]);
                    terrainFeatureIncident.MaxDelay = int.Parse(values[4]);
                    terrainFeatureIncident.MinDelay = int.Parse(values[5]);
                    terrainFeatureIncident.CanLaunchOutsideTile = bool.Parse(values[6]);
                    terrainFeatureIncident.Serialize = bool.Parse(values[7]);
                    terrainFeatureIncident.RequiresHumanInTile = bool.Parse(values[8]);
                    terrainFeatureIncident.RunOnTileEnter = bool.Parse(values[9]);
                    terrainFeatureIncident.ContainStructures = bool.Parse(values[10]);
                    response.Add(terrainFeatureIncident);
                    EventsMod.log("SetWorldEnvironmentsPatch:: confiIncidents --> adicionado", EventsMod.Logs.DEBUG);
                }
            }
            return response;
        }

        private void addMethodNames()
        {
            EventsMod.log("SetWorldEnvironmentsPatch:: addMethodNames --> adicionando incidents names", EventsMod.Logs.DEBUG);
            String incMetName = (EventsMod.fconfigEvents["incMetName"] as ConfigEntry<string>).Value;
            if (!String.IsNullOrEmpty(incMetName)) {
                List<String> configEvent = incMetName.Split('#').ToList();
                foreach (var item in configEvent)
                {
                    EventsMod.log("SetWorldEnvironmentsPatch:: addMethodNames --> adicionando " + item, EventsMod.Logs.DEBUG);
                    incidentsName.Add(item);
                    EventsMod.log("SetWorldEnvironmentsPatch:: addMethodNames --> adicionado", EventsMod.Logs.DEBUG);
                }
            }
        }

        private TerrainFeatureIncident testIncident(TerrainFeatureIncident terrainFeatureIncident)
        {
            terrainFeatureIncident = AddIncident(44, 1, 2000, true, 5, 0, true, false, false, false, true,
                terrainFeatureIncident, System.Reflection.MethodBase.GetCurrentMethod().Name);
            return terrainFeatureIncident;
        }


        private TerrainFeatureIncident AddIncident(int type, int maxPerTile, int spawnChance, bool isRepeating,
            int maxDelay, int minDelay, bool canLaunchOutsideTile, bool serialize, bool requiresHumanInTile,
            bool runOnTileEnter, bool containStructures, TerrainFeatureIncident terrainFeatureIncident, String methodname = null){

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

            if (methodname != null && !String.IsNullOrEmpty(chanincparam))
            {
                List<String> configEvent = chanincparam.Split('#').ToList();

                foreach (var item in configEvent)
                {
                    List<String> values = item.Split('|').ToList();

                    //nameMethod | Param | value
                    if (methodname.Equals(values[0]))
                    {
                        EventsMod.log("SetWorldEnvironmentsPatch:: AddIncident --> Alterando  Incidents " + values[0] +" -> "+ values[1], EventsMod.Logs.INFO);

                        terrainFeatureIncident.GetType().GetField(values[1]).SetValue(terrainFeatureIncident, values[2]);
                    }
                }
            }

            EventsMod.log("SetWorldEnvironmentsPatch:: AddIncident --> carregou Incidents " + type, EventsMod.Logs.INFO);

            return terrainFeatureIncident;
        }





    }
    /*
    [HarmonyPatch(typeof(TerrainFeatureSettings))]
    [HarmonyPatch("Read")]
    public class IncidentsPatch
    {
        static List<string> incidentsName = new List<string>();

        [UsedImplicitly]
        [HarmonyPostfix]
        public static void TakePlantDrinkPatch(RocketBinaryReader reader, ref TerrainFeatureSettings __instance)
        {
            EventsMod.Log("IncidentsPatch:: TakePlantDrinkPatch --> carregando Incidents", EventsMod.Logs.INFO);
            IncidentsPatch incidentsPatch = new IncidentsPatch();
            __instance.IncidentFeatures.AddRange(incidentsPatch.addIncidents(incidentsName));
        }


        public IEnumerable<TerrainFeatureIncident> addIncidents(List<string> incidentsName)
        {
            List<TerrainFeatureIncident> response = new List<TerrainFeatureIncident>();
            foreach (string incident in incidentsName)
            {
                TerrainFeatureIncident terrainFeatureIncident = new TerrainFeatureIncident();

                response.Add((TerrainFeatureIncident)GetType().GetMethod(incident).Invoke(this, new object[] { terrainFeatureIncident }));

            }
            return response;
        }

        public void AddIncident(TerrainFeatureIncident terrainFeatureIncident)
        {
            EventsMod.Log("IncidentsPatch:: TakePlantDrinkPatch --> carregou Incidents " + 44, EventsMod.Logs.INFO);
            terrainFeatureIncident.Type = 44;
            terrainFeatureIncident.MaxPerTile = 1;
            terrainFeatureIncident.SpawnChance = 2000;
            terrainFeatureIncident.IsRepeating = true;
            terrainFeatureIncident.MaxDelay = 5;
            terrainFeatureIncident.MinDelay = 0;
            terrainFeatureIncident.CanLaunchOutsideTile = true;
            terrainFeatureIncident.Serialize = false;
            terrainFeatureIncident.RequiresHumanInTile = false;
            terrainFeatureIncident.RunOnTileEnter = false;
            terrainFeatureIncident.ContainStructures = true;
        }
    }*/
}
