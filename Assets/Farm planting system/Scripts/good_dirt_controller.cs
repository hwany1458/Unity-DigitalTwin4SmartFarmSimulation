using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This is the main actual placeholder and controller for plants (plants with cycles type is not supported!!!)
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 1.0     RASKALOF    11.11.2018    2018
/// -------------------------------------------
/// - Initial development
/// -------------------------------------------
/// 1.1     RASKALOF    22.12.2018    2018
/// -------------------------------------------
/// - Fixed code

public class good_dirt_controller : MonoBehaviour {
    enum INFO_TYPE { PROGRESS_BAR, TEXT, ALL, NONE} // Growing info to display
    enum TIME_TYPE { LEFT, CURRENT } // If info time type selected (or ALL): left - how much time left, or current progress of growing
    enum INTERACTION_TYPE { KEY, MOUSE }; // Interaction type (if ifs FPS better to use keys than mouseclick, but i prefer mouseclick in this example)
    [Header("MAIN SETTINGS")]
    [SerializeField] INFO_TYPE info_type = INFO_TYPE.ALL;
    [SerializeField] TIME_TYPE time_type = TIME_TYPE.LEFT;
    [SerializeField] bool hide_text_when_done = true; // Hide text when plant ready to harvest
    [SerializeField] Vector3 text_offset = new Vector3(); // Text offset (default)
    [SerializeField] bool give_reward = true; // Should player get harvest reward?
    [Space]
    [Header("PC INPUT SETTINGS")]
    [SerializeField] INTERACTION_TYPE interaction_type = INTERACTION_TYPE.KEY;
    [SerializeField] KeyCode plant_key = KeyCode.E; // Key to plant
    [SerializeField] KeyCode harvest_key = KeyCode.B; // Key to harvest
    [SerializeField] KeyCode destroy_key = KeyCode.Delete; // Key to delete plant
    [Space]
    [Header("MEDIA")]
    [SerializeField] Color highlighted_color = Color.green; // GO selected color
    [SerializeField] Color normal_color = Color.green; // GO unselected color
    [SerializeField] AudioClip plant_snd; // Plant sound
    [SerializeField] AudioClip reward_sound; // Reward sound (if plant harvested)
    [SerializeField] AudioClip destroy_sound; // Destroy sound (if player delete growing plant)
    [SerializeField] GameObject plant_prefab; // Plant prefab of this cell
    [SerializeField] GameObject cell_prefab; // Prefab to instantiate after harvest (if you want player to cultivate this cell again, set here bad dirt prefab, in other case)
    [SerializeField] GameObject start_particles; // Start particles
    [SerializeField] Vector3 particle_start_position_offset; // Offset from center of this dirt
    bool selected; // Is this cell selected?
    bool planted; // Is this cell already have plant? (to do not plant again)
    bool completed; // Can player harvest?
    Slider progress_bar; // Progress bar component
    Text time_left_label; // Time text component
    int plant_kursor; // Stores link of this plant in plant database to get info about (price, reward etc)
    Transform player; // Link to player position to calculate distance
    int reharvest_count; // How much reharvest left

