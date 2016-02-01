using Assets.Scripts.BuildableObjects;
using Assets.Scripts.GameControllers.SpacialPartioning;
using UnityEngine;

namespace Assets.Scripts.GameControllers
{
    class QuadTreeController : MonoBehaviour
    {
        public static QuadTreeController Instance;

        private int _buildableObjectCount = 0;
        
        private BuildableObject[] BuildableObjects = new BuildableObject[1000];

        public Branch Branch;

        void Awake()
        {
            Instance = this;

            GameObject branchGO = new GameObject("Branch");

            Branch = branchGO.AddComponent<Branch>();

            Branch.transform.position = new Vector3(256, 0, 256);
            Branch.transform.parent = transform;

            Branch.Bounds = new Bounds(new Vector3(256, 0, 256), new Vector3(512, 50, 512));
        }

        public void AddObject(BuildableObject buildableObject)
        {
            Branch.AddObject(buildableObject);
        }

        public void RemoveObject(BuildableObject buildableObject)
        {
            Branch.RemoveObject(buildableObject);
        }

        public bool Intersects(Bounds bounds)
        {
            return Branch.Intersects(bounds);
        }

        public bool IntersectRay(Ray ray)
        {
            return Branch.IntersectRay(ray);
        }

        public BuildableObject GetObject(Ray ray)
        {
            return Branch.GetObject(ray);
        }
    }
}
