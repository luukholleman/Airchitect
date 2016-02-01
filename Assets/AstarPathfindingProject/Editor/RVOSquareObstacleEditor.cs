using Assets.AstarPathfindingProject.RVO;
using UnityEditor;

namespace Assets.AstarPathfindingProject.Editor
{
    [CustomEditor(typeof(RVOSquareObstacle))]
    public class RVOSquareObstacleEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI ()
        {
            DrawDefaultInspector ();
            /*EditorGUILayout.HelpBox ("Due to recent changes to the local avoidance system. RVO obstacles are currently not functional." +
			" They will be enabled in a comming update. In the meantime you can use agents with the 'locked' field set to true as simple obstacles.", MessageType.Warning );*/
        }
    }
}