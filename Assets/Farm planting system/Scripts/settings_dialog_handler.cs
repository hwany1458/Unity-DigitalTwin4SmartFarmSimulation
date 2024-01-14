using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles settings in levels
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 1.0     RASKALOF    13.11.2018    2018
/// -------------------------------------------
/// - Initial development

public class settings_dialog_handler : MonoBehaviour {
    [SerializeField] GameObject settings_dialog; // Assign here settings dialog

	public void SwitchSettingsDialog() {
        if(settings_dialog == null) return; // If GO not assigned - interrupt execution
        if(settings_dialog.activeInHierarchy) settings_dialog.SetActive(false); else settings_dialog.SetActive(true); // Switch active status
        plants_manager.Instance.PlayTypicalSound(); // play switch sound
    }
}
