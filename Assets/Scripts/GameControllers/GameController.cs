using Assets.AstarPathfindingProject.Core.AI;
using UnityEngine;

namespace Assets.Scripts.GameControllers
{
    class GameController : MonoBehaviour
    {
        public static GameController Instance;

        void Awake()
        {
            Instance = this;
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.T))
            {
                GameObject person = Instantiate(Resources.Load<GameObject>("Prefabs/People/Person"), new Vector3(5, 0, 0), Quaternion.identity) as GameObject;

                person.GetComponent<AIPath>().target = GameObject.Find("Target").transform;
            }
        }
    }
}
