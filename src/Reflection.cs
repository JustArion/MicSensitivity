namespace Dawn.Mic
{
    using System;
    using System.Linq;
    using System.Reflection;
    using MelonLoader;
    using MonoMod.Utils;

    internal static partial class Core
    {
        private static Il2CppSystem.Object FindInstance(IReflect WhereLooking, Type WhatLooking) // Credits to Teo
        {
            try
            {
                var methodInfo = WhereLooking.GetMethods(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(m => m.ReturnType == WhatLooking && m.GetParameters().Length == 0);
                if (methodInfo != null)
                {
                    return (Il2CppSystem.Object)methodInfo.Invoke(null, null);
                }
                MelonLogger.Error("[FindInstance] MethodInfo for " + WhatLooking.Name + " is null");
            }
            catch (Exception e)
            {
                MelonLogger.Error($"[FindInstance] {e}");
            }
            return null;
        }
        private static PropertyInfo GetInfo(string originalValue)
        {
            Log($"Caching USpeaker PropertyInfo {infoIndex+1} "); infoIndex =+ 1;
            var uPropInfos = typeof(USpeaker).GetProperties().Where(p => p.PropertyType == typeof(float));

            return uPropInfos.FirstOrDefault(uInfo => uInfo.GetValue(uInstance).ToString() == originalValue);
        }
        
        private static Func<USpeaker, float> GetVolumePeak;
        private static Action<USpeaker, float> SetVolumePeak;
        private static Func<USpeaker, float> GetVolumeThreshold;
        private static Action<USpeaker, float> SetVolumeThreshold;
        private static readonly Func<VRCPlayer, USpeaker> USpeakerInstance = FetchUSpeakInfo()!.GetMethod.CreateDelegate<Func<VRCPlayer, USpeaker>>();
        private static PropertyInfo FetchUSpeakInfo()
        {
            var props = typeof(VRCPlayer)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(propInfo => propInfo.PropertyType == typeof(USpeaker)).ToArray();
            var len = props.Length;
            switch (len)
            {
                case 1:
                    return props.First();
                case <= 1:
                    return null;
                default:
                {
                    var preferProp = props.FirstOrDefault(pinfo => pinfo.Name.StartsWith("prop_"));
                    return preferProp != null ? preferProp : props.First();
                }
            }
        }
    }
}