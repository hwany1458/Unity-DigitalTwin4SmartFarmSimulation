using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
/// <summary>
/// This script contains plant growing main system logic
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 3.0     Deatrocker    12.12.2017  2017.2.0f3
/// -------------------------------------------
/// - JS script replaced with CS
/// - System logic are remaded from scratch
/// - Added additional functional (SCALE_BY_VALUES) (recommended to use instead of grow/decrease by speed)
/// - Billboard functinality are extruded from script to separated CS
/// - Added finity loop with loop times counter and stages range select
/// - Added custom editor script which controlls in editor presentation of this script
/// ------- ------------ -----------  ---------
/// Vers    Author       Date         Unity
/// ------- ------------ ------------ ---------
/// 3.5     RASKALOF     03.11.2018   2018.2.6f1
/// ------- ------------ ------------ ---------
/// - Changed 'None_Delay' Ienumerator to calculates passed time
/// - Fixed 'Grow_By_Speed', 'Decrease_By_Speed' and 'Scale_By_Values' Ienumerator to exclude incorrect time calculation
/// - Added 'GetGrowTime' method for getting total grow time count
/// - Added 'GetTimeLeft' method for getting grow time left
/// - Added functional to play sound when stage starts and ends
/// - Added functional to play sound on initial stage (for buying sound f.e.)
/// -------------------------------------------
/// Vers    Author       Date         Unity
/// ------- ------------ ------------ ---------
/// 3.6     RASKALOF     28.11.2018   2018.2.6f1
/// ------- ------------ ------------ ---------
/// - All system remaded
/// -------------------------------------------

[System.Serializable]
public class plant_controller : MonoBehaviour {
    #region vars
    // Main variables
    [SerializeField] public bool auto_grow; // Should plant start grow when its appears or let other scripts controlls when it should start growing
    [SerializeField] public GameObject plant_go; // Current displayed plant GO
    public Vector3 start_scale; // Start scale for initialize stage
    // private system purpose variables
    good_dirt_controller my_dirt_controller;
    
    DateTime grow_start_time; // Start datetime of grow
    float total_time_grow; // Total seconds of grow
    DateTime grow_end_time; // End datetime of grow
    DateTime go_to_sleep_date_time; // Datetime when app go to sleep
    float time_in_sleep; // Time in seconds how much we was sleeping
    #endregion

    #region TimeCalc
    void SetGrowStartTime() {
        grow_start_time = DateTime.Now;
    }
    DateTime GetGrowStartTime() {
        return grow_start_time;
    }
    void SetGrowEndTime() { // Get end date and time of plant growing
        grow_end_time = DateTime.Now.Add(TimeSpan.FromSeconds(GetGrowTime())); // Get current datetime and add to it calculated grow time duration => we now got date and time when our plant should be counted as growed
    }
    DateTime GetGrowEndTime() {
        return grow_end_time;
    }
    void FillStagesDateTimes() {
        DateTime tmp_time = GetGrowStartTime();
        for(int i = 0; i <= stages.Count - 1; i++) { // Main cycle
            tmp_time += TimeSpan.FromSeconds(stages[i].speed);
            stages[i].stage_end_time = tmp_time;
        }
        for(int i = 0; i <= stages.Count - 1; i++) { // Main cycle
            stages[i].stage_start_time = stages[i].stage_end_time - TimeSpan.FromSeconds(stages[i].speed);
        }
    }
    public double GetTimeCurrent() {
        return (DateTime.Now - GetGrowStartTime()).TotalSeconds;
    }
    public double GetTimeLeft() {
        return GetGrowTime() - GetTimeCurrent();
    }
    public float GetGrowTime() { // Calculates growing duration
        float time = 0.0f;
        foreach(plant_stage ps in stages) {
            time += ps.speed;
        }
        return time;
    }
    public float GetGrowTimeGlobal() { // Gets total grow time of this plant
        return total_time_grow;
    }
    #endregion

    #region PlantTransformation
    void REPLACE_PLANT(int i) {
        if(stages[i].new_plant_go == null) { // If current stage got no GO
            Debug.LogErrorFormat("ERROR#1: Null reference in {0} stage, assign GO to {1} or change GO_MODE to 'NONE'", i, "new_plant_go"); // Error
        } else {
            if(plant_go != null) {
                Transform old_trans = plant_go?.transform; // Save transform
                Destroy(plant_go); // Destroy current GO
                plant_go = Instantiate(stages[i].new_plant_go, old_trans.position, old_trans.rotation); // Ceate new one with prefab from current stage
                if(i != 0) plant_go.transform.localScale = old_trans.localScale; // Apply saved transform to new GO
                plant_go.transform.SetParent(transform); // Child it to this transform
            }
        }
    }

    void SCALE_PLANT(int i) {
        plant_go.transform.localScale = stages[i].new_scale; // Child it to this transform
    }
    #endregion

