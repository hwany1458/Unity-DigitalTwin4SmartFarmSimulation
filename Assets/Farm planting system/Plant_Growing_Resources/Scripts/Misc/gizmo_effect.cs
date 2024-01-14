using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Visual indication for gameobjects (plants).
/// Attach this script for any GO which should have gizmo indication
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 3.0     Deatrocker    12.12.2017  2017.2.0f3
/// -------------------------------------------
/// - Added gizmo types
/// -------------------------------------------
public class gizmo_effect : MonoBehaviour {
	[SerializeField] Color gizmo_color = new Color(255,0,0,255); // Color of gizmo
	[SerializeField] float gizmo_size = 0.5f; // Size of gizmo
	public enum GIZMO_TYPE { CUBE, SPHERE, WIRED_QUBE, WIRED_SPHERE }
	[SerializeField] GIZMO_TYPE gizmo_type;

	void OnDrawGizmos () {
		Gizmos.color = gizmo_color;
		switch (gizmo_type) {
		case(GIZMO_TYPE.CUBE):
			{
				Gizmos.DrawCube (transform.position, new Vector3(gizmo_size,gizmo_size,gizmo_size));
				break;
			}
		case(GIZMO_TYPE.SPHERE):
			{
				Gizmos.DrawSphere (transform.position, gizmo_size);
				break;
			}
		case(GIZMO_TYPE.WIRED_QUBE):
			{
				Gizmos.DrawWireCube (transform.position, new Vector3(gizmo_size,gizmo_size,gizmo_size));
				break;
			}
		case(GIZMO_TYPE.WIRED_SPHERE):
			{
				Gizmos.DrawWireSphere (transform.position, gizmo_size);
				break;
			}
		}

	}
}