    private void Start() {
        if(start_particles != null) {
            GameObject ps = Instantiate(start_particles, transform.position, Quaternion.identity, null);
            ps.transform.position += particle_start_position_offset;
        }
        player = Camera.main.transform; // Get camera as player position to define distance to select cell
        time_left_label = transform.GetChild(0)?.GetChild(0)?.Find("time")?.GetComponent<Text>(); // Assign text to display time
        if(time_left_label == null) Debug.LogError("time_left_label text component of Good dirt not found!"); // If text not found - display a error message
        if(cam_controller.Instance.GetInputType() == CONSTS_ENUMS.input_type.PC) {
            if(interaction_type == INTERACTION_TYPE.KEY) // If input type key, we need to display which key should be pressed
                time_left_label.text = "PRESS '" + plant_key.ToString() + "' TO PLANT"; // Display text
            if(interaction_type == INTERACTION_TYPE.MOUSE)
                time_left_label.text = "CLICK TO PLANT"; // Display harvest text
        }
        time_left_label.transform.parent.localPosition = text_offset; // Set text position
        progress_bar = transform.GetChild(0)?.GetChild(0)?.Find("progress_bar")?.GetComponent<Slider>(); // Assign slider to be a progressbar
        if(progress_bar == null) Debug.LogError("progress_bar slider component of Good dirt not found!"); // If component not found - display a error message
        else {
            progress_bar.minValue = 0; // Set minimum value of progress bar to 0 its equal 0% to our current value (see further)
            progress_bar.value = progress_bar.minValue; // Set start progressbar value equal to 0
            progress_bar.maxValue = 100; // Set maximum value of progress bar to 100 its equal 100% to our current value (see further)
            progress_bar?.gameObject.SetActive(false); // Disable progress bar from start
        }
    }

    void Update() {
        if(!resources_controller.Instance.GetGameStopStatus()) {
            // NOTE: If interaction type is MOUSE - LMB: plant/harvest, RMB: destroy/harvest 
            if((interaction_type == INTERACTION_TYPE.KEY && Input.GetKeyDown(plant_key)) || (interaction_type == INTERACTION_TYPE.MOUSE && Input.GetMouseButtonUp(0))) { // If input detected
                if(!planted && selected) Plant(); // If not complited - plant
            }

            if((interaction_type == INTERACTION_TYPE.KEY && Input.GetKeyDown(harvest_key)) || (interaction_type == INTERACTION_TYPE.MOUSE && Input.GetMouseButtonUp(0))) { // If input detected
                if(completed && selected) Harvest(); // If complited - harvest
            }

            if((interaction_type == INTERACTION_TYPE.KEY && Input.GetKeyDown(destroy_key)) || (interaction_type == INTERACTION_TYPE.MOUSE && Input.GetMouseButtonUp(1))) { // If input detected
                if(selected) { // If this cell selected
                    if(completed) Harvest(); // If grow done, we harvest it
                    else if(planted && cam_controller.Instance.GetInputType() != CONSTS_ENUMS.input_type.MOBILE) DestroyThis(true); // In other case - destroy this plant
                }
            }
        }
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
        if(Vector3.Distance(transform.position, player.position) < plants_manager.Instance.GetSelectionDistance()) { // If player (camera) distance is in awailable range 
            Highlight(true); // Set this cell active 
        }
    }

    private void OnMouseExit() { // if mouse out of this cell
        Highlight(false); // Set this cell innactive
    }

    void Plant(bool _rehavest = false) { // Planting method
        if(((cam_controller.Instance.GetInputType() == CONSTS_ENUMS.input_type.MOBILE) && !cam_controller.Instance.GetTouchMoveLock() && !cam_controller.Instance.GetTouchActionLock()) || (cam_controller.Instance.GetInputType() == CONSTS_ENUMS.input_type.PC && !cam_controller.Instance.GetPCScrollLocked())) { // If player have enough money and this cell selected and there is no plant already
            if(!_rehavest) plant_kursor = plants_manager.Instance.GetCursor(); // Save link to plant to get info about
            if(CanBuy(plant_kursor) || _rehavest) { // if can buy selected plant
                planted = true; // Set this cell as busy (have plant)
                if(_rehavest) {
                    Destroy(plant_prefab.gameObject); // Destroy old prefab
                    plant_prefab = null;
                }
                if(!_rehavest) {
                    resources_controller.Instance.SetResourcesQuantity(-plants_manager.Instance.plants_base[plant_kursor].GetPrice()); // Subtract price from player money
                    reharvest_count = plants_manager.Instance.plants_base[plant_kursor].GetReharvestCount(); // Get reharvest counts
                }
                plant_prefab = Instantiate(plants_manager.Instance.GetPlant(plant_kursor), transform.position, transform.rotation); // Assign to this prefab new plant by cursor position in plants data base
                plant_prefab.transform.SetParent(transform); // Child this plant to cell
                plant_prefab.GetComponent<plant_controller>().AssignDirtController(this); // Send this controller to plant prefab
                GetComponent<AudioSource>().PlayOneShot(plant_snd); // Play plant sound
                plant_prefab.GetComponent<plant_controller>().StartGrow(); // Manually start plant growing
                StartCoroutine("CalcTime"); // Start time calculation
            }
        }
    }

