using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectScatterer))]
public class ObjectScattererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ObjectScatterer myTarget = (ObjectScatterer)target;

        if (GUILayout.Button("Scatter"))
        {
            myTarget.PerformScatter();
        }

        if (GUILayout.Button("Reset"))
        {
            myTarget.PerformReset();
        }


    }
}

