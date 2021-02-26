using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace WendigoProject
{
	[HarmonyPatch(typeof(PawnRenderer), "RenderPawnAt", new Type[]
	{
		typeof(Vector3)
	})]
	public static class PawnOnBedRenderer_Patch
	{
		public static bool Prefix(PawnRenderer __instance, Pawn ___pawn, Vector3 drawLoc)
		{
			if (CompWendigoProgram.pawnsWithComp.TryGetValue(___pawn, out var comp))
            {
				if (comp.invisibilityEnabled && comp.IsInvisible)
                {
					return false;
                }
            }
			return true;
		}
	}
}
