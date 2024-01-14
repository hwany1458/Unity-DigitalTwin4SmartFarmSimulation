using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This script used for tapping on plant to harvest it instead of tapping dirt (but you still can tap dirt to harvest)
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 3.0     Deatrocker    19.01.2019  2017.2.0f3
/// -------------------------------------------
/// - Initial development
/// -------------------------------------------

public class plant_trigger : MonoBehaviour {
    bool i_highlighted;
    good_dirt_controller g_d_c;

    private void Start() {
        g_d_c = transform?.parent?.parent?.GetComponent<good_dirt_controller>();
    }
    private void OnMouseEnter() {
        if(g_d_c != null) {
            if(!cam_controller.Instance.GetTouchActionLock()) {
                g_d_c.Highlight(true);
                i_highlighted = true;
            }
        }
    }

    private void OnMouseUp() {
        if(g_d_c != null) {
            if(!cam_controller.Instance.GetTouchActionLock()) {
                if(i_highlighted) g_d_c.GetReward();
            }
        }
    }

    private void OnMouseExit() {
        if(g_d_c != null) {
            if(!cam_controller.Instance.GetTouchActionLock()) {
                g_d_c.Highlight(false);
                i_highlighted = false;
            }
        }
    }
}
