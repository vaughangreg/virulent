using UnityEngine;
using UnityEditor;
using System.Collections;

public static class EditorHelper
{
	/// <summary>
	/// Displays the given PopulationGoal in the inspector
	/// </summary>
	/// <param name="p">
	/// A <see cref="PopulationGoal"/>
	/// </param>
	public static PopulationGoal DisplayPopulationGoal(PopulationGoal p, int indentLevel)
	{
		EditorGUI.indentLevel = indentLevel;
		p.showInInspector = EditorGUILayout.Foldout(p.showInInspector, p.watchedPopulation);
		if (p.showInInspector) {
			EditorGUI.indentLevel += 1;
			int index = 0;
			if (Constants.TAGS_LIST.Contains(p.watchedPopulation)) {
				index = Constants.TAGS_LIST.IndexOf(p.watchedPopulation);
			}
			index = EditorGUILayout.Popup("Watched Population",index, Constants.TAGS_LIST.ToArray());
			p.watchedPopulation = Constants.TAGS_LIST[index];
			p.populationGoal = EditorGUILayout.IntField("Population Value", p.populationGoal);
			p.PopulationOperator = (PopulationGoal.Type)EditorGUILayout.EnumPopup("Trigger Type",p.PopulationOperator);
		}
		return p;
	}
	public static PopulationGoal DisplayPopulationGoal(PopulationGoal p, int indentLevel, string label)
	{
		EditorGUI.indentLevel = indentLevel;
		p.showInInspector = EditorGUILayout.Foldout(p.showInInspector, label);
		if (p.showInInspector) {
			EditorGUI.indentLevel += 1;
			int index = 0;
			if (Constants.TAGS_LIST.Contains(p.watchedPopulation)) {
				index = Constants.TAGS_LIST.IndexOf(p.watchedPopulation);
			}
			index = EditorGUILayout.Popup("Watched Population",index, Constants.TAGS_LIST.ToArray());
			p.watchedPopulation = Constants.TAGS_LIST[index];
			p.populationGoal = EditorGUILayout.IntField("Population Value", p.populationGoal);
			p.PopulationOperator = (PopulationGoal.Type)EditorGUILayout.EnumPopup("Trigger Type",p.PopulationOperator);
		}
		return p;
	}
	
	public static ObjectManager.Unit DisplayUnitEnum(ObjectManager.Unit unit)
	{
		unit = (ObjectManager.Unit)EditorGUILayout.EnumPopup("Unit Type", unit);
		return unit;
	}
}

