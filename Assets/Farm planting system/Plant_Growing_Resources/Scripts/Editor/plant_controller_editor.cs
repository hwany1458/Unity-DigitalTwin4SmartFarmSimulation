using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
/// <summary>
/// This script configures presentation of plant_controller.cs in editor
/// </summary>
/// Log:
/// -------------------------------------------
/// Vers    Author        Date        Unity
/// -------------------------------------------
/// 3.0     Deatrocker    12.12.2017  2017.2.0f3
/// -------------------------------------------
/// - Initial development
/// -------------------------------------------
/// Vers    Author       Date         Unity
/// ------- ------------ ------------ ---------
/// 3.5     RASKALOF     03.11.2018   2018.2.6f1
/// ------- ------------ ------------ ---------
/// - Implemented AudioClips for start/end sounds of stages
/// -------------------------------------------

    [CustomEditor(typeof(plant_controller))]

public class plant_controller_editor : Editor {
	// Theres will not be so much comments becouse if you don't know what heppens here, better dont'change anything to prevent all plant system crush
	plant_controller t; // Target script
	SerializedObject GetTarget;
	SerializedProperty ThisList;
	int ListSize; // Stages list size

	void OnEnable(){ // Each time when script becomes enabled
		t = (plant_controller)target; // Assign to script instance target to script
		GetTarget = new SerializedObject(t); // Assign instance to serialized object
		ThisList = GetTarget.FindProperty("stages"); // Find the List in our script and create a refrence of it
	}

	public override void OnInspectorGUI(){
		GetTarget.Update();
		ListSize = ThisList.arraySize;
		EditorStyles.label.richText = true;
		ListSize = EditorGUILayout.IntField ("Stages count:", ListSize);

		if(ListSize != ThisList.arraySize){ // Stages list displaying logic
			while(ListSize > ThisList.arraySize){
				ThisList.InsertArrayElementAtIndex(ThisList.arraySize);
			}
			while(ListSize < ThisList.arraySize){
				ThisList.DeleteArrayElementAtIndex(ThisList.arraySize - 1);
			}
		}
		
        t.auto_grow = EditorGUILayout.Toggle("Auto grow: ", t.auto_grow);

        if(GUILayout.Button("Add stage")){ // Add new stage realization
			t.stages.Add(new plant_controller.plant_stage());
		}

        GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(4)}); // Separation line
		for(int i = 0; i < ThisList.arraySize; i++){ // Main display logic
			GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)}); // Separation line
			SerializedProperty MyListRef = ThisList.GetArrayElementAtIndex(i);
			SerializedProperty my_info = MyListRef.FindPropertyRelative("my_info");
			SerializedProperty stage_go_action = MyListRef.FindPropertyRelative("stage_go_action");
			SerializedProperty grow_mode = MyListRef.FindPropertyRelative("grow_mode");
			SerializedProperty grow_speed = MyListRef.FindPropertyRelative("speed");
			SerializedProperty new_plant_go = MyListRef.FindPropertyRelative("new_plant_go");
			SerializedProperty new_scale = MyListRef.FindPropertyRelative("new_scale");
			SerializedProperty stage_start_sound = MyListRef.FindPropertyRelative("stage_start_sound");
			SerializedProperty first_sound = MyListRef.FindPropertyRelative("first_sound");


            if (i != 0) { // If not initialization stage
                EditorStyles.label.alignment = TextAnchor.MiddleCenter;
                EditorStyles.label.fontSize = 12;
                EditorGUILayout.LabelField (System.String.Format("<b>{0}</b>", my_info.stringValue));
                EditorStyles.label.fontSize = 0;
                EditorStyles.label.alignment = TextAnchor.MiddleLeft;
                EditorGUILayout.PropertyField (my_info, new GUIContent("Stage name"));
                EditorGUILayout.PropertyField (stage_go_action, new GUIContent ("Stage action"));
				EditorGUILayout.PropertyField (grow_mode, new GUIContent ("Grow mode"));
            }

			if (i == 0) { // If initialization stage
				t.start_scale.x = Mathf.Clamp (t.start_scale.x, 0.001f, float.MaxValue);
				t.start_scale.y = Mathf.Clamp (t.start_scale.y, 0.001f, float.MaxValue);
				t.start_scale.z = Mathf.Clamp (t.start_scale.z, 0.001f, float.MaxValue);
				EditorStyles.label.alignment = TextAnchor.MiddleCenter;
				EditorStyles.label.fontSize = 12;
				EditorGUILayout.LabelField ("<b>INITIALIZATION STAGE</b>");
				EditorStyles.label.fontSize = 0;
				grow_speed.floatValue = 0;
				t.start_scale = EditorGUILayout.Vector3Field ("Start scale:", t.start_scale);
				EditorStyles.label.alignment = TextAnchor.MiddleLeft;
                EditorGUILayout.PropertyField(first_sound, new GUIContent("Start sound"));
            }

			if (i != 0) { // If GROW_MODE = DELAY and its not init stage
				EditorGUILayout.PropertyField (grow_speed, new GUIContent("Stage delay")); // Display delay counter to set
            }

			if (grow_mode.enumValueIndex != 1) { // If GROW_MODE = SCALE
				if (i != 0) { // If its not init stage
                    EditorGUILayout.PropertyField(new_scale, new GUIContent("Target Scale"));
                }
			}

			if (stage_go_action.enumValueIndex == 0) { // If GO_MODE = Replace
				EditorGUILayout.PropertyField (new_plant_go, new GUIContent ("New prefab"));
			}

            if(i != 0) {
                EditorGUILayout.PropertyField(stage_start_sound, new GUIContent("Stage start sound"));
            }

            if((i !=0 && ListSize > 1) || (ListSize == 1))
			if(GUILayout.Button("Remoove stage (" + i.ToString() + ")")){ // Remoove selected stage realization
				ThisList.DeleteArrayElementAtIndex(i);
			}

			if (i != 0) { // If not init stage
				EditorGUILayout.BeginHorizontal ();
				if (i - 1 != 0) { // If previous stage != init stage
					if (GUILayout.Button ("Move ↑")) { // Display moove up button
						ThisList.MoveArrayElement (i, i - 1);
					}
				}
				if (i + 1 < ThisList.arraySize) { // If this is not last stage
					if (GUILayout.Button ("Move ↓")) { // Display moove down button
						ThisList.MoveArrayElement (i, i + 1);
					}
				}
				EditorGUILayout.EndHorizontal ();
			}
		}
		GetTarget.ApplyModifiedProperties();
	}
}