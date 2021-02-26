using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WendigoProject
{
	public class IngestionOutcomeDoer_ConvertPawnToBerserker : IngestionOutcomeDoer
	{
		protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
		{
			if (CompWendigoProgram.pawnsWithComp.TryGetValue(pawn, out var comp))
            {
				comp.isOnRageBaker = true;
            }
		}
	}
}
