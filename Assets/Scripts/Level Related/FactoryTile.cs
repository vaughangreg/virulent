using UnityEngine;

public class FactoryTile : MonoBehaviour {
	public GameObject[] boundaries;
	public GameObject[] backgrounds;
	public float        heightOffset = -300.0f;
	public float        tileWidth = 500.0f;
	public float        boundaryPresentPercentage = 0.33f;
	public float        boundaryBuffer = 50;
	
	protected const int TILE_DIVISIONS = 2;
	
	public Tile BuildTile(int givenId, int withSeed) {
		Debug.Log("Building " + givenId + " with seed " + withSeed);
		Random.seed = withSeed + givenId;
		
		float originX    = tileWidth * givenId;
		Tile newTile     = new Tile(givenId);
		float widthStep  = tileWidth * (1.0f / (float)TILE_DIVISIONS);
		float heightStep = tileWidth * (1.0f / (float)TILE_DIVISIONS);
		if (givenId != 0) {
			bool[,] backgroundHits = new bool[TILE_DIVISIONS, TILE_DIVISIONS];
			for (int row = 0; row < backgroundHits.GetLength(0); ++row) {
				int numFound = 0;
				for (int col = 0; col < backgroundHits.GetLength(1); ++col) {
					backgroundHits[row, col] = Random.value < boundaryPresentPercentage;
					numFound += backgroundHits[row, col] ? 1 : 0;
				}
				if (numFound == 3) {
					backgroundHits[row, Mathf.FloorToInt(Random.value * backgroundHits.GetLength(1))] = false;
					Debug.Log("Too many objects.");
				}
				else if (numFound == 0) {
					backgroundHits[row, Mathf.FloorToInt(Random.value * backgroundHits.GetLength(1))] = true;
					Debug.Log("Not enough objects.");
				}
			}
			
			int hitRow = 0;
			int hitCol = 0;
			float[] colOffset = new float[3];
			for (int i = 0; i < colOffset.Length; ++i) {
				colOffset[i] = Random.value * boundaryBuffer;	
			}
			for (float row = heightOffset; row < tileWidth + heightOffset; row += heightStep) {
				hitCol = 0;
				for (float col = originX; col < originX + tileWidth; col += widthStep) {
					if (backgroundHits[hitRow, hitCol]) {
						GameObject aBoundary = (GameObject)Instantiate(boundaries[Mathf.FloorToInt(Random.value * boundaries.Length)]);
						aBoundary.transform.rotation = Quaternion.Euler(new Vector3(0.0f, Random.Range(225, 315), 0.0f));
						aBoundary.transform.position = new Vector3(col + Random.Range(boundaryBuffer, Mathf.FloorToInt(widthStep) - boundaryBuffer),
						                                           0.0f,
						                                           row + Random.Range(boundaryBuffer, Mathf.FloorToInt(widthStep) - boundaryBuffer) + colOffset[hitCol]);
						aBoundary.transform.localScale = Vector3.ClampMagnitude(aBoundary.transform.localScale, widthStep);
						newTile.staticObjects.Add(aBoundary);
						// Debug.Log("     @" + row + ", " + col, aBoundary);
						++hitCol;
					}
				}
				++hitRow;
			}
		}
		else {
			GameObject leftBorder = (GameObject)Instantiate(boundaries[0]);
			leftBorder.transform.position = new Vector3(-238.4325f, 0.0f, -14.51302f);
			leftBorder.transform.rotation = Quaternion.Euler(0.0f, 340.3008f, 0.0f);
			newTile.staticObjects.Add(leftBorder);
		}
		
		/*
		// Add the background
		GameObject theBackground = (GameObject)Instantiate(backgrounds[Mathf.FloorToInt(Random.value * backgrounds.Length)]);
		theBackground.transform.position = new Vector3(givenId * tileWidth,
		                                               -20.0f,
		                                               heightOffset + 0.5f * tileWidth);
		newTile.staticObjects.Add(theBackground);
		*/
		
		return newTile;
	}
}
