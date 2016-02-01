using Assets.AstarPathfindingProject.Generators;
using Pathfinding.Serialization.JsonFx;

namespace Assets.AstarPathfindingProject.Core.Misc {
	[JsonOptIn]
	/** Defined here only so non-editor classes can use the #target field */
	public class GraphEditorBase {
		/** NavGraph this editor is exposing */
		public NavGraph target;
	}
}