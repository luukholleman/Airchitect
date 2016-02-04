using Assets.AstarPathfindingProject.Core.AI;
using UnityEngine;

namespace Assets.Scripts.People.Goals
{
    class GoToSecurity : Goal
    {
        private Transform _security;

        public override void Activate()
        {
            _security = GameObject.Find("Security").transform;

            Person.GetComponent<AIPath>().target = _security;
        }

        public override STATUS Process()
        {
            if (Vector3.Distance(_security.transform.position, Person.transform.position) < 2.5f)
            {
                return SetStatus(STATUS.Completed);
            }

            return SetStatus(STATUS.Active);
        }

        public override void Terminate()
        {
            Object.Destroy(Person.gameObject);
        }
    }
}