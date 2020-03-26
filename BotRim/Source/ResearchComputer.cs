using System;
using RimWorld;
using Verse;

namespace Botrim
{
    public class ResearchComputer : Building
    {
        public int ticksBetweenPoint = 1000;
        private int ticksLeft = 1000;
        private ResearchManager manager;
        private CompPowerTrader powerTrader;
        private float points;

        public override string GetInspectString()
        {
            if (powerTrader == null) powerTrader = GetComp<CompPowerTrader>();
            if (manager == null) manager = Find.ResearchManager;
            if (manager.currentProj == null)
            {
                return "No project selected!";
            }
            string inspect;
            float storage = this.GetStatValue(StatDef.Named("BOTR_Storage"), false);
            float remainingWork = manager.currentProj.baseCost - manager.currentProj.ProgressReal;
            if (storage <= remainingWork && powerTrader.PowerOn)
            {
                inspect = "Not enough storage for current project. Add drives or research some of it manually. /n";
                inspect = inspect + "Storage: " + storage.ToString();
            }
            else if (powerTrader.PowerOn)
            {
                inspect = "Storage: " + storage.ToString() + "\n";
                inspect = inspect + "Memory: " + this.GetStatValue(StatDef.Named("BOTR_Memory"), false) + "\n";
                inspect = inspect + "Research Progress (Pts/Required): " + Math.Round(points,2).ToString() + "/" + Math.Round(remainingWork,2).ToString();
            } else
            {
                inspect = "No power. Any research progress is now lost.";
            }
            return inspect;
        }

        public override void Tick()
        {
            
            ticksLeft--;
            if (ticksLeft <= 0)
            {
                if (manager == null) manager = Find.ResearchManager;

                float storage = this.GetStatValue(StatDef.Named("BOTR_Storage"), false);
                if (manager.currentProj != null)
                {
                    float remainingWork = manager.currentProj.baseCost - manager.currentProj.ProgressReal;
                    if (manager != null && storage >= remainingWork)
                    {

                        if (powerTrader == null) powerTrader = GetComp<CompPowerTrader>();
                        if (powerTrader.PowerOn)
                        {
                            if (points >= manager.currentProj.baseCost - manager.currentProj.ProgressReal)
                            {
                                manager.FinishProject(manager.currentProj, true);
                                points -= manager.currentProj.baseCost - manager.currentProj.ProgressReal;
                            }
                            else
                            {
                                ticksLeft = ticksBetweenPoint;
                                points += this.GetStatValue(StatDef.Named("BOTR_Memory"), false);
                            }
                        }
                        else
                        {
                            points = 0;
                        }
                    }
                }
            }
            base.Tick();
        }
    }
}