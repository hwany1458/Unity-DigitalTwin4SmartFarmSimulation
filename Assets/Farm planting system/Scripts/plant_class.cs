using UnityEngine;
/// <summary>
/// Basic plant class, contains all info about this plant
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
/// - Added global cursor memory in case of adding sorting
/// 

[System.Serializable]
public class plant_class {
    
    [SerializeField] string name; // Plant name
    [SerializeField] int price; // Price of this plant
    [SerializeField] int reward; // Reward (money to give in case of harvesting)
    [SerializeField] Sprite ico; // Plant ICO for UI
    [SerializeField] GameObject plant_prefab; // Actual plant prefab
    [SerializeField] CONSTS_ENUMS.category category = CONSTS_ENUMS.category.ALL; // Category type for sorting
    [SerializeField] Vector3 text_offset = new Vector3(); // Text offset in case of configuring
    [SerializeField] CONSTS_ENUMS.REHARVEST reharvest_mode; // If you want to add reharvest feature to trees for example
    [SerializeField] int reharvest_count; // How much reharvests left
    int global_cursor_pos; // In case of sorting we need to know which is id of this item in sorted list

    // Next bloks return actual values of variables
    public GameObject GetPlantPrefab() {
        return plant_prefab;
    }

    public int GetPrice() {
        return price;
    }

    public Sprite GetIco() {
        return ico;
    }

    public int GetReward() {
        return reward;
    }

    public string GetName() {
        return name;
    }

    public CONSTS_ENUMS.category GetCategory() {
        return category;
    }

    public Vector3 GetTextOffset() {
        return text_offset;
    }

    public void SetGlobalCursor(int i) {
        global_cursor_pos = i;
    }

    public int GetGlobalCursor() {
        return global_cursor_pos;
    }

    public CONSTS_ENUMS.REHARVEST GetReharvestMode() {
        return reharvest_mode;
    }

    public int GetReharvestCount() {
        return reharvest_count;
    }
}
