using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventsMod
{
    [BepInPlugin("EventsMod", "Events Mod", "0.0.0.1")]
    public class EventsMod : BaseUnityPlugin
    {
        public static EventsMod Instance;

        private static string loglevel = "INFO";

        public enum Logs
        {
            DEBUG = 1,
            ERROR = 2,
            INFO = 0,
        }

        public static void log(string line, Logs level)
        {
            //Debug.Log((int)Enum.Parse(typeof(Logs), loglevel));

            if ((int)Enum.Parse(typeof(Logs), loglevel) - (int)level < 1)
            {
                Debug.Log("["+ level + "    :   EventsMod] " + line);
            }
        }


        public static Dictionary<string, object> fmainconfig = new Dictionary<string, object>();
        public static Dictionary<string, object> fconfigEvents = new Dictionary<string, object>();
        private Dictionary<string, object> mainconfigs = new Dictionary<string, object>();
        private Dictionary<string, object> configs = new Dictionary<string, object>();



        private void Awake()
        {
            try
            {
                log("Start - EventsMod", Logs.INFO);
                Instance = this;
                //Harmony.DEBUG = true;
                var harmony = new Harmony("net.pch91.stationeers.EventsMod.patch");
                LoadConfigsOnDictionary();
                Handleconfig();
                harmony.PatchAll();
                log("Finish patch", Logs.INFO);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void Handleconfig()
        {
            mainconfigs.Add("LogEnabled", Config.Bind("0 - General configuration", "Log Level", "debug", "Enable or disable logs. values can be debug , info or error"));
            mainconfigs.Add("EnabledMod", Config.Bind("0 - General configuration", "Eneble mod", true, "Enable or disable mod. values can be false or true"));
            
            fconfigEvents.Add("confcinc", Config.Bind("1 - Incidents", "Config custom incidents Values", "", "Type number need have grater than 100 \n For add more than one only separate with # \n For each one need stay in format - type|MaxPerTile|SpawnChance|IsRepeating|MaxDelay|MinDelay|CanLaunchOutsideTile|Serialize|RequiresHumanInTile|RunOnTileEnter|ContainStructures"));
            fconfigEvents.Add("incMetName", Config.Bind("1 - Incidents", "Incdents Method Name", "testIncident", "for use one pre configured incidents method put there.\n For more than one use # for separeted"));
            fconfigEvents.Add("chanincparam", Config.Bind("1 - Incidents", "Change Incdents Method Param", "", "need stay in this format nameMethod|Param|value \n For more than one use # for separeted"));
            fmainconfig.Add("EnabledMod", (mainconfigs["EnabledMod"] as ConfigEntry<bool>)?.Value);
 
            loglevel = (mainconfigs["LogEnabled"] as ConfigEntry<string>).Value.ToUpper();
        }

        private void LoadConfigsOnDictionary()
        {

        }
    }
}