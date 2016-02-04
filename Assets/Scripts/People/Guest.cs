using System.Collections;
using Assets.Scripts.People.Goals;
using UnityEngine;
using Queue = Assets.Scripts.People.Goals.Queue;

namespace Assets.Scripts.People
{
    class Guest : MonoBehaviour
    {
        private Goal _goal;

        void Start()
        {
            _goal = new GuestThink();

            _goal.RemoveAllSubGoals();

            _goal.SetPerson(this);
            _goal.Activate();
        }

        void Update()
        {
            _goal.Process();
        }

        void TargetReached()
        {
            
        }
    }
}
