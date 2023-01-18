using RimWorld;
using Verse;

namespace RBB_Code;

public class PlaceWorker_FishingSpot : PlaceWorker
{
    public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map,
        Thing thingToIgnore = null, Thing thing = null)
    {
        var terrainDef = map.terrainGrid.TerrainAt(loc);
        if (!terrainDef.defName.Contains("Water") && !terrainDef.defName.Equals("Marsh"))
        {
            return new AcceptanceReport("RBB.FishingSpot1".Translate());
        }

        var c = ThingUtility.InteractionCellWhenAt(checkingDef as ThingDef, loc, rot, map);
        if (!c.InBounds(map))
        {
            return new AcceptanceReport("InteractionSpotOutOfBounds".Translate());
        }

        var list = map.listerThings.ThingsOfDef(ThingDef.Named("RBB_FishingSpot"));
        if (list != null && list.Count(building => loc.InHorDistOf(building.Position, 9f)) > 0)
        {
            return new AcceptanceReport("RBB.FSTooClose".Translate());
        }

        if (RBB_Initializer.iceFishing.Equals(true))
        {
            var list2 = map.listerThings.ThingsOfDef(ThingDef.Named("RBB_IceFishingSpot"));
            if (list2 != null && list2.Count(building => loc.InHorDistOf(building.Position, 9f)) > 0)
            {
                return new AcceptanceReport("RBB.FSTooClose".Translate());
            }

            var list3 = map.listerThings.ThingsOfDef(ThingDef.Named("Blueprint_RBB_IceFishingSpot"));
            if (list3 != null && list3.Count(building => loc.InHorDistOf(building.Position, 9f)) > 0)
            {
                return new AcceptanceReport("RBB.FSTooClose".Translate());
            }

            var list4 = map.listerThings.ThingsOfDef(ThingDef.Named("Frame_RBB_IceFishingSpot"));
            if (list4 != null && list4.Count(building => loc.InHorDistOf(building.Position, 9f)) > 0)
            {
                return new AcceptanceReport("RBB.FSTooClose".Translate());
            }
        }

        var list5 = map.thingGrid.ThingsListAtFast(c);
        foreach (var thingOnSpot in list5)
        {
            if (thingOnSpot == thingToIgnore)
            {
                continue;
            }

            if (thingOnSpot.def.passability == Traversability.Impassable)
            {
                return new AcceptanceReport("InteractionSpotBlocked".Translate(thingOnSpot.LabelNoCount)
                    .CapitalizeFirst());
            }

            if (thingOnSpot is Blueprint blueprint && blueprint.def.entityDefToBuild.passability ==
                Traversability.Impassable)
            {
                return new AcceptanceReport("InteractionSpotWillBeBlocked"
                    .Translate(blueprint.LabelNoCount).CapitalizeFirst());
            }
        }

        var terrainDef2 = map.terrainGrid.TerrainAt(c);
        return !terrainDef2.defName.Contains("Water") && terrainDef2 != TerrainDef.Named("Marsh") ? true :
            !terrainDef2.defName.Contains("Bridge") ? new AcceptanceReport("RBB.FishingSpot2".Translate()) :
            (AcceptanceReport)true;
    }
}