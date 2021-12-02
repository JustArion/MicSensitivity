using MelonLoader;
using static Dawn.Mic.Core;

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
                    if (!m_UseMod) return;
                    MelonCoroutines.Start(WorldJoinedCoroutine());
                    break;
            }
        }

        public override void OnPreferencesSaved()
        {
            InternalConfigRefresh();
            switch (m_UseMod)
            {
                case true when isInstantiated:
                    userVolumeThreshold = m_SensitivityValue; userVolumePeak = m_SensitivityValue * 2;
                    break;
                // This is most likely due to the need to update to 1.4.3, People who don't use the Mod get a null ref if another mod calls the method.
                case false when isInstantiated:
                    userVolumeThreshold = DefaultPeak; //Defaulted if Mod is not used.
                    userVolumePeak = DefaultThreshold; //Defaulted if Mod is not used.
                    break;
            }
        }

        #endregion
        #region The Actual Mod
        internal static bool m_UseMod = false;
        internal static float m_SensitivityValue = 0;
        private const float DefaultThreshold = 0.01f;
        private const float DefaultPeak = 0.02f;
        #endregion
    }
}