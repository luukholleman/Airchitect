using System.Collections;
using System.Linq;
using Assets.AstarPathfindingProject.Core.AI;
using Assets.Scripts.BuildableObjects;
using UnityEngine;

namespace Assets.Scripts.People.Goals
{
    class Queue : Goal
    {
        private Queuable _queue;

        public override void Activate()
        {
            Queuable[] queues = Object.FindObjectsOfType<Queuable>().OrderBy(x => Random.value).ToArray();

            foreach (Queuable queue in queues)
            {
                _queue = queue;
//
//                Guest lastInQueue = _queue.LastInQueue;

//                if (lastInQueue != null)
//                {
//                    Person.GetComponent<AIPath>().target = _queue.LastInQueue.transform;
//                }
//                else
//                {
                    Person.GetComponent<AIPath>().target = _queue.transform.FindChild("Target").transform;
//                }

//                _queue.Enqueue(Person);

                break;
            }
        }

        public override STATUS Process()
        {
			
//            if (_queue.FrontInQueue == Person)
//			{
				if (Vector3.Distance(_queue.transform.position, Person.transform.position) < 2f)
				{
//					_queue.Dequeue ();

					return SetStatus(STATUS.Completed);
				}
//
//                Person.GetComponent<AIPath>().target = _queue.transform;
//
//                return SetStatus(STATUS.Active);
//            }
//            
            return SetStatus(STATUS.Active);
        }

        public override void Terminate()
        {
        }
    }
}
