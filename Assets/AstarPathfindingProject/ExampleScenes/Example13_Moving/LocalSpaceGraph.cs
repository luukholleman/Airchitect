using UnityEngine;

/** Helper for LocalSpaceRichAI */
namespace Assets.AstarPathfindingProject.ExampleScenes.Example13_Moving
{
    public class LocalSpaceGraph : MonoBehaviour {

        protected Matrix4x4 originalMatrix;
	
        void Start () {
            originalMatrix = transform.localToWorldMatrix;
        }

        public Matrix4x4 GetMatrix ( ) {
            return transform.worldToLocalMatrix * originalMatrix;
        }
    }
}
