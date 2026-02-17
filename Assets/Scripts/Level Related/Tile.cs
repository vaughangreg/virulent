using UnityEngine;
using System.Collections.Generic;

public class Tile : System.Object {
	public List<GameObject> staticObjects = new List<GameObject>();
	public int              id;
	
	public Tile(int anId) {
		id = anId;
	}
}
