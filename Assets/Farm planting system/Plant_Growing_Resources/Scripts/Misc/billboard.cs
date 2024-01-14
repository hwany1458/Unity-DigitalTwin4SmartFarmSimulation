using UnityEngine;
/// <summary>
/// Simple billboard script which rotates object to facing camera
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 3.0     Deatrocker    12.12.2017  2017.2.0f3
/// -------------------------------------------
/// - Initial development
/// -------------------------------------------
public class billboard : MonoBehaviour {
	Transform m_camera;

	void Start(){
		m_camera = GameObject.FindWithTag ("MainCamera").transform;
	}
	void Update () {
        if(m_camera != null)
		transform.LookAt(transform.position + m_camera.rotation * Vector3.forward,	m_camera.rotation * Vector3.up);
	}
}
