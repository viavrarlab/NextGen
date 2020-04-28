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
            //EditorGUI.LabelField(new Rect(rect.x,rect.y,rect.width,lineheight), elementObj.FindProperty("Name").stringValue);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width/2, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("obj"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x+200, rect.y, rect.width/2, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("set"),GUIContent.none);

            //SerializedProperty propertyIterator = elementObj.GetIterator();

            //if (propertyIterator.NextVisible(true))
            //{

            //    do
            //    {
            //        if (propertyIterator.name == "obj")
            //        {
            //            EditorGUI.indentLevel++;
            //        }
            //        else
            //        {
            //            EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineheight, rect.width, lineheight), element.FindPropertyRelative("obj"), GUIContent.none);
            //            EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineheight, rect.width, lineheight), element.FindPropertyRelative("set"));
            //        }
            //    } while (propertyIterator.NextVisible(false));
            //}

            //int i = 0;
            //while (propertyIterator.NextVisible(true))
            //{
            //    //EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineheightspace * i), rect.width, lineheight), propertyIterator);
            //    //EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, lineheight), element.FindPropertyRelative("obj"));
            //    Debug.Log("fignja");
            //    i++;
            //    break;
            //}
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
        if(GUILayout.Button("Clear List"))
        {
            ClearList();
        }
    }
    public void GetParts()
    {
        GameObject armatureRoot = m_CorrOrder.gameObject.transform.Find("Armature").gameObject;
        foreach (Transform child in m_CorrOrder.gameObject.transform)
        {
            SkinnedMeshRenderer rend = child.GetComponent<SkinnedMeshRenderer>();
            if (rend != null)
            {
                item = new CustomListClass
                {
                    obj = child.gameObject,
                    set = 0
                };
                m_CorrOrder.Parts.Add(item);
            }
        }
        int index = 0;
        foreach (Transform bone in armatureRoot.transform)
        {
            m_CorrOrder.Parts[index].Bone = bone.gameObject;
            index++;
        }
    }
    public void ClearList()
    {
        m_CorrOrder.Parts.Clear();
    }
}
