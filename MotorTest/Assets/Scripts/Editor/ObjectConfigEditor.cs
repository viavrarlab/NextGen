using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectConfig))]
public class ObjectConfigEditor : Editor
{
    GameObject armature;
    List<GameObject> Bones;
    ObjectConfig Configs;
    private void OnEnable()
    {
        Configs = (ObjectConfig)target;
        Bones = new List<GameObject>();
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if (GUILayout.Button("Configur"))
        {
            AddComponents();
            AddTag();
        }
    }
    public void FindArmature()
    {
        foreach (Transform child in Configs.gameObject.transform)
        {
            if (child.name == "Armature")
            {
                armature = child.gameObject;
                Debug.Log("Armatura atrasta");
            }
        }
        foreach (Transform child in armature.transform)
        {
            Bones.Add(child.gameObject);
        }
    }
    public void AddComponents()
    {
        FindArmature();
        foreach (GameObject obj in Bones)
        {
            obj.AddComponent<Rigidbody>();
            obj.AddComponent<MeshCollider>();
            Debug.Log("RigidBody and MeshCollider added");
        }
    }
    public void AddTag()
    {
        foreach(GameObject obj in Bones)
        {
            armature.tag = "Armature";
            obj.tag = "Grab";
        }
    }
}
