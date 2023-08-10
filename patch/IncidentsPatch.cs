using Assets.Scripts;
using Assets.Scripts.Networking;
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

        public SetWorldEnvironmentsPatch()
        {
            incidentsName.Add("AddIncident");
        }

        [UsedImplicitly]
        [HarmonyPrefix]
        public static void prefixPatch(ref TerrainFeatureSettings __instance)
        {
            EventsMod.Log("SetWorldEnvironmentsPatch:: prefixPatch --> carregando Incidents", EventsMod.Logs.DEBUG);
            SetWorldEnvironmentsPatch incidentsPatch = new SetWorldEnvironmentsPatch();
            EventsMod.Log("SetWorldEnvironmentsPatch:: prefixPatch --> get incidentsName size " + incidentsName.Count(), EventsMod.Logs.DEBUG);
            IEnumerable<TerrainFeatureIncident> lt = incidentsPatch.addIncidents(incidentsName);
            EventsMod.Log("SetWorldEnvironmentsPatch:: prefixPatch --> carregando "+ lt.Count()+ " Incidents", EventsMod.Logs.INFO);
            foreach (TerrainFeatureIncident incident in lt)
            {
                if (!TileSystem.Instance.CurrentIncidentTypes.ContainsKey(incident.Type))
                {
                    TileSystem.Instance.CurrentIncidentTypes.Add(incident.Type, incident);
                    TileSystem.WorldContainsIncidents = true;
                    EventsMod.Log("SetWorldEnvironmentsPatch:: TakePlantDrinkPatch --> adicionado Incident " + incident.Type, EventsMod.Logs.DEBUG);
                }
            }
            EventsMod.Log("SetWorldEnvironmentsPatch:: prefixPatch --> carregado", EventsMod.Logs.INFO);

            //__instance.IncidentFeatures.AddRange(incidentsPatch.addIncidents(incidentsName));
        }

        private IEnumerable<TerrainFeatureIncident> addIncidents(List<string> incidentsName)
        {
            EventsMod.Log("SetWorldEnvironmentsPatch:: addIncidents --> carregando Incidents", EventsMod.Logs.DEBUG);
            List<TerrainFeatureIncident> response = new List<TerrainFeatureIncident>();
            foreach (string incident in incidentsName)
            {
                EventsMod.Log("SetWorldEnvironmentsPatch:: addIncidents --> Incident "+ incident, EventsMod.Logs.DEBUG);
                TerrainFeatureIncident terrainFeatureIncident = new TerrainFeatureIncident();

                try
                {
                    response.Add((TerrainFeatureIncident)GetType().GetMethod(incident, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, new object[] { terrainFeatureIncident }));
                }catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
                EventsMod.Log("SetWorldEnvironmentsPatch:: addIncidents --> adicionado " + incident, EventsMod.Logs.DEBUG);
            }
            return response;
        }

        // -------------------------------incidents-------------------------------------------------------

        private TerrainFeatureIncident AddIncident(TerrainFeatureIncident terrainFeatureIncident)
        {
            EventsMod.Log("SetWorldEnvironmentsPatch:: AddIncident --> carregou Incidents " + 44, EventsMod.Logs.INFO);
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