    #region Grow
    public void StartGrow() {
        plant_go = new GameObject(); // Needed for system purposes, creates instance to reassign later
        plant_go.transform.SetParent(transform); // Child it to this transform
        plant_go.transform.localScale = stages[0].new_scale; // Assign scale from zero stage
        plant_go.transform.SetPositionAndRotation(transform.position, transform.rotation); // Set same position and rotation to this transform
        SetGrowStartTime(); // Set total grow start time
        SetGrowEndTime(); // Set total grow end time
        FillStagesDateTimes(); // Depends on calculated times - calculate start and end datetimes of each stage
        REPLACE_PLANT(0); // Set start model of a plant
        if(GetComponent<AudioSource>() != null && stages[0]?.first_sound != null) GetComponent<AudioSource>().PlayOneShot(stages[0].first_sound); // Play start sound
        plant_go.transform.localScale = start_scale; // Set initialize stage's scale
        GrowController();
    }
    void GrowController() {
        StartCoroutine("GrowDelay"); // Start main method
    }
    IEnumerator GrowDelay() {
        while(DateTime.Now <= GetGrowEndTime()) {
            for(int i = 1; i < stages.Count; i++) {
                if(DateTime.Now >= stages[i].stage_start_time && DateTime.Now <= stages[i].stage_end_time) {
                    if(!stages[i].stage_flag_good) {
                        if(GetComponent<AudioSource>() != null && stages[i]?.stage_start_sound != null) GetComponent<AudioSource>().PlayOneShot(stages[i].stage_start_sound);
                        if(stages[i].stage_go_action == plant_stage.GO_MODE.REPLACE) {
                            REPLACE_PLANT(i);
                        }
                        if(stages[i].grow_mode == plant_stage.GROW_MODE.SCALE) { // If GROW_MODE = SCALE and its not initial stage
                            SCALE_PLANT(i);
                        }
                        stages[i].stage_flag_good = true;
                    }
                }
            }
            yield return null;
        }
        ExecuteLastStage(true);
    }
    void On_Grow_Stop() {
        // Use this method if you need to do something after finite or no loop type grow mode ended
        my_dirt_controller?.GrowCompleted(); // Send message to dirt controller what this plant is growed
        StopAllCoroutines(); // and stop all coroutines
        Destroy(this); // Destroy this script (free up memory)
    }
    #endregion

    #region Stages
    [System.Serializable]
    public class plant_stage { // Stage class
        public enum GO_MODE { REPLACE, NONE }; // GO mode, now its only replace method, but new TBD in v4
        public enum GROW_MODE { SCALE, NONE }; // Grow modesela
        public string my_info = "New stage"; // Name of current stage
        public GO_MODE stage_go_action; // Instance
        public GROW_MODE grow_mode; // Instance
        public Vector3 new_scale; // Target scale in case of SCALE_BY_VALUES selected
        public GameObject new_plant_go; // New plant GO prefab (if leave it null, will get prefab from previous stage (if current != initialization stage))
        public float speed; // Time for different purposes for each GROW_MODE
        public AudioClip stage_start_sound; // Sound to play when stage starts
        public AudioClip first_sound; // Sound to play ones when grow starts
        public DateTime stage_start_time;
        public DateTime stage_end_time;
        public bool stage_flag_good; // Needed when we open app from background to define if this stage transformations not applyed yet to plant_prefab
    }
    public List<plant_stage> stages = new List<plant_stage>(1); // Stages list (do not modify directly)
    void AddNew() { // Add new stage (use this)
        stages.Add(new plant_stage());
    }
    void Remove(int index) { // Remoove selected stage (use this)
        stages.RemoveAt(index);
    }
    #endregion

    #region Systems
    public void AssignDirtController(good_dirt_controller dirt_c) { // Assign dirt controller to send message when this plant is growed
        my_dirt_controller = dirt_c;
    }
    private void Awake() {
        total_time_grow = GetGrowTime(); // Get total grow time
    }
    void Start() { // Initialization
        if(stages[0].new_plant_go == null) { // If start GO are not assigned destroy this script to prevent app denial
            Destroy(this);
        }
        if(auto_grow) StartGrow(); // Start main recursive method with end flag = false
    }
    void OnApplicationFocus(bool pauseStatus) {
        if(pauseStatus) {
            //your app is NO LONGER in the background
            if(DateTime.Now >= GetGrowEndTime()) {
                ExecuteLastStage();
            } else {
                StopAllCoroutines();
                GrowController();
            }
        } else {
            //your app is now in the background
        }
    }
    void ExecuteLastStage(bool play_sound = false) {
        if(play_sound) if(GetComponent<AudioSource>() != null && stages[stages.Count-1]?.stage_start_sound != null) GetComponent<AudioSource>().PlayOneShot(stages[stages.Count - 1].stage_start_sound);
        if(stages[stages.Count - 1].stage_go_action == plant_stage.GO_MODE.REPLACE) REPLACE_PLANT(stages.Count - 1);
        else {
            for(int j = stages.Count - 1; j > 0; j--) {
                if(stages[j].stage_go_action == plant_stage.GO_MODE.REPLACE) REPLACE_PLANT(j);
            }
        }
        if(stages[stages.Count - 1].grow_mode == plant_stage.GROW_MODE.SCALE) plant_go.transform.localScale = stages[stages.Count - 1].new_scale;
        else {
            for(int j = stages.Count - 1; j > 0; j--) {
                if(stages[j].grow_mode == plant_stage.GROW_MODE.SCALE) plant_go.transform.localScale = stages[j].new_scale;
            }
        }
        On_Grow_Stop();
    }
    #endregion
}