using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.People.Goals
{
    class CheckIn : Goal
    {
        public override void Activate()
        {
            AddSubGoal(new GoToSecurity());
            AddSubGoal(new Queue());
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
