using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.People;
using UnityEngine;

namespace Assets.Scripts.BuildableObjects
{
    class Queuable : MonoBehaviour
    {
        public Guest FrontInQueue;

        public Guest LastInQueue;

        private Queue<Guest> _queue = new Queue<Guest>();

        public void Enqueue(Guest person)
        {
            _queue.Enqueue(person);

            LastInQueue = person;

            if (FrontInQueue == null)
            {
                FrontInQueue = person;
            }
        }

        public void Dequeue()
        {
            Guest p = _queue.Dequeue();

            if (p == LastInQueue)
            {
                LastInQueue = null;
            }
            FrontInQueue = _queue.Peek();
            LastInQueue = _queue.LastOrDefault();
        }
        
    }
}
