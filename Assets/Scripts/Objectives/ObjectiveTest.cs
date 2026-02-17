using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Is complete when ONE child is completed.
/// </summary>
public class ObjectiveTest : System.Object, IObjective {
	public enum Type {
		And,
		Or,
		Not,
		Equals,
		GreaterThan,
		LessThan
	}
	
	private List<IObjective> m_children = new List<IObjective>();
	
	private delegate bool ComparisonDelegate();
	private ComparisonDelegate CheckCompletion;
	
	public bool isComplete { 
		get { return CheckCompletion(); } 
	}
	
	public int  count {
		get {
			int numComplete = 0;
			foreach (IObjective aChild in m_children) {
				numComplete += (aChild.isComplete ? 1 : 0);
			}
			return numComplete;
		} 
	}
	
	public ObjectiveTest (Type aType) {
		switch (aType) {
			case Type.And:		CheckCompletion = And;				break;
			case Type.Or:		CheckCompletion = Or;				break;
			case Type.Not:		CheckCompletion = Not;				break;
			case Type.Equals:	CheckCompletion = Equals;			break;
			case Type.GreaterThan:	CheckCompletion = GreaterThan;	break;
			case Type.LessThan:		CheckCompletion = LessThan;		break;
		}
	}
	
	public void OnObjectCreated(GameObject anObject)   { 
		foreach (IObjective aChild in m_children) {
			aChild.OnObjectCreated(anObject);	
		}
	}
	public void OnObjectDestroyed(GameObject anObject) { 
		foreach (IObjective aChild in m_children) {
			aChild.OnObjectDestroyed(anObject);	
		}
	}
	
	public void AddChild(IObjective aChild) {
		// NOTE: Can add asserts here to check for NOT & number of children.
		m_children.Add(aChild);
	}
	
	private bool And() { 
		bool result = m_children[0].isComplete;
		foreach (IObjective aChild in m_children) {
			result &= aChild.isComplete;	
		}
		return result;
	}
	private bool Or()  { 
		bool result = m_children[0].isComplete;
		foreach (IObjective aChild in m_children) {
			result |= aChild.isComplete;	
		}
		return result;
	}
	
	private bool Not() {
		return !m_children[0].isComplete; 
	}
	
	private bool Equals() {
		int testValue = m_children[0].count;
		bool result = true;
		foreach (IObjective aChild in m_children) {
			result &= (testValue == aChild.count);
		}
		
		return result;
	}
	
	private bool GreaterThan() {
		return m_children[0].count > m_children[1].count;
	}
	
	private bool LessThan() {
		return m_children[0].count < m_children[1].count;
	}
}
