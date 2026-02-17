using UnityEngine;
using System.Collections;

public class SpriteAnimate : MonoBehaviour {
	
	private bool iHaveInfo = false;
	//private Material mySpriteSheet;
	private float spriteSheetWidth;
	private float spriteSheetHeight;
	private int spriteCounterWidth = 0;
	private int spriteCounterHeight = 0;
	private float spriteTime = 0f;
	private float timePerSprite = 0f;
	//private float timeLength;
	private MonoBehaviour behaviorThatCalledMe;
	private float xOffsetIncrement;
	private float yOffsetIncrement;
	private Vector2 texOffset;
	
	void Update() {
		if(iHaveInfo) {
			iHaveInfo = AnimateSprites();
		} else if (spriteCounterHeight == spriteSheetHeight) {
			GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0,0);
			if (behaviorThatCalledMe != null) behaviorThatCalledMe.enabled = true;
			Destroy(this);
		}
	}
	
	/// <summary>
	/// This is where you pass the sprite sheet info to this script.  If you just pass it a sheet, 
	/// the number of sprites in each direction, and the animation speed it does what it's supposed to.
	/// If you add on the 'useInitial' it either uses the initial sprite or it skips it.
	/// If you give it a Monobehavior then it re-enables it after the animation is complete.
	/// </summary>
	public void PassSpriteInfo(Material someSpriteSheet, float widthInSprites, float heightInSprites, float timeForSpriteAnimation) {
		PassSpriteInfo(someSpriteSheet, widthInSprites, heightInSprites, timeForSpriteAnimation, false, null);
	}
	
	public void PassSpriteInfo(Material someSpriteSheet, float widthInSprites, float heightInSprites, float timeForSpriteAnimation, bool useInitial) {
		PassSpriteInfo(someSpriteSheet, widthInSprites, heightInSprites, timeForSpriteAnimation, useInitial, null);
	}
	
	public void PassSpriteInfo(Material someSpriteSheet, float widthInSprites, float heightInSprites, float timeForSpriteAnimation, bool useInitial, MonoBehaviour callingScriptToEnable) {
		iHaveInfo  = true;
		//mySpriteSheet = someSpriteSheet;
		spriteSheetWidth = widthInSprites;
		spriteSheetHeight = heightInSprites;
		//timeLength = timeForSpriteAnimation;
		if (useInitial) {
			timePerSprite = timeForSpriteAnimation / (float)(spriteSheetWidth * spriteSheetHeight); 
		} else {
			timePerSprite = timeForSpriteAnimation / (float)(spriteSheetWidth * spriteSheetHeight - 1); 
			spriteTime = timePerSprite + 10f;
		}  
		if (callingScriptToEnable) behaviorThatCalledMe = callingScriptToEnable;
		
		xOffsetIncrement = 1f / spriteSheetWidth;
		yOffsetIncrement = 1f / spriteSheetHeight;
	}
	
	/// <summary>
	/// Moves the offset to the appropriate sprite location.  Returns a bool saying if it is still animating.
	/// </summary>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	
	bool AnimateSprites() {
		spriteTime += Time.deltaTime;
		if (spriteTime > timePerSprite) { 
			spriteTime = 0f;
			spriteCounterWidth++; 
			if (spriteCounterWidth == spriteSheetWidth) {
				spriteCounterWidth = 0;
				spriteCounterHeight++;
				if (spriteCounterHeight == spriteSheetHeight) return false;
			}
			texOffset.x = spriteCounterWidth * xOffsetIncrement;
			texOffset.y = spriteCounterHeight * yOffsetIncrement;
			GetComponent<Renderer>().material.mainTextureOffset = texOffset;
		}
		return true;
		
	}
	
}
