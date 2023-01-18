using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RBB_Code;

public class JobDriver_CatchFish : JobDriver
{
    public readonly TargetIndex fishingSpotIndex = TargetIndex.A;
    public Mote fishingRodMote;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        var fishingSpot = TargetThingA as Building_FishingSpot;
        Passion passion;
        var skillGainFactor = 0f;
        AddEndCondition(delegate
        {
            var thing2 = pawn.jobs.curJob.GetTarget(fishingSpotIndex).Thing;
            return !(thing2 is Building) || thing2.Spawned ? JobCondition.Ongoing : JobCondition.Incompletable;
        });
        this.FailOnBurningImmobile(fishingSpotIndex);
        rotateToFace = TargetIndex.B;
        yield return Toils_Reserve.Reserve(fishingSpotIndex);
        var fishingSkillLevel = pawn.skills.AverageOfRelevantSkillsFor(WorkTypeDefOf.Fishing);
        var num = fishingSkillLevel / 20f;
        var fishingDuration2 = (int)(2000f * (1.5f - num));
        fishingDuration2 = (int)(fishingDuration2 / (Controller.Settings.fishingEfficiency / 100f));
        if (fishingSpot != null)
        {
            yield return Toils_Goto.GotoThing(fishingSpotIndex, fishingSpot.InteractionCell)
                .FailOnDespawnedOrNull(fishingSpotIndex);
            var toil = new Toil
            {
                initAction = delegate
                {
                    var def = fishingSpot.Rotation == Rot4.North ? ThingDef.Named("Mote_FishingRodNorth") :
                        fishingSpot.Rotation == Rot4.East ? ThingDef.Named("Mote_FishingRodEast") :
                        !(fishingSpot.Rotation == Rot4.South) ? ThingDef.Named("Mote_FishingRodWest") :
                        ThingDef.Named("Mote_FishingRodSouth");
                    fishingRodMote = (Mote)ThingMaker.MakeThing(def);
                    fishingRodMote.exactPosition = fishingSpot.fishingSpotCell.ToVector3Shifted();
                    fishingRodMote.Scale = 1f;
                    GenSpawn.Spawn(fishingRodMote, fishingSpot.fishingSpotCell, Map);
                    var fishing = WorkTypeDefOf.Fishing;
                    passion = pawn.skills.MaxPassionOfRelevantSkillsFor(fishing);
                    switch (passion)
                    {
                        case Passion.None:
                            skillGainFactor = 0.3f;
                            break;
                        case Passion.Minor:
                            skillGainFactor = 1f;
                            break;
                        default:
                            skillGainFactor = 1.5f;
                            break;
                    }
                },
                tickAction = delegate
                {
                    pawn.skills.Learn(SkillDefOf.Animals, 0.01f * skillGainFactor);
                    if (ticksLeftThisToil != 1)
                    {
                        return;
                    }

                    fishingRodMote?.Destroy();

                    var list = Map.thingGrid.ThingsListAt(fishingSpot.fishingSpotCell);
                    // ReSharper disable once ForCanBeConvertedToForeach, Item despawns
                    for (var j = 0; j < list.Count; j++)
                    {
                        if (list[j].def.defName.Contains("Mote_Fishing"))
                        {
                            list[j].DeSpawn();
                        }
                    }
                },
                defaultDuration = fishingDuration2,
                defaultCompleteMode = ToilCompleteMode.Delay
            };
            yield return toil.WithProgressBarToilDelay(fishingSpotIndex);
            yield return new Toil
            {
                initAction = delegate
                {
                    var num2 = Rand.Value;
                    var terrainDef = Map.terrainGrid.TerrainAt(fishingSpot.fishingSpotCell);
                    if (terrainDef.defName.Equals("Marsh") || Map.Biome.defName.Contains("Swamp"))
                    {
                        num2 -= 0.0025f;
                    }

                    if (num2 < 0.0025)
                    {
                        pawn.jobs.EndCurrentJob(JobCondition.Incompletable);
                        if (Map.Biome.defName.Contains("Tropical") && !terrainDef.defName.Contains("Ocean") &&
                            Rand.Value < 0.33)
                        {
                            string text = pawn.Name.ToStringShort.CapitalizeFirst() +
                                          "RBB.TragedyPiranhaText".Translate();
                            Find.LetterStack.ReceiveLetter("RBB.TragedyTitle".Translate(), text,
                                LetterDefOf.NegativeEvent,
                                pawn);
                            var num3 = Rand.Range(3, 6);
                            for (var i = 0; i < num3; i++)
                            {
                                pawn.TakeDamage(new DamageInfo(DamageDefOf.Bite, Rand.Range(1, 4)));
                            }
                        }
                        else if (terrainDef.defName.Contains("Ocean") && Rand.Value < 0.33)
                        {
                            string text2 = pawn.Name.ToStringShort.CapitalizeFirst() +
                                           "RBB.TragedyJellyfishText".Translate();
                            Find.LetterStack.ReceiveLetter("RBB.TragedyTitle".Translate(), text2,
                                LetterDefOf.NegativeEvent,
                                pawn);
                            pawn.TakeDamage(new DamageInfo(DamageDefOf.Burn, Rand.Range(3, 8)));
                        }
                        else
                        {
                            string text3 = pawn.Name.ToStringShort.CapitalizeFirst() + "RBB.TragedyText".Translate();
                            Find.LetterStack.ReceiveLetter("RBB.TragedyTitle".Translate(), text3,
                                LetterDefOf.NegativeEvent,
                                pawn);
                            pawn.TakeDamage(new DamageInfo(DamageDefOf.Bite, Rand.Range(3, 8)));
                        }
                    }
                    else
                    {
                        var num4 = 0.2f + (fishingSkillLevel / 40f);
                        num4 *= Controller.Settings.fishingEfficiency / 100f;
                        if (Map.Biome == BiomeDef.Named("AridShrubland") || Map.Biome.defName.Contains("DesertArchi") ||
                            Map.Biome.defName.Contains("Boreal"))
                        {
                            num4 -= 0.05f;
                        }
                        else if (Map.Biome.defName.Contains("Oasis"))
                        {
                            num4 -= 0.075f;
                        }
                        else if (Map.Biome.defName.Contains("ColdBog") || Map.Biome == BiomeDef.Named("Desert") ||
                                 Map.Biome.defName.Contains("Tundra"))
                        {
                            num4 -= 0.1f;
                        }
                        else if (Map.Biome.defName.Contains("Permafrost") || Map.Biome.defName == "RRP_TemperateDesert")
                        {
                            num4 -= 0.125f;
                        }
                        else if (Map.Biome == BiomeDef.Named("ExtremeDesert") ||
                                 Map.Biome == BiomeDef.Named("IceSheet") ||
                                 Map.Biome == BiomeDef.Named("SeaIce"))
                        {
                            num4 -= 0.15f;
                        }

                        Thing thing;
                        if (Rand.Value > num4)
                        {
                            string text4 = pawn.Name.ToStringShort.CapitalizeFirst() + "RBB.CaughtNothing".Translate();
                            if (Rand.Value < 0.75f)
                            {
                                pawn.jobs.EndCurrentJob(JobCondition.Incompletable);
                                if (!(Controller.Settings.failureLevel >= 1f))
                                {
                                    return;
                                }

                                switch (Controller.Settings.failureLevel)
                                {
                                    case < 2f:
                                        Messages.Message(text4, new TargetInfo(pawn.Position, Map),
                                            MessageTypeDefOf.SilentInput);
                                        break;
                                    case < 3f:
                                        Messages.Message(text4, new TargetInfo(pawn.Position, Map),
                                            MessageTypeDefOf.NeutralEvent);
                                        break;
                                    default:
                                        Find.LetterStack.ReceiveLetter("RBB.CaughtNothingTitle".Translate(), text4,
                                            LetterDefOf.NeutralEvent, pawn);
                                        break;
                                }

                                return;
                            }

                            var value = Rand.Value;
                            var value2 = Rand.Value;
                            thing = value < 0.75f
                                ? !(value2 < 0.5f)
                                    ? GenSpawn.Spawn(ThingDefOf.Cloth, pawn.Position, Map)
                                    : GenSpawn.Spawn(ThingDefOf.WoodLog, pawn.Position, Map)
                                : value2 < 0.4f
                                    ? GenSpawn.Spawn(ThingDefOf.WoodLog, pawn.Position, Map)
                                    : value2 < 0.7f
                                        ? GenSpawn.Spawn(ThingDefOf.Cloth, pawn.Position, Map)
                                        : value2 < 0.8f
                                            ? GenSpawn.Spawn(ThingDefOf.Steel, pawn.Position, Map)
                                            : value2 < 0.85f
                                                ? GenSpawn.Spawn(ThingDef.Named("SquidLeather"), pawn.Position, Map)
                                                : value2 < 0.9f
                                                    ? GenSpawn.Spawn(ThingDef.Named("Leather_Plain"), pawn.Position,
                                                        Map)
                                                    : value2 < 0.97f
                                                        ? GenSpawn.Spawn(ThingDef.Named("EelLeather"), pawn.Position,
                                                            Map)
                                                        : !(value2 < 0.975f)
                                                            ? GenSpawn.Spawn(ThingDef.Named("Synthread"), pawn.Position,
                                                                Map)
                                                            : GenSpawn.Spawn(ThingDef.Named("Leather_Human"),
                                                                pawn.Position,
                                                                Map);
                            if (Rand.Value < 0.75f)
                            {
                                fishingSpot.fishStock--;
                            }

                            text4 = pawn.Name.ToStringShort.CapitalizeFirst() + "RBB.SnaggedJunk".Translate() +
                                    thing.def.label + ".";
                            if (Controller.Settings.junkLevel >= 1f)
                            {
                                if (Controller.Settings.junkLevel < 2f)
                                {
                                    Messages.Message(text4, new TargetInfo(pawn.Position, Map),
                                        MessageTypeDefOf.SilentInput);
                                }
                                else if (Controller.Settings.junkLevel < 3f)
                                {
                                    Messages.Message(text4, new TargetInfo(pawn.Position, Map),
                                        MessageTypeDefOf.NeutralEvent);
                                }
                                else
                                {
                                    Find.LetterStack.ReceiveLetter("RBB.SnaggedJunkTitle".Translate(), text4,
                                        LetterDefOf.NeutralEvent, pawn);
                                }
                            }
                        }
                        else
                        {
                            var num5 = fishingSkillLevel * 0.0002f;
                            if (terrainDef.defName.Contains("Deep"))
                            {
                                num5 += 0.001f;
                            }

                            if (Rand.Value <= num5)
                            {
                                var value3 = Rand.Value;
                                var value4 = Rand.Value;
                                if (value3 < 0.75f)
                                {
                                    thing = GenSpawn.Spawn(ThingDefOf.Silver, pawn.Position, Map);
                                    thing.stackCount = Rand.RangeInclusive(5, 50);
                                }
                                else if (value4 < 0.6f)
                                {
                                    thing = GenSpawn.Spawn(ThingDefOf.Silver, pawn.Position, Map);
                                    thing.stackCount = Rand.RangeInclusive(10, 100);
                                }
                                else if (!(value4 < 0.9f))
                                {
                                    thing = value4 < 0.96f
                                        ? GenSpawn.Spawn(ThingDef.Named("Gun_ChargeRifle"), pawn.Position, Map)
                                        : !(value4 < 0.98f)
                                            ? GenSpawn.Spawn(ThingDef.Named("SimpleProstheticLeg"), pawn.Position, Map)
                                            : GenSpawn.Spawn(ThingDef.Named("SimpleProstheticArm"), pawn.Position, Map);
                                }
                                else
                                {
                                    thing = GenSpawn.Spawn(ThingDefOf.Gold, pawn.Position, Map);
                                    thing.stackCount = Rand.RangeInclusive(10, 100);
                                }

                                var text5 = string.Concat(
                                    pawn.Name.ToStringShort.CapitalizeFirst() + "RBB.SunkenTreasureText".Translate(),
                                    thing.stackCount, " ", thing.def.label, ".");
                                if (Controller.Settings.treasureLevel >= 1f)
                                {
                                    if (Controller.Settings.treasureLevel < 2f)
                                    {
                                        Messages.Message(text5, new TargetInfo(pawn.Position, Map),
                                            MessageTypeDefOf.SilentInput);
                                    }
                                    else if (Controller.Settings.treasureLevel < 3f)
                                    {
                                        Messages.Message(text5, new TargetInfo(pawn.Position, Map),
                                            MessageTypeDefOf.PositiveEvent);
                                    }
                                    else
                                    {
                                        Find.LetterStack.ReceiveLetter("RBB.SunkenTreasureTitle".Translate(), text5,
                                            LetterDefOf.PositiveEvent, pawn);
                                    }
                                }
                            }
                            else
                            {
                                var num6 = Rand.Value + num4;
                                var text6 = pawn.Name.ToStringShort.CapitalizeFirst();
                                if (num6 is >= 0.4f and < 0.9f)
                                {
                                    thing = GenSpawn.Spawn(ThingDef.Named("RF_FishTiny"), pawn.Position, Map);
                                    text6 += "RBB.TinyFish".Translate();
                                }
                                else if (terrainDef.defName == "Marsh" && num6 is >= 0.9f and < 1.15f)
                                {
                                    thing = GenSpawn.Spawn(ThingDef.Named("RF_Crayfish"), pawn.Position, Map);
                                    text6 += "RBB.Crayfish".Translate();
                                }
                                else if (num6 is >= 1.15f and < 1.4f)
                                {
                                    thing = GenSpawn.Spawn(ThingDef.Named("RF_Eel"), pawn.Position, Map);
                                    text6 += "RBB.Eel".Translate();
                                }
                                else
                                {
                                    var value5 = Rand.Value;
                                    if (terrainDef.defName.Contains("Ocean"))
                                    {
                                        if (terrainDef.defName.Contains("Deep") && value5 > 0.7)
                                        {
                                            if (value5 > 0.75)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Squid"), pawn.Position, Map);
                                                text6 += "RBB.Squid".Translate();
                                            }
                                            else if (value5 > 0.85)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Octopus"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.Octopus".Translate();
                                            }
                                            else if (value5 > 0.86)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Tuna"), pawn.Position, Map);
                                                text6 += "RBB.Tuna".Translate();
                                            }
                                            else if (value5 > 0.9)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Sardines"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.Sardines".Translate();
                                            }
                                            else
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_SeaCucumber"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.SeaCucumber".Translate();
                                            }
                                        }
                                        else if (Map.Biome.defName == "IceSheet" || Map.Biome.defName == "SeaIce")
                                        {
                                            if (value5 < 0.025)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Tuna"), pawn.Position, Map);
                                                text6 += "RBB.Tuna".Translate();
                                            }
                                            else if (value5 < 0.15)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Salmon"), pawn.Position, Map);
                                                text6 += "RBB.Salmon".Translate();
                                            }
                                            else if (value5 < 0.25)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Jellyfish"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.Jellyfish".Translate();
                                            }
                                            else if (value5 < 0.35)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Kingcrab"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.Kingcrab".Translate();
                                            }
                                            else if (value5 < 0.475)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_FishTiny"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.TinyFish".Translate();
                                            }
                                            else if (value5 > 0.975)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Squid"), pawn.Position, Map);
                                                text6 += "RBB.Squid".Translate();
                                            }
                                            else if (value5 > 0.95)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Octopus"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.Octopus".Translate();
                                            }
                                            else if (value5 > 0.9)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_SeaCucumber"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.SeaCucumber".Translate();
                                            }
                                            else
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Fish"), pawn.Position, Map);
                                                text6 += "RBB.Fish".Translate();
                                            }
                                        }
                                        else if (Map.Biome.defName.Contains("Tundra") ||
                                                 Map.Biome.defName.Contains("Permafrost") ||
                                                 Map.Biome.defName.Contains("Boreal") ||
                                                 Map.Biome.defName.Contains("ColdBog"))
                                        {
                                            if (value5 < 0.05)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Tuna"), pawn.Position, Map);
                                                text6 += "RBB.Tuna".Translate();
                                            }
                                            else if (value5 < 0.15)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Salmon"), pawn.Position, Map);
                                                text6 += "RBB.Salmon".Translate();
                                            }
                                            else if (value5 < 0.25)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Jellyfish"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.Jellyfish".Translate();
                                            }
                                            else if (value5 < 0.3)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Kingcrab"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.Kingcrab".Translate();
                                            }
                                            else if (value5 < 0.35)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_RawFishTiny"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.TinyFish".Translate();
                                            }
                                            else if (value5 < 0.4)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Bass"), pawn.Position, Map);
                                                text6 += "RBB.Bass".Translate();
                                            }
                                            else if (value5 > 0.95)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Squid"), pawn.Position, Map);
                                                text6 += "RBB.Squid".Translate();
                                            }
                                            else if (value5 > 0.965)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Octopus"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.Octopus".Translate();
                                            }
                                            else if (value5 > 0.9)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_SeaCucumber"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.SeaCucumber".Translate();
                                            }
                                            else
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Fish"), pawn.Position, Map);
                                                text6 += "RBB.Fish".Translate();
                                            }
                                        }
                                        else if (Map.Biome.defName.Contains("Temperate") &&
                                                 Map.Biome.defName != "RRP_TemperateDesert" ||
                                                 Map.Biome.defName.Contains("Steppes") ||
                                                 Map.Biome.defName.Contains("Grassland") ||
                                                 Map.Biome.defName.Contains("Savanna"))
                                        {
                                            if (value5 < 0.02)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Tuna"), pawn.Position, Map);
                                                text6 += "RBB.Tuna".Translate();
                                            }
                                            else if (value5 < 0.05)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Salmon"), pawn.Position, Map);
                                                text6 += "RBB.Salmon".Translate();
                                            }
                                            else if (value5 < 0.15)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Jellyfish"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.Jellyfish".Translate();
                                            }
                                            else if (value5 < 0.25)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Pufferfish"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.Pufferfish".Translate();
                                            }
                                            else if (value5 < 0.45)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Bass"), pawn.Position, Map);
                                                text6 += "RBB.Bass".Translate();
                                            }
                                            else if (value5 < 0.65)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_FishB"), pawn.Position, Map);
                                                text6 += "RBB.Fish".Translate();
                                            }
                                            else if (value5 > 0.95)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Squid"), pawn.Position, Map);
                                                text6 += "RBB.Squid".Translate();
                                            }
                                            else if (value5 > 0.965)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Octopus"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.Octopus".Translate();
                                            }
                                            else if (value5 > 0.9)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_SeaCucumber"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.SeaCucumber".Translate();
                                            }
                                            else
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Fish"), pawn.Position, Map);
                                                text6 += "RBB.Fish".Translate();
                                            }
                                        }
                                        else if (Map.Biome.defName.Contains("Tropical") ||
                                                 Map.Biome.defName == "AridShrubland" ||
                                                 Map.Biome.defName.Contains("Desert") ||
                                                 Map.Biome.defName.Contains("Oasis"))
                                        {
                                            if (value5 < 0.1)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Jellyfish"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.Jellyfish".Translate();
                                            }
                                            else if (value5 < 0.25)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Pufferfish"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.Pufferfish".Translate();
                                            }
                                            else if (value5 < 0.45)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Bass"), pawn.Position, Map);
                                                text6 += "RBB.Bass".Translate();
                                            }
                                            else if (value5 < 0.55)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Clownfish"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.Clownfish".Translate();
                                            }
                                            else if (value5 < 0.65)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_FishB"), pawn.Position, Map);
                                                text6 += "RBB.Fish".Translate();
                                            }
                                            else if (value5 > 0.95)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Squid"), pawn.Position, Map);
                                                text6 += "RBB.Squid".Translate();
                                            }
                                            else if (value5 > 0.965)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Octopus"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.Octopus".Translate();
                                            }
                                            else if (value5 > 0.9)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_SeaCucumber"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.SeaCucumber".Translate();
                                            }
                                            else
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Fish"), pawn.Position, Map);
                                                text6 += "RBB.Fish".Translate();
                                            }
                                        }
                                        else if (value5 < 0.1)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Jellyfish"), pawn.Position, Map);
                                            text6 += "RBB.Jellyfish".Translate();
                                        }
                                        else if (value5 < 0.2)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Pufferfish"), pawn.Position, Map);
                                            text6 += "RBB.Pufferfish".Translate();
                                        }
                                        else if (value5 < 0.3)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_FishTiny"), pawn.Position, Map);
                                            text6 += "RBB.TinyFish".Translate();
                                        }
                                        else if (value5 < 0.4)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Bass"), pawn.Position, Map);
                                            text6 += "RBB.Bass".Translate();
                                        }
                                        else if (value5 < 0.5)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Anchovies"), pawn.Position, Map);
                                            text6 += "RBB.Anchovies".Translate();
                                        }
                                        else if (value5 > 0.95)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Squid"), pawn.Position, Map);
                                            text6 += "RBB.Squid".Translate();
                                        }
                                        else if (value5 > 0.965)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Octopus"), pawn.Position, Map);
                                            text6 += "RBB.Octopus".Translate();
                                        }
                                        else if (value5 > 0.9)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_SeaCucumber"), pawn.Position,
                                                Map);
                                            text6 += "RBB.SeaCucumber".Translate();
                                        }
                                        else
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Fish"), pawn.Position, Map);
                                            text6 += "RBB.Fish".Translate();
                                        }
                                    }
                                    else if (Map.Biome.defName.Contains("Tundra") ||
                                             Map.Biome.defName.Contains("Permafrost"))
                                    {
                                        if (value5 < 0.5)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Salmon"), pawn.Position, Map);
                                            text6 += "RBB.Salmon".Translate();
                                        }
                                        else if (value5 < 0.25)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_FishB"), pawn.Position, Map);
                                            text6 += "RBB.Fish".Translate();
                                        }
                                        else if (value5 < 0.7)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Bass"), pawn.Position, Map);
                                            text6 += "RBB.Bass".Translate();
                                        }
                                        else if (value5 < 0.75)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Catfish"), pawn.Position, Map);
                                            text6 += "RBB.Catfish".Translate();
                                        }
                                        else
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Fish"), pawn.Position, Map);
                                            text6 += "RBB.Fish".Translate();
                                        }
                                    }
                                    else if (Map.Biome.defName.Contains("Boreal") ||
                                             Map.Biome.defName.Contains("ColdBog"))
                                    {
                                        if (value5 < 0.02)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_SturgeonCaviar"), pawn.Position,
                                                Map);
                                            text6 += "RBB.Sturgeon".Translate();
                                        }
                                        else if (value5 < 0.1)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Sturgeon"), pawn.Position, Map);
                                            text6 += "RBB.Sturgeon".Translate();
                                        }
                                        else if (value5 < 0.3)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Salmon"), pawn.Position, Map);
                                            text6 += "RBB.Salmon".Translate();
                                        }
                                        else if (value5 < 0.7)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Bass"), pawn.Position, Map);
                                            text6 += "RBB.Bass".Translate();
                                        }
                                        else if (value5 < 0.75)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Catfish"), pawn.Position, Map);
                                            text6 += "RBB.Catfish".Translate();
                                        }
                                        else
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Fish"), pawn.Position, Map);
                                            text6 += "RBB.Fish".Translate();
                                        }
                                    }
                                    else if (Map.Biome.defName.Contains("Temperate") &&
                                             Map.Biome.defName != "RRP_TemperateDesert" ||
                                             Map.Biome.defName.Contains("Steppes") ||
                                             Map.Biome.defName.Contains("Grassland") ||
                                             Map.Biome.defName.Contains("Savanna"))
                                    {
                                        if (Map.Biome.defName.Contains("Swamp"))
                                        {
                                            if (value5 < 0.02)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_SturgeonCaviar"),
                                                    pawn.Position,
                                                    Map);
                                                text6 += "RBB.Sturgeon".Translate();
                                            }
                                            else if (value5 < 0.1)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Sturgeon"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.Sturgeon".Translate();
                                            }
                                            else if (value5 < 0.2)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Salmon"), pawn.Position, Map);
                                                text6 += "RBB.Salmon".Translate();
                                            }
                                            else if (value5 < 0.05)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_FishB"), pawn.Position, Map);
                                                text6 += "RBB.Fish".Translate();
                                            }
                                            else if (value5 < 0.45)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Bass"), pawn.Position, Map);
                                                text6 += "RBB.Bass".Translate();
                                            }
                                            else if (value5 > 0.75)
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Catfish"), pawn.Position,
                                                    Map);
                                                text6 += "RBB.Catfish".Translate();
                                            }
                                            else
                                            {
                                                thing = GenSpawn.Spawn(ThingDef.Named("RF_Fish"), pawn.Position, Map);
                                                text6 += "RBB.Fish".Translate();
                                            }
                                        }
                                        else if (value5 < 0.04)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_SturgeonCaviar"), pawn.Position,
                                                Map);
                                            text6 += "RBB.Sturgeon".Translate();
                                        }
                                        else if (value5 < 0.2)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Sturgeon"), pawn.Position, Map);
                                            text6 += "RBB.Sturgeon".Translate();
                                        }
                                        else if (value5 < 0.3)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Salmon"), pawn.Position, Map);
                                            text6 += "RBB.Salmon".Translate();
                                        }
                                        else if (value5 < 0.6)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Bass"), pawn.Position, Map);
                                            text6 += "RBB.Bass".Translate();
                                        }
                                        else if (value5 > 0.75)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Catfish"), pawn.Position, Map);
                                            text6 += "RBB.Catfish".Translate();
                                        }
                                        else
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Fish"), pawn.Position, Map);
                                            text6 += "RBB.Fish".Translate();
                                        }
                                    }
                                    else if (Map.Biome.defName.Contains("Tropical"))
                                    {
                                        if (value5 < 0.01)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_SturgeonCaviar"), pawn.Position,
                                                Map);
                                            text6 += "RBB.Sturgeon".Translate();
                                        }
                                        else if (value5 < 0.05)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Sturgeon"), pawn.Position, Map);
                                            text6 += "RBB.Sturgeon".Translate();
                                        }
                                        else if (value5 < 0.15)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Arapaima"), pawn.Position, Map);
                                            text6 += "RBB.Arapaima".Translate();
                                        }
                                        else if (value5 < 0.45)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Piranha"), pawn.Position, Map);
                                            text6 += "RBB.Piranha".Translate();
                                        }
                                        else if (value5 < 0.65)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Bass"), pawn.Position, Map);
                                            text6 += "RBB.Bass".Translate();
                                        }
                                        else if (value5 > 0.75)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Catfish"), pawn.Position, Map);
                                            text6 += "RBB.Catfish".Translate();
                                        }
                                        else
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Fish"), pawn.Position, Map);
                                            text6 += "RBB.Fish".Translate();
                                        }
                                    }
                                    else if (Map.Biome.defName == "AridShrubland" ||
                                             Map.Biome.defName.Contains("DesertArchi"))
                                    {
                                        if (value5 < 0.02)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_SturgeonCaviar"), pawn.Position,
                                                Map);
                                            text6 += "RBB.Sturgeon".Translate();
                                        }
                                        else if (value5 < 0.1)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Sturgeon"), pawn.Position, Map);
                                            text6 += "RBB.Sturgeon".Translate();
                                        }
                                        else if (value5 < 0.2)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_FishTiny"), pawn.Position, Map);
                                            text6 += "RBB.TinyFish".Translate();
                                        }
                                        else if (value5 < 0.7)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Bass"), pawn.Position, Map);
                                            text6 += "RBB.Bass".Translate();
                                        }
                                        else if (value5 > 0.75)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Catfish"), pawn.Position, Map);
                                            text6 += "RBB.Catfish".Translate();
                                        }
                                        else
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Fish"), pawn.Position, Map);
                                            text6 += "RBB.Fish".Translate();
                                        }
                                    }
                                    else if (Map.Biome.defName == "Desert" || Map.Biome.defName.Contains("Oasis"))
                                    {
                                        if (value5 < 0.01)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_SturgeonCaviar"), pawn.Position,
                                                Map);
                                            text6 += "RBB.Sturgeon".Translate();
                                        }
                                        else if (value5 < 0.05)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Sturgeon"), pawn.Position, Map);
                                            text6 += "RBB.Sturgeon".Translate();
                                        }
                                        else if (value5 < 0.3)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_FishTiny"), pawn.Position, Map);
                                            text6 += "RBB.TinyFish".Translate();
                                        }
                                        else if (value5 < 0.75)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Bass"), pawn.Position, Map);
                                            text6 += "RBB.Bass".Translate();
                                        }
                                        else
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Fish"), pawn.Position, Map);
                                            text6 += "RBB.Fish".Translate();
                                        }
                                    }
                                    else if (Map.Biome.defName == "ExtremeDesert" ||
                                             Map.Biome.defName == "RRP_TemperateDesert")
                                    {
                                        if (value5 < 0.5)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_FishTiny"), pawn.Position, Map);
                                            text6 += "RBB.TinyFish".Translate();
                                        }
                                        else if (value5 < 0.75)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Bass"), pawn.Position, Map);
                                            text6 += "RBB.Bass".Translate();
                                        }
                                        else
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Fish"), pawn.Position, Map);
                                            text6 += "RBB.Fish".Translate();
                                        }
                                    }
                                    else if (Map.Biome.defName == "IceSheet" || Map.Biome.defName == "SeaIce")
                                    {
                                        if (value5 < 0.2)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Salmon"), pawn.Position, Map);
                                            text6 += "RBB.Salmon".Translate();
                                        }
                                        else if (value5 < 0.35)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Sardines"), pawn.Position, Map);
                                            text6 += "RBB.Sardines".Translate();
                                        }
                                        else if (value5 < 0.65)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_FishTiny"), pawn.Position, Map);
                                            text6 += "RBB.TinyFish".Translate();
                                        }
                                        else if (value5 < 0.75)
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Bass"), pawn.Position, Map);
                                            text6 += "RBB.Bass".Translate();
                                        }
                                        else
                                        {
                                            thing = GenSpawn.Spawn(ThingDef.Named("RF_Fish"), pawn.Position, Map);
                                            text6 += "RBB.Fish".Translate();
                                        }
                                    }
                                    else if (value5 < 0.01)
                                    {
                                        thing = GenSpawn.Spawn(ThingDef.Named("RF_SturgeonCaviar"), pawn.Position, Map);
                                        text6 += "RBB.Sturgeon".Translate();
                                    }
                                    else if (value5 < 0.05)
                                    {
                                        thing = GenSpawn.Spawn(ThingDef.Named("RF_Sturgeon"), pawn.Position, Map);
                                        text6 += "RBB.Sturgeon".Translate();
                                    }
                                    else if (value5 < 0.2)
                                    {
                                        thing = GenSpawn.Spawn(ThingDef.Named("RF_FishTiny"), pawn.Position, Map);
                                        text6 += "RBB.TinyFish".Translate();
                                    }
                                    else if (value5 < 0.7)
                                    {
                                        thing = GenSpawn.Spawn(ThingDef.Named("RF_Bass"), pawn.Position, Map);
                                        text6 += "RBB.Bass".Translate();
                                    }
                                    else if (value5 > 0.75)
                                    {
                                        thing = GenSpawn.Spawn(ThingDef.Named("RF_Catfish"), pawn.Position, Map);
                                        text6 += "RBB.Catfish".Translate();
                                    }
                                    else
                                    {
                                        thing = GenSpawn.Spawn(ThingDef.Named("RF_Fish"), pawn.Position, Map);
                                        text6 += "RBB.Fish".Translate();
                                    }
                                }

                                fishingSpot.fishStock--;
                                if (Controller.Settings.successLevel >= 1f)
                                {
                                    if (Controller.Settings.successLevel < 2f)
                                    {
                                        Messages.Message(text6, new TargetInfo(pawn.Position, Map),
                                            MessageTypeDefOf.SilentInput);
                                    }
                                    else if (Controller.Settings.successLevel < 3f)
                                    {
                                        Messages.Message(text6, new TargetInfo(pawn.Position, Map),
                                            MessageTypeDefOf.PositiveEvent);
                                    }
                                    else
                                    {
                                        Find.LetterStack.ReceiveLetter("RBB.FishingSuccessTitle".Translate(), text6,
                                            LetterDefOf.PositiveEvent, new TargetInfo(pawn.Position, Map));
                                    }
                                }
                            }
                        }

                        pawn.carryTracker.TryStartCarry(thing, thing.stackCount);
                        pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Near, out thing);
                        if (!Controller.Settings.fishersHaul.Equals(true))
                        {
                            pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
                        }
                        else if (StoreUtility.TryFindBestBetterStoreCellFor(thing, pawn, Map, StoragePriority.Unstored,
                                     pawn.Faction, out var foundCell))
                        {
                            pawn.Reserve(thing, job);
                            pawn.Reserve(foundCell, job);
                            pawn.CurJob.SetTarget(TargetIndex.B, foundCell);
                            pawn.CurJob.SetTarget(TargetIndex.A, thing);
                            pawn.CurJob.count = 1;
                            pawn.CurJob.haulMode = HaulMode.ToCellStorage;
                        }
                        else
                        {
                            pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
                        }
                    }
                }
            };
        }

        yield return Toils_Haul.StartCarryThing(TargetIndex.A);
        var carryToCell = Toils_Haul.CarryHauledThingToCell(TargetIndex.B);
        yield return carryToCell;
        yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.B, carryToCell, true);
        yield return Toils_Reserve.Release(fishingSpotIndex);
    }
}