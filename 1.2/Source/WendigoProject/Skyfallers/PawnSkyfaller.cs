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
	public class PawnSkyfaller : Skyfaller
	{
		public override void Tick()
		{
			this.innerContainer.ThingOwnerTick(true);
			this.ticksToImpact--;
			if (this.ticksToImpact == 0)
			{
				this.EjectPawn();
				this.Impact();
			}
			else if(this.ticksToImpact < 0)
			{
				Log.Error("ticksToImpact < 0. Was there an exception? Destroying skyfaller.", false);
				this.EjectPawn();
				this.Destroy(0);
			}
		}

		private void EjectPawn()
		{
			Pawn pawn = GetInnerPawn();
			if (pawn != null)
			{
				GenPlace.TryPlaceThing(pawn, base.Position, base.Map, ThingPlaceMode.Near, delegate (Thing thing, int count)
				{
					PawnUtility.RecoverFromUnwalkablePositionOrKill(thing.Position, thing.Map);
				}, null, default(Rot4));
			}
		}
		public override void DrawAt(Vector3 drawLoc, bool flip = false)
		{
			Pawn pawn = GetInnerPawn();
			if (pawn != null)
			{
				new PawnRenderer(pawn).RenderPawnAt(drawLoc);
			}
		}
		private Pawn GetInnerPawn()
		{
			if (this.innerContainer.Any && this.innerContainer.Count > 0)
			{
				for (int i = 0; i < this.innerContainer.Count; i++)
				{
					if (this.innerContainer[i] is Pawn pawn)
					{
						return pawn;
					}
				}
			}
			return null;
		}

        protected override void Impact()
        {
			for (int i = this.innerContainer.Count - 1; i >= 0; i--)
			{
				GenPlace.TryPlaceThing(this.innerContainer[i], base.Position, base.Map, ThingPlaceMode.Near, delegate (Thing thing, int count)
				{
					PawnUtility.RecoverFromUnwalkablePositionOrKill(thing.Position, thing.Map);
				}, null, default(Rot4));
			}
			this.innerContainer.ClearAndDestroyContents(0);
			if (this.def.skyfaller.impactSound != null)
			{
				SoundStarter.PlayOneShot(this.def.skyfaller.impactSound, SoundInfo.InMap(new TargetInfo(base.Position, base.Map, false), 0));
			}
			this.Destroy(0);
		}
    }
}