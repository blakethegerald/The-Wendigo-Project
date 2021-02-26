using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WendigoProject
{
    public class CompProperties_WendigoProgram : CompProperties
    {
        public CompProperties_WendigoProgram()
        {
            this.compClass = typeof(CompWendigoProgram);
        }
    }

    public class CompWendigoProgram : ThingComp
    {
        public static Dictionary<Pawn, CompWendigoProgram> pawnsWithComp = new Dictionary<Pawn, CompWendigoProgram>();
        public bool isOnRageBaker;
        public bool activeRageState;

        public Pawn Pawn => this.parent as Pawn;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            pawnsWithComp[this.parent as Pawn] = this;
        }

        public void ActivateBerserkState()
        {
            isOnRageBaker = false;
            activeRageState = true;
            Pawn.mindState.mentalStateHandler.TryStartMentalState(WP_DefOf.WB_Berserk, null, true);
        }

        public Pawn target;
        public void BeginTargetingForSkinWalkerCore(Pawn target)
        {
            this.target = target;
        }

        public bool skinWalkerCoreIsActivated;
        public void ActivateSkinWalkerCore()
        {
            if (Pawn.Faction == Faction.OfPlayer)
            {
                skinWalkerCoreIsActivated = true;
                Pawn.SetFaction(target.Faction);
                target = null;
            }
        }
        public void DisactivateSkinWalkerCore()
        {
            skinWalkerCoreIsActivated = false;
            Pawn.SetFaction(Faction.OfPlayer);
        }

        public int invisibilityDuration;
        public bool invisibilityEnabled;
        public bool IsInvisible => invisibilityEnabled && this.invisibilityDuration < 20 * 60;
        public override void CompTick()
        {
            base.CompTick();
            if (invisibilityEnabled)
            {
                invisibilityDuration++;
            }
            screecherHornCooldown--;
        }
        public void ActivateCloakingSystem()
        {
            invisibilityDuration = 0;
            invisibilityEnabled = true;
        }

        public void DisactivateCloakingSystem()
        {
            invisibilityDuration = 0;
            invisibilityEnabled = false;
        }

        public int screecherHornCooldown;
        public void ActivateScreecherHorn()
        {
            screecherHornCooldown = 20 * 60;
            MoteMaker.MakeStaticMote(parent.Position, parent.Map, ThingDefOf.Mote_PsycastAreaEffect, 10f);
            foreach (var cell in GenRadial.RadialCellsAround(parent.Position, 10f, true))
            {
                foreach (var pawn in cell.GetThingList(parent.Map).OfType<Pawn>())
                {
                    if (pawn != this.parent)
                    {
                        pawn.stances?.stunner?.StunFor(10 * 60, this.parent);
                    }
                }
            }
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref isOnRageBaker, "isOnRageBaker");
            Scribe_Values.Look(ref activeRageState, "activeRageState");
            Scribe_Values.Look(ref invisibilityDuration, "invisibilityDuration");
            Scribe_Values.Look(ref invisibilityEnabled, "isInvisible");
            Scribe_Values.Look(ref screecherHornCooldown, "screecherHornCooldown");
            Scribe_Values.Look(ref skinWalkerCoreIsActivated, "skinWalkerCoreIsActivated");
            Scribe_References.Look(ref target, "target");
        }
    }
}
