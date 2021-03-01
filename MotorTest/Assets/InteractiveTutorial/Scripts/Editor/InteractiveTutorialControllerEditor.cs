using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InteractiveTutorialController))]
public class InteractiveTutorialControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        InteractiveTutorialController myTarget = (InteractiveTutorialController)target;

        if (GUILayout.Button("Get Target Objects"))
        {
            myTarget.GetTargetObjects();
        }

        if (GUILayout.Button("Load Motor Part Config CSV"))
        {
            myTarget.LoadMotorPartConfigCSV();
        }

        if (GUILayout.Button("Populate Interactable Object Part Information"))
        {
            myTarget.PopulateInteractableObjectPartInformation();
        }

    }
}

