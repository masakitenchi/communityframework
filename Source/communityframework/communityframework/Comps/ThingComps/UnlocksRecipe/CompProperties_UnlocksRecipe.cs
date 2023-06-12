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


        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);
            foreach(var item in linkableFacilities)
            {
                ULog.Message($"Adding reciped into {item.targetFacility.defName}");
                parentDef.AllRecipes.AddRange(item.recipes);
            }
        }
        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            HashSet<RecipeDef> recipes = new HashSet<RecipeDef>();
            foreach(var item in linkableFacilities)
            {
                foreach(var recipe in item.recipes)
                {
                    if (!recipes.Add(recipe))
                        yield return $"Error when adding recipe to {parentDef}: "+ "trying to add same unlockable recipe twice: " + recipe.defName;
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
