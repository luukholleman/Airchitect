using System;
using System.Collections.Generic;
using Assets.AstarPathfindingProject.Core;
using Assets.AstarPathfindingProject.Core.Misc;
using Assets.AstarPathfindingProject.Core.Nodes;
using Assets.AstarPathfindingProject.Utilities;
using UnityEngine;

namespace Assets.AstarPathfindingProject.Pathfinders
{
	/** Floods the area completely for easy computation of any path to a single point.
This path is a bit special, because it does not do anything useful by itself. What it does is that it calculates paths to all nodes it can reach, it floods the graph.
This data will remain stored in the path. Then you can call a FloodPathTracer path, that path will trace the path from it's starting point all the way to where this path started flooding and thus generating a path extremely quickly.\n
It is very useful in for example TD (Tower Defence) games where all your AIs will walk to the same point, but from different places, and you do not update the graph or change the target point very often,
what changes is their positions and new AIs spawn all the time (which makes it hard to use the MultiTargetPath).\n

With this path type, it can all be handled easily.
- At start, you simply start ONE FloodPath and save the reference (it will be needed later).
- Then when a unit is spawned or needs its path recalculated, start a FloodPathTracer path from it's position.
   It will then find the shortest path to the point specified when you called the FloodPath extremely quickly.
- If you update the graph (for example place a tower in a TD game) or need to change the target point, you simply call a new FloodPath (and store it's reference).

\version From 3.2 and up, path traversal data is now stored in the path class.
So you can now use other path types in parallel with this one.

Here follows some example code of the above list of steps:
\code
public static FloodPath fpath;

public void Start () {
	fpath = FloodPath.Construct (someTargetPosition, null);
	AstarPath.StartPath (fpath);
}
\endcode

When searching for a new path to \a someTargetPosition from let's say \a transform.position, you do
\code
FloodPathTracer fpathTrace = FloodPathTracer.Construct (transform.position,fpath,null);
seeker.StartPath (fpathTrace,OnPathComplete);
\endcode
Where OnPathComplete is your callback function.
\n
Another thing to note is that if you are using NNConstraints on the FloodPathTracer, they must always inherit from Pathfinding.PathIDConstraint.\n
The easiest is to just modify the instance of PathIDConstraint which is created as the default one.

\astarpro

\shadowimage{floodPathExample.png}

\ingroup paths

*/
	public class FloodPath : Path {

		public Vector3 originalStartPoint;
		public Vector3 startPoint;
		public GraphNode startNode;

		/** If false, will not save any information.
		 * Used by some internal parts of the system which doesn't need it.
		 */
		public bool saveParents = true;

		protected Dictionary<GraphNode,GraphNode> parents;

		public override bool FloodingPath {
			get {
				return true;
			}
		}

		public bool HasPathTo (GraphNode node) {
			return parents != null && parents.ContainsKey (node);
		}

		public GraphNode GetParent (GraphNode node) {
			return parents[node];
		}

		/** Default constructor.
		 * Do not use this. Instead use the static Construct method which can handle path pooling.
		 */
		public FloodPath () {}

		public static FloodPath Construct (Vector3 start, OnPathDelegate callback = null) {
			FloodPath p = PathPool<FloodPath>.GetPath ();
			p.Setup (start, callback);
			return p;
		}

		public static FloodPath Construct (GraphNode start, OnPathDelegate callback = null) {
			if ( start == null ) throw new ArgumentNullException ("start");

			FloodPath p = PathPool<FloodPath>.GetPath ();
			p.Setup (start, callback);
			return p;
		}

		protected void Setup (Vector3 start, OnPathDelegate callback) {
			this.callback = callback;
			originalStartPoint = start;
			startPoint = start;
			heuristic = Heuristic.None;
		}

		protected void Setup (GraphNode start, OnPathDelegate callback) {
			this.callback = callback;
			originalStartPoint = (Vector3)start.position;
			startNode = start;
			startPoint = (Vector3)start.position;
			heuristic = Heuristic.None;
		}

