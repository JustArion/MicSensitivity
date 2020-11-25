using VRC.Core;

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
        }
        #endregion
        #region The Actual Mod
        private static float MicSensitivityValue;
        private static bool UseMod;
        private static void InternalConfigRefresh() //The Divide by 10k sets it back to a managable float number
        {
            MicSensitivityValue = MelonPrefs.GetFloat("MicSensitivity", "Mic - Microphone Sensitivity") / 10000;
            UseMod = MelonPrefs.GetBool("MicSensitivity", "Mic - Enable Mic Sensitivity Mod");
        }
        
        private static CurrentUserDelegate CurrentUserInstance;
        internal delegate VRCPlayer CurrentUserDelegate();
        internal static CurrentUserDelegate CurrentUser //Cached CurrentUser
        {
            get
            {
                if (CurrentUserInstance != null) return CurrentUserInstance;
                var MethodInfo = typeof(VRCPlayer).GetMethods().First(x => x.ReturnType == typeof(VRCPlayer));
                CurrentUserInstance = (CurrentUserDelegate)Delegate.CreateDelegate(typeof(CurrentUserDelegate), MethodInfo);
                return CurrentUserInstance;
            }
        }
        internal static ApiWorld GetWorld()
        {
            #if Unobfuscated
             return RoomManager.currentRoom; //field_Internal_Static_ApiWorld_0
            #else
            return RoomManager.field_Internal_Static_ApiWorld_0;
            #endif
        }
        internal static ApiWorldInstance GetWorldInstance()
        {
            #if Unobfuscated
            return RoomManager.currentWorldInstance; //field_Internal_Static_ApiWorldInstance_0 
            #else
            return RoomManager.field_Internal_Static_ApiWorldInstance_0;
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
                if (CurrentUser() != null && IsInWorld()) { yield return new WaitForSeconds(1); CurrentUser()._uSpeaker.VolumeThresholdRMS = MicSensitivityValue; yield break; //field_Private_USpeaker_0 }
                #else
                if (CurrentUser() != null && IsInWorld()) { yield return new WaitForSeconds(1); CurrentUser().field_Private_USpeaker_0.VolumeThresholdRMS = MicSensitivityValue; yield break; }
                #endif
                yield return new WaitForSeconds(1);
            }
        }
        #endregion
    }
}
