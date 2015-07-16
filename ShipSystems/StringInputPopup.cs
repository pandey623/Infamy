using UnityEngine;
using UnityEditor;

public class StringInputPopup : EditorWindow {
    private string input;

    public delegate void OnInputSet(string groupName);
    public OnInputSet onInputSet;

    public string prompt = "Input";

    public void OnGUI() {
        input = EditorGUILayout.TextField(prompt, input);
        if (GUILayout.Button("Ok")) {
            if (onInputSet != null) {
                onInputSet(input);
            }
            Close();
        }

        if (GUILayout.Button("Cancel")) {
            Close();
        }
    }

}