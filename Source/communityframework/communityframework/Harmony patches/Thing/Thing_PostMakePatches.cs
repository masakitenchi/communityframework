﻿using Verse;
using HarmonyLib;

namespace CF
{
    [ClassWithPatches("PostMakePatch")]
    public static class Thing_PostMakePatches
    {
        [HarmonyPatch(typeof(Thing))]
        public static class PostMakePatch
        {
            [HarmonyPatch(nameof(Thing.PostMake))]
            [HarmonyPostfix]
            public static void PostMakeThingPatch(
                ref Thing __instance
            )
            {
                if (__instance.def.modExtensions.NullOrEmpty())
                    return;

                foreach (DefModExtension extension in __instance.def.modExtensions)
                    if (extension is IExtensionPostMake postMake)
                        postMake.PostMake(__instance);
            }

            [HarmonyPatch(nameof(Thing.PostPostMake))]
            [HarmonyPostfix]
            public static void PostPostMakeThingPatch(
                ref Thing __instance
            )
            {
                if (__instance.def.modExtensions.NullOrEmpty())
                    return;

                foreach (DefModExtension extension in __instance.def.modExtensions)
                    if (extension is IExtensionPostMake postMake)
                        postMake.PostPostMake(__instance);
            }
        }
        
        
    }
}