		public override void Reset () {
			base.Reset ();
			originalStartPoint = Vector3.zero;
			startPoint = Vector3.zero;
			startNode = null;
			/** \todo Avoid this allocation */
			parents = new Dictionary<GraphNode,GraphNode> ();
			saveParents = true;
		}

		protected override void Recycle () {
			PathPool<FloodPath>.Recycle (this);
		}

		public override void Prepare (){
			AstarProfiler.StartProfile ("Get Nearest");

			if ( startNode == null ) {
				//Initialize the NNConstraint
				nnConstraint.tags = enabledTags;
				NNInfo startNNInfo 	= AstarPath.active.GetNearest (originalStartPoint,nnConstraint);

				startPoint = startNNInfo.clampedPosition;
				startNode = startNNInfo.node;
			} else {
				startPoint = (Vector3)startNode.position;
			}

			AstarProfiler.EndProfile ();

#if ASTARDEBUG
			Debug.DrawLine ((Vector3)startNode.position,startPoint,Color.blue);
#endif

			if (startNode == null) {
				Error ();
				LogError ("Couldn't find a close node to the start point");
				return;
			}

			if (!startNode.Walkable) {
#if ASTARDEBUG
				Debug.DrawRay (startPoint,Vector3.up,Color.red);
				Debug.DrawLine (startPoint,(Vector3)startNode.position,Color.red);
#endif
				Error ();
				LogError ("The node closest to the start point is not walkable");
				return;
			}
		}

		public override void Initialize () {

			PathNode startRNode = pathHandler.GetPathNode (startNode);
			startRNode.node = startNode;
			startRNode.pathID = pathHandler.PathID;
			startRNode.parent = null;
			startRNode.cost = 0;
			startRNode.G = GetTraversalCost (startNode);
			startRNode.H = CalculateHScore (startNode);
			parents[startNode] = null;

			startNode.Open (this,startRNode,pathHandler);

			searchedNodes++;

			// Any nodes left to search?
			if (pathHandler.HeapEmpty ()) {
				CompleteState = PathCompleteState.Complete;
			}

			currentR = pathHandler.PopNode ();
		}

		/** Opens nodes until there are none left to search (or until the max time limit has been exceeded) */
		public override void CalculateStep (long targetTick) {

			int counter = 0;

			//Continue to search while there hasn't ocurred an error and the end hasn't been found
			while (CompleteState == PathCompleteState.NotCalculated) {

				searchedNodes++;

				AstarProfiler.StartFastProfile (4);
				//Debug.DrawRay ((Vector3)currentR.node.Position, Vector3.up*2,Color.red);

				//Loop through all walkable neighbours of the node and add them to the open list.
				currentR.node.Open (this,currentR,pathHandler);

				// Insert into internal search tree
				if ( saveParents ) parents[currentR.node] = currentR.parent.node;

				AstarProfiler.EndFastProfile (4);

				//any nodes left to search?
				if (pathHandler.HeapEmpty()) {
					CompleteState = PathCompleteState.Complete;
					break;
				}

				//Select the node with the lowest F score and remove it from the open list
				AstarProfiler.StartFastProfile (7);
				currentR = pathHandler.PopNode ();
				AstarProfiler.EndFastProfile (7);

				//Check for time every 500 nodes, roughly every 0.5 ms usually
				if (counter > 500) {

					//Have we exceded the maxFrameTime, if so we should wait one frame before continuing the search since we don't want the game to lag
					if (DateTime.UtcNow.Ticks >= targetTick) {
						//Return instead of yield'ing, a separate function handles the yield (CalculatePaths)
						return;
					}
					counter = 0;

					if (searchedNodes > 1000000) {
						throw new Exception ("Probable infinite loop. Over 1,000,000 nodes searched");
					}
				}

				counter++;
			}
		}
	}
}
