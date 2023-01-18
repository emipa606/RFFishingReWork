using RimWorld;
using Verse;
using Verse.AI;

namespace RBB_Code;

public class WorkGiver_IceFish : WorkGiver_Scanner
{
    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(ThingDef.Named("RBB_IceFishingSpot"));

    public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is not Building_FishingSpot buildingFishingSpot)
        {
            return false;
        }

        return !buildingFishingSpot.IsBurning() && !pawn.Dead && !pawn.Downed && !pawn.IsBurning() &&
               pawn.CanReserveAndReach(buildingFishingSpot, PathEndMode, Danger.Some) &&
               buildingFishingSpot.fishStock >= 1;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is Building_FishingSpot building_FishingSpot)
        {
            return new Job(DefDatabase<JobDef>.GetNamed("JobDef_CatchFish"), building_FishingSpot,
                building_FishingSpot.fishingSpotCell);
        }

        return null;
    }
}