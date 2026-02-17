using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class LoadText : MonoBehaviour {
	public TextAsset textToLoad;
	
	void Awake() {
		GetComponent<TextMesh>().text = textToLoad.text;
	}
}
