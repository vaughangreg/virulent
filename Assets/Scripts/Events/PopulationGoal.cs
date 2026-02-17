using UnityEngine;
using System.Collections;

[System.Serializable]
public class PopulationGoal
{
	public enum Type {
		Equals,
		GreaterThan,
		LessThan,
		GreaterThanOrEqualTo,
		LessThanOrEqualTo,
		NotEqualTo
	}
	public string watchedPopulation = "Population Goal";
	public int populationGoal;
	public PopulationGoal.Type PopulationOperator { 
		get { return m_type; } 
		set {
			m_type = value;
			//Change our operator method based on type
			operatorMethod = SetOperator(m_type);
		}
	}
	[SerializeField]private PopulationGoal.Type m_type = PopulationGoal.Type.Equals;
	[HideInInspector][System.NonSerialized]public bool showInInspector;
			
	//Operator Delegate
	private delegate bool Operator(int a, int b);
	[SerializeField]private Operator operatorMethod;
	
	public void Refresh() {
		operatorMethod = SetOperator(m_type);	
	}
	
	#region OperatorMethods (for Delegate)
	//Sets operator delegate for the checkpoint goal
	Operator SetOperator(PopulationGoal.Type type) {
		switch (type) {
			case PopulationGoal.Type.Equals:               return OpEquals;
			case PopulationGoal.Type.GreaterThan:          return OpGreater;
			case PopulationGoal.Type.LessThan:             return OpLess;
			case PopulationGoal.Type.GreaterThanOrEqualTo: return OpGreaterEquals;
			case PopulationGoal.Type.LessThanOrEqualTo:    return OpLessEquals;
			case PopulationGoal.Type.NotEqualTo:           return OpNotEqual;
			default:                                       return OpGreater;
		}
	}
	bool OpEquals(int a, int b) {
		return a == b;
	}
	bool OpGreater(int a, int b) {
		return a > b;
	}
	bool OpLess(int a, int b) {
		return a < b;
	}
	bool OpGreaterEquals(int a, int b) {
		return a >= b;
	}
	bool OpLessEquals(int a, int b) {
		return a <= b;
	}
	bool OpNotEqual(int a, int b) {
		return a != b;
	}
	#endregion
	
	/// <summary>
	/// Returns whether the goal has been met yet or not
	/// </summary>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	public bool MetGoal() {
		operatorMethod = SetOperator(m_type);
		return operatorMethod(ObjectManager.instance.GetCount(watchedPopulation), populationGoal);
	}
	
	public override string ToString() {
		return string.Format("Pop " + m_type + " using " + operatorMethod);
	}
}

