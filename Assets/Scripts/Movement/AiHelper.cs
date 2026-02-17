using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Utility methods for AI scripts.
/// </summary>
public class AiHelper : MonoBehaviour {
	protected static AiHelper m_singleton;
	
	private static bool rescanMap = false;
	private static Bounds m_boundsToScan;
	private static bool delayMeOneLoop = false;
	
	/// <summary>
	/// Assigns the singleton.
	/// </summary>
	void Awake() {
		m_singleton = this;
	}
	
	public static AiHelper instance {
		get { return m_singleton; }
	}
	
	public static void RescanWithDelay() {
		rescanMap = true; 
	} 
	
	void LateUpdate() {
		if (delayMeOneLoop) {
			delayMeOneLoop = false;
			return;
		}
		if(rescanMap) {
			Rescan();
			rescanMap = false;
		}
		if (m_boundsToScan != new Bounds()) {
			AstarPath.active.SetNodes (false,m_boundsToScan,true);
			m_boundsToScan = new Bounds();
		}
	}
	
	public static void RescanColliderArea(Collider c) {
		m_boundsToScan = c.bounds; 
	}
	
	public static void RescanColliderAreaWithDelay(Collider c) {
		m_boundsToScan = c.bounds;
		delayMeOneLoop = true;
	} 
	
	public static void RescanRectArea(Vector3 position, float radius) {
		Bounds t_bounds = new Bounds(position, new Vector3(radius, 1f, radius));
		AstarPath.active.SetNodes(false, t_bounds, true);
	}
	
	public static void PopGrid(int targetGrid) {
		List<AstarClasses.Grid> temp = new List<AstarClasses.Grid>(AstarPath.active.grids);
		temp.RemoveAt(targetGrid);
		AstarPath.active.grids = temp.ToArray();
	}
	
	public static void MoveGrid(Vector3 nextOffset, int[] incomingDimensions) {
		AstarPath.active.grids[0].offset = nextOffset;
		if (incomingDimensions.Length == 4) {
			AstarPath.active.grids[0].width = incomingDimensions[0];
			AstarPath.active.grids[0].depth = incomingDimensions[1];
			AstarPath.active.grids[0].nodeSize = incomingDimensions[2];
			AstarPath.active.grids[0].height = incomingDimensions[3];
		}
		rescanMap = true;
		//Rescan();
	}
	
	/// <summary>
	/// Recomputes movable spaces in the world.
	/// </summary>
	public static void Rescan() {
		m_singleton.GetComponent<AstarPath>().Scan();
	}
	
	/// <summary>
	/// Calculates and returns a Rect of the exact dimensions of the AStar grid object in scene units.
	/// </summary>
	/// <returns>
	/// A <see cref="Rect"/>
	/// </returns>
	public Rect CalcGridDimensions() {
		AstarPath apath = GetComponent<AstarPath>();
		
		float left		= transform.position.x + apath.grids[0].offset.x;
		float top		= transform.position.z + apath.grids[0].offset.z;
		float width		= apath.grids[0].width * apath.grids[0].nodeSize;
		float height	= apath.grids[0].depth * apath.grids[0].nodeSize;
		
		return new Rect(left, top, width, height);
	}
}
