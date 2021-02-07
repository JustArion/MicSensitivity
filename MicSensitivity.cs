using System.Reflection;
using Il2CppSystem.Threading;
using Photon.Realtime;
using UnhollowerBaseLib;
using VRC.Core;
// ReSharper disable PossibleNullReferenceException

namespace Dawn.Mic
{
    using System;
    using System.Collections;
    using MelonLoader;
    using UnityEngine;
    using System.Linq;
    internal class MicSensitivity : MelonMod
    {
        #region MelonMod Native
        public override void OnApplicationStart()
        {
            MelonPrefs.RegisterCategory("MicSensitivity", "Mic Sensitivity");
            MelonPrefs.RegisterBool("MicSensitivity", "Mic - Enable Mic Sensitivity Mod", false);
            MelonPrefs.RegisterFloat("MicSensitivity", "Mic - Microphone Sensitivity", 100);
            InternalConfigRefresh();
        }
        public override void OnLevelWasLoaded(int level)
        {
            switch (level) //Prevents being called 3x
            {
                case 0:
                case 1:
                    break;
                default:
                    MelonCoroutines.Start(USpeakerAdjust());
                    break;
            }
        }
        public override void OnModSettingsApplied()
        {
            InternalConfigRefresh();
            MelonCoroutines.Start(USpeakerAdjust());
            if (UseMod) return;
            VolumeThreshold = DefaultThreshold;
             VolumePeak = DefaultPeak;
        }
        
        internal static float VolumePeak
        {
            get => float.Parse(PeakInstance.GetValue(_uSpeaker).ToString());
            set 
            { 
                if (PeakInstance == null)
                { Reflection("0.02", PeakInstance); }
                PeakInstance.SetValue(_uSpeaker, value);
            }
        }

        internal static float VolumeThreshold
        {
            get => float.Parse(ThresholdInstance.GetValue(_uSpeaker).ToString()); // need to change instances
            set 
            { 
                if (ThresholdInstance == null)
                { Reflection("0.01", ThresholdInstance); }
                ThresholdInstance.SetValue(_uSpeaker, value);
            }
        }


        private static readonly FieldInfo[] uSpeakerFieldInfos = typeof(USpeaker).GetFields();
        private static FieldInfo PeakInstance;
        private static FieldInfo ThresholdInstance;
        internal static void Reflection(string String, FieldInfo instance)
        {
            if (instance != null) return;
            foreach (var fld in uSpeakerFieldInfos)
            {
                if (fld.FieldType == typeof(float) && fld.GetValue(instance).ToString() == String )
                    instance = fld;
            }
        }

        #endregion
        #region The Actual Mod
        private static float MicSensitivityValue;
        private static bool UseMod;
        private const float DefaultThreshold = 0.01f;
        private const float DefaultPeak = 0.02f;
        private static readonly USpeaker _uSpeaker = CurrentUser.field_Private_USpeaker_0;

        private static void InternalConfigRefresh() //The Divide by 10k sets it back to a managable float number
        {
            MicSensitivityValue = MelonPrefs.GetFloat("MicSensitivity", "Mic - Microphone Sensitivity") / 10000;
            UseMod = MelonPrefs.GetBool("MicSensitivity", "Mic - Enable Mic Sensitivity Mod");
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
        
        /// <summary>
        /// Current User Instance
        /// </summary>
        private static VRCPlayer CurrentUserInstance;
        internal static VRCPlayer CurrentUser
        {
            get
            {
                if (CurrentUserInstance != null) return CurrentUserInstance;
                CurrentUserInstance = FindInstance(typeof(VRCPlayer), typeof(VRCPlayer))?.TryCast<VRCPlayer>();
                return CurrentUserInstance;
                
            }
        }
        internal static ApiWorld GetWorld()
        {
            #if Unobfuscated
             return RoomManager.currentRoom; //field_Internal_Static_ApiWorld_0
            #else
            return FindInstance(typeof(RoomManager), typeof(ApiWorld)).TryCast<ApiWorld>(); //Used to be RoomManager.field_Internal_Static_ApiWorld_0
            #endif
        }
        internal static ApiWorldInstance GetWorldInstance()
        {
            #if Unobfuscated
            return RoomManager.currentWorldInstance; //field_Internal_Static_ApiWorldInstance_0 
            #else
            return FindInstance(typeof(RoomManager), typeof(ApiWorldInstance)).TryCast<ApiWorldInstance>(); //Used to be RoomManager.field_Internal_Static_ApiWorldInstance_0
            #endif
        }
        internal static bool IsInWorld()
        {
            return GetWorld() != null || GetWorldInstance() != null;
        }
        private static IEnumerator USpeakerAdjust()
        {
            for (;;)
            {
                if (!UseMod) yield break;
                #if Unobfuscated
                if (CurrentUser != null && IsInWorld()) { yield return new WaitForSeconds(1); CurrentUser()._uSpeaker.VolumeThresholdRMS = MicSensitivityValue; CurrentUser().field_Private_USpeaker_0.VolumeThresholdPeak = (MicSensitivityValue * 2); yield break; //field_Private_USpeaker_0 }
                #else
                if (CurrentUser != null && IsInWorld()) { yield return new WaitForSeconds(1); VolumeThreshold = MicSensitivityValue; VolumePeak = (MicSensitivityValue * 2); yield break; }
                #endif
                yield return new WaitForSeconds(1);
            }
        }
        #endregion
    }
}