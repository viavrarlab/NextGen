using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectConfig))]
public class ObjectConfigEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DrawDefaultInspector();

        ObjectConfig Configs = (ObjectConfig)target;
        if (GUILayout.Button("Configur"))
        {
            Configs.AddComponents();
        }
    }
}
