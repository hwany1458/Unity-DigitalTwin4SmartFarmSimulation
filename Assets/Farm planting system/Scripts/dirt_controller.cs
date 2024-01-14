using System.Linq;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This script controlls bad dirt cells, used for cultivate and plant. 
/// The idea of this, is when player gathered plant, dirt becomes bad (becomes this) so player needs to cultivate it before can plant again
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

public class dirt_controller : MonoBehaviour {
    [SerializeField] int cultivation_price; // Cultivation price
    [SerializeField] bool display_text; // Should we show displaytext
    [Space]
    [Header("MEDIA")]
    [SerializeField] string text_to_display = "PREPARE FOR: "; // Defines price notation
    [SerializeField] Color highlighted_color = Color.green; // GO selected color
    [SerializeField] Color normal_color = Color.green; // GO unselected color
    [SerializeField] AudioClip cultivation_snd; // Cultivation sound
    [SerializeField] GameObject item_prefab; // Cultivated dirt prefab (f.e.)
    [SerializeField] GameObject start_particles; // Start particles
    [SerializeField] Vector3 particle_start_position_offset; // Offset from center of this dirt
    // private vars
    Transform player; // Link to player position to calculate distance
    bool selected; // Is this cell selected?
    Text price_text; // Price text to display

    void Start() {
        if(start_particles != null) {
            GameObject ps = Instantiate(start_particles, transform.position, Quaternion.identity, null);
            ps.transform.position += particle_start_position_offset;
        }
        player = Camera.main.transform; // Get camera as player position to define distance to select cell
        price_text = transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
        if(price_text == null) Debug.LogError("Price text component of Cell not found!");
        Highlight(false); // Unhighlight cell
        if(display_text) ShowPrice(); // Fill price text (if needed)
    }

    public void Highlight(bool highlight) { // Used when player focuse mouse on this cell or take off focus from this cell
        if(highlight) { // If need to highlight
            selected = true; // This cell selected
            transform.gameObject.GetComponent<Renderer>().material.color = highlighted_color; // Switch material color of this cell
        } else {
            selected = false; // in other case this cell is unhighlighted and unselected
            transform.gameObject.GetComponent<Renderer>().material.color = normal_color; // Switch material color of this cell
        }
    }

    private void OnMouseEnter() { // If mouse over this cell
        if(Vector3.Distance(transform.position, player.position) <= plants_manager.Instance?.GetSelectionDistance()) {// If player (camera) distance is in awailable range 
            Highlight(true); // Set this cell active 
        }
    }

    private void OnMouseExit() { // if mouse out of this cell
        Highlight(false); // Set this cell innactive
    }

    void Update() {
        if(Input.GetMouseButtonUp(0)) { // If MB or Finger up
            if(selected && !resources_controller.Instance.GetGameStopStatus()) // if selected and game not over
            CultivateDirt(); // Try to make action with cell
        }
    }

    void CultivateDirt() {
        if(((cam_controller.Instance.GetInputType() == CONSTS_ENUMS.input_type.MOBILE) && !cam_controller.Instance.GetTouchMoveLock() && !cam_controller.Instance.GetTouchActionLock()) || (cam_controller.Instance.GetInputType() == CONSTS_ENUMS.input_type.PC && !cam_controller.Instance.GetPCScrollLocked())) { // if player can controll (mouse over)
            if(resources_controller.Instance.GetResourceQuantity() >= cultivation_price) { // if player have enough money
                resources_controller.Instance.SetResourcesQuantity(-cultivation_price); // Take player's money
                plants_manager.Instance.gameObject.GetComponent<AudioSource>()?.PlayOneShot(cultivation_snd); // Play Sound
                Instantiate(item_prefab, transform.position, transform.rotation); // Instantiate item prefab to this coordinates
                Destroy(gameObject); // Destroy this cell
            }
        }
    }

    void ShowPrice() {
        price_text.text = text_to_display + cultivation_price + " " + resources_controller.Instance.GetCurrency(); // Fills price text
    }
}