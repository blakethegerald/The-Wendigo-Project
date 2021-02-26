using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace WendigoProject
{
	[HarmonyPatch(typeof(Pawn_HealthTracker), "MakeDowned")]
	public static class MakeDowned_Patch
	{
		private static bool Prefix(Pawn ___pawn, DamageInfo? dinfo, Hediff hediff)
		{
			if (___pawn.MentalStateDef == WP_DefOf.WB_Berserk)
            {
				return false;
            }
			return true;
		}
	}

	[HarmonyPatch(typeof(Pawn), "PostApplyDamage")]
	public static class PostApplyDamage_Patch
	{
		private static void Postfix(Pawn __instance, DamageInfo dinfo, float totalDamageDealt)
		{
			Log.Message("PostApplyDamage_Patch - Postfix - if (__instance.MentalStateDef != WP_DefOf.WB_BerserkDamagedSkinwalkerCore) - 1", true);
			if (__instance.MentalStateDef != WP_DefOf.WB_BerserkDamagedSkinwalkerCore)
			{
				Log.Message("PostApplyDamage_Patch - Postfix - if (dinfo.HitPart.def == BodyPartDefOf.Head && __instance.health.hediffSet.GetPartHealth(dinfo.HitPart) <= 10) - 2", true);
				if (dinfo.HitPart.def == BodyPartDefOf.Head && __instance.health.hediffSet.GetPartHealth(dinfo.HitPart) <= 10)
				{
					Log.Message("PostApplyDamage_Patch - Postfix - var hediff = __instance.health.hediffSet.GetFirstHediffOfDef(WP_DefOf.WP_SkinwalkerPersonaCore); - 3", true);
					var hediff = __instance.health.hediffSet.GetFirstHediffOfDef(WP_DefOf.WP_SkinwalkerPersonaCore);
					Log.Message("PostApplyDamage_Patch - Postfix - if (hediff != null) - 4", true);
					if (hediff != null)
					{
						Log.Message("PostApplyDamage_Patch - Postfix - __instance.mindState.mentalStateHandler.TryStartMentalState(WP_DefOf.WB_BerserkDamagedSkinwalkerCore); - 5", true);
						__instance.mindState.mentalStateHandler.TryStartMentalState(WP_DefOf.WB_BerserkDamagedSkinwalkerCore);
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(Pawn), "Kill")]
	public static class Kill_Patch
	{
		private static void Prefix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit = null)
		{
			if (CompWendigoProgram.pawnsWithComp.TryGetValue(__instance, out var comp))
            {
				var screecherHorn = __instance.health.hediffSet.hediffs.Where(x => x.def == WP_DefOf.WP_ScreecherHorn);
				if (screecherHorn.Count() == 1)
                {
					comp.ActivateScreecherHorn();
				}
			}
			if (dinfo.HasValue && dinfo.Value.Instigator is Pawn killer && CompWendigoProgram.pawnsWithComp.TryGetValue(killer, out var comp2) && comp2.target == __instance)
            {
				comp2.ActivateSkinWalkerCore();
            }
		}
	}

	[HarmonyPatch(typeof(Pawn), "IsColonist", MethodType.Getter)]
	public static class IsColonist_Patch
	{
		private static void Postfix(Pawn __instance, ref bool __result)
		{
			if (!__result && CompWendigoProgram.pawnsWithComp.TryGetValue(__instance, out var comp) && comp.skinWalkerCoreIsActivated)
			{
				__result = true;
			}
		}
	}

	[HarmonyPatch(typeof(Thing), "Faction", MethodType.Getter)]
	public static class Faction_Patch
	{
		public static bool SetToPlayer;
		private static void Postfix(Pawn __instance, ref Faction __result)
		{
			if (SetToPlayer)
            {
				Log.Message("Custom");
				__result = Faction.OfPlayer;
            }
		}
	}

	[HarmonyPatch(typeof(PawnComponentsUtility), "AddAndRemoveDynamicComponents")]
	public static class AddAndRemoveDynamicComponents_Patch
	{
		private static void Prefix(Pawn pawn, bool actAsIfSpawned = false)
		{
			if (CompWendigoProgram.pawnsWithComp.TryGetValue(pawn, out var comp) && comp.skinWalkerCoreIsActivated)
            {
				Faction_Patch.SetToPlayer = true;
			}
		}
		private static void Postfix(Pawn pawn, bool actAsIfSpawned = false)
		{
			Faction_Patch.SetToPlayer = false;
		}
	}
}
