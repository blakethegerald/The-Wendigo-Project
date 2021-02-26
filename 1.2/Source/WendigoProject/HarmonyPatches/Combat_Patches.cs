using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WendigoProject
{
    [HarmonyPatch(typeof(AttackTargetFinder), "BestAttackTarget")]
    public class BestAttackTarget_Patch
    {
        [HarmonyPriority(0)]
        public static void Postfix(ref IAttackTarget __result, IAttackTargetSearcher searcher, TargetScanFlags flags, Predicate<Thing> validator = null, float minDist = 0f, float maxDist = 9999f, IntVec3 locus = default(IntVec3), float maxTravelRadiusFromLocus = 3.40282347E+38f, bool canBash = false, bool canTakeTargetsCloserThanEffectiveMinRange = true)
        {
            if (__result is Pawn pawn && CompWendigoProgram.pawnsWithComp.TryGetValue(pawn, out var comp) && comp.IsInvisible)
            {
                __result = null;
            }
        }
    }

    [HarmonyPatch(typeof(Verb), "CanHitTargetFrom")]
    public class CanHitTargetFrom_Patch
    {
        [HarmonyPriority(800)]
        public static void Postfix(ref Verb __instance, ref bool __result, IntVec3 root, LocalTargetInfo targ)
        {
            if (__result && targ.HasThing && targ.Thing is Pawn pawn && CompWendigoProgram.pawnsWithComp.TryGetValue(pawn, out var comp) && comp.IsInvisible)
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(AttackTargetFinder), "CanSee")]
    public class CanSee_Patch
    {
        [HarmonyPriority(800)]
        public static void Postfix(ref bool __result, Thing seer, Thing target, Func<IntVec3, bool> validator = null)
        {
            if (target is Pawn pawn && CompWendigoProgram.pawnsWithComp.TryGetValue(pawn, out var comp) && comp.IsInvisible)
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(CastPositionFinder), "TryFindCastPosition")]
    public class TryFindCastPosition_Patch
    {
        [HarmonyPriority(800)]
        public static bool Prefix(ref bool __result, CastPositionRequest newReq, ref IntVec3 dest)
        {
            if (newReq.target is Pawn pawn && CompWendigoProgram.pawnsWithComp.TryGetValue(pawn, out var comp) && comp.IsInvisible)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}