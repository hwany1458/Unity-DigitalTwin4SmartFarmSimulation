using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// This script handle main menu functional
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 1.0     RASKALOF    13.11.2018    2018
/// -------------------------------------------
/// - Initial development

public class menu_controller : MonoBehaviour {
    [SerializeField] int first_scene_id = 1; // id from build settings for first scene (when new game button hitted)
    [SerializeField] GameObject dialog_menu; // Link to doalog menu
    [SerializeField] GameObject load_game_menu; // Link to load game menu
    [SerializeField] GameObject load_game_error; // Link to error message box if have no save file
    [SerializeField] Button[] menu_buttons_to_disable; // Menu buttons to disable when option selected

    private void Start() {
        dialog_menu.SetActive(false); // From start disable dialog box
    }

    public void NewGameClick() { // Used for new game click
        dialog_menu.SetActive(true); // enable dialog
        Animation anim = dialog_menu.GetComponent<Animation>(); // Get animation component
        anim.clip = anim.GetClip("dialog_box_show"); // Get animation by name
        anim.Play(); // play animation
        ChangeButtonsStatus(false); // disable all buttons
    }

    public void NewGameReaction(bool new_game) { // Reaction to new game selection variants (if yes - erase, or if cancel - cancel erasing and return to main menu)
        if(!new_game) { // if cancel
            Animation anim = dialog_menu.GetComponent<Animation>(); // Get animation component
            anim.clip = anim.GetClip("dialog_box_hide"); // Get clip by name
            anim.Play(); // Play
            ChangeButtonsStatus(true); // Change MM buttons to active
        } else { // If erase and start new game
            EraseSaveData(); // Erase all saves
            LoadLevel(first_scene_id); // Load first level
        }
    }

    public void ContinueGameClick() { // Continue game reaction button
        if(HasSaveFile()) { // If have save file
            load_game_menu.SetActive(true); // Activate Load menu GO
            Animation anim = load_game_menu.GetComponent<Animation>(); // Get animation component
            anim.clip = anim.GetClip("dialog_box_show");
            anim.Play(); // Show load level menu
            ChangeButtonsStatus(false); // Disable all buttons
        } else { // if no saves founded
            load_game_error.SetActive(true); // Show error dialog
            Animation anim = load_game_error.GetComponent<Animation>();
            anim.clip = anim.GetClip("dialog_box_show");
            anim.Play();
            ChangeButtonsStatus(false); // Disable all buttons
        }
    }

    public void ContinueGameCancel() { // If cancel in load game button dialog clicked
        Animation anim = load_game_menu.GetComponent<Animation>();
        anim.clip = anim.GetClip("dialog_box_hide");
        anim.Play(); // Hide load game dialog
        ChangeButtonsStatus(true); // Activate all buttons
    }

    public void NoSaveCancel() { // Reaction to cancel button on error loading dialog
        Animation anim = load_game_error.GetComponent<Animation>();
        anim.clip = anim.GetClip("dialog_box_hide");
        anim.Play(); // Hide error dialog
        ChangeButtonsStatus(true); // Activate all buttons
    }

    public void QuitGame() { // If quit game button pressed - quit game
        Application.Quit();
    }

    void EraseSaveData() { // Method erases all saves
        if(PlayerPrefs.GetInt(CONSTS_ENUMS.SAVE_HAS_SAVE) == 1) PlayerPrefs.DeleteAll(); // Erase all data if have some
        PlayerPrefs.SetInt(CONSTS_ENUMS.SAVE_HAS_SAVE, 1); // After erasing we need to set, what now we have new save
        PlayerPrefs.SetInt(CONSTS_ENUMS.SAVE_MAP_ID_ + first_scene_id, 1); // Unlock first level
    }

    bool HasSaveFile() { // Method for checking have player saves or not
        if(PlayerPrefs.GetInt(CONSTS_ENUMS.SAVE_HAS_SAVE) == 1) return true; // If have - return true
        return false; // else return false
    }

    void ChangeButtonsStatus(bool status) { // Method for activate or disactivate buttons in MM by received status value
        foreach(Button btn in menu_buttons_to_disable) {
            btn.interactable = status;
        }
    }

    public void LoadLevel(int id) { // Load level method
        gameObject.SetActive(false); // Hide menu
        SceneManager.LoadScene(id); // Load selected scene

    }
}
