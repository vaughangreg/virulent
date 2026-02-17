using UnityEngine;

/// <summary>
/// Stores info and sends messages to ObjectManager for population counts
/// </summary>
public class CountedObject: MonoBehaviour {
	private GameObject m_creator;
	private string m_destroyer;
	
	/// <summary>
	/// On Start, send a message containing this object and it's creator object
	/// </summary>
	void Start() {
		if (!m_creator) m_creator = gameObject;
		new MessageObjectCreated(gameObject, m_creator, gameObject);
	}
	
	/// <summary>
	/// When this object is destroyed, send a message containing this and the object destroying this (when possible)
	/// </summary>
	void OnDestroy() {
		new MessageObjectDestroyed(gameObject, m_destroyer, gameObject);
	}
	
	/// <summary>
	/// Optionally, set creator equal to production GameObject
	/// </summary>
	/// <param name="creator">
	/// A <see cref="GameObject"/>
	/// </param>
	public void SetCreator(GameObject creator) {
		m_creator = creator;
	}
	
	/// <summary>
	/// Optionally, set creator equal to production GameObject
	/// </summary>
	/// <param name="destroyer">
	/// A <see cref="GameObject"/>
	/// </param>
	public void SetDestroyer(GameObject destroyer) {
		m_destroyer = destroyer.tag;
	}
	
	/// <summary>
	/// An single method alternative to SetDestroyer(this) and Destroy(target).
	/// </summary>
	/// <param name="destroyer">
	/// A <see cref="GameObject"/>
	/// </param>
	public void Destroy(GameObject destroyer) {
		SetDestroyer(destroyer);
		Destroy(gameObject);
	}
}
