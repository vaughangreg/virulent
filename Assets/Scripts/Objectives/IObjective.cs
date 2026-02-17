using UnityEngine;

/// <summary>
/// Watches for specified events and sends completion message when its conditions are met
/// </summary>
public interface IObjective {
	bool isComplete { get; }
	int  count      { get; }
	
	void OnObjectCreated(GameObject anObject);
	void OnObjectDestroyed(GameObject anObject);
	
	void AddChild(IObjective aChild);
}