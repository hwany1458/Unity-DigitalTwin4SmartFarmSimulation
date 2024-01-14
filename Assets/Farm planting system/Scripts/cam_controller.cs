using System.Collections;
using UnityEngine;
/// <summary>
/// This is sample camera script
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 1.0     RASKALOF    13.11.2018    2018
/// -------------------------------------------
/// - Initial development
/// -------------------------------------------
/// 1.1     RASKALOF    22.12.2018    2018
/// -------------------------------------------
/// - Touch controlls bug fixes

public class cam_controller : MonoBehaviour {
    // Configurable variables
    [SerializeField] CONSTS_ENUMS.input_type INPUT_TYPE; // Input type
    [Space]
    [Header("PC INPUT SETTINGS")]
    [SerializeField] float screen_border_size = 10f; // Cursor distance to edge of the screen to moove camera
    [SerializeField] float min_moove_speed = 10f; // Start moovement speed
    [SerializeField] float max_moove_speed = 20f; // Max moovement speed
    [SerializeField] float min_to_max_speed = 0.2f; // Speed to reach max speed
    [Space]
    [Header("MOBILE INPUT SETTINGS")]
    [SerializeField] float touch_moove_speed = 0.02f; // Touch move speed (if mobile input is choosed)
    [Space]
    [Header("COMMON SETTINGS")]
    [Tooltip("GET NOTE: the value is very different between PC and Mobile. For example for mobile good 0.01, for PC 20.0")]
    [SerializeField] float zoom_speed = 20f; // Zoom speed
    [SerializeField] bool limit_moovement = true; // Should camera position be limited
    [SerializeField] Vector2 x_lim = new Vector2(9, 24); // X Limits
    [SerializeField] Vector2 z_lim = new Vector2(-9, 8); // Z Limits
    [SerializeField] Vector2 zoom_limit = new Vector2(4, 15); // Y Limits
    [Tooltip("Defines where is UI dead zone, calculated by: screen width / this value (if finger touching in this zone - you are not abble to plant (through UI)")]
    [SerializeField] float T_dead_zone_X = 4f; // Touch dead zone when buy mode is active

    float speed_multiplier; // Used for speed calculations
    Vector3 moovement_vector; // Used for speed calculations
    float speed_increase_rate = 0.0f; // Used for speed calculations
    bool scroll_locked = false; // Is scroll locked ? (for UI scroll) 
    Vector3 drag_vector; // Drag vector
    Vector3 touch_start_pos = Vector3.zero; // Move start position
    Vector3 cam_start_position = Vector3.zero; // Released position
    bool actions_locked; // Is camera moved? (for touch input)
    Touch first_finger = new Touch(); // Stores first finger info
    Touch second_finger = new Touch(); // Stores second finger info
    public static cam_controller Instance;
    bool T_first_finger_released; // In case of finger switch during zooming, we need to switch some values depends on this event

    private void Awake() {
        Instance = this; // Singleton pattern realization
    }

