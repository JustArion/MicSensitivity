using MelonLoader;
using static Dawn.Core;

namespace Dawn.Mic
{
    internal sealed class MicSensitivity : MelonMod
    {
        #region MelonMod Native
        public override void OnApplicationStart()
        {
            MelonPreferences.CreateCategory("MicSensitivity", "Mic Sensitivity");
            MelonPreferences.CreateEntry("MicSensitivity", "Mic - Enable Mic Sensitivity Mod", false);
            MelonPreferences.CreateEntry("MicSensitivity", "Mic - Microphone Sensitivity", 100f);
            InternalConfigRefresh();
        } //Settings Registration and Refresh


        public override void OnSceneWasLoaded(int buildIndex, string sceneName) // World Join
        {
            switch (buildIndex) //Prevents being called 3x
            {
                case 0:
                case 1:
                    break;
                default:
                    MelonCoroutines.Start(WorldJoinedCoroutine());
                    break;
            }
        }

        public override void OnPreferencesSaved()
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
        internal static float SensitivityValue = 0;
        private const float DefaultThreshold = 0.01f;
        private const float DefaultPeak = 0.02f;

        internal static void SensitivitySetup()
        {
            if (!UseMod) return;
            userVolumeThreshold = SensitivityValue; userVolumePeak = (SensitivityValue * 2); }
        #endregion
        }
        
}