using UnityEngine;
using Verse;

namespace RBB_Code;

public class Settings : ModSettings
{
    public float failureLevel = 1.5f;

    public bool fishersHaul;
    public float fishingEfficiency = 100f;

    public float junkLevel = 1.5f;

    public float successLevel = 1.5f;

    public float trapEfficiency = 100f;

    public float trapsuccessLevel = 1.5f;

    public float treasureLevel = 2.5f;

    public void DoWindowContents(Rect canvas)
    {
        var listing_Standard = new Listing_Standard
        {
            ColumnWidth = canvas.width
        };
        listing_Standard.Begin(canvas);
        Text.Font = GameFont.Small;
        listing_Standard.Label("RBB.Efficiency".Translate());
        Text.Font = GameFont.Tiny;
        listing_Standard.Label("RBB.FishingEfficiency".Translate() + "  " + ((int)fishingEfficiency).ToString() + "%",
            -1f, "RBB.FishingEfficiencyTip".Translate());
        fishingEfficiency = listing_Standard.Slider(fishingEfficiency, 50f, 150.99f);
        listing_Standard.Gap(8f);
        listing_Standard.Label("RBB.TrapEfficiency".Translate() + "  " + ((int)trapEfficiency).ToString() + "%", -1f,
            "RBB.TrapEfficiencyTip".Translate());
        trapEfficiency = listing_Standard.Slider(trapEfficiency, 50f, 150.99f);
        listing_Standard.Gap();
        Text.Font = GameFont.Small;
        listing_Standard.Label("RBB.NotificationType".Translate());
        Text.Font = GameFont.Tiny;
        listing_Standard.Label("RBB.failureLevel".Translate(failureLevel));
        if (failureLevel < 1f)
        {
            listing_Standard.Label("          " + "RBB.NoticeNone".Translate());
        }
        else if (failureLevel < 2f)
        {
            GUI.contentColor = Color.yellow;
            listing_Standard.Label("          " + "RBB.NoticeSilent".Translate());
            GUI.contentColor = Color.white;
        }
        else if (failureLevel < 3f)
        {
            listing_Standard.Label("          " + "RBB.NoticeDing".Translate());
        }
        else
        {
            listing_Standard.Label("          " + "RBB.NoticeBox".Translate());
        }

        failureLevel = listing_Standard.Slider(failureLevel, 0f, 4f);
        listing_Standard.Gap(8f);
        listing_Standard.Label("RBB.junkLevel".Translate(junkLevel));
        if (junkLevel < 1f)
        {
            listing_Standard.Label("          " + "RBB.NoticeNone".Translate());
        }
        else if (junkLevel < 2f)
        {
            GUI.contentColor = Color.yellow;
            listing_Standard.Label("          " + "RBB.NoticeSilent".Translate());
            GUI.contentColor = Color.white;
        }
        else if (junkLevel < 3f)
        {
            listing_Standard.Label("          " + "RBB.NoticeDing".Translate());
        }
        else
        {
            listing_Standard.Label("          " + "RBB.NoticeBox".Translate());
        }

        junkLevel = listing_Standard.Slider(junkLevel, 0f, 4f);
        listing_Standard.Gap(8f);
        listing_Standard.Label("RBB.successLevel".Translate(successLevel));
        Text.Font = GameFont.Tiny;
        if (successLevel < 1f)
        {
            listing_Standard.Label("          " + "RBB.NoticeNone".Translate());
        }
        else if (successLevel < 2f)
        {
            GUI.contentColor = Color.yellow;
            listing_Standard.Label("          " + "RBB.NoticeSilent".Translate());
            GUI.contentColor = Color.white;
        }
        else if (successLevel < 3f)
        {
            listing_Standard.Label("          " + "RBB.NoticeDing".Translate());
        }
        else
        {
            listing_Standard.Label("          " + "RBB.NoticeBox".Translate());
        }

        successLevel = listing_Standard.Slider(successLevel, 0f, 4f);
        listing_Standard.Gap(8f);
        listing_Standard.Label("RBB.trapsuccessLevel".Translate(trapsuccessLevel));
        if (trapsuccessLevel < 1f)
        {
            listing_Standard.Label("          " + "RBB.NoticeNone".Translate());
        }
        else if (trapsuccessLevel < 2f)
        {
            GUI.contentColor = Color.yellow;
            listing_Standard.Label("          " + "RBB.NoticeSilent".Translate());
            GUI.contentColor = Color.white;
        }
        else if (trapsuccessLevel < 3f)
        {
            listing_Standard.Label("          " + "RBB.NoticeDing".Translate());
        }
        else
        {
            listing_Standard.Label("          " + "RBB.NoticeBox".Translate());
        }

        trapsuccessLevel = listing_Standard.Slider(trapsuccessLevel, 0f, 4f);
        listing_Standard.Gap(8f);
        listing_Standard.Label("RBB.treasureLevel".Translate(treasureLevel));
        if (treasureLevel < 1f)
        {
            listing_Standard.Label("          " + "RBB.NoticeNone".Translate());
        }
        else if (treasureLevel < 2f)
        {
            listing_Standard.Label("          " + "RBB.NoticeSilent".Translate());
        }
        else if (treasureLevel < 3f)
        {
            GUI.contentColor = Color.yellow;
            listing_Standard.Label("          " + "RBB.NoticeDing".Translate());
            GUI.contentColor = Color.white;
        }
        else
        {
            listing_Standard.Label("          " + "RBB.NoticeBox".Translate());
        }

        treasureLevel = listing_Standard.Slider(treasureLevel, 0f, 4f);
        Text.Font = GameFont.Small;
        listing_Standard.Gap();
        listing_Standard.CheckboxLabeled("RBB.FishersHaul".Translate(), ref fishersHaul,
            "RBB.FishersHaulTip".Translate());
        listing_Standard.Gap();
        if (Controller.currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("RBB.CurrentModVersion".Translate(Controller.currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.End();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref fishingEfficiency, "fishingEfficiency", 100f);
        Scribe_Values.Look(ref trapEfficiency, "trapEfficiency", 100f);
        Scribe_Values.Look(ref failureLevel, "failureLevel", 1.5f);
        Scribe_Values.Look(ref junkLevel, "junkLevel", 1.5f);
        Scribe_Values.Look(ref successLevel, "successLevel", 1.5f);
        Scribe_Values.Look(ref trapsuccessLevel, "trapsuccessLevel", 1.5f);
        Scribe_Values.Look(ref treasureLevel, "treasureLevel", 2.5f);
        Scribe_Values.Look(ref fishersHaul, "fishersHaul");
    }
}