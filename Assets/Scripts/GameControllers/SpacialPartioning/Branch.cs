using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.BuildableObjects;
using UnityEngine;

namespace Assets.Scripts.GameControllers.SpacialPartioning
{
    class Branch : MonoBehaviour
    {
        public const int MaxObjectsInBranch = 5;

        private int _currentObjectCount = 0;

        public int Count
        {
            get
            {
                return _currentObjectCount;
            }
        }

        private BuildableObject[] _buildableObjects = new BuildableObject[MaxObjectsInBranch];

        private Branch[] _branches;

        public Bounds Bounds;

        public void AddObject(BuildableObject buildableObject)
        {
            if (_currentObjectCount < MaxObjectsInBranch && _branches == null)
            {
                _buildableObjects[_currentObjectCount] = buildableObject;

                buildableObject.transform.parent = transform;

                _currentObjectCount++;
            }
            else if (_branches != null)
            {
                DivideInSubranch(buildableObject);
            }
            else
            {
                CreateSubBranches();

                foreach (BuildableObject b in _buildableObjects)
                {
                    DivideInSubranch(b);
                }

                _buildableObjects = new BuildableObject[MaxObjectsInBranch];

                DivideInSubranch(buildableObject);
            }
        }

        public void RemoveObject(BuildableObject buildableObject)
        {
            if (_branches == null)
            {
                for (int i = 0; i < _currentObjectCount; i++)
                {
                    if (_buildableObjects[i] == buildableObject)
                    {
                        _buildableObjects[i] = null;
                        _buildableObjects[i] = _buildableObjects[_currentObjectCount - 1];

                        _currentObjectCount--;

                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    _branches[i].RemoveObject(buildableObject);
                }
            }
        }

        private void DivideInSubranch(BuildableObject buildableObject)
        {
            for (int i = 0; i < 4; i++)
            {
                if (_branches[i].Bounds.Contains(buildableObject.transform.position))
                {
                    _branches[i].AddObject(buildableObject);

                    break;
                }
            }
        }

        private void CreateSubBranches()
        {
            _branches = new Branch[4];

            int i = 0;

            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (Math.Abs(x) == 1 && Math.Abs(z) == 1)
                    {
                        GameObject branchGO = new GameObject("Branch " + x + " " + z);
                        Branch branch = branchGO.AddComponent<Branch>();
                        
                        branch.Bounds = new Bounds(new Vector3(Bounds.center.x + Bounds.extents.x / 2 * x, 0, Bounds.center.z - Bounds.extents.z / 2 * z), new Vector3(Bounds.extents.x, Bounds.size.y, Bounds.extents.z));
                        branch.transform.parent = transform;

                        _branches[i++] = branch;
                    }
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5F);
            Gizmos.DrawWireCube(Bounds.center, Bounds.size);
        }

        public bool Intersects(Bounds bounds)
        {
            if (_branches == null)
            {
                for (int i = 0; i < _currentObjectCount; i++)
                {
                    if (_buildableObjects[i].BoundingBox.Intersects(bounds))
                    {
                        // create a smaller bounds, no way to detect if objects are laying next to each other
                        Bounds smallerBounds = new Bounds(bounds.center, bounds.size * 0.99f);

                        if (_buildableObjects[i].BoundingBox.Intersects(smallerBounds))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (_branches[i].Intersects(bounds))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IntersectRay(Ray ray)
        {
            return GetObject(ray) != null;
        }

        public BuildableObject GetObject(Ray ray)
        {
            if (_branches == null)
            {
                for (int i = 0; i < _currentObjectCount; i++)
                {
                    if (_buildableObjects[i].BoundingBox.IntersectRay(ray))
                    {
                        return _buildableObjects[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    BuildableObject buildableObject = _branches[i].GetObject(ray);

                    if (buildableObject != null)
                    {
                        return buildableObject;
                    }
                }
            }

            return null;
        }
    }
}
