using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using VRC.Core;
using static Dawn.Mic.MicSensitivity;

namespace Dawn
{
    internal static class Core
    {
        internal static ApiWorld currentRoom()
        {
        #if Unobfuscated
             return RoomManager.currentRoom; //field_Internal_Static_ApiWorld_0
        #else
            return FindInstance(typeof(RoomManager), typeof(ApiWorld)).TryCast<ApiWorld>(); //Used to be RoomManager.field_Internal_Static_ApiWorld_0
        #endif
        }
        internal static ApiWorldInstance currentWorldInstance()
        {
        #if Unobfuscated
            return RoomManager.currentWorldInstance; //field_Internal_Static_ApiWorldInstance_0 
        #else
            return FindInstance(typeof(RoomManager), typeof(ApiWorldInstance)).TryCast<ApiWorldInstance>(); //Used to be RoomManager.field_Internal_Static_ApiWorldInstance_0
        #endif
        }
        internal static bool IsInWorld()
        {
            return currentRoom() != null || currentWorldInstance() != null;
        }
        private static float infoIndex;
        internal static float userVolumeThreshold
        {
            get 
            {
                if (VolumeThreasholdInfo != null) return float.Parse(VolumeThreasholdInfo.GetValue(uInstance).ToString());
                VolumeThreasholdInfo = GetInfo("0.01"); // This should only ever be called on WorldJoin and never again.
                return float.Parse(VolumeThreasholdInfo.GetValue(uInstance).ToString());
            }
            set
            {
                if (VolumeThreasholdInfo != null) VolumeThreasholdInfo.SetValue(uInstance, value);
                else
                {
                    VolumeThreasholdInfo = GetInfo("0.01"); // This should only ever be called on WorldJoin and never again.
                    VolumeThreasholdInfo.SetValue(uInstance, value);
                }
            }
        }
        internal static float userVolumePeak
        {
            get 
            {
                if (VolumePeakInfo != null) return float.Parse(VolumePeakInfo.GetValue(uInstance).ToString());

                VolumePeakInfo = GetInfo("0.02"); // This should only ever be called on WorldJoin and never again.
                return float.Parse(VolumePeakInfo.GetValue(uInstance).ToString());
            }
            set
            {
                if (VolumePeakInfo != null) VolumePeakInfo.SetValue(uInstance, value);
                else
                {
                    VolumePeakInfo = GetInfo("0.02"); // This should only ever be called on WorldJoin and never again.
                    VolumePeakInfo.SetValue(uInstance, value);
                }
            }
        }
        internal static IEnumerator WorldJoinedCoroutine()
        {
            for (;;)
            {
                var sw = new Stopwatch();
                sw.Start();
                if (CurrentUser != null && IsInWorld()) 
                { 
                    yield return new WaitForSeconds(1);
                    {
                        SensitivitySetup();
                    }
                    sw.Stop();
                    yield break; 
                }

                if (sw.Elapsed.Seconds >= 100) // This should never happen but a check for it is in place just in case.
                {
                    Log("WorldJoinedCoroutine took too long and was stopped.");
                    yield break;
                }
                yield return new WaitForSeconds(1);
            }
        }
        internal static Il2CppSystem.Object FindInstance(Type WhereLooking, Type WhatLooking) // Credits to Teo
        {
            try
            {
                var methodInfo = WhereLooking.GetMethods(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(m => m.ReturnType == WhatLooking && m.GetParameters().Length == 0);
                if (methodInfo != null)
                {
                    return (Il2CppSystem.Object)methodInfo.Invoke(null, null);
                }
                MelonLogger.LogError("[FindInstance] MethodInfo for " + WhatLooking.Name + " is null");
            }
            catch (Exception e)
            {
                MelonLogger.LogError($"[FindInstance] {e}");
            }
            return null;
        }
        internal static PropertyInfo GetInfo(string originalValue)
        {
            Log($"Caching USpeaker PropertyInfo {infoIndex+1} "); infoIndex =+ 1;
            var uPropInfos = typeof(USpeaker).GetProperties().Where(p => p.PropertyType == typeof(float));

            return uPropInfos.FirstOrDefault(uInfo => uInfo.GetValue(uInstance).ToString() == originalValue);
        }
        
        private static PropertyInfo VolumePeakInfo;
        private static PropertyInfo VolumeThreasholdInfo;
        internal static USpeaker uInstance => CurrentUser.field_Private_USpeaker_0;
        internal static void InternalConfigRefresh() //The Divide by 10k sets it back to a managable float number
        {
            MicSensitivityValue = MelonPrefs.GetFloat("MicSensitivity", "Mic - Microphone Sensitivity") / 10000;
            UseMod = MelonPrefs.GetBool("MicSensitivity", "Mic - Enable Mic Sensitivity Mod");
        }
        /// <summary>
        /// Log an object to the MelonConsole
        /// </summary>
        /// <param name="obj"></param>
        internal static void Log(object obj)
        {
            MelonLogger.Log(obj);
        }
        /// <summary>
        /// Current User Instance.
        /// </summary>
        private static VRCPlayer CurrentUserInstance;
        /// <summary>
        /// Returns the Current User aka the Player object.
        /// </summary>
        internal static VRCPlayer CurrentUser
        {
            get
            {
                if (CurrentUserInstance != null) return CurrentUserInstance;
                CurrentUserInstance = FindInstance(typeof(VRCPlayer), typeof(VRCPlayer))?.TryCast<VRCPlayer>();
                return CurrentUserInstance;
                
            }
        }
    }
}