using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This script scales UI element (or any transform) to be the same size on screen depends on camera distance and limits
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 1.0     RASKALOF    13.11.2018    2018
/// -------------------------------------------
/// - Initial development

public class UI_scaler : MonoBehaviour {
    [SerializeField] float scale_current; // Current scale
    [SerializeField] float scale_max; // Maximum scale
    [SerializeField] float scale_min; // Minimum scale
    Vector3 startd; // Start scale (local, do not modify)

    private void Start() {
        startd = transform.localScale; // Get start scale from start
    }
    // Update is called once per frame
    void Update () {
        if(Camera.main != null) {
            var size = (Camera.main.transform.position - transform.position).magnitude; // Gets distance to camera
            transform.localScale = new Vector3(Mathf.Clamp(startd.x * size, scale_min * startd.x, scale_max * startd.x), Mathf.Clamp(startd.y * size, scale_min * startd.y, scale_max * startd.y), Mathf.Clamp(startd.z * size, scale_min * startd.z, scale_max * startd.z)) * scale_current; // Change size of this UI based on distance to camera and limits
        }
    }
}
