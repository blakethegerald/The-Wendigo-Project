using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WendigoProject
{
	public class Command_Leap : Command
	{
		internal TargetingParameters ForLeapDestination()
		{
			TargetingParameters targetingParameters = new TargetingParameters();
			targetingParameters.canTargetLocations = true;
			targetingParameters.canTargetSelf = false;
			targetingParameters.canTargetPawns = false;
			targetingParameters.canTargetFires = false;
			targetingParameters.canTargetBuildings = false;
			targetingParameters.canTargetItems = false;
			targetingParameters.validator = (TargetInfo x) => IsRightSpot(x);
			return targetingParameters;
		}

		public bool IsRightSpot(TargetInfo x)
        {
			var maxRange = 10;
			GenDraw.DrawRadiusRing(pawn.Position, maxRange);
			return maxRange >= pawn.Position.DistanceTo(x.Cell) && DropCellFinder.IsGoodDropSpot(x.Cell, x.Map, true, true, true);
		}
		public override void ProcessInput(Event ev)
		{
			base.ProcessInput(ev);
			SoundStarter.PlayOneShotOnCamera(SoundDefOf.Tick_Tiny, null);
			Texture2D texture2D = ContentFinder<Texture2D>.Get("UI/Leap", true);
			Find.Targeter.BeginTargeting(ForLeapDestination(), delegate (LocalTargetInfo target)
			{
				this.action(target.Cell);
			}, this.pawn, null, texture2D);
		}

        public override void GizmoUpdateOnMouseover()
        {
            base.GizmoUpdateOnMouseover();
			var maxRange = 10f;
			GenDraw.DrawRadiusRing(pawn.Position, maxRange);
		}

		public Action<IntVec3> action;
		public Pawn pawn;
	}
}
