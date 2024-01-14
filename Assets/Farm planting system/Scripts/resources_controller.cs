using System.Collections;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Resource controller, handles all resources and its methods in game (in this case - money)
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 1.0     RASKALOF    13.11.2018    2018
/// -------------------------------------------
/// - Initial development

public class resources_controller : MonoBehaviour {
    enum RES_ADD_TYPE { INSTANT, TIMED }
    [Header("COMMON")]
    [SerializeField] int resource_current_amount = 0; // resource quantity (RQ)
    [SerializeField] string resources_name = "coin"; // resource name
    [SerializeField] Text resources_label; // Label to display resource quantity
    [SerializeField] int start_res_amount = 11; // Start amount of resource
    [Space]
    [Header("AUTO ADD RES SETTINGS")] // Works only if player cant loose
    [SerializeField] RES_ADD_TYPE res_add_type; // Type of money add in case of player cant loose
    [SerializeField] int money_add_limit = 100; // If can loose = false, player cant loose, so in case of no money, we need to give some money to player, this is the free money limit for both res_add_type types
    [SerializeField] [Tooltip("Only for res_add_type == TIMED")]int money_add_delay = 5; // Delay in seconds to give setted amount of free money (only for res_add_type == TIMED)
    [SerializeField] int money_amount = 5; // How much free money give to player each delay or instantly (depends on res_add_type value)
    [Space]
    [Header("WIN SETTINGS")]
    [SerializeField] bool can_win; // If selected, than every resource operation will check amout of resources to win depends on value below
    [SerializeField] int resources_to_win; // If can_win selected its defines how much resources player need to collect to win
    [SerializeField] GameObject win_dialog; // Win dialog GO
    [SerializeField] GameObject win_particles; // Win particles
    [SerializeField] AudioClip win_audio; // Audio to play if player win
    [Space]
    [Header("LOSE SETTINGS")]
    [SerializeField] bool can_lose; // If selected, than player can lose if resource amount will be less or equal to value below
    [SerializeField] int resources_to_lose; // resource less or equal that value - player lose
    [SerializeField] GameObject lose_dialog; // Lose dialog GO
    [SerializeField] AudioClip lose_audio; // Audio to play if player lose
    bool game_stop; // Needs for other scripts to stop interacting with the input if game stop
    bool money_goes; // bool for define is player getting money
    public static resources_controller Instance;

    private void Awake() {
        Instance = this; // Singleton realization
        if(resources_label == null) resources_label = GameObject.Find("resources_label")?.GetComponent<Text>(); // If resource_label not found, find it
    }

    private void Start() {
        SetResourcesQuantity(start_res_amount); // Set start money amount
    }

    public int GetResourceQuantity() { // Get current RQ
        return resource_current_amount;
    }

    public void SetResourcesQuantity(int amount = 0) { // Set RQ
        resource_current_amount += amount; // Add or Subtract received amount (to subtract just send here negative value f.e -5 or -13 etc)
        if(resource_current_amount < 0) {
            resource_current_amount = 0; // Exclude bad calculations if money less than 0
        }
        if(!game_stop) {
            UI_item_controller.Instance?.UpdateUIItems(); // When resource amount changed, we need to update UI items list
            UpdateResourcesUI(); // Update UI
        }
        if(can_lose && resource_current_amount <= resources_to_lose && !game_stop) LevelLose(); // If player can lose and money less or equal to money for lose - call lose method
        else if(can_win && resource_current_amount >= resources_to_win && !game_stop) { // or if player can win and current money is enough for win
            LevelCompleted(); // Call win method
        }
        if(!can_lose && resource_current_amount < money_add_limit) { // If player cant lose, and his money less than low money limit and we already not giving money to player
            if(res_add_type == RES_ADD_TYPE.TIMED && !money_goes) StartCoroutine("ADDFreeMoney"); // start adding money to player
            if(res_add_type == RES_ADD_TYPE.INSTANT) SetResourcesQuantity(money_amount); // Add money instantly
        }
    }

    public string GetCurrency() { // Get resource currency
        return resources_name;
    }

    public void UpdateResourcesUI() { // If label assigned, needed to update UI label with new values
        if(resources_label != null) {
            resources_label.text = "You have <b>" + GetResourceQuantity() + "</b> " + GetCurrency(); // Concatinate all resources quantitys
            if(can_win) resources_label.text += ". You need <b>" + resources_to_win + "</b> " + GetCurrency() + " to win";
        }
    }

    private void OnEnable() {
        UpdateResourcesUI(); // On enable we need to update UI if something changed
    }

    public void LevelCompleted() { // Win method
        game_stop = true; // game over flag is on
        GetComponent<plants_manager>().SwitchInterfaceVisible(false); // Disable ingame interface
        win_dialog?.SetActive(true); // If win dialog assigned - open it
        GameObject.Find("audio_player")?.GetComponent<AudioSource>().Stop(); // Stop all music if audio_player GO founded
        GetComponent<AudioSource>().PlayOneShot(win_audio); // Play win sound
        win_particles?.SetActive(true); // Activate win particles if assigned
        if(!win_lose_dialog_handlers.Instance.IsLastLevel()) PlayerPrefs.SetInt(CONSTS_ENUMS.SAVE_MAP_ID_ + win_lose_dialog_handlers.Instance.GetNextLevelIndex(), 1); // Unlock next level
    }

    public void LevelLose() { // Lose method
        game_stop = true; // game over flag is on
        GetComponent<plants_manager>().SwitchInterfaceVisible(false); // Disable ingame interface
        lose_dialog?.SetActive(true); // If lose dialog assigned - open it
        GameObject.Find("audio_player")?.GetComponent<AudioSource>().Stop(); // Stop all music if audio_player GO founded
        GetComponent<AudioSource>().PlayOneShot(lose_audio); // Play lose sound
    }

    public bool GetGameStopStatus() { // Returns game over status
        return game_stop;
    }

    public IEnumerator ADDFreeMoney() { // Method for giving free money if player cant loose
        money_goes = true; // set flag to do not call this again if we already giving money to players
        while(resource_current_amount < money_add_limit) { // While money less than low money level
            yield return new WaitForSeconds(money_add_delay); // Delay setted amount of time in seconds
            SetResourcesQuantity(money_amount); // Add setted amount of money
        }
        money_goes = false; // When all done - disable flag, player again can get money if he have little
        yield return null;
    }

}
