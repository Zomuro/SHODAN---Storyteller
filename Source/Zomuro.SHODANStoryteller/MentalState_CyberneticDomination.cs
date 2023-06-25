using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace Zomuro.SHODANStoryteller
{
    public class MentalState_CyberneticDomination : MentalState
    {
        public override string InspectLine
        {
            get
            {
                return def.baseInspectLine.Translate();
            }
        }

        public override void PostStart(string reason)
        {
            // sends notification about the colonist being converted to SHODAN's team
            if (PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                string title = def.beginLetterLabel.Translate(pawn.LabelShortCap);
                string desc = def.beginLetter.Translate(pawn.NameShortColored);
                if (!reason.NullOrEmpty())
                {
                    desc = desc + "\n\n" + reason;
                }
                Find.LetterStack.ReceiveLetter(title, desc, def.beginLetterDef ?? LetterDefOf.ThreatSmall, pawn, null, null, null, null);
            }

            // save original faction & set faction to SHODAN's
            orgFaction = pawn.Faction;
            pawn.SetFactionDirect(Find.FactionManager.FirstFactionOfDef(FactionDefOf.Zomuro_SHODAN_Faction));
            base.PostStart(reason);
        }

        public override void PostEnd()
        {
            base.PostEnd();

            // return pawn to player faction and end state.
            pawn.SetFactionDirect(orgFaction);
            if (pawn.jobs != null) pawn.jobs.StopAll(false, true);

            if (PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                Messages.Message("Zomuro_SHODAN_CyberneticDomination_Message".Translate(pawn.LabelShort), pawn, MessageTypeDefOf.SituationResolved, true);
            }
        }

        public override bool ForceHostileTo(Thing t)
        {
            if (t as Pawn != null && t.Faction != null && t.Faction != pawn.Faction) return true;
            return false;
        }

        public override bool ForceHostileTo(Faction f)
        {
            if (f != null && f != pawn.Faction) return true;
            return false;
        }

        public override RandomSocialMode SocialModeMax()
        {
            return RandomSocialMode.Off;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref orgFaction, "orgFaction");
        }

        public Faction orgFaction;
    }
}
