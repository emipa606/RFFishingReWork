using Verse;

namespace RBB_Code;

public class PlaceWorker_ShellfishTrap : PlaceWorker
{
    public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map,
        Thing thingToIgnore = null, Thing thing = null)
    {
        var terrainDef = map.terrainGrid.TerrainAt(loc);
        if (!terrainDef.defName.Contains("Water") && terrainDef != TerrainDef.Named("Marsh"))
        {
            return new AcceptanceReport("RBB.ShellfishTrap".Translate());
        }

        var list = map.thingGrid.ThingsListAtFast(loc);
        foreach (var thingOnSpot in list)
        {
            if (thingOnSpot != thingToIgnore && thingOnSpot.def.defName == "RBB_FishingSpot")
            {
                return new AcceptanceReport("RBB.NotOnFS".Translate());
            }
        }

        var isWaterAdjacent = false;
        for (var j = -1; j < 2; j++)
        {
            for (var k = -1; k < 2; k++)
            {
                var newX = loc.x + j;
                var newZ = loc.z + k;
                var c = new IntVec3(newX, 0, newZ);
                var terrainDef2 = map.terrainGrid.TerrainAt(c);
                if (!terrainDef2.defName.Contains("Water") && terrainDef2 != TerrainDef.Named("Marsh"))
                {
                    isWaterAdjacent = true;
                }

                if (terrainDef2.defName.Contains("Bridge"))
                {
                    isWaterAdjacent = true;
                }
            }
        }

        if (!isWaterAdjacent.Equals(true))
        {
            return new AcceptanceReport("RBB.ShellfishTrap".Translate());
        }

        var list2 = map.listerThings.ThingsOfDef(ThingDef.Named("ShellfishTrap"));
        if (list2 != null && list2.Count(building => loc.InHorDistOf(building.Position, 6f)) > 0)
        {
            return new AcceptanceReport("RBB.TrapTooClose".Translate());
        }

        var list3 = map.listerThings.ThingsOfDef(ThingDef.Named("Blueprint_ShellfishTrap"));
        if (list3 != null && list3.Count(building => loc.InHorDistOf(building.Position, 6f)) > 0)
        {
            return new AcceptanceReport("RBB.TrapTooClose".Translate());
        }

        var list4 = map.listerThings.ThingsOfDef(ThingDef.Named("Frame_ShellfishTrap"));
        if (list4 != null && list4.Count(building => loc.InHorDistOf(building.Position, 6f)) > 0)
        {
            return new AcceptanceReport("RBB.TrapTooClose".Translate());
        }

        return true;
    }
}