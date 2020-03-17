using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(CorrectOrderTests))]
public class PlacementOrderEditor : Editor
{
    CorrectOrderTests m_CorrOrder;
    private void OnEnable()
    {
        m_CorrOrder = (CorrectOrderTests)target;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}
