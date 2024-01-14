using System.Linq;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This script controlls simple cells, used for buying this place for planting
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
/// - Fixed code
/// - Added text display feature and custom text for displaying

public class cell_controller : MonoBehaviour {
    [SerializeField] int dirt_price; // Price to buy this cell
    [SerializeField] bool display_text; // Should we show displaytext
    [Space]
    [Header("MEDIA")]
    [SerializeField] string text_to_display = "BUY FOR: "; // Defines price notation
    [SerializeField] Color highlighted_color = Color.green; // GO selected color
    [SerializeField] Color normal_color = Color.green; // GO unselected color
    [SerializeField] AudioClip buy_dirt_snd; // Buy sound effect
    [SerializeField] GameObject item_prefab; // Prefab to instantiate after buying (bad or good cell)
    // private vars
    Transform player; // Link to player position to calculate distance
    bool selected; // Is this cell selected?
    bool active; // Is this cell active? (visible)
    Text price_text; // Price text to display
    GameObject my_effect; // Visual effect to display

    void Start () {
        player = Camera.main.transform; // Get camera as player position to define distance to select cell
        price_text = transform.GetChild(0)?.GetChild(0)?.gameObject?.GetComponent<Text>(); // Assign text to display price
        if(price_text == null) Debug.LogError("Price text component of Cell not found!");
        my_effect = transform.Find("my_effect")?.gameObject; // Assign particles system
        if(my_effect == null) Debug.LogError("Effect of Cell not found!");
        if(display_text) ShowPrice(); // Fill price text (if needed)
        SwitchActive(0); // Disable cell by start
        Highlight(false); // Unhighlight cell
    }
	
	void Update () {
        if(Input.GetMouseButtonUp(0)) { // If player hit LMB
            CheckAction(); // Try to make action with cell
        }
    }

    public void Highlight(bool highlight) { // Used when player focuse mouse on this cell or take off focus from this cell
        if(highlight) { // If need to highlight
            selected = true; // This cell selected
            transform.gameObject.GetComponent<Renderer>().material.color = highlighted_color; // Switch material color of this cell
        }
        else {
            selected = false; // in other case this cell is unhighlighted and unselected
            transform.gameObject.GetComponent<Renderer>().material.color = normal_color; // Switch material color of this cell
        }
    }

    private void OnMouseEnter() { // If mouse over this cell
        if(Vector3.Distance(transform.position, player.position) <= plants_manager.Instance.GetSelectionDistance()) { // If player (camera) distance is in awailable range 
            Highlight(true); // Set this cell active 
        }
    }

    private void OnMouseExit() { // if mouse out of this cell
        Highlight(false); // Set this cell innactive
    }

    public void SwitchActive(sbyte state = -1) { // -1 means switch, without meaning of current value
        if(state <= -1) {
            if(gameObject.GetComponent<Renderer>().enabled) SwitchActive(0); // If we need to disactivate this cell, recoursive call with argument 0
            else SwitchActive(1); // if we need to activate this cell, recoursive call with argument 1
        }
        if(state == 0) { // if state 0
            gameObject.GetComponent<Renderer>().enabled = false; // Renderer is innactive
            gameObject.GetComponent<Collider>().enabled = false; // Collider is innactive (to not be abble to manipulate hidden cell)
            my_effect?.SetActive(false); // Particle effect disabled
            price_text?.gameObject.SetActive(false); // Price text disabled
        }
        if(state >= 1) { // if state >= 1
            gameObject.GetComponent<Renderer>().enabled = true; // Render is active
            gameObject.GetComponent<Collider>().enabled = true; // Collider is active (to be abble to manipulate this cell)
            my_effect?.SetActive(true); // Particle effect enabled
            price_text?.gameObject.SetActive(true); // Price text enabled
        }
        active = gameObject.GetComponent<Renderer>().enabled; // Set active status from renderer status
    }

    void CheckAction() {
        if(selected & active && resources_controller.Instance.GetResourceQuantity() >= dirt_price && !resources_controller.Instance.GetGameStopStatus()) { // if cell active and selected and player have enough money and game not over (mouse over)
            PlantItem(); // Call plant method
        }
    }

    void PlantItem() {
        if(((cam_controller.Instance.GetInputType() == CONSTS_ENUMS.input_type.MOBILE) && !cam_controller.Instance.GetTouchMoveLock() && !cam_controller.Instance.GetTouchActionLock()) || (cam_controller.Instance.GetInputType() == CONSTS_ENUMS.input_type.PC && !cam_controller.Instance.GetPCScrollLocked())) { // if player can controll and its tap or click
            resources_controller.Instance.SetResourcesQuantity(-dirt_price); // Take player's money
            plants_manager.Instance.gameObject.GetComponent<AudioSource>()?.PlayOneShot(buy_dirt_snd); // Play Sound
            Instantiate(item_prefab, transform.position, transform.rotation); // Instantiate item prefab to this coordinates
            Destroy(gameObject); // Destroy this cell
        }
    }

    void ShowPrice() {
        price_text.text = text_to_display + dirt_price + " " + resources_controller.Instance.GetCurrency(); // Fills price text
    }

}