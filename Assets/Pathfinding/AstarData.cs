using UnityEngine;
using System.Collections;
using AstarClasses;

[System.Serializable]
public class AstarData : ScriptableObject {
	public int x = 0;
	
	[SerializeField]
	public Int3 test;
	
	[SerializeField]
	public SerializedNode[] staticNodes;
	public AstarClasses.Grid grid;
	
}