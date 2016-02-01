using System.Collections.Generic;
using Assets.Scripts.GameControllers;
using UnityEngine;

namespace Assets.Scripts.BuildableObjects
{
    public class BuildableObject : MonoBehaviour
    {
        public Bounds BoundingBox;

        public float YDelta = 0;

        // Use this for initialization
        void Start () {
            QuadTreeController.Instance.AddObject(this);

            BoundingBox.center = transform.position;
        }

        void OnDestroy()
        {
            QuadTreeController.Instance.RemoveObject(this);
        }
	
        // Update is called once per frame
        void Update () {
	
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5F);
            Gizmos.DrawWireCube(BoundingBox.center, BoundingBox.size);
        }
    }
}
