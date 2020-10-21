using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlacementSocketGenerator))]
public class PlacementSocketGeneratorEditor : Editor
{
    PlacementSocketGenerator m_SocketGen;
    private void OnEnable()
    {
        m_SocketGen = (PlacementSocketGenerator)target;

    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate sockets"))
        {
            m_SocketGen.GenerateSockets();
        }
        if (GUILayout.Button("Generate grabbable bones"))
        {
            m_SocketGen.GenerateGrabbableObjects();
        }
        if (GUILayout.Button("Clear"))
        {
            m_SocketGen.ClearAll();
        }
    }


}
