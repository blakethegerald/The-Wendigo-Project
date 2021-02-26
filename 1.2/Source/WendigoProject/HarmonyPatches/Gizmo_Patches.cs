using HarmonyLib;
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
	[HarmonyPatch(typeof(Pawn), "GetGizmos")]
	public class Pawn_GetGizmos_Patch
	{
		public static TargetingParameters ForPawn(Pawn caller = null, float? radius = null)
		{
			TargetingParameters targetingParameters = new TargetingParameters();
			targetingParameters.canTargetPawns = true;
			targetingParameters.validator = (TargetInfo target) => target.Thing is Pawn;
			return targetingParameters;
		}
		public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Pawn __instance)
		{
			foreach (var g in __result)
			{
				yield return g;
			}
			if (__instance.MentalState is null)
            {
				if (CompWendigoProgram.pawnsWithComp.TryGetValue(__instance, out var comp))
				{
					if (comp.isOnRageBaker && !comp.activeRageState)
					{
						Command_Action command_Action = new Command_Action();
						command_Action.defaultLabel = "WP.ActivateBerserkState".Translate();
						command_Action.defaultLabel = "WP.ActivateBerserkStateDesc".Translate();
						command_Action.action = delegate
						{
							comp.ActivateBerserkState();
						};
						yield return command_Action;
					}

					if (__instance.IsColonistPlayerControlled)
					{
						var hareGradeLegs = __instance.health.hediffSet.hediffs.Where(x => x.def == WP_DefOf.WP_HareGradeLeg);
						if (hareGradeLegs.Count() == 2)
						{
							yield return new Command_Leap
							{
								defaultLabel = "WP.Leap".Translate(),
								defaultDesc = "WP.LeapDesc".Translate(),
								pawn = __instance,
								action = delegate (IntVec3 cell)
								{
									StartLeap(__instance, cell);
								}
							};
						}

						var skinWalkerCore = __instance.health.hediffSet.hediffs.Where(x => x.def == WP_DefOf.WP_SkinwalkerPersonaCore);
						if (skinWalkerCore.Count() == 1)
						{
							if (__instance.Faction == Faction.OfPlayer)
                            {
								Command_Action command_Action = new Command_Action();
								command_Action.defaultLabel = "WP.ActivateSkinWalkerPersona".Translate();
								command_Action.defaultLabel = "WP.ActivateSkinWalkerPersonaDesc".Translate();
								command_Action.action = delegate
								{
									Find.Targeter.BeginTargeting(ForPawn(), delegate (LocalTargetInfo x)
									{
										comp.BeginTargetingForSkinWalkerCore(x.Thing as Pawn);
									}, null, null, null);

								};
								yield return command_Action;
							}
							else
                            {
								Command_Action command_Action = new Command_Action();
								command_Action.defaultLabel = "WP.DisactivateSkinWalkerPersona".Translate();
								command_Action.defaultLabel = "WP.DisactivateSkinWalkerPersonaDesc".Translate();
								command_Action.action = delegate
								{
									comp.DisactivateSkinWalkerCore();
								};
								yield return command_Action;
							}
						}

						var cloakingSystem = __instance.health.hediffSet.hediffs.Where(x => x.def == WP_DefOf.WP_BlackWidowCloakingSystem);
						if (cloakingSystem.Count() == 1)
						{
							Command_Action command_Action2 = new Command_Action();
							command_Action2.defaultLabel = "WP.ActivateCloakingSystem".Translate();
							command_Action2.defaultLabel = "WP.ActivateCloakingSystemDesc".Translate();
							command_Action2.action = delegate
							{
								if (comp.invisibilityEnabled)
								{
									comp.DisactivateCloakingSystem();

								}
								else
								{
									comp.ActivateCloakingSystem();
								}
							};
							yield return command_Action2;
						}

						var screecherHorn = __instance.health.hediffSet.hediffs.Where(x => x.def == WP_DefOf.WP_ScreecherHorn);
						if (screecherHorn.Count() == 1)
						{
							Command_Action command_Action3 = new Command_Action();
							command_Action3.defaultLabel = "WP.ActivateScreecherHorn".Translate();
							command_Action3.defaultLabel = "WP.ActivateScreecherHornDesc".Translate();
							command_Action3.disabled = comp.screecherHornCooldown > 0;
							command_Action3.action = delegate
							{
								comp.ActivateScreecherHorn();
							};
							yield return command_Action3;
						}
					}
				}
			}
		}

		private static void StartLeap(Pawn pawn, IntVec3 cell)
		{
			float num;
			RotatePawn(pawn, cell, out num);
			var exactPos = pawn.DrawPos;
			var cellPos = cell.ToVector3Shifted();
			exactPos.y = 0;
			cellPos.y = 0;

			float speed = WP_DefOf.WP_PawnSkyfaller.skyfaller.speed;
			float distance = Vector3.Distance(exactPos, cellPos);
			int ticksToImpact = (int)(distance / speed);
			num += 270f;
			bool flag4 = num >= 360f;
			if (flag4)
			{
				num -= 360f;
			}
			PawnSkyfaller skyfaller = SkyfallerMaker.SpawnSkyfaller(WP_DefOf.WP_PawnSkyfaller, cell, pawn.Map) as PawnSkyfaller;
			skyfaller.ticksToImpact = ticksToImpact;
			skyfaller.angle = num;
			pawn.DeSpawn(0);
			skyfaller.innerContainer.TryAdd(pawn, false);
		}

		internal static void RotatePawn(Pawn pawn, IntVec3 targCell, out float angle)
		{
			angle = Vector3Utility.AngleToFlat(pawn.Position.ToVector3(), targCell.ToVector3());
			float num = angle + 90f;
			bool flag = num > 360f;
			if (flag)
			{
				num -= 360f;
			}
			Rot4 rotation = Rot4.FromAngleFlat(num);
			pawn.Rotation = rotation;
		}
	}
}
