using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using RimWorld;
using Verse;

namespace CF
{
    public class CompProperties_UnlocksRecipe : CompProperties
    {
        public class LinkableFacilities
        {
            public ThingDef targetFacility;
            public QualityCategory minQuality;
            public List<RecipeDef> recipes = new List<RecipeDef>();

            public void LoadDataFromXmlCustom(XmlNode xmlRoot)
            {
                DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "targetFacility", xmlRoot.Name);
                if (xmlRoot["minQuality"] != null)
                    minQuality = ParseHelper.FromString<QualityCategory>(xmlRoot["minQuality"].Value);
                foreach (XmlNode node in xmlRoot["recipes"].ChildNodes)
                {
                    DirectXmlCrossRefLoader.RegisterListWantsCrossRef(this.recipes, node.InnerText);
                }
            }
        }
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
