using UnityEngine;

/// <summary>
/// Records the population count for ONE tag.
/// </summary>
public class ObjectiveLiteral : System.Object, IObjective {
	private int    m_currentValue;
	
	/// <summary>
	/// Sets a literal value.
	/// </summary>
	/// <param name="aValue">
	/// A <see cref="System.Int32"/>
	/// </param>
	public ObjectiveLiteral(int aValue) {
		m_currentValue = aValue;
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
	public void OnObjectCreated(GameObject anObject) { }
	
	/// <summary>
	/// Decrements the current value.
	/// </summary>
	/// <param name="anObject">
	/// A <see cref="GameObject"/>
	/// </param>
	public void OnObjectDestroyed(GameObject anObject) { }
	
	/// <summary>
	/// Raise an error.
	/// </summary>
	/// <param name="aChild">
	/// A <see cref="IObjective"/>
	/// </param>
	public void AddChild(IObjective aChild) {
		Debug.LogError("Attempted to add child to literal: " + m_currentValue);	
	}
}
