﻿using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;

namespace CF
{
    class CompUnlocksRecipe : CompAffectedByFacilities
    {
        public CompProperties_UnlocksRecipe Props => this.props as CompProperties_UnlocksRecipe;

        //Had a hard time trying to save current state between save, so I guess this is the best I could achieve atm
        //PostExposeDate works per se, but there are multiple methods that's calling Notify_LinkRemoved and Notify_NewLink and would mess with the log in a pretty bad way
        [Unsaved(false)]
        public HashSet<RecipeDef> _currentlyUnlocked = new HashSet<RecipeDef>();

        /*public static void AddRecipe(Thing _facility, CompAffectedByFacilities _compAffectedByFacilities)
        {
            if (_compAffectedByFacilities.parent.TryGetComp<CompUnlocksRecipe>() != null)
            {
                CompUnlocksRecipe compUnlocksRecipe = _compAffectedByFacilities.parent.GetComp<CompUnlocksRecipe>();
                CompAffectedByFacilities compAffectedByFacilities = _compAffectedByFacilities.parent.GetComp<CompAffectedByFacilities>();
                CompProperties_UnlocksRecipe props = compUnlocksRecipe.Props;
                List<Thing> connectedFacilities = compAffectedByFacilities.LinkedFacilitiesListForReading;

                // Cycle through each ThingDef from the XML tags. Then check if any of the connected facilities have the same defName and other requirements. If so, add +1 to matches. If more than one match is found, cancel the process.
                foreach (var var in props.linkableFacilities)
                {
                    int matches = 0; // How many facilities have the requirements.
                    foreach (Thing thing in connectedFacilities)
                    {
                        if (thing.def.defName == var.targetFacility.defName)
                        {
                            // Checking if both the XML tag and building have a quality, and if the quality of the building is equal to or higher than the XML tag.
                            if (thing.TryGetComp<CompQuality>() != null & var.minQuality != 0)
                            {
                                thing.TryGetQuality(out QualityCategory qc);

                                if (qc < var.minQuality)
                                {
                                   continue;
                                }
                            }
                            matches++;
                        }
                    }
                    if (matches != 1)
                    {
                        return;
                    }
                }

                // Cycle through all the recipes from the XML tag. Check if any of them already exist at the workbench. Skip if this is the case, add it if it is not.
                foreach (RecipeDef recipe in props.recipes)
                {
                    if (!_compAffectedByFacilities.parent.def.AllRecipes.Contains(recipe))
                    {
                        _compAffectedByFacilities.parent.def.AllRecipes.Add(recipe);
                    }
                }
            }
        }*/

        /*public static void RemoveRecipe(Thing _facility, CompAffectedByFacilities _compAffectedByFacilities)
        {
            if (_compAffectedByFacilities.parent.TryGetComp<CompUnlocksRecipe>() != null)
            {
                CompUnlocksRecipe compUnlocksRecipe = _compAffectedByFacilities.parent.GetComp<CompUnlocksRecipe>();
                CompAffectedByFacilities compAffectedByFacilities = _compAffectedByFacilities.parent.GetComp<CompAffectedByFacilities>();
                CompProperties_UnlocksRecipe props = compUnlocksRecipe.Props;
                List<Thing> connectedFacilities = compAffectedByFacilities.LinkedFacilitiesListForReading;

                // Cycle through each ThingDef from the XML tags. Then check if any of the connected facilities have the same defName and other requirements. If so, add +1 to matches.
                // So if for one XML tag no match is found, remove its recipes.
                foreach (var var in props.linkableFacilities)
                {
                    int matches = 0; // How many facilities have the requirements.
                    foreach (Thing thing in connectedFacilities)
                    {
                        if (thing.def.defName == var.targetFacility.defName)
                        {
                            // Checking if both the XML tag and building have a quality, and if the quality of the building is equal to or higher than the XML tag.                          
                            if (thing.TryGetComp<CompQuality>() != null & var.minQuality != 0)
                            {
                                thing.TryGetQuality(out QualityCategory qc);

                                if (qc < var.minQuality)
                                {
                                    continue;
                                }
                            }
                            matches++;                           
                        }
                    }

                    if (matches == 0)
                    {
                        // Cycle through all the recipes from the XML tag and check if they exist at the workbench. If so, remove it.
                        foreach (RecipeDef recipe in props.recipes)
                        {
                            if (_compAffectedByFacilities.parent.def.AllRecipes.Contains(recipe))
                            {
                                _compAffectedByFacilities.parent.def.AllRecipes.Remove(recipe);
                            }
                        }
                    }
                }


            }
        }*/


    }
}
