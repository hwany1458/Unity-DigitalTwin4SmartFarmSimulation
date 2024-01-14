using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Plants manager, contain plants database and cursor for this database
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 1.0     RASKALOF    13.11.2018    2018
/// -------------------------------------------
/// - Initial development
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 1.1     RASKALOF    02.01.2019    2018
/// -------------------------------------------
/// - Fixing code, adding comments, added sorting types

public class plants_manager : MonoBehaviour {
    
    [SerializeField] CONSTS_ENUMS.PLANT_SORT_TYPE UI_sorting_type = CONSTS_ENUMS.PLANT_SORT_TYPE.NONE; // Default is none sorting
    [SerializeField] CONSTS_ENUMS.PLANT_SORT_DIRECTION UI_sorting_direction = CONSTS_ENUMS.PLANT_SORT_DIRECTION.ASCENDING; // If sorting type != none than defines sorting direction (ascending, descending)
    [SerializeField] public List<plant_class> plants_base = new List<plant_class>(); // Plants database
    [SerializeField] int cursor; // Cursor for point in DB
    [SerializeField] float cell_selection_distance = 500f; // Distance where cells can be selected (usefull for FPS farming simulators)
    [SerializeField] KeyCode buy_mode_key = KeyCode.B; // HOT Key to show/hide cells
    [SerializeField] AudioClip typical_sound; // Sound when buy mode switching on and off etc
    [SerializeField] GameObject[] buy_mode_switch_go; // GOs for hiding/showing when buy mode on/off
    bool buy_mode_active; // Is buy mode active?
    public static plants_manager Instance;

    private void Start() {
        Instance = this; // Singleton realization
        SetBuyMode(false); // Disable buy mode by default
    }

    public GameObject GetPlant(int i) { // Returns plant GO by i
        return plants_base[i].GetPlantPrefab();
    }
	
    public int GetCursor() { // Get current cursor position in DB
        return cursor;
    }

    public void SetCursor(int value, bool need_update = true) { // Set cursor position
        cursor = value; // Set new cursor value
        if(need_update) UI_item_controller.Instance.UpdateUIItems(); // If controller founded, update UI info about plants (used for highlighting selected UI plant element)
    }

    public float GetSelectionDistance() { // Get selection distance to know how far away is player (camera)
        return cell_selection_distance;
    }

    private void Update() {
        if(Input.GetKeyDown(buy_mode_key)) { // If player hit show_key
            SwitchBuyMode(); // Switch buy mode
        }
    }

    public void SwitchBuyMode() { // Switch buy mode status method
        if(!resources_controller.Instance.GetGameStopStatus()) { // If not game over
            foreach(cell_controller cell in GameObject.FindObjectsOfType<cell_controller>()) {
                cell.SwitchActive(); // Send to each finded cell in game command to switch its active status
            }
            foreach(GameObject go in buy_mode_switch_go) {
                go.SetActive(!go.activeInHierarchy); // For each linked GO in buy_mode_switch_go array - revert current active status (true to false or vice versa)
                buy_mode_active = go.activeInHierarchy; // Set buy mode status equals to GOs status
            }
            PlayTypicalSound(); // If this GO have AS - play switch sound
        }
    }

    public void SetBuyMode(bool state) { // Like SwitchBuyMode but manually set target status 
        if(!resources_controller.Instance.GetGameStopStatus()) { // If not game over
            foreach(cell_controller cell in GameObject.FindObjectsOfType<cell_controller>()) {
                if(state) cell.SwitchActive(1); // Send to each finded cell in game command to switch on
                else cell.SwitchActive(0); // Send to each finded cell in game command to switch off
            }
            foreach(GameObject go in buy_mode_switch_go) {
                go.SetActive(state); // For each linked GO in buy_mode_switch_go array - set target state status
                buy_mode_active = state; // Set buy mode status equals to state status
            }
        }
    }

    public void SwitchInterfaceVisible(bool status) { // Used for main menu to just switch off interface then game is over, but can be used for another purposes
        foreach(GameObject go in buy_mode_switch_go) {
            go.SetActive(status);
            buy_mode_active = status;
        }
    }

    public bool GetBuyModeStatus() { // Returns actual buy mode status
        return buy_mode_active;
    }

    public void PlayTypicalSound() {
        GetComponent<AudioSource>()?.PlayOneShot(typical_sound); // If this GO have AS - play typical sound
    }

    public CONSTS_ENUMS.PLANT_SORT_TYPE GetSortType() {
        return UI_sorting_type; // Get sorting type for other scripts
    }

    public CONSTS_ENUMS.PLANT_SORT_DIRECTION GetSortDirection() {
        return UI_sorting_direction; // Get sorting direction for other scripts
    }
}
