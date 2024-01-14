using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This script is used for dynamic created UI items (elements) to be abble click it
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 1.0     RASKALOF    13.11.2018    2018
/// -------------------------------------------
/// - Initial development
/// -------------------------------------------
/// 1.1     RASKALOF    22.12.2018    2018
/// -------------------------------------------
/// - Fixed code

public class UI_item_counter : MonoBehaviour {
    int my_count; // Counter of this element

    void Start() {
        GetComponent<Button>().onClick.AddListener(TrySelectItem); // Add listener (button click interceptor), when this button will be clicked - specified method will be called
    }

    public void SetCount(int i) {
        my_count = i; // Set count for this item (set from outside)
    }

    public int GetCount() {
        return my_count; // Get count of this item
    }

    void TrySelectItem() { // This method called when button is clicked (event)
        if(resources_controller.Instance.GetResourceQuantity() - plants_manager.Instance.plants_base[GetCount()].GetPrice() >= 0) { // If player have enough money to buy this item
            SelectUIItem(); // Select
        } else SelectUIDenial(); // Call denial method
    }

    private void OnDestroy() {
        GetComponent<Button>().onClick.RemoveAllListeners(); // When this object destroying - unsubscribe from events
    }

    void SelectUIItem() {
        UI_item_controller.Instance.PlayAccess(); // Play denial sound
        plants_manager.Instance.SetCursor(GetCount()); // If we selecting this item (TrySelectItem() is passed) - select and update UI
    }

    void SelectUIDenial() {
        // here you can show f.e. error message.
        UI_item_controller.Instance.PlayDenial(); // Play denial sound
    }
            
}
