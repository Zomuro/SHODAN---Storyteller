using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;

namespace Zomuro.SHODANStoryteller
{
    public class MentalState_CyberneticDomination : MentalState
    {
        public override void PostStart(string reason)
        {
            orgFaction = pawn.Faction;
            // alter to SHODAN's faction here

            base.PostStart(reason);
            if (PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                string title = "Zomuro_SHODAN_CyberneticDomination_Letter".Translate(pawn.LabelShortCap);
                string desc = "Zomuro_SHODAN_CyberneticDomination_LetterDesc".Translate(pawn.Label);
                if (!reason.NullOrEmpty())
                {
                    desc = desc + "\n\n" + reason;
                }
                Find.LetterStack.ReceiveLetter(title, desc, LetterDefOf.ThreatSmall, pawn, null, null, null, null);
            }
        }

        public override void PostEnd()
        {
            pawn.SetFactionDirect(orgFaction);

            base.PostEnd();
            if (pawn.jobs != null) pawn.jobs.StopAll(false, true);
            if (PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                Messages.Message("Zomuro_SHODAN_CyberneticDomination_Message".Translate(pawn.LabelShort), pawn, MessageTypeDefOf.SituationResolved, true);
            }
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
