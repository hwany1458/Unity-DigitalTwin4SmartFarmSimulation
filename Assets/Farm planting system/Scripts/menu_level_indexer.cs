using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This script should be attached to every load level's level selection button. It checks if this level available and change its interaction status and color
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 1.0     RASKALOF    28.12.2018    2018
/// -------------------------------------------
/// - Initial development

public class menu_level_indexer : MonoBehaviour {
    [SerializeField] byte my_id; // id from BuildSettings of this level to load and check its unlock status
    [SerializeField] Color awailable_color; // text color for awailable level text
    [SerializeField] Color notawailable_color; // text color for not awailable level text
    [SerializeField] string level_name; // level name to display

    void Start () {
        if(PlayerPrefs.GetInt(CONSTS_ENUMS.SAVE_MAP_ID_ + my_id) == 1) { // If this level id saved status == 1
            gameObject.GetComponent<Button>().interactable = true; // Button is Interactable
            transform.GetChild(0).gameObject.SetActive(true); // Activates image
            transform.GetChild(1).GetComponent<Text>().color = awailable_color; // Change color 
            transform.GetChild(1).GetComponent<Text>().text = level_name; // Display level name
        } else { // If this level id != 1
            gameObject.GetComponent<Button>().interactable = false; // Its not interactable
            transform.GetChild(0).gameObject.SetActive(false); // No image
            transform.GetChild(1).GetComponent<Text>().color = notawailable_color; // Disabled color
            transform.GetChild(1).GetComponent<Text>().text = "LOCKED"; // Instead of level name - LOCKED
        }
    }

}
