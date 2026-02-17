using UnityEngine;
using System.Collections.Generic;

public class ObjectManager : MonoBehaviour {
	/// <summary>
	/// Defines units for menus, production, etc.
	/// </summary>
	public enum Unit {
		Capsid = 0,
		MRnaG,
		MRnaM,
		MRnaN,
		MRnaLP,
		Antigenome,
		Genome,
		Ribosome,
		ProteinG,
		ProteinM,
		ProteinN,
		ProteinLP,
		PRR,
		Slicer,
		Proteasome,
		MRnaProteasome,
		MRnaPRR,
		MRnaSlicer,
		Caspace,
		MRnaCaspace,
		Vesicle,
		AICapsid,
		GenomeMax,
		Count
	};
	
	public GameObject[] unitPrefabs;
	protected static ObjectManager m_singleton = null;
	
	public static ObjectManager instance {
		get { return m_singleton; }
	}
	
	//dictionary of object tags(key) and how many of that object there are(value)
	private Dictionary<string, int> m_counts = new Dictionary<string, int>();
	
	/// <summary>
	/// Assigns the singleton & tests prefabs.
	/// </summary>
	void Awake() {
		if (m_singleton == null) m_singleton = this;
		if (unitPrefabs.Length != (int)Unit.Count) Debug.LogError("ObjectManager: Prefabs don't match unit count.", gameObject);
		
		Dispatcher.Listen(Dispatcher.OBJECT_CREATED, gameObject);
		Dispatcher.Listen(Dispatcher.OBJECT_DESTROYED, gameObject);
		Dispatcher.Listen(Dispatcher.TAG_TRANSITION, gameObject);
		
		//for each collection, and each tag in collections, add a key of that name with a value of zero
		foreach(KeyValuePair<string, List<string>> collection in Constants.TAG_COLLECTIONS) {
			m_counts.Add(collection.Key, 0);
			foreach (string tag in collection.Value) {
				if (!m_counts.ContainsKey(tag)) {
					m_counts.Add(tag, 0);
				}
			}
		}
		
		//InvokeRepeating("Report", 0.0f, 5.0f);
	}
	
	/*
	/// <summary>
	/// spits out a large string for population debugging
	/// </summary>
	void Report() {
		string printString = "";
		printString += "Populations: \n";
		foreach (KeyValuePair<string, int> count in m_counts) {
			printString += (count.Key + "- " + count.Value + ", ");
		}
		Debug.Log(printString);
	}*/
	
	#region Object creation and destruction
	
	/// <summary>
	/// Returns a new gameobject of the specified unit.
	/// </summary>
	/// <param name="aUnitId">
	/// A <see cref="Unit"/>
	/// </param>
	/// <returns>
	/// A <see cref="GameObject"/>
	/// </returns>
	public static GameObject Build(Unit aUnitId, Vector3 position, Quaternion rotation) {
		ObjectManager manager = instance;
		
		GameObject product = (GameObject)Instantiate(manager.unitPrefabs[(int)aUnitId], position, rotation);
		return product;
	}
	
	/// <summary>
	/// Increments tag population and tag collection if the created object is a counted object.
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessageObjectCreated"/>
	/// </param>
	void _OnObjectCreated(MessageObjectCreated m) {
		if (m_counts.ContainsKey(m.createdObject.tag)) {
			m_counts[m.createdObject.tag]++;
			
			//check to see if this tag is part of a tag collection - increments collection if it is
			foreach (KeyValuePair<string, List<string>> tagList in Constants.TAG_COLLECTIONS) {
				if (tagList.Value.Contains(m.createdObject.tag)) m_counts[tagList.Key]++;
			}
		} 
		else {
			m_counts.Add(m.createdObject.tag, 1);
			Debug.LogWarning("Tag not found, creating new population count: " + m.createdObject.tag);
		}
		//Let the world know that the population was changed
		new MessagePopulationChanged(gameObject);
	}
	
	/// <summary>
	/// Decrements tag population and tag collection if the destroyed object is a counted object.
	/// </summary>
	/// <param name="m">
	/// A <see cref="MessageDestroyObject"/>
	/// </param>
	void _OnObjectDestroyed(MessageObjectDestroyed m) {
		m_counts[m.destroyedObject.tag]--;
		//check to see if this tag is part of a tag collection - decrements collection if it is
		foreach (KeyValuePair<string, List<string>> collection in Constants.TAG_COLLECTIONS) {
			if (collection.Value.Contains(m.destroyedObject.tag)) m_counts[collection.Key]--;
		}
		//Let the world know that the population was changed
		//Debug.Log(m.destroyedObject.tag + "    taken from population of: " + m_counts[m.destroyedObject.tag]);
		new MessagePopulationChanged(gameObject);
	}
	
	void _OnTagTransition(MessageTagTransition m) {
		if (m_counts.ContainsKey(m.tag2)) m_counts[m.tag2]++;
		else m_counts.Add(m.tag2, 1);
		
		m_counts[m.tag1]--;
		//check to see if this tag is part of a tag collection - decrements collection if it is
		foreach (KeyValuePair<string, List<string>> collection in Constants.TAG_COLLECTIONS) {
			foreach (string tag in collection.Value) {
				if (tag.Equals(m.tag1)) {
					m_counts[collection.Key]--;
				}
				if (tag.Equals(m.tag2)) {
					m_counts[collection.Key]++;
				}
			}
		}
	}
	
	
	
	#endregion
	
	/// <summary>
	/// Gets the count of a particular tag or tag collection
	/// </summary>
	/// <param name="Either tag or tag collection">
	/// A <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Int32"/>
	/// </returns>
	public int GetCount(string population) {
		if (!m_counts.ContainsKey(population)) {m_counts.Add(population, 0); print(population + " needed an add");}
		return m_counts[population];
	}
	
	/// <summary>
	/// Returns true if the specified tag is in the specified collection
	/// </summary>
	/// <param name="tag">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="collection">
	/// A <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	public bool isTagInCollection(string tag, string collection) {
		foreach (string aTag in Constants.TAG_COLLECTIONS[collection]) {
			if(aTag.Equals(tag)) return true;	
		}
		return false;
	}
}
