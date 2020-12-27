using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine.Windows;
using JetBrains.Annotations;
using System;

[CustomEditor(typeof(CorrectOrderTests))]
[CanEditMultipleObjects]
public class PlacementOrderEditor : Editor
{ 
    CorrectOrderTests m_CorrOrder;
    GameObject armature;

    CustomListClass item;
    
    ReorderableList PartsList;

    GameObject SortedMotor;

    float lineheight;
    float lineheightspace;
    private void OnEnable()
    {
        m_CorrOrder = (CorrectOrderTests)target;

        PartsList = new ReorderableList(serializedObject, serializedObject.FindProperty("Parts"),true,true,true,true);

    }
    private void drawList()
    {
        //List Item Headers
        //PartsList.drawHeaderCallback = (Rect rect) =>
        //{
        //    EditorGUI.LabelField(rect, "Part");
        //};
        // List main body
        PartsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = PartsList.serializedProperty.GetArrayElementAtIndex(index);

            var elementObj = element.serializedObject as SerializedObject;

            float width = rect.width;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, width * 0.7f, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("obj"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + (width * 0.7f), rect.y, width * 0.05f, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("set"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + (width * 0.8f), rect.y, width * 0.1f, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("OrderNotMandatory"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + (width * 0.9f), rect.y, width * 0.1f, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("DifferentObject"), GUIContent.none);

            elementObj.ApplyModifiedProperties();
        };
        // ---- Line Spacing for reordable list --- currently not usable
        PartsList.elementHeightCallback = (int index) =>
        {
            SerializedProperty element = PartsList.serializedProperty.GetArrayElementAtIndex(index);

            var elementHeight = EditorGUI.GetPropertyHeight(element.FindPropertyRelative("set"), true);
            var margin = EditorGUIUtility.standardVerticalSpacing;
            return elementHeight + margin;
        };
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();
        drawList();
        PartsList.DoLayoutList();

        if (GUILayout.Button("Fill List"))
        {
            GetParts();
        }

        if (GUILayout.Button("Clear List"))
        {
            ClearList();
        }

        if (GUILayout.Button("Export List"))
        {
            ExportList();
        }
        if (GUILayout.Button("Import List"))
        {
            ListImport();
        }
    }
    public void GetParts()
    {
        m_CorrOrder.Parts = new List<CustomListClass>();
        foreach (Transform child in m_CorrOrder.gameObject.transform)
        {
            MeshRenderer rend = child.GetComponent<MeshRenderer>();
            if (rend != null)
            {
                item = new CustomListClass
                {
                    obj = child.gameObject,
                    set = 0,
                    OrderNotMandatory = false,
                    DifferentObject = false
                };
                m_CorrOrder.Parts.Add(item);
            }
        }
    }
    public void ClearList()
    {
        m_CorrOrder.Parts.Clear();
    }
    public void ExportList()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var item in m_CorrOrder.Parts)
        {
            sb.AppendLine(item.ToString());
        }
        System.IO.File.WriteAllText(
                System.IO.Path.Combine("Assets/Scripts/","SortedList.txt"),
                sb.ToString());
    }
    public void ListImport()
    {
        string path = EditorUtility.OpenFilePanel("Find List CSV", "", "txt");

        m_CorrOrder.Parts = System.IO.File.ReadAllLines(path).Select(v => CustomListClass.FromCSV(v)).ToList();
    }
}

