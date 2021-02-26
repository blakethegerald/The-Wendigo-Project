using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace WendigoProject
{
	public class MentalState_WendigoBerserk : MentalState
	{
		public override bool ForceHostileTo(Thing t)
		{
			return true;
		}

		public override bool ForceHostileTo(Faction f)
		{
			return true;
		}

		public override RandomSocialMode SocialModeMax()
		{
			return RandomSocialMode.Off;
		}

        public override void PreStart()
        {
            base.PreStart();
			if (this.pawn.health.hediffSet.GetFirstHediffOfDef(WP_DefOf.WB_CombatWendigoBerserk) is null)
            {
				var hediff = HediffMaker.MakeHediff(WP_DefOf.WB_CombatWendigoBerserk, this.pawn);
				pawn.health.AddHediff(hediff);
            }
        }

        public override void PostEnd()
        {
            base.PostEnd();
			var hediff = this.pawn.health.hediffSet.GetFirstHediffOfDef(WP_DefOf.WB_CombatWendigoBerserk);
			if (hediff != null)
            {
				this.pawn.health.RemoveHediff(hediff);
            }
		}
    }
}