    void Harvest() {
        GetReward(); // If player trying to harvest and this cell is selected
    }

    public void GetReward() {
        if(completed && !resources_controller.Instance.GetGameStopStatus() ) { // If grow done
            if(give_reward) { // If give reward selected
                resources_controller.Instance.SetResourcesQuantity(plants_manager.Instance.plants_base[plant_kursor].GetReward()); // Add money to player
                PlaySoundOnce(reward_sound); // Play give reward sound
            }
            if((plants_manager.Instance.plants_base[plant_kursor].GetReharvestMode() == CONSTS_ENUMS.REHARVEST.NONE || (plants_manager.Instance.plants_base[plant_kursor].GetReharvestMode() == CONSTS_ENUMS.REHARVEST.REHARVEST && reharvest_count <= 0))) DestroyThis(); // If no reharvest - call destroying and refreshing method
            if(plants_manager.Instance.plants_base[plant_kursor].GetReharvestMode() == CONSTS_ENUMS.REHARVEST.REHARVEST && reharvest_count > 0) { // If need to reharvest
                completed = false; // Reset completed status
                Plant(true); // Start planting with reharvest flag
                reharvest_count--; // Decrease reharvest counter
            }
        }
    }

    void DestroyThis(bool play_snd = false) {
        if(selected) { // If this cell is selected
            if(play_snd) PlaySoundOnce(destroy_sound); // If need to play sound
            Instantiate(cell_prefab, transform.position, transform.rotation); // Instantiate new cell
            selected = false; // Disable all statuses
            planted = false;
            completed = false;
            Destroy(gameObject); // Destroy this cell (in this place will already created new cell)
        }
    }

    public bool CanBuy(int item_num) { // Method for checking buy awailabness
        if(resources_controller.Instance.GetResourceQuantity() - plants_manager.Instance.plants_base[item_num].GetPrice() >= 0) { // If player have enough money to buy
            return true; // Return what player can buy
        }
        return false; // Else - cant
    }

