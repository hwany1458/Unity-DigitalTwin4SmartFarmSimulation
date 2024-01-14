using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// Controlls UI presentation of plants DB
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 1.0     RASKALOF    13.11.2018    2018
/// -------------------------------------------
/// - Initial development
/// -------------------------------------------
/// 1.1     RASKALOF    02.01.2019    2018
/// -------------------------------------------
/// - Added sorting feature

public class UI_item_controller : MonoBehaviour {
    [SerializeField] GameObject ui_item_prefab; // UI prefab to display in scroll view
    [SerializeField] Color selected_color = Color.yellow;
    [SerializeField] Color normal_color = Color.white;
    [SerializeField] Color denial_color = Color.grey; // Color used for background when player are not abble to buy this item
    [SerializeField] AudioClip denial_sound; // Sound to play when you need to show an error
    [SerializeField] AudioClip access_sound; // Sound to play when you need to play clicks, presses, etc
    CONSTS_ENUMS.category current_category = CONSTS_ENUMS.category.ALL; // Plant (item) category
    RectTransform ui_content; // Link to ui content container (auto find)
    public static UI_item_controller Instance;
    bool set_first_item; // If we changing category - its better to select first available item in new category

    private void Awake() {
        Instance = this; // Singleton pattern realization
    }

    void Start () {
        ui_content = transform.GetComponent<RectTransform>(); // Get link to rect transform
        //UpdateUIItems(); // Update UI elements
    }

    public void UpdateUIItems() { // Enter point for updating UI
        if(ui_content != null) {
            if(!resources_controller.Instance.GetGameStopStatus() && plants_manager.Instance.GetBuyModeStatus()) {
                if(plants_manager.Instance?.plants_base?.Count > 0) { // If have items in list
                    ui_content?.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f); // Set start size of UI container
                    DestroyAll(); // First of all, destroy all elements
                    FillAll(); // Fill list with new elements
                }
            }
        }
    }

    void DestroyAll() {
        // destroy all list element
        for(byte i = 0; i < (byte)transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject); // For each item in list, destroy it
        }
    }

    void FillAll() {
        // add all elements
        List<plant_class> unsorted_list = new List<plant_class>();
        List<plant_class> sorted_list = new List<plant_class>();
        for(byte i = 0; i < plants_manager.Instance.plants_base.Count; i++) { // For each plant in plants DB
            plants_manager.Instance.plants_base[i].SetGlobalCursor(i); // Record this item blobal position for cursor
            if(plants_manager.Instance.plants_base[i].GetCategory() == GetCurrentCategory() || GetCurrentCategory() == CONSTS_ENUMS.category.ALL) { // Fill list with items only from selected category
                unsorted_list.Add(plants_manager.Instance.plants_base[i]);
            }
        }
        // Fills sorted list with sorted items depends on sorting type and direction from plants_manager
        if(plants_manager.Instance.GetSortType() == CONSTS_ENUMS.PLANT_SORT_TYPE.NAME) {
            if(plants_manager.Instance.GetSortDirection() == CONSTS_ENUMS.PLANT_SORT_DIRECTION.ASCENDING)
                sorted_list = unsorted_list.OrderBy(o => o.GetName()).ToList();
            else sorted_list = unsorted_list.OrderByDescending(o => o.GetName()).ToList();
        }else
        if(plants_manager.Instance.GetSortType() == CONSTS_ENUMS.PLANT_SORT_TYPE.PRICE) {
            if(plants_manager.Instance.GetSortDirection() == CONSTS_ENUMS.PLANT_SORT_DIRECTION.ASCENDING)
                sorted_list = unsorted_list.OrderBy(o => o.GetPrice()).ToList();
            else sorted_list = unsorted_list.OrderByDescending(o => o.GetPrice()).ToList();
        } else {
            sorted_list.AddRange(unsorted_list); // If no sorting type - just copy unsorted list to work with
        }

        if(set_first_item) {
            for(int x = 0; x < sorted_list.Count; x++) {
                if(plants_manager.Instance.GetComponent<resources_controller>().GetResourceQuantity() - sorted_list[x].GetPrice() >= 0 && set_first_item) {
                    plants_manager.Instance.GetComponent<plants_manager>().SetCursor(sorted_list[x].GetGlobalCursor(), false);
                    break;
                }
            }
            set_first_item = false;
        }

        for(int i = 0; i < sorted_list.Count; i++) {
            GameObject item = Instantiate(ui_item_prefab, transform.position, transform.rotation, transform); // Add item to display
            item.AddComponent<UI_item_counter>().SetCount(sorted_list[i].GetGlobalCursor()); // Assign UI item counter component
            ui_content?.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ui_content.sizeDelta.y + item.GetComponent<RectTransform>().sizeDelta.y + ui_content.gameObject.GetComponent<VerticalLayoutGroup>().padding.top / 2); // Increase size of UI scroll view container by height and offset of UI item prefab
            item.GetComponent<UI_content_filler>().SetIco(sorted_list[i].GetIco()); // Set UI ico to prefab ico
            if(plants_manager.Instance.GetCursor() != sorted_list[i].GetGlobalCursor()) {
                item.GetComponent<Image>().color = normal_color; // If current cursor not on this element - change its color (not selected)
            } else {
                item.GetComponent<Image>().color = selected_color; // If current cursor on this element - change its color (selected)
            }
            if(plants_manager.Instance.GetComponent<resources_controller>().GetResourceQuantity() - sorted_list[i].GetPrice() < 0) { // If player doesnt have enough money to buy
                item.GetComponent<Image>().color = denial_color; // If player cant buy it - change it color
                item.GetComponent<UI_content_filler>().SetName(sorted_list[i].GetName(), true); // Set item name to UI prefab label
                item.GetComponent<UI_content_filler>().SetReward(sorted_list[i].GetReward().ToString(), true); // Set reward to UI prefab label
                item.GetComponent<UI_content_filler>().SetTime(sorted_list[i].GetPlantPrefab().GetComponent<plant_controller>().GetGrowTime(), true); // Set time based to UI prefab label
                item.GetComponent<UI_content_filler>().SetPrice(sorted_list[i].GetPrice().ToString(), true); // Set price to UI prefab label
            } else {
                item.GetComponent<UI_content_filler>().SetName(sorted_list[i].GetName()); // Set item name to UI prefab label
                item.GetComponent<UI_content_filler>().SetReward(sorted_list[i].GetReward().ToString()); // Set reward to UI prefab label
                item.GetComponent<UI_content_filler>().SetIco(sorted_list[i].GetIco()); // Set UI ico to prefab ico
                item.GetComponent<UI_content_filler>().SetTime(sorted_list[i].GetPlantPrefab().GetComponent<plant_controller>().GetGrowTime()); // Set time based to UI prefab label
                item.GetComponent<UI_content_filler>().SetPrice(sorted_list[i].GetPrice().ToString()); // Set price to UI prefab label
            }
        }
    }

    public void SetCurrentCategory(CONSTS_ENUMS.category cat) {
        current_category = cat; // Assign nev value to category id
        set_first_item = true;
        UpdateUIItems(); // Update UI items list
    }

    public CONSTS_ENUMS.category GetCurrentCategory() {
        return current_category; // Get current category
    }

    public void PlayDenial() { // Plays denial sound
        if(denial_sound != null)
            GetComponent<AudioSource>()?.PlayOneShot(denial_sound);
    }

    public void PlayAccess() { // Plays access sound
        if(access_sound != null)
            GetComponent<AudioSource>()?.PlayOneShot(access_sound);
    }

    private void OnEnable() {
        UpdateUIItems(); // On enable we need to update UI in case of something changed
    }

}
