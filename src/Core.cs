using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.SDK3.Components;
using static Dawn.Mic.MicSensitivity;
namespace Dawn.Mic
{
    using MonoMod.Utils;

    internal static partial class Core
    {
        private static ApiWorld currentRoom => FindInstance(typeof(RoomManager), typeof(ApiWorld)).TryCast<ApiWorld>();

        private static ApiWorldInstance currentWorldInstance => FindInstance(typeof(RoomManager), typeof(ApiWorldInstance)).TryCast<ApiWorldInstance>();
        
        private static bool IsInWorld => currentRoom != null || currentWorldInstance != null;
        private static float infoIndex;
        internal static float userVolumeThreshold
        {
            get => GetVolumeThreshold?.Invoke(uInstance) ?? (GetVolumeThreshold = GetInfo("0.01")!.GetMethod.CreateDelegate<Func<USpeaker, float>>())(uInstance);
            set
            {
                if (SetVolumeThreshold != null)
                    SetVolumeThreshold(uInstance, value);
                else
                    (SetVolumeThreshold = GetInfo("0.01")!.SetMethod.CreateDelegate<Action<USpeaker, float>>())(uInstance, value);
            }
        }
        internal static float userVolumePeak
        {
            get => GetVolumePeak?.Invoke(uInstance) ?? (GetVolumePeak = GetInfo("0.02")!.GetMethod.CreateDelegate<Func<USpeaker, float>>())(uInstance);
            set
            {
                if (SetVolumePeak != null)
                    SetVolumePeak(uInstance, value);
                else
                    (SetVolumePeak = GetInfo("0.02")!.SetMethod.CreateDelegate<Action<USpeaker, float>>())(uInstance, value);
            }
        }

        internal static bool isInstantiated
        {
            get
            {
                if (CurrentUser == null || !IsInWorld) return false;
                return uInstance != null; // Im guessing having it complete the check above ^ first, prevents uInstance from throwing a null ref at the CurrentUser level and checks if uspeaker is present.
            }
        }

        private static bool Running;
        private static DateTime CoroutineInitiationTime;
        internal static IEnumerator WorldJoinedCoroutine() // This is likely more complicated than it should be.
        {
            if (CoroutineInitiationTime > CoroutineInitiationTime.AddSeconds(25)) Running = false;  // Timeout for Running to reset.
            CoroutineInitiationTime = DateTime.Now;
            if (Running) yield break; // Prevents Coroutine Running multiple times if WorldJoin is diverted.
            Running = true;
            var sw = new Stopwatch();
            sw.Start();
            for (;;)
            {
                if (isInstantiated) // 1 Extra check for the rare case scenario that uInstance is not set up fast enough from CurrentUser -> uInstance.
                {
                    Running = false;
                    yield return new WaitForSeconds(1);
                    {
                        userVolumeThreshold = m_SensitivityValue; userVolumePeak = m_SensitivityValue * 2;
                    }
                    sw.Stop();
                    yield break; 
                }

                if (sw.Elapsed.Seconds >= 100) // This should never happen but a check for it is in place just in case.
                {
                    Running = false;
                    MelonLogger.Warning("WorldJoinedCoroutine took too long and was stopped.");
                    yield break;
                }
                yield return new WaitForSeconds(1); // IEnumerator Speed Control
            }
        }
        private static USpeaker _USpeaker(this VRCPlayer _Player) => USpeakerInstance(_Player);
        private static USpeaker uInstance => CurrentUser._USpeaker();
        //TODO - Convert to Discord Sensitivity Numbers ( -50 -100 )
        internal static void InternalConfigRefresh() //The Divide by 10k sets it back to a manageable float number
        {
            m_SensitivityValue = MelonPreferences.GetEntryValue<float>("MicSensitivity", "Mic - Microphone Sensitivity") / 10000;
            m_UseMod = MelonPreferences.GetEntryValue<bool>("MicSensitivity", "Mic - Enable Mic Sensitivity Mod");
        }
        /// <summary>
        /// Log an object to the MelonConsole
        /// </summary>
        /// <param name="obj"></param>
        private static void Log(object obj) => MelonLogger.Msg(obj);
        /// <summary>
        /// Current User Instance Cache.
        /// </summary>
        private static VRCPlayer CurrentUserInstance;
        /// <summary>
        /// Returns the Current User aka the Player object.
        /// </summary>
        private static VRCPlayer CurrentUser
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