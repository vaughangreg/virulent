using UnityEngine;

/// <summary>
/// Records the population count for ONE tag.
/// </summary>
public class ObjectivePopulationCount : System.Object, IObjective {
	private string m_watchedTag;
	private int    m_currentValue = 0;
	
	/// <summary>
	/// Sets the tag to watch.
	/// </summary>
	/// <param name="aTag">
	/// A <see cref="System.String"/>
	/// </param>
	public ObjectivePopulationCount(string aTag) {
		m_watchedTag  = aTag;
	}
	
	/// <summary>
	/// Always true.
	/// </summary>
	public bool isComplete { get { return true; } }
	
	/// <summary>
	/// How many of the population exists.
	/// </summary>
	public int  count      { get { return m_currentValue; } }
	
	/// <summary>
	/// Increments the current value.
	/// </summary>
	/// <param name="anObject">
	/// A <see cref="GameObject"/>
	/// </param>
	public void OnObjectCreated(GameObject anObject) {
		if (anObject.CompareTag(m_watchedTag)) m_currentValue++;
	}
	
	/// <summary>
	/// Decrements the current value.
	/// </summary>
	/// <param name="anObject">
	/// A <see cref="GameObject"/>
	/// </param>
	public void OnObjectDestroyed(GameObject anObject) {
		if (anObject.CompareTag(m_watchedTag)) m_currentValue--;
	}
	
	public void AddChild(IObjective aChild) {
		Debug.LogError("Attempted to add child to population count of " + m_watchedTag);
	}
}
