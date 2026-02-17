using UnityEngine;

[RequireComponent(typeof(FactoryTile))]
public class CameraTiler : MonoBehaviour {
	public GameObject target;
	public GameObject backgroundPlane;
	public int        rngSeed = 0;
	
	public float      cameraGrowthRate = 0.5f;
	public float      cameraSizeMin    = 200.0f;
	public float      cameraSizeMax    = 380.0f;
	public float      cameraDeathZoom  = 50.0f;
	
	public FactoryTile factory;
	public Tile[]      tiles = new Tile[3];
	public int         lastTileSpawned = -1;
	
	public float        minSpawnRate    = 0.2f;
	public float        spawnGrowthRate = 0.01f;
	public GameObject[] enemies;
	public GameObject[] decorations;
	public float        driftLifetime   = 30.0f;
	
	protected Vector3 m_lastTargetPosition;
	
	void Awake() {
		// Only create a seed if we're using the default.
		if (rngSeed == 0) rngSeed = (int)(System.DateTime.Now.Ticks >> (sizeof(long) / 2));
		factory = GetComponent<FactoryTile>();
		factory.BuildTile(0, rngSeed);
	}
	
	/// <summary>
	/// Pans the camera to keep the virion in view.
	/// </summary>
	void LateUpdate() {
		if (target == null) {
			ZoomIn();
			return;
		}
		Vector3 targetPosition = target.transform.position;
		m_lastTargetPosition   = targetPosition;
		m_lastTargetPosition.y = transform.position.y;
		targetPosition.x       = Mathf.Max(0, targetPosition.x);
		targetPosition.y       = transform.position.y;
		targetPosition.z       = transform.position.z;
		
		
		GetComponent<Camera>().orthographicSize = Mathf.Clamp(cameraSizeMin + targetPosition.x * cameraGrowthRate, 
		                                      cameraSizeMin, cameraSizeMax);
		
		transform.position = targetPosition;
		backgroundPlane.transform.position = new Vector3(targetPosition.x,
		                                                 backgroundPlane.transform.position.y,
		                                                 backgroundPlane.transform.position.z);
		UpdateTiles();
	}
	
	public int tileIndexFor(float x) {
		return Mathf.FloorToInt(x / factory.tileWidth);
	}
	
	protected int m_lastThisTile = -1;
	void UpdateTiles() {
		int thisTile = tileIndexFor(transform.position.x);
		int prevTile = thisTile - 1;
		int nextTile = thisTile + 1;
		
		if (nextTile > lastTileSpawned) {
			SpawnAntibodies(nextTile);
			SpawnDecorations(nextTile);
			lastTileSpawned = Mathf.Max(lastTileSpawned, nextTile);
			new MessageFeedbackCountChanged(gameObject, lastTileSpawned);
		}
		
		if (m_lastThisTile == prevTile) {		// Moving right
			DestroyTile(tiles[0]);
			tiles[0] = tiles[1];
			tiles[1] = tiles[2];
			tiles[2] = factory.BuildTile(nextTile, rngSeed);
		}
		else if (m_lastThisTile == nextTile) {	// Moving left
			DestroyTile(tiles[2]);
			tiles[2] = tiles[1];
			tiles[1] = tiles[0];
			tiles[0] = factory.BuildTile(prevTile, rngSeed);
		}
		m_lastThisTile = thisTile;
	}
	
	void SpawnAntibodies(int atTileId) {
		SpawnObjects(atTileId, enemies, false);
	}
	
	void SpawnDecorations(int atTileId) {
		SpawnObjects(atTileId, decorations, true);
	}
	
	void SpawnObjects(int atTileId, GameObject[] fromArray, bool changeDrift) {
		float spawnChance = minSpawnRate + spawnGrowthRate * atTileId;
		Random.seed = rngSeed + atTileId;
		int maxSpawned = Mathf.Max(1, atTileId / 5);
		
		for (int i = 0; i < maxSpawned; ++i) {
			if (Random.value < spawnChance) {
				GameObject spawn = (GameObject)Instantiate(fromArray[Mathf.FloorToInt(Random.value * fromArray.Length)]);
					spawn.transform.position = new Vector3(factory.tileWidth * atTileId + Random.value * factory.tileWidth,
				    	                                   0.0f,
				        	                               ((Random.value < 0.5f) 
				                                       		 ? (factory.heightOffset + factory.tileWidth)
				                                       		 : (factory.heightOffset)));
				
				if (changeDrift) {
					Drift spawnDrift = spawn.GetComponent<Drift>();
					if (spawnDrift) spawnDrift.lifeTime = spawnDrift.maxLifeTime = driftLifetime;
				}
			}
		}	
	}
	
	void DestroyTile(Tile aTile) {
		if (aTile == null) return;
		for (int i = 0; i < aTile.staticObjects.Count; ++i) {
			Destroy(aTile.staticObjects[i]);
		}
	}
	
	/// <summary>
	/// Zooms into the last position of the target.
	/// </summary>
	void ZoomIn() {
		transform.position = Vector3.Lerp(transform.position, m_lastTargetPosition, Time.deltaTime);
		GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, cameraDeathZoom, Time.deltaTime);
	}
}
