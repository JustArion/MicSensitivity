using MelonLoader;
using static Dawn.Core;

namespace Dawn.Mic
{
    internal class MicSensitivity : MelonMod
    {
        #region MelonMod Native
        public override void OnApplicationStart()
        {
            MelonPrefs.RegisterCategory("MicSensitivity", "Mic Sensitivity");
            MelonPrefs.RegisterBool("MicSensitivity", "Mic - Enable Mic Sensitivity Mod", false);
            MelonPrefs.RegisterFloat("MicSensitivity", "Mic - Microphone Sensitivity", 100);
            InternalConfigRefresh();
            Log("On App Finished.");
        } //Settings Registration and Refresh
        public override void OnLevelWasLoaded(int level) // World Join
        {
            switch (level) //Prevents being called 3x
            {
                case 0:
                case 1:
                    break;
                default:
                    MelonCoroutines.Start(WorldJoinedCoroutine());
                    break;
            }
        }
        public override void OnModSettingsApplied()
        {
            InternalConfigRefresh();
            SensitivitySetup();
            if (UseMod) return;
            userVolumeThreshold = DefaultPeak; //Defaulted if Mod is not used.
            userVolumePeak = DefaultThreshold; //Defaulted if Mod is not used.
        }
        #endregion
        #region The Actual Mod
        internal static bool UseMod;
        internal static float MicSensitivityValue = 0;
        private const float DefaultThreshold = 0.01f;
        private const float DefaultPeak = 0.02f;

        internal static void SensitivitySetup()
        {
            if (!UseMod) return;
        #if Unobfuscated
            CurrentUser._uSpeaker.VolumeThresholdRMS = MicSensitivityValue; CurrentUser.field_Private_USpeaker_0.VolumeThresholdPeak = (MicSensitivityValue * 2); //field_Private_USpeaker_0 }
        #else
            userVolumeThreshold = MicSensitivityValue; userVolumePeak = (MicSensitivityValue * 2); }
        #endif
        #endregion
        }
        
}