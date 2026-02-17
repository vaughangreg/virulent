using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshDecomposer : MonoBehaviour {
	
	public Mesh[] meshObjects;
	public float timeBetweenMeshes = 0.5f;
	public GameObject particleEffect;
	public GameObject myReplacement;
	private GameObject m_particles;
	private int counter = 0;
	private MeshFilter m_filter;
	
	void Start() {
		m_filter = GetComponent<MeshFilter>();
		m_particles = (GameObject)Instantiate(particleEffect, transform.position, transform.rotation);
		m_particles.transform.parent = transform;
		NextMesh();
	}
	
	public void NextMesh() {
		m_filter.mesh = null;
		m_filter.mesh = meshObjects[counter];
		counter++;
		Invoke("NextMesh", timeBetweenMeshes);
		if (counter == meshObjects.Length) {
			Destroy(this);
			Destroy(m_particles);
			GameObject mr = (GameObject)Instantiate(myReplacement, transform.position, transform.rotation);
			mr.transform.localScale = transform.localScale;
			Destroy(gameObject);
			return;
		}
	}
	
	/*
	private Mesh m_mesh;
	private List<Vector3> m_originalVertices;
	//private 
	private List<int> m_originalTris;
	private List<int> m_newTris;
	private List<int> m_facesToKill;
	private int numOfAttacks;
	private float timeBetweenAttacks;
	private int m_counter;
	private float range = 0.5f;
	
	void Start() {
		SetVars();
	}
	
	void SetVars() {
		if (m_mesh) return;
		m_mesh = GetComponent<MeshFilter>().mesh;
		m_originalTris = new List<int>(m_mesh.triangles);
		m_originalVertices = new List<Vector3>(m_mesh.vertices);
	}
	
	public void StartKillingMesh(List<int> incFaces, int numOfOccurences, float timeBetweenOccurences) {
		if (!m_mesh) SetVars();
		m_facesToKill = incFaces;
		numOfAttacks = numOfOccurences;
		timeBetweenAttacks = timeBetweenOccurences;
		m_counter = 0;
		KillSomeMesh();
	}
	
	void KillSomeMesh() {
		if (m_counter >= numOfAttacks) {
			Destroy(this);
			return;
		}
		m_originalTris.RemoveRange(3 * m_facesToKill[0], 3 * m_facesToKill.Count);
		m_mesh.triangles = m_originalTris.ToArray();
		for (int i = 0; i < (int)(m_facesToKill.Count / numOfAttacks); i++) {
			
		}
		
	}
	
	List<int> FacesWithinRange(float trange) {
		float m_mag = (trange * Vector3.one).sqrMagnitude;
		List<int> vertsToTarget;
		foreach(Vector3 i in m_originalVertices) {
			if (i.sqrMagnitude < m_mag) {
				
			}
		}
		return new List<int>();
	}
	*/
	
}
