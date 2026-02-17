/*
using UnityEngine;

public class GameCenter : MonoBehaviour {
	// Use this for initialization
	void Start () {
		if (GameCenterBinding.isGameCenterAvailable()
		    && !GameCenterBinding.isPlayerAuthenticated()) 
		{
			GameCenterBinding.authenticateLocalPlayer();	
		}
		else if (!GameCenterBinding.isGameCenterAvailable()) {
			Application.LoadLevel(1);	
		}
		DontDestroyOnLoad(gameObject);
	}
}
*/
