using System.Collections.Generic;
using Assets.AstarPathfindingProject.Core;
using Assets.Scripts.BuildableObjects;
using Assets.Scripts.GameControllers;
using UnityEngine;

namespace Assets.Scripts.Cursors
{
    public class Builder : MonoBehaviour
    {
        public GameObject ObjectsHolder;

        public AstarPath Path;

        private List<GameObject> _buildableObjects;

        private BuildableObject _currentObject;

        private GameObject _currentObjectPreview;

        void Awake()
        {
            _buildableObjects = new List<GameObject>()
            {
                Resources.Load<GameObject>("Prefabs/Floors/Floor"),
                Resources.Load<GameObject>("Prefabs/Furniture/ComputerDesk"),
            };
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (_currentObject != null)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    int x = Mathf.RoundToInt(hit.point.x);
                    int z = Mathf.RoundToInt(hit.point.z);

                    _currentObjectPreview.transform.position = new Vector3(x, _currentObject.YDelta, z);

                    Bounds bounds = _currentObject.BoundingBox;

                    bounds.center = new Vector3(bounds.center.x + x, bounds.center.y, bounds.center.z + z);

                    Color color = new Color(0, 255, 0, 0);

                    if (QuadTreeController.Instance.Intersects(bounds))
                    {
                        color = new Color(255, 0, 0, 0);
                    }

                    Renderer[] renderers = _currentObjectPreview.GetComponentsInChildren<Renderer>();

                    foreach (Renderer renderer in renderers)
                    {
                        renderer.material.SetColor("_EmissionColor", color);
                    }
                }
            }

            if (Input.GetButton("Fire1"))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    int x = Mathf.RoundToInt(hit.point.x);
                    int z = Mathf.RoundToInt(hit.point.z);

                    Bounds bounds = _currentObject.BoundingBox;

                    bounds.center = new Vector3(bounds.center.x + x, bounds.center.y, bounds.center.z + z);

                    if (!QuadTreeController.Instance.Intersects(bounds))
                    {
                        GameObject instantiated = Instantiate(_currentObject.gameObject, new Vector3(x, _currentObject.YDelta, z), Quaternion.identity) as GameObject;

                        instantiated.transform.parent = ObjectsHolder.transform;

                        Path.Scan();
                        Path.UpdateGraphs(new Bounds(Vector3.zero, Vector3.one * 100));
                    }
                }
            }

            if (Input.GetButtonUp("Fire2"))
            {
                Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

                BuildableObject buildableObject = QuadTreeController.Instance.GetObject(ray);

                Destroy(buildableObject.gameObject);

                Path.Scan();
                Path.UpdateGraphs(new Bounds(Vector3.zero, Vector3.one * 100));
            }
        }

        void OnGUI()
        {
            for (int i = 0; i < _buildableObjects.Count; i++)
            {
                if (GUI.Button(new Rect(10, i * 25, 100, 20), _buildableObjects[i].name))
                {
                    if (_currentObjectPreview != null)
                    {
                        Destroy(_currentObjectPreview);
                    }

                    _currentObject = _buildableObjects[i].GetComponent<BuildableObject>();
                    _currentObjectPreview = Instantiate(_buildableObjects[i].gameObject, Vector3.zero, Quaternion.identity) as GameObject;
                }
            }
        }
    }
}
