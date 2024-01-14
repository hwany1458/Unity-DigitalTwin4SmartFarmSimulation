using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Handles win and lose dialogs functionality
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 1.0     RASKALOF    13.11.2018    2018
/// -------------------------------------------
/// - Initial development

public class win_lose_dialog_handlers : MonoBehaviour {
    [SerializeField] int main_menu_build_indx = 0;
    [SerializeField] int next_level_build_indx;
    [SerializeField] bool is_last_level = false; // Is this is last level? If yes - than we cant unlock next level and after click button we will go to main menu
    public static win_lose_dialog_handlers Instance;

    public void Start() {
        Instance = this; // Singleton realization
    }

    public void NextLevel() {
        if(!is_last_level) { // If not last level
            SceneManager.LoadScene(next_level_build_indx); // Load next level
        } else {
            ExitToMenu(); // If last level - go to main menu
        }
    }

    public int GetNextLevelIndex() { // Returns next level index
        return next_level_build_indx;
    }

    public bool IsLastLevel() { // If its last level returns true else returns false
        if(is_last_level) return true;
        return false;
    }

    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Restart level
    }

    public void ExitToMenu() {
        SceneManager.LoadScene(main_menu_build_indx); // Exit to main menu
    }
}
