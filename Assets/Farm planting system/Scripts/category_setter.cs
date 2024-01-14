using System;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This script controlls selected items category and sends call to update items list when category changed
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 1.0     RASKALOF    09.11.2018    2018
/// -------------------------------------------
/// - Initial development
/// -------------------------------------------
/// 1.1     RASKALOF    22.12.2018    2018
/// -------------------------------------------
/// - Changed logic
/// - Added start_active_category var and functional

public class category_setter : MonoBehaviour {
    [Space]
    [Header("COMMON SETTINGS")]
    [SerializeField] int start_active_category = 1; // Defines which category of item should be activated from level start
    [SerializeField] GameObject[] buttons; // All buttons for choosing cathegory
    [SerializeField] Text cat_label; // Label which should display current cat name
    [Space]
    [Header("MEDIA")]
    [SerializeField] Color selected_color = Color.yellow; // Button color
    [SerializeField] Color normal_color = Color.white; // Button color

    void Start () {
        ActivateCategory(start_active_category); // Activate start category
    }

    public void ActivateCategory(int iter = 0) { // When player select any category (by buttons)
        for(int i = 0; i < buttons.Length; i++) { // For each button
            if(i == iter) buttons[i].GetComponent<Image>().color = selected_color; // If selected caegory == current button from loop - button active and selected
            else buttons[i].GetComponent<Image>().color = normal_color; // Else, button is not selected
        }
        if(cat_label != null) cat_label.text = Enum.GetName(typeof(CONSTS_ENUMS.category), iter);
        UI_item_controller.Instance.SetCurrentCategory((CONSTS_ENUMS.category)iter); // After changing category, send info to UI controller
    }

    private void Update() { // Hotkeys for selecting categorys
        if(cam_controller.Instance?.GetInputType() == CONSTS_ENUMS.input_type.PC) {
            if(Input.GetKeyDown(KeyCode.Alpha1)) { // HOTKEY '1'
                ActivateCategory(0);
            }
            if(Input.GetKeyDown(KeyCode.Alpha2)) { // HOTKEY '2'
                ActivateCategory(1);
            }
            if(Input.GetKeyDown(KeyCode.Alpha3)) { // HOTKEY '3'
                ActivateCategory(2);
            }
            if(Input.GetKeyDown(KeyCode.Alpha4)) { // HOTKEY '4'
                ActivateCategory(3);
            }
            if(Input.GetKeyDown(KeyCode.Alpha5)) { // HOTKEY '5'
                ActivateCategory(4);
            }
        }
    }
}
