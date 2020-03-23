using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


[CustomEditor(typeof(CorrectOrderTests))]
[CanEditMultipleObjects]
public class PlacementOrderEditor : Editor
{ 
    CorrectOrderTests m_CorrOrder;
    GameObject armature;
    CustomListClass item;

    ReorderableList PartsList;

    float lineheight;
    float lineheightspace;
    private void OnEnable()
    {       
        m_CorrOrder = (CorrectOrderTests)target;

        lineheight = EditorGUIUtility.singleLineHeight;
        lineheightspace = lineheight + 5;

        PartsList = new ReorderableList(serializedObject, serializedObject.FindProperty("Parts"),true,true,true,true);

        PartsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = PartsList.serializedProperty.GetArrayElementAtIndex(index);

            var elementObj = element.serializedObject as object as SerializedObject;

            //EditorGUI.LabelField(new Rect(rect.x,rect.y,rect.width,lineheight), elementObj.FindProperty("Name").stringValue);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, lineheight), element.FindPropertyRelative("obj"));
            //EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineheight, rect.width, lineheight), element.FindPropertyRelative("set"));

            SerializedProperty propertyIterator = elementObj.GetIterator();

            int i = 0;
            //while (propertyIterator.NextVisible(true))
            //{
            //    EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineheightspace * i), rect.width, lineheight), propertyIterator);
            //    //EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, lineheight), element.FindPropertyRelative("obj"));
            //    Debug.Log("fignja");
            //    i++;
            //}

        };

        //PartsList.elementHeightCallback = (int index) =>
        //{
        //    float height = 0;

        //    SerializedProperty element = PartsList.serializedProperty.GetArrayElementAtIndex(index);

        //    var elementObj = element.serializedObject as object as SerializedObject;

        //    SerializedProperty propertyIterator = elementObj.GetIterator();
        //    int i = 0;
        //    while (i < listLenght)
        //    {
        //        i++;
        //    }
        //    height = lineheightspace * i;

        //    return height;
        //};
    }


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PartsList.DoLayoutList();

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
            item = new CustomListClass
            {
                obj = child.gameObject,
                set = 0
            };
            m_CorrOrder.Parts.Add(item);

        }
    }
    public void ClearList()
    {
        m_CorrOrder.Parts.Clear();;
    }
}
