using UnityEngine;

public class KeptWithinCamera : MonoBehaviour {
	public Camera		targetCamera = null;
	public float		minRadius = 0.0f;
	public Transform	watchedTransform;
	public float		yScale = 0.934f;
	
	protected Rect		m_bounds;
	protected Vector3	m_originalOffset;
	
	void OnEnable() {
		if (!targetCamera) targetCamera = Camera.main;
		if (!watchedTransform) {
			watchedTransform = transform.parent ? transform.parent : transform;
			//Debug.Log("Autoselected transform: " + watchedTransform, gameObject);
		}
		
		if (minRadius == 0.0f) {
			minRadius = transform.lossyScale.magnitude;	
		}
		UpdateOffset();
	}
	
	public void UpdateOffset() {
		Rect cameraRect = targetCamera.rect;
		Vector3 bottomLeft = targetCamera.ViewportToWorldPoint(new Vector3(cameraRect.xMin, cameraRect.yMin, 0.0f));
		Vector3 upperRight = targetCamera.ViewportToWorldPoint(new Vector3(cameraRect.xMax, yScale/*cameraRect.yMax * yScale*/, 0.0f));
		
		m_bounds = new Rect(bottomLeft.x + minRadius,
		                    bottomLeft.z + minRadius,
		                    upperRight.x - bottomLeft.x - 2.0f * minRadius,
		                    upperRight.z - bottomLeft.z - 2.0f * minRadius);
		m_originalOffset = transform.localPosition;
		
		bl = bottomLeft;
		ur = upperRight;
	}
	
	void LateUpdate() {
		Vector2 position = new Vector2(watchedTransform.position.x, 
		                               watchedTransform.position.z);
		
		if (m_bounds.Contains(position)) {
			//transform.localPosition = new Vector3(m_originalOffset.x,
			//                                      transform.localPosition.y,
			//                                      m_originalOffset.z);
			return;
		}
		
		Vector3 newPosition = new Vector3(Mathf.Min(Mathf.Max(m_bounds.xMin, 
		                                                      watchedTransform.position.x + m_originalOffset.x), 
		                                            m_bounds.xMax),
		                                  transform.position.y,
		                                  Mathf.Min(Mathf.Max(m_bounds.yMin,
		                                                      watchedTransform.position.z + m_originalOffset.z),
		                                            m_bounds.yMax));

		transform.position = newPosition;
	}
	
	Vector3 bl;
	Vector3 ur;
	void OnDrawGizmosSelected() {
		Gizmos.color = Color.green;
		Gizmos.DrawLine(bl, ur);	
	}
}
