using UnityEngine;
using UnityEditor;

public class AddTurretGroupPopup : EditorWindow {
    private string groupName;

    public delegate void OnGroupNameSet(string groupName);
    public OnGroupNameSet onGroupNameSet;

    public void OnGUI() {
        groupName = EditorGUILayout.TextField("Turret group name:", groupName);
        if (GUILayout.Button("Ok")) {
            if (onGroupNameSet != null) {
                onGroupNameSet(groupName);
            }
            Close();
        }

        if (GUILayout.Button("Cancel")) {
            Close();
        }
    }

}