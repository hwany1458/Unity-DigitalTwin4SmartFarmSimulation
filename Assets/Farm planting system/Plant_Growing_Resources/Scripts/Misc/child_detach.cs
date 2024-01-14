using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Unparenting all childs of this Transform (for example plant should drop its fruits)
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 3.0     Deatrocker    12.12.2017  2017.2.0f3
/// -------------------------------------------
/// - Initial development
/// -------------------------------------------
public class child_detach : MonoBehaviour {
	void Start () {
		foreach (Transform child in transform) {
			child.parent = null;
		}
	}
}