    IEnumerator CalcTime() { // Method calculates grow time and display progress bar and text with new values
        plant_controller p_c = plant_prefab.GetComponent<plant_controller>(); // Get link to plant controller
        
        string total_time = String.Empty; // Clear total time
        time_left_label.transform.parent.localPosition += plants_manager.Instance.plants_base[plant_kursor].GetTextOffset(); // Set offset based on plant offset
        time_left_label.text = String.Empty; // Clear label

        if(TimeSpan.FromSeconds(p_c.GetGrowTimeGlobal()).Hours < 1) { // If hours less than 1
            if(TimeSpan.FromSeconds(p_c.GetGrowTimeGlobal()).Minutes < 1) { // If minutes less than 1
                total_time = TimeSpan.FromSeconds(p_c.GetGrowTimeGlobal()).ToString(@"ss"); // Display only seconds
            } else {
                total_time = TimeSpan.FromSeconds(p_c.GetGrowTimeGlobal()).ToString(@"mm\:ss"); // Display seconds and minutes
            }
        } else {
            total_time = TimeSpan.FromSeconds(p_c.GetGrowTimeGlobal()).ToString(@"hh\:mm\:ss"); // Display all time format
        }

        if(info_type == INFO_TYPE.TEXT)
            time_left_label?.gameObject.SetActive(true); // If text info type selected - show text
        if(info_type == INFO_TYPE.PROGRESS_BAR) {
            progress_bar?.gameObject.SetActive(true); // If progress bar info type selected - show progress bar
        }
        if(info_type == INFO_TYPE.ALL) {
            time_left_label?.gameObject.SetActive(true); // If all info type selected - display all (text and progress bar)
            progress_bar.gameObject.SetActive(true);
        }

        while(!completed) { // While grow time left
            if(p_c != null) {
                if(info_type == INFO_TYPE.ALL || info_type == INFO_TYPE.TEXT) {
                    if(time_type == TIME_TYPE.CURRENT) {
                        if(TimeSpan.FromSeconds(p_c.GetTimeCurrent()).Hours < 1) { // If hours less than 1
                            if(TimeSpan.FromSeconds(p_c.GetTimeCurrent()).Minutes < 1) { // If minutes less than 1
                                time_left_label.text = TimeSpan.FromSeconds(p_c.GetTimeCurrent()).ToString(@"ss"); // Display only seconds
                            } else {
                                time_left_label.text = TimeSpan.FromSeconds(p_c.GetTimeCurrent()).ToString(@"mm\:ss"); // Display seconds and minutes
                            }
                        } else {
                            time_left_label.text = TimeSpan.FromSeconds(p_c.GetTimeCurrent()).ToString(@"hh\:mm\:ss"); // Display all time format
                        }
                    }
                    if(time_type == TIME_TYPE.LEFT) {
                        if(TimeSpan.FromSeconds(p_c.GetTimeLeft()).Hours < 1) { // If hours less than 1
                            if(TimeSpan.FromSeconds(p_c.GetTimeLeft()).Minutes < 1) { // If minutes less than 1
                                time_left_label.text = TimeSpan.FromSeconds(p_c.GetTimeLeft()).ToString(@"ss"); // Display time left in seconds
                            } else {
                                time_left_label.text = TimeSpan.FromSeconds(p_c.GetTimeLeft()).ToString(@"mm\:ss"); // Display seconds and minutes
                            }
                        } else {
                            time_left_label.text = TimeSpan.FromSeconds(p_c.GetTimeLeft()).ToString(@"hh\:mm\:ss"); // Display all time format
                        }
                    }
                }

                if(info_type == INFO_TYPE.TEXT || info_type == INFO_TYPE.ALL) { // If info type = text
                    if(time_type == TIME_TYPE.CURRENT) // if time type = current
                        time_left_label.text += "/" + total_time; // Display current time and total time
                }
                if(info_type == INFO_TYPE.PROGRESS_BAR || info_type == INFO_TYPE.ALL) { // If info type = progress bar
                    progress_bar.value = (float)(p_c.GetTimeCurrent() / p_c.GetGrowTimeGlobal() * progress_bar.maxValue); // Calculate current progress bar value as a pecent between 0 and 100 (min and max progress bar values)
                }
            }
            yield return null;
        }
    }

    void PlaySoundOnce(AudioClip a_c) { // Play sound
        AudioSource snd = new GameObject().AddComponent<AudioSource>(); // Create audiosource
        snd.clip = a_c; // Assign audio clip to audio source
        snd.Play(); // Play audiosource
        Destroy(snd, a_c.length); // Destroy audiosource after sound complete playing
    }

    public void Initiate() {

    }

    public void GrowCompleted() {
        progress_bar?.gameObject.SetActive(false); // If plant growed
        if(!hide_text_when_done) { // If not hide text selected
            if(interaction_type == INTERACTION_TYPE.KEY)
                time_left_label.text = "PRESS '" + harvest_key.ToString() + "' TO HARVEST"; // Display harvest text
            if(interaction_type == INTERACTION_TYPE.MOUSE)
                time_left_label.text = "HARVEST!"; // Display harvest text
        } else {
            time_left_label.text = String.Empty; // Else clear text
        }
        completed = true; // Completed - true, cos grow time left <= 0
        StopAllCoroutines();
    }

}
