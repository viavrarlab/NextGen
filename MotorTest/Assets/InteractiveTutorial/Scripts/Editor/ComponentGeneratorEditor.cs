using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ComponentGenerator))]
public class ComponentGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ComponentGenerator myTarget = (ComponentGenerator)target;

        if (GUILayout.Button("Generate Components"))
        {
            myTarget.Generate();
        }
    }
}
