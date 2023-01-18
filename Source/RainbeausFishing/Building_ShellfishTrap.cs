using System;
using RimWorld;
using Verse;

namespace RBB_Code;

public class Building_ShellfishTrap : Building
{
    private readonly Random rnd = new Random();

    private int ticksToCatch;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ticksToCatch, "ticksToCatch");
    }

    public override void SpawnSetup(Map currentGame, bool respawningAfterLoad)
    {
        base.SpawnSetup(currentGame, respawningAfterLoad);
        if (ticksToCatch > 0)
        {
            return;
        }

        var num = rnd.Next(24);
        var num2 = rnd.Next(24);
        ticksToCatch = (6 + num + num2) * 10;
        if (currentGame.Biome.defName == "AridShrubland" || currentGame.Biome.defName.Contains("DesertArchi") ||
            currentGame.Biome.defName.Contains("Boreal"))
        {
            var num3 = rnd.Next(6);
            var num4 = rnd.Next(6);
            ticksToCatch += (6 + num3 + num4) * 10;
        }
        else if (Map.Biome.defName.Contains("Oasis"))
        {
            var num5 = rnd.Next(9);
            var num6 = rnd.Next(9);
            ticksToCatch += (6 + num5 + num6) * 10;
        }
        else if (currentGame.Biome.defName.Contains("ColdBog") || currentGame.Biome.defName == "Desert" ||
                 currentGame.Biome.defName.Contains("Tundra"))
        {
            var num7 = rnd.Next(12);
            var num8 = rnd.Next(12);
            ticksToCatch += (6 + num7 + num8) * 10;
        }
        else if (Map.Biome.defName.Contains("Permafrost") || Map.Biome.defName == "RRP_TemperateDesert")
        {
            var num9 = rnd.Next(18);
            var num10 = rnd.Next(18);
            ticksToCatch += (6 + num9 + num10) * 10;
        }
        else if (currentGame.Biome.defName == "IceSheet" || currentGame.Biome.defName == "SeaIce" ||
                 currentGame.Biome.defName == "ExtremeDesert")
        {
            var num11 = rnd.Next(24);
            var num12 = rnd.Next(24);
            ticksToCatch += (6 + num11 + num12) * 10;
        }

        ticksToCatch = (int)(ticksToCatch / (Controller.Settings.trapEfficiency / 100f));
    }

    public override void TickRare()
    {
        base.TickRare();
        var terrainDef = Map.terrainGrid.TerrainAt(Position);
        if (terrainDef.defName.Contains("Water") || terrainDef.defName.Equals("Marsh"))
        {
            if (ticksToCatch > 0)
            {
                ticksToCatch--;
            }

            if (ticksToCatch <= 0)
            {
                PlaceProduct();
            }
        }
        else if (terrainDef.defName.Contains("Ice") && Rand.Value > 0.5f)
        {
            if (ticksToCatch > 0)
            {
                ticksToCatch--;
            }

            if (ticksToCatch <= 0)
            {
                PlaceProduct();
            }
        }
    }

    private void PlaceProduct()
    {
        var value = Rand.Value;
        var value2 = Rand.Value;
        value2 *= Controller.Settings.trapEfficiency / 100f;
        var num = 0f;
        var text = "";
        var num2 = 0;
        if (Rand.Value <= 0.25)
        {
            num2 = Rand.RangeInclusive(0, 10);
            if (Rand.Value < 0.5)
            {
                num2 += Rand.RangeInclusive(0, 15);
            }
        }

        if (HitPoints <= num2)
        {
            Destroy();
            text = "RBB.TrapDestroyed".Translate();
        }
        else
        {
            TakeDamage(new DamageInfo(DamageDefOf.Blunt, num2));
            var list = Map.listerThings.ThingsOfDef(ThingDef.Named("ShellfishTrap"));
            if (list != null)
            {
                float num3 = list.Count(building => Position.InHorDistOf(building.Position, 21f));
                var num4 = list.Count(building => Position.InHorDistOf(building.Position, 15f));
                var num5 = list.Count(building => Position.InHorDistOf(building.Position, 9f));
                num = (num3 * 0.025f) + (num4 * 0.05f) + (num5 * 0.1f);
                if (num > 0.75f)
                {
                    num = 0.75f;
                }
            }

            if (value2 > num)
            {
                if (Map.terrainGrid.TerrainAt(Position).defName.Contains("Ocean"))
                {
                    if (value < 0.25)
                    {
                        var newThing = ThingMaker.MakeThing(ThingDef.Named("RF_Lobster"));
                        GenSpawn.Spawn(newThing, Position, Map);
                        text = "RBB.TrapLobster".Translate();
                    }
                    else if (value < 0.3)
                    {
                        var newThing2 = ThingMaker.MakeThing(ThingDef.Named("RF_Octopus"));
                        GenSpawn.Spawn(newThing2, Position, Map);
                        text = "RBB.TrapOctopus".Translate();
                    }
                    else if (value < 0.4)
                    {
                        var newThing3 = ThingMaker.MakeThing(ThingDef.Named("RF_Scallops"));
                        GenSpawn.Spawn(newThing3, Position, Map);
                        text = "RBB.TrapScallops".Translate();
                    }
                    else if (value < 0.5)
                    {
                        var newThing4 = ThingMaker.MakeThing(ThingDef.Named("RF_Crab"));
                        GenSpawn.Spawn(newThing4, Position, Map);
                        text = "RBB.TrapCrab".Translate();
                    }
                    else if (value < 0.6)
                    {
                        var newThing5 = ThingMaker.MakeThing(ThingDef.Named("RF_Mussel"));
                        GenSpawn.Spawn(newThing5, Position, Map);
                        text = "RBB.TrapMussel".Translate();
                    }
                    else if (value < 0.7)
                    {
                        var newThing6 = ThingMaker.MakeThing(ThingDef.Named("RF_Anchovies"));
                        GenSpawn.Spawn(newThing6, Position, Map);
                        text = "RBB.TrapAnchovies".Translate();
                    }
                    else if (value < 0.8)
                    {
                        var newThing7 = ThingMaker.MakeThing(ThingDef.Named("RF_Shrimp"));
                        GenSpawn.Spawn(newThing7, Position, Map);
                        text = "RBB.TrapShrimp".Translate();
                    }
                    else if (value < 0.9)
                    {
                        var newThing8 = ThingMaker.MakeThing(ThingDef.Named("RF_Clownfish"));
                        GenSpawn.Spawn(newThing8, Position, Map);
                        text = "RBB.TrapClownfish".Translate();
                    }
                    else
                    {
                        var newThing9 = ThingMaker.MakeThing(ThingDef.Named("RF_FishTiny"));
                        GenSpawn.Spawn(newThing9, Position, Map);
                        text = "RBB.TrapFish".Translate();
                    }
                }
                else if (value < 0.4)
                {
                    var newThing10 = ThingMaker.MakeThing(ThingDef.Named("RF_Crayfish"));
                    GenSpawn.Spawn(newThing10, Position, Map);
                    text = "RBB.TrapCrayfish".Translate();
                }
                else if (value < 0.6)
                {
                    var newThing11 = ThingMaker.MakeThing(ThingDef.Named("RF_Crab"));
                    GenSpawn.Spawn(newThing11, Position, Map);
                    text = "RBB.TrapCrab".Translate();
                }
                else if (value < 0.8)
                {
                    var newThing12 = ThingMaker.MakeThing(ThingDef.Named("RF_Snail"));
                    GenSpawn.Spawn(newThing12, Position, Map);
                    text = "RBB.TrapSnail".Translate();
                }
                else
                {
                    var newThing13 = ThingMaker.MakeThing(ThingDef.Named("RF_FishTiny"));
                    GenSpawn.Spawn(newThing13, Position, Map);
                    text = "RBB.TrapFish".Translate();
                }
            }

            var num6 = rnd.Next(24);
            var num7 = rnd.Next(24);
            ticksToCatch = (6 + num6 + num7) * 10;
            if (Map.Biome.defName == "AridShrubland" || Map.Biome.defName.Contains("DesertArchi") ||
                Map.Biome.defName.Contains("Boreal"))
            {
                var num8 = rnd.Next(6);
                var num9 = rnd.Next(6);
                ticksToCatch += (6 + num8 + num9) * 10;
            }
            else if (Map.Biome.defName.Contains("Oasis"))
            {
                var num10 = rnd.Next(9);
                var num11 = rnd.Next(9);
                ticksToCatch += (6 + num10 + num11) * 10;
            }
            else if (Map.Biome.defName.Contains("ColdBog") || Map.Biome.defName == "Desert" ||
                     Map.Biome.defName.Contains("Tundra"))
            {
                var num12 = rnd.Next(12);
                var num13 = rnd.Next(12);
                ticksToCatch += (6 + num12 + num13) * 10;
            }
            else if (Map.Biome.defName.Contains("Permafrost") || Map.Biome.defName == "RRP_TemperateDesert")
            {
                var num14 = rnd.Next(18);
                var num15 = rnd.Next(18);
                ticksToCatch += (6 + num14 + num15) * 10;
            }
            else if (Map.Biome.defName == "IceSheet" || Map.Biome.defName == "SeaIce" ||
                     Map.Biome.defName == "ExtremeDesert")
            {
                var num16 = rnd.Next(24);
                var num17 = rnd.Next(24);
                ticksToCatch += (6 + num16 + num17) * 10;
            }

            ticksToCatch = (int)(ticksToCatch / (Controller.Settings.trapEfficiency / 100f));
        }

        if (!(Controller.Settings.trapsuccessLevel >= 1f))
        {
            return;
        }

        switch (Controller.Settings.trapsuccessLevel)
        {
            case < 2f:
                Messages.Message(text, new TargetInfo(Position, Map), MessageTypeDefOf.SilentInput);
                break;
            case < 3f:
                Messages.Message(text, new TargetInfo(Position, Map), MessageTypeDefOf.PositiveEvent);
                break;
            default:
            {
                if (!(value2 < num))
                {
                    Find.LetterStack.ReceiveLetter("RBB.TrapSuccessTitle".Translate(), text, LetterDefOf.PositiveEvent,
                        new TargetInfo(Position, Map));
                }

                break;
            }
        }
    }
}