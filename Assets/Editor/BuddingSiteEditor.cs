using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BuddingSite))]
public class BuddingSiteEditor : Editor
{
	BuddingSite m_bs;
	
	/// <summary>
	/// Display base UI.
	/// </summary>
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();
	}
	
	/// <summary>
	/// Creates handles to show where the capsid will spwn and in what direction.
	/// </summary>
	public void OnSceneGUI() {
		m_bs = (BuddingSite)target;
		//reenable these if the position of the spawn point somehow gets moved
		//m_bs.spawnPoint.transform.position = Handles.PositionHandle(m_bs.spawnPoint.transform.position, Quaternion.identity);
		//m_bs.spawnPoint.transform.rotation = Handles.RotationHandle(m_bs.spawnPoint.transform.rotation, m_bs.spawnPoint.transform.position);
		Handles.color = Color.yellow;
		Handles.DrawWireDisc(m_bs.spawnPoint.transform.position, Vector3.up, 15);
		Handles.Label(m_bs.spawnPoint.transform.position, "SpawnPoint");
		Handles.ArrowCap(0, m_bs.spawnPoint.transform.position, m_bs.spawnPoint.transform.rotation, 25f);
	}
}
