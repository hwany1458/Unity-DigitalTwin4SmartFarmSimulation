using System;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This script fills UI prefab of plant
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 1.0     RASKALOF    13.11.2018    2018
/// -------------------------------------------
/// - Initial development

public class UI_content_filler : MonoBehaviour {
    [Header("AUTOFIND SETTINGS")]
    [SerializeField] string name_search; // Type here (and other string next) (in Editor' inspector) your actual GO names to find in child if you are not manually assigned all component
    [SerializeField] string ico_search;
    [SerializeField] string price_search;
    [SerializeField] string reward_search;
    [SerializeField] string time_search;
    [Space]
    [Header("MANUAL SETTINGS")]
    [SerializeField] Text name_label; // Assign here relevant components if you want to assign it manually or script will try to find it by string names
    [SerializeField] Image ico_picture;
    [SerializeField] Text price_label;
    [SerializeField] Text reward_label;
    [SerializeField] Text time_label;
    [SerializeField] string price_text_display = String.Empty; // Text to display in UI element
    [SerializeField] string reward_text_display = String.Empty; // Text to display in UI element
    [SerializeField] string time_text_display = String.Empty; // Text to display in UI element

    private void Awake() { // Find objects by its names and assign links of components to local vars
       foreach(Transform item in transform) { // If some component not assigned manually - try to get it from childs by string names
            if(ico_picture == null)  if(item.name == ico_search) ico_picture = item.GetComponent<Image>();
            if(name_label == null)   if(item.name == name_search) name_label = item.GetComponent<Text>();
            if(price_label == null)  if(item.name == price_search) price_label = item.GetComponent<Text>();
            if(reward_label == null) if(item.name == reward_search) reward_label = item.GetComponent<Text>();
            if(time_label == null)   if(item.name == time_search) time_label = item.GetComponent<Text>();
        }
    }

    public void SetName(string value, bool no_money = false) { // Assign value to name
        if(!no_money) // If have money
            name_label.text = value; // Just set value
        else
            name_label.text = "<Color=red>" + value + "</color>"; // if not have money for this - display name in red (same logic in next methods)
    }

    public void SetPrice(string value, bool no_money = false) { // Assign price
        if(!no_money)
            price_label.text = price_text_display + value;
        else
            price_label.text = "<Color=red>" + price_text_display + value + "</color>";
    }

    public void SetReward(string value, bool no_money = false) { // Assign reward
        if(!no_money)
            reward_label.text = reward_text_display + value;
        else
            reward_label.text = "<Color=red>" + reward_text_display + value + "</color>";
    }

    public void SetIco(Sprite value) { // Assign ico
        ico_picture.sprite = value;
    }

    public void SetTime(float value, bool no_money = false) { // Assign time (grow duration)
        if(!no_money)
            time_label.text = time_text_display + TimeSpan.FromSeconds(value).ToString(@"hh\:mm\:ss"); // If have money just display time
        else
            time_label.text = "<Color=red>" + time_text_display + TimeSpan.FromSeconds(value).ToString(@"hh\:mm\:ss") + "</color>"; // If no money - display time in red
    }


}
