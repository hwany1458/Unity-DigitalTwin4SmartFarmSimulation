using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Resource controller, defines all resources in game like money, wood, gems etc
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 1.0     RASKALOF    13.11.2018    2018
/// -------------------------------------------
/// - Initial development

public class UI_color_changer : MonoBehaviour {
    [Header("UI Objects")] 
    [SerializeField] Material UI_elements_material; // Material for UI images and elements (material from here should be assigned in each UI element)
    [SerializeField] Material UI_texts_material; // Material for UI text (material from here should be assigned in each UI text)
    [Space]
    [Header("Target colors")]
    [SerializeField] Color target_element_color; // Target UI elements color
    [SerializeField] Color target_text_color; // Target Text color

#pragma warning disable 0414    // suppress value not used warning
    Color default_target_background_color = new Color(0.4056604f, 0.1768785f, 0f, 1f); // Dark orange (this is as i think the best and default values)
    Color default_target_controur_color = new Color(1f, 0.5586883f, 0f, 1f); // Orange this is as i think the best and default values)
#pragma warning restore 0414    // restore value not used warning

    // Use this for initialization
    void Start () {
        ChangeColor(); // Change color
    }

    public void ChangeColor() {
        UI_elements_material.color = target_element_color; // Assign color to UI elements
        UI_texts_material.color = target_text_color; // Assign color to text
    }
}
