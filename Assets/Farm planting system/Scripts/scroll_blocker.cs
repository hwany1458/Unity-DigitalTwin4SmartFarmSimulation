using UnityEngine.EventSystems;
using UnityEngine;
/// <summary>
/// When mouse over this UI element, block zoom in and zoom out feature for the camera. 
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 1.0     RASKALOF    23.11.2018    2018
/// -------------------------------------------
/// - Initial development

public class scroll_blocker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public void OnPointerEnter(PointerEventData eventData) {
        if(cam_controller.Instance?.GetInputType() == CONSTS_ENUMS.input_type.PC)
            cam_controller.Instance.SetPCScrollLocked(true); // If mouse over this element and input type - PC - scrolling (zooming) and interacting witch cells and dirt is blocked
    }

    public void OnPointerExit(PointerEventData eventData) {
        if(cam_controller.Instance?.GetInputType() == CONSTS_ENUMS.input_type.PC)
            cam_controller.Instance.SetPCScrollLocked(false); // Release all locks when mouse exit from this object
    }

}
