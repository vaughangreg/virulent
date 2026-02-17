using UnityEngine;
using System.Collections.Generic;

public class Cytoskeleton : MonoBehaviour {
	public GameObject dummyPrefab;
	public GameObject[] objectsToDisable;	
	public Color fadeColor;
	public float timeBetweenMeshDestructions = 0.5f;
	public int trisToDeletePerTry = 10;
	private MeshFilter m_mesh;
	//private int initialFace = 14;
	//private int finalFace = 65;
	private List<int> facesToDelete;
	//public 
	
	void DeathPrep() {
		
		
		//Create a dummy object
		if (dummyPrefab != null) {
			//Instantiate(dummyPrefab,transform.position,transform.rotation);
			
		}
		FadeToColor fade;
		
		//Disable the children objects with you!
		if (objectsToDisable.Length > 0) {
			foreach(GameObject g in objectsToDisable) {
				//Do specific actions to ribosomes and mitochondria
				if (g.CompareTag("Ribosome") || g.CompareTag("Mitochondria")) {
					//Setup fade properties
					fade = g.AddComponent<FadeToColor>();
					fade.endColor = fadeColor;
					fade.startColor = g.GetComponent<Renderer>().material.color;
					fade.rendererTarget = g.GetComponent<Renderer>();
					
					//Disable the proper components  
					if (g.CompareTag("Ribosome")) Destroy(g.GetComponent<ProducerOnCollision>());
					else { //Mitochondria
						Destroy(g.GetComponent<EnergyProvider>());
						Destroy(g.transform.Find("Energy Glow").gameObject);
					}
					
					g.tag = "Disabled";
				}
				else Destroy(g);
			}
		}
		
		/*
		//Apply the fade to us now
		fade = gameObject.AddComponent<FadeToColor>();
		fade.endColor = fadeColor;
		fade.startColor = gameObject.renderer.material.color;
		fade.renderer = gameObject.renderer;*/
		//animation.Play();
		GetComponent<MeshDecomposer>().enabled = true; 
		
		//Destroy the components
		//new MessageObjectDestroyed(gameObject, gameObject.name, gameObject);
		Destroy(GetComponent<CountedObject>());
		Destroy(GetComponent<Combat>());
		//gameObject.layer = LayerMask.NameToLayer("Enemies");
		
		AiHelper.RescanColliderAreaWithDelay(GetComponent<Collider>());
		//AiHelper.RescanColliderArea(collider);
		Destroy(GetComponent<Collider>());
		//Destroy(GetComponent<Selectable>());
		Destroy(this); 
		 
	}
	
	
}