    void Update() {
        if(INPUT_TYPE == CONSTS_ENUMS.input_type.PC) { // If input type = PC
            moovement_vector = Vector3.zero; // reset moovement vector
            if(Input.GetAxis("Vertical") > 0.0 || Input.mousePosition.y >= Screen.height - screen_border_size) { // If moove forward
                moovement_vector += Vector3.forward * speed_multiplier * Time.deltaTime; // Add values to moovement vector
            }
            if(Input.GetAxis("Vertical") < 0.0 || Input.mousePosition.y <= screen_border_size) { // If moove backward
                moovement_vector -= Vector3.forward * speed_multiplier * Time.deltaTime; // Add values to moovement vector
            }
            if(Input.GetAxis("Horizontal") < 0.0 || Input.mousePosition.x <= screen_border_size) { // If moove left
                moovement_vector += Vector3.left * speed_multiplier * Time.deltaTime; // Add values to moovement vector
            }
            if(Input.GetAxis("Horizontal") > 0.0 || Input.mousePosition.x >= Screen.width - screen_border_size) { // If moove right
                moovement_vector += Vector3.right * speed_multiplier * Time.deltaTime; // Add values to moovement vector
            }
            if((Input.GetAxis("Mouse ScrollWheel") > 0) && !GetPCScrollLocked()) { // If zoom in
                moovement_vector += Vector3.up * zoom_speed * Time.deltaTime; // Add values to moovement vector
            }
            if((Input.GetAxis("Mouse ScrollWheel") < 0) && !GetPCScrollLocked()) { // If zoom out
                moovement_vector += Vector3.down * zoom_speed * Time.deltaTime; // Add values to moovement vector
            }
            transform.Translate(moovement_vector, Space.World); // Moove player (camera) with calculated values

            if(Input.GetAxis("Vertical") != 0.0 || Input.GetAxis("Horizontal") != 0.0
        || Input.mousePosition.y >= Screen.height - screen_border_size
        || Input.mousePosition.y <= screen_border_size
        || Input.mousePosition.x <= screen_border_size
        || Input.mousePosition.x >= Screen.width - screen_border_size) { // Detects if mouse hit edges?
                speed_increase_rate += Time.deltaTime / min_to_max_speed; // Calculate speed of interpolation
                speed_multiplier = Mathf.Lerp(min_moove_speed, max_moove_speed, speed_increase_rate); // Interpolate speed
            } else {
                speed_increase_rate = 0; // reset value
                speed_multiplier = min_moove_speed; // reset value
            }

        }

        if(INPUT_TYPE == CONSTS_ENUMS.input_type.MOBILE) { // If input type = mobile
            
            if(Input.touchCount == 2) { // If detected 2 fingers on screen (means we want to zoom)
                Vector3 new_vector = transform.position; // Creates new vector to store there calculated values to apply to transform of a camera
                first_finger = Input.GetTouch(0); // Get info about first finger
                second_finger = Input.GetTouch(1); // Get info about second finger
                Vector2 first_finger_prev_pos = first_finger.position - first_finger.deltaPosition; // Calculates previous position of first finger
                Vector2 second_finger_prev_pos = second_finger.position - second_finger.deltaPosition; // Calculates previous position of second finger
                float previous_touch_magnitude = (first_finger_prev_pos - second_finger_prev_pos).magnitude; // Gets previous magnitude
                float current_touch_magnitude = (first_finger.position - second_finger.position).magnitude; // Gets current magnitude
                new_vector.y += (previous_touch_magnitude - current_touch_magnitude) * zoom_speed; // Apply calculated values to a vector component Y
                transform.position = new Vector3(transform.position.x, new_vector.y, transform.position.z); // Apply calculated vector Y value to transform position
                if(first_finger.phase == TouchPhase.Ended) { // If first finger released but second still touching screen, we need to switch fingers info
                    T_first_finger_released = true; // Determines what first finger released
                }
            }

            if(Input.touchCount == 1) { // If only one finger (means we want to moove camera on field)
                first_finger = Input.GetTouch(0); // Get info about finger
                if(first_finger.phase == TouchPhase.Moved || first_finger.phase == TouchPhase.Stationary) { // If finger mooving or holded
                    if(T_first_finger_released) { // If it was a two fingers, but first finger was released
                        touch_start_pos = second_finger.position; // Swap start pose of first finger to second finger
                        cam_start_position = transform.position; // Start position of camera
                        T_first_finger_released = false; // Now we swapped fingers, need to reset flag
                    } else
                    if(touch_start_pos == Vector3.zero) { // In other case If we just hit the screen, we need to set start positions
                        touch_start_pos = first_finger.position; // Touch start position equals first finger position
                        cam_start_position = transform.position; // Start position of camera
                    }
                    
                    drag_vector = first_finger.position; // Get current finger position
                    if(Vector2.Distance(drag_vector, touch_start_pos) > 15f && !GetTouchMoveLock()) { // If distance from finger start point to current position more than 15 its means what its drag (moove) (you can change 15 to some other value)
                        SetTouchActionsLock(true); // Set moove status true (needed for exclude plants selection when we dont want to select fe UI selecting which position above plants)
                    }
                } else {
                    T_first_finger_released = false; // Now we swapped fingers, need to reset flag
                }

                if(first_finger.phase == TouchPhase.Ended) { // If finger released
                    touch_start_pos = Vector3.zero; // Set zero position of finger start position (means we released finger)
                    if(actions_locked == true) SetTouchActionsLock(false); // If camera mooving flag active - release moove status
                    return;
                }

                if(first_finger.phase == TouchPhase.Moved && !GetTouchMoveLock()) // If finger mooving and its mooving not on UI
                    transform.position = new Vector3(cam_start_position.x + ((touch_start_pos.x - drag_vector.x) * touch_moove_speed), transform.position.y, cam_start_position.z + ((touch_start_pos.y - drag_vector.y) * touch_moove_speed)); // Apply all calculations to player (camera) position
            }

            if(Input.touchCount == 0) {
                T_first_finger_released = false;
                touch_start_pos = Vector3.zero; // Set zero position of finger start position (means we released finger)
            }

        }

        if(limit_moovement == true) { // If need to limit moovement
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, x_lim.x, x_lim.y), Mathf.Clamp(transform.position.y, zoom_limit.x, zoom_limit.y), Mathf.Clamp(transform.position.z, z_lim.x, z_lim.y)); // Clamp position depends on limits
        }

    }

    public bool GetPCScrollLocked() { // Get zoom lock status
        return scroll_locked;
    }

    public void SetPCScrollLocked(bool status) { // Set zoom lock status
        scroll_locked = status;
    }

    public bool GetTouchMoveLock() { // Get status of camera moving availabness
        if(Input.touchCount > 0) { // If has some fingers on screen
            if(plants_manager.Instance.GetBuyModeStatus()) {
                if(first_finger.position.x <= Screen.width / T_dead_zone_X) { // If finger hit UI zone - stop moving - lock status - true
                    return true;
                } else return false;
            } else return false;
        }
        return false;
    }

    public CONSTS_ENUMS.input_type GetInputType() { // Get current input type
        return INPUT_TYPE;
    }

    public void SetInputType(CONSTS_ENUMS.input_type in_type) { // For example if you will want to hot change input type - call this method
        INPUT_TYPE = in_type;
    }

    public bool GetTouchActionLock() { // Can player plant?
        return actions_locked;
    }

    public void SetTouchActionsLock(bool status) { // When finger was mooved and released, we need a bit of delay, where player can not affect to plants, this is needed for situation, when player mooving camera above plants cells, to do not call actions on cells
        if(status == true) {
            StopAllCoroutines(); // Stop all coroutines to prevend bugs
            actions_locked = true; // Locked - player cant plant
        } else {
            if(status == false && actions_locked == true) { // If needed to unlock lock status
                StartCoroutine("TouchStatusDelay"); // Start coroutine (needed to add some delay)
            }
        }
    }

    IEnumerator TouchStatusDelay() { // Used for SetActionsLock to add some delay after finger mooved and relesed
        yield return new WaitForSecondsRealtime(0.3f); // Delay
        actions_locked = false; // Get off lock status
    }

}