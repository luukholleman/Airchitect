using System.Collections;
using Assets.AstarPathfindingProject.Core.AI;
using Assets.Scripts.People;
using UnityEngine;

namespace Assets.Scripts.GameControllers
{
    class GameController : MonoBehaviour
    {
        public static GameController Instance;

        public float SpawnRate;

        void Awake()
        {
            Instance = this;

            StartCoroutine(SpawnGuests());
        }

        private IEnumerator SpawnGuests()
        {
            for (;;)
            {
                GameObject person = Instantiate(Resources.Load<GameObject>("Prefabs/People/Person"), new Vector3(Random.value * 20 + 100, 0, 2), Quaternion.identity) as GameObject;

                person.AddComponent<Guest>();

                yield return new WaitForSeconds(SpawnRate);
            }
        }
    }
}
