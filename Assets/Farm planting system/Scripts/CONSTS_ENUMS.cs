/// <summary>
/// This script stores global vars to work with from another scripts
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 1.0     RASKALOF    11.11.2018    2018
/// -------------------------------------------
/// - Initial development

public static class CONSTS_ENUMS {
    // Global enums
    public enum category { ALL, VEGETABLES, FRUITS, BERRY, BUILDINGS  } // Plants  categorys
    public enum input_type { PC, MOBILE } // Input type
    public enum PLANT_SORT_TYPE { NONE, PRICE, NAME } // Sorting type for plants UI list
    public enum PLANT_SORT_DIRECTION { ASCENDING, DESCENDING } // Sorting direction for plants UI list
    public enum REHARVEST { NONE, REHARVEST }; // If you want to add reharvest feature to trees for example
    // Global strings
    public static string SAVE_MAP_ID_ = "MAP_ID_"; // Defines save map id for saving and loading
    public static string SAVE_HAS_SAVE = "HAS_SAVE"; // Defines is there saves
    // Global vars
    static int TOTAL_MONEY_COUNT; // Total players money
    
}
