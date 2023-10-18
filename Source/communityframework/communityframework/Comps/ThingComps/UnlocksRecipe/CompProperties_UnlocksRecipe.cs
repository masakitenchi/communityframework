using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using RimWorld;
using Verse;

namespace CF
{
    /// <summary>
    /// The properties class for <see cref="CompUnlocksRecipe"/>. Used to
    /// define which recipes are unlocked, and by what facilities.
    /// </summary>
    public class CompProperties_UnlocksRecipe : CompProperties
    {
        /// <summary>
        /// Used to store a single facility that is able to unlock the listed
        /// recipes, and the minimum quality of the facility required for the
        /// recipe to be unlocked.
        /// </summary>
        public class LinkableFacilities
        {
            /// <summary>
            /// Thing <see cref="ThingDef"/> of the facility that unlocks the
            /// given list of recipes.
            /// </summary>
            public ThingDef targetFacility;
            /// <summary>
            /// The minimum quality that <see cref="targetFacility"/> must be
            /// for it to be able to unlock the listed recipes.
            /// </summary>
            public QualityCategory minQuality;
        }
        /// <summary>
        /// The list of facilities that unlock the recipes defined in
        /// <see cref="recipes"/>, and the minimum
        /// quality that the facility must be to do so.
        /// </summary>
        public List<LinkableFacilities> linkableFacilities;
        /// <summary>
        /// The recipes that will become available when one of the facility
        /// requirements defined by <see cref="linkableFacilities"/> is met.
        /// </summary>
        public List<RecipeDef> recipes;

        /// <summary>
        /// Constructor, automatically initializes <c>compClass</c> to
        /// <see cref="CompUnlocksRecipe"/>.
        /// </summary>
        public CompProperties_UnlocksRecipe() => compClass = typeof(CompUnlocksRecipe);
        public List<LinkableFacilities> linkableFacilities = new List<LinkableFacilities>(); // Which facilities should be targeted


        //Not sure if I should do this, but in case someone doesn't add the recipedef to the worktable
        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);
            for (int i = 0; i < linkableFacilities.Count; i++)
            {
                LinkableFacilities item = linkableFacilities[i];
                ULog.Message($"Adding recipe into {item.targetFacility.defName}");
                parentDef.AllRecipes.AddRange(item.recipes);
            }
        }

        //Currently unlockable recipes should be identical (Cannot have different buildings unlock the same recipe, or multiple buildings are required for one recipe)
        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            HashSet<RecipeDef> recipes = new HashSet<RecipeDef>();
            for (int i = 0; i < linkableFacilities.Count; i++)
            {
                LinkableFacilities item = linkableFacilities[i];
                for (int j = 0; j < item.recipes.Count; j++)
                {
                    RecipeDef recipe = item.recipes[j];
                    if (!recipes.Add(recipe))
                        yield return $"UnlockRecipeError: "+ "trying to add same unlockable recipe twice: " + recipe.defName;
                }
            }
        }
        //public List<RecipeDef> recipes; // Which recipes should be added
    }

    /* An example
        <li Class="CF.CompProperties_UnlocksRecipe">
            <linkableFacilities>
                <TableLoom>
                    <recipes>
                        <li>MakePolymerFibers</li>
                        <li>MakePolyester</li>
                        <li>MakeRayonViscose</li>
                        <li>MakeMycotanLeather</li>
                        <li>MakeModacrylic</li>
                        <li>MakePVCLeather</li>
                        <li>MakeNomex</li>
                        <li>MakeKevlar</li>
                        <li>MakeMicropel</li>
                        <li>MakeDyneema</li>
                        <li>MakeZylon</li>
                    </recipes>
                </TableLoom>
            </linkableFacilities>
        </li>
    */
}
