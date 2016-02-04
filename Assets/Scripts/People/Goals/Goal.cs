using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.People.Goals
{
    abstract class Goal
    {
        public Guest Person;

        public Stack<Goal> SubGoals = new Stack<Goal>();

        internal enum STATUS
        {
            Inactive,
            Active,
            Completed,
            Failed
        }

        public STATUS Status;

        public abstract void Activate();
        public abstract STATUS Process();
        public abstract void Terminate();
        public virtual bool HandleMessage()
        {
            return true;
        }

        public void AddSubGoal(Goal subGoal)
        {
            if (SubGoals.Any())
                SubGoals.Peek().SetStatus(STATUS.Inactive);

            SubGoals.Push(subGoal);

            subGoal.SetPerson(Person);
        }

        public virtual void SetPerson(Guest person)
        {
            Person = person;
        }

        public STATUS ProcessSubGoals()
        {
            while (SubGoals.Any() && (SubGoals.Peek().Status == STATUS.Completed || SubGoals.Peek().Status == STATUS.Failed))
            {
                Goal goal = SubGoals.Pop();

                goal.Terminate();
            }

            if (SubGoals.Any())
            {
                Goal subGoal = SubGoals.Peek();

                if (subGoal.Status == STATUS.Inactive)
                {
                    subGoal.Activate();
                    subGoal.Status = STATUS.Active;
                }

                STATUS status = subGoal.Process();

                if (status == STATUS.Completed && subGoal.SubGoals.Any())
                {
                    return STATUS.Active;
                }

                return status;
            }

            return STATUS.Completed;
        }

        public void RemoveAllSubGoals()
        {
            foreach (Goal subGoal in SubGoals)
            {
                subGoal.RemoveAllSubGoals();
                subGoal.Terminate();
            }

            SubGoals.Clear();
        }

        public STATUS SetStatus(STATUS status)
        {
            Status = status;
            return status;
        }

        public virtual bool IsSameGoal(Goal goal)
        {
            return false;
        }
    }
}
