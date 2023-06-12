using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;

namespace CF
{
    [ClassWithPatches("ApplyCompAffectedByFacilitiesPatch")]
    static class CompAffectedByFacilitiesPatch
    {
        /// <summary>
        /// This patches the method Notify_LinkRemoved so the CompUnlocksRecipe specific code is executed after the regular Notify_LinkRemoved code is run.
        /// CompUnlocksRecipe in this case checks when link is removed, if the removed facility was the only one of that type, and if so, remove the recipes from the target workbench. 
        /// </summary>
        /*[HarmonyPatch(typeof(CompAffectedByFacilities), "Notify_LinkRemoved")]
        class Notify_LinkRemoved
        {
            public static void Postfix(Thing thing, CompAffectedByFacilities __instance)
            {
                CompUnlocksRecipe.RemoveRecipe(thing, __instance);
            }
        }*/
        /// <summary>
        /// This patches the method Notify_NewLink so the CompUnlocksRecipe specific code is executed after the regular Notify_NewLink code is run.
        /// CompUnlocksRecipe in this case checks when a new link is created. If this new link is one with a new unique facility, and the added recipes are also not yet added to the workbench, they will be added.
        /// </summary>
        /*[HarmonyPatch(typeof(CompAffectedByFacilities), "Notify_NewLink")]
        class Notify_NewLink
        {
            public static void Postfix(Thing facility, CompAffectedByFacilities __instance)
            {
                CompUnlocksRecipe.AddRecipe(facility, __instance);
            }
        }*/
        [HarmonyPatch]
        class Patch_Link
        {

            //Checks if the facility has CompUnlocksRecipe, and update its recipe according to new links
            [HarmonyPatch(typeof(CompAffectedByFacilities), nameof(CompAffectedByFacilities.Notify_NewLink))]
            [HarmonyPostfix]
            public static void AddRecipe(Thing facility, CompAffectedByFacilities __instance)
            {
                CompUnlocksRecipe comp = __instance.parent.TryGetComp<CompUnlocksRecipe>();
                if (comp == null || !comp.Props.linkableFacilities.Exists(x => x.targetFacility == facility.def)) return;
                //Log.Message($"Adding recipes from {facility.def.defName} for {__instance.parent.def.defName}");
                //comp._currentlyUnlocked.AddRange(comp.Props.linkableFacilities.Find(x => x.targetFacility == facility.def)?.recipes);
                foreach (var thing in comp.Props.linkableFacilities)
                    foreach (var recipe in thing.recipes)
                        comp._currentlyUnlocked.Add(recipe);
            }

            //Checks if the facility has CompUnlocksRecipe, and update its recipe according to new links
            [HarmonyPatch(typeof(CompAffectedByFacilities), nameof(CompAffectedByFacilities.Notify_LinkRemoved))]
            [HarmonyPostfix]
            public static void RemoveRecipe(Thing thing, CompAffectedByFacilities __instance)
            {
                CompUnlocksRecipe comp = __instance.parent.TryGetComp<CompUnlocksRecipe>();
                if (comp == null || !comp.Props.linkableFacilities.Exists(x => x.targetFacility == thing.def)) return;
                //Log.Message($"Removing recipes from {thing.def.defName} for {__instance.parent.def.defName}");
                //Iterate through each facility that unlocks recipes, remove all recipes that's missing its facility
                foreach (var facility in comp.Props.linkableFacilities)
                    foreach (var recipe in facility.recipes)
                        comp._currentlyUnlocked.Remove(recipe);
            }
        }


        /* The whole stacktrace:
         * ITab_Bills.FillTab() -> delegate that checks SelTable.def.AllRecipes[i].AvailableNow && SelTable.def.AllRecipes[i].AvailableOnNow(SelTable)
         * So first we need to add all unlockable recipes to the building's def
         * Then we can patch RecipeDef.AvailableOnNow(SelTable) to check if this recipe is unlocked through CompProperties_UnlocksRecipe
         * In conclusion, we need to check the building has CompUnlocksRecipe first, then check if this recipe is in its CompProperties_UnlocksRecipe (no matter which building unlocks it)
         */
        [HarmonyPatch]
        public static class Patch_RecipeDef
        {
            [HarmonyPatch(typeof(RecipeDef), nameof(RecipeDef.AvailableOnNow))]
            [HarmonyPrefix]
            public static bool AvailablePrefix(RecipeDef __instance, ref bool __result, Thing thing, BodyPartRecord part = null)
            {
                //Redirect to vanilla if:
                // - comp is null;
                // - this recipe is not unlocked by this CompProperties
                CompUnlocksRecipe comp = thing.TryGetComp<CompUnlocksRecipe>();
                if (comp is null || !comp.Props.linkableFacilities.Any(x => x.recipes.Contains(__instance))) return true;
                __result = comp._currentlyUnlocked.Contains(__instance);
                return false;
            }
        }
    }

}

