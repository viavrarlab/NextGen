using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SocketGenerator))]
public class SocketGeneratorEditor : Editor
{
    SocketGenerator m_SocketGen;
    private void OnEnable()
    {
        m_SocketGen = (SocketGenerator)target;

    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate colliders"))
        {
            m_SocketGen.GenerateColliders();
        }
        if (GUILayout.Button("GenerateSockets"))
        {
            m_SocketGen.GenerateSockets();
        }

        if (GUILayout.Button("Generate Grabbables"))
        {
            m_SocketGen.GenerateGrabbables();
        }


        if (GUILayout.Button("Post Gen Setup"))
        {
            m_SocketGen.SetPostGenerationOptions();
        }

        //if (GUILayout.Button("Clear"))
        //{
        //    m_SocketGen.Clear();
        //}
    }


}
