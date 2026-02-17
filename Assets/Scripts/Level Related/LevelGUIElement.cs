using UnityEngine;
using System.Collections;

/// <summary>
/// Contains size, position, and style information of a LevelGUI object
/// </summary>
//[System.Serializable]
public class LevelGUIElement
{
	public Vector2 position;
	public Vector2 size;
	public string text;
	public float padding = 0.0f; //pixels
	
	/*
	public Texture2D activeTexture;
	public Texture2D inactiveTexture;
	*/
	
	protected GUIStyle style;
	
	public LevelGUIElement(string displayedText, Vector2 aPosition, Vector2 aSize)
	{
		text = displayedText;
		position = aPosition;
		size = aSize;
	}
	public LevelGUIElement(Vector2 aPosition, Vector2 aSize, GUIStyle aStyle)
	{
		position = aPosition;
		size = aSize;
		style = aStyle;
	}
	
	/// <summary>
	/// Returns the calculated rectangle of the LevelGUIElement
	/// </summary>
	/// <returns>
	/// The calculated rectangle
	/// </returns>
	public Rect GetGUIRect()
	{
		//Scale the elements in case teh resolution is different
		float x, y, w, h, scale;
		scale = GuiResources.GetScale();
		x = (position.x + padding) * scale;
		y = (position.y + padding) * scale;
		w = (size.x - padding*2) * scale;
		h = (size.y - padding*2) * scale;
		return new Rect(x,y,w,h);
	}
	
	public GUIStyle GetGUIStyle()
	{
		return style;
	}
	public void SetGUIStyle(GUIStyle aStyle)
	{
		style = aStyle;
	}
}

