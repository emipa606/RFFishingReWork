using RimWorld;
using Verse;

namespace RBB_Code;

public class Building_FishingSpot : Building_WorkTable
{
    public IntVec3 fishingSpotCell = new IntVec3(0, 0, 0);

    public int fishStock = 2;

    private int fishStockRespawnInterval = 24000;

    private int fishStockRespawnTick;

    private int maxFishStock = 3;

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        fishingSpotCell = Position + new IntVec3(0, 0, 0).RotatedBy(Rotation);
        if (Map.Biome == BiomeDef.Named("AridShrubland") || Map.Biome.defName.Contains("DesertArchi") ||
            Map.Biome.defName.Contains("Boreal"))
        {
            maxFishStock = 2;
            fishStock = 1;
            fishStockRespawnInterval = 30000;
        }
        else if (Map.Biome.defName.Contains("Oasis"))
        {
            maxFishStock = 2;
            fishStock = 1;
            fishStockRespawnInterval = 33000;
        }
        else if (Map.Biome.defName.Contains("ColdBog") || Map.Biome == BiomeDef.Named("Desert") ||
                 Map.Biome.defName.Contains("Tundra"))
        {
            maxFishStock = 2;
            fishStock = 1;
            fishStockRespawnInterval = 36000;
        }
        else if (Map.Biome.defName.Contains("Permafrost") || Map.Biome.defName == "RRP_TemperateDesert")
        {
            maxFishStock = 1;
            fishStock = 1;
            fishStockRespawnInterval = 42000;
        }
        else if (Map.Biome == BiomeDef.Named("ExtremeDesert") || Map.Biome == BiomeDef.Named("IceSheet") ||
                 Map.Biome == BiomeDef.Named("SeaIce"))
        {
            maxFishStock = 1;
            fishStock = 1;
            fishStockRespawnInterval = 48000;
        }

        var terrainDef = map.terrainGrid.TerrainAt(fishingSpotCell);
        if (terrainDef.defName.Contains("Moving") || terrainDef.defName.Contains("Ocean"))
        {
            maxFishStock++;
        }

        if (!terrainDef.defName.Contains("Deep"))
        {
            return;
        }

        maxFishStock++;
        fishStock++;
    }

    public override void TickRare()
    {
        base.TickRare();
        fishingSpotCell = Position + new IntVec3(0, 0, 0).RotatedBy(Rotation);
        var terrainDef = Map.terrainGrid.TerrainAt(fishingSpotCell);
        if (def.defName == "RBB_FishingSpot" && !terrainDef.defName.Contains("Water") &&
            !terrainDef.defName.Equals("Marsh"))
        {
            Destroy();
        }
        else if (def.defName == "RBB_IceFishingSpot" && !terrainDef.defName.Contains("Ice"))
        {
            Destroy();
        }
        else
        {
            var terrainDef2 = Map.terrainGrid.TerrainAt(InteractionCell);
            if ((terrainDef2.defName.Contains("Water") || terrainDef2.defName.Equals("Marsh")) &&
                !terrainDef2.defName.Contains("Bridge"))
            {
                Destroy();
            }
        }

        if (fishStock >= maxFishStock)
        {
            return;
        }

        if (fishStockRespawnTick == 0)
        {
            fishStockRespawnTick =
                Find.TickManager.TicksGame + (int)(fishStockRespawnInterval * Rand.Range(0.8f, 1.2f));
            fishStockRespawnTick = (int)(fishStockRespawnTick / (Controller.Settings.fishingEfficiency / 100f));
        }

        if (Find.TickManager.TicksGame < fishStockRespawnTick)
        {
            return;
        }

        fishStock++;
        fishStockRespawnTick = 0;
    }

    public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
    {
        var list = Map.thingGrid.ThingsListAt(fishingSpotCell);
        // ReSharper disable once ForCanBeConvertedToForeach, Items despawn
        for (var i = 0; i < list.Count; i++)
        {
            if (list[i].def.defName.Contains("Mote_Fishing"))
            {
                list[i].DeSpawn();
            }
        }

        base.Destroy(mode);
    }
}