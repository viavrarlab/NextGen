using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SubjectNerd.Utilities;
[CustomEditor(typeof(CorrectOrderTests))]
[CanEditMultipleObjects]
public class PlacementOrderEditor : ReorderableArrayInspector
{
    CorrectOrderTests m_CorrOrder;
    GameObject armature;
    SerializedProperty lists;
    private void OnEnable()
    {
        m_CorrOrder = (CorrectOrderTests)target;
        lists = serializedObject.FindProperty("Parts");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(lists);
        serializedObject.ApplyModifiedProperties();
        if (GUILayout.Button("Fill List"))
        {
            GetParts();
        }
        if(GUILayout.Button("Clear List"))
        {
            ClearList();
        }
    }
    public void GetParts()
    {
        foreach (Transform child in m_CorrOrder.gameObject.transform)
        {
            if (child.name == "Armature")
            {
                armature = child.gameObject;
                Debug.Log("Armatura atrasta");
            }
        }
        foreach (Transform child in armature.transform)
        {
            m_CorrOrder.Parts.Add(child.gameObject);
        }
    }
    public void ClearList()
    {
        m_CorrOrder.Parts.Clear();
    }
}
