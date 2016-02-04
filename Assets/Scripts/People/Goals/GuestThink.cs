using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.People.Goals
{
    class GuestThink : Goal
    {
        public override void Activate()
        {
            AddSubGoal(new CheckIn());
        }

        public override STATUS Process()
        {
            return ProcessSubGoals();
        }

        public override void Terminate()
        {
        }
    }
}
