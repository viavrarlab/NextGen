using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

[CustomEditor(typeof(ObjectConfig))]
public class ObjectConfigEditor : Editor
{
    GameObject armature;
    List<GameObject> Bones;
    ObjectConfig Configs;
    public List<Tuple<GameObject, int>> BonesWithId = new List<Tuple<GameObject, int>>();
    //CorrectOrderTests lists;
    private void OnEnable()
    {
        Configs = (ObjectConfig)target;
        Bones = new List<GameObject>();
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if (GUILayout.Button("Configure"))
        {
            AddComponents();
            AddTag();
            //CreateScripts();
        }



        if (GUILayout.Button("ReturListValue"))
        {
            GetCustomListValues();
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
    //void CreateScripts()
    //{
    //    int Index = 1;
    //    foreach (GameObject bone in Bones)
    //    {
    //        // remove whitespace and minus
    //        string name = bone.name.Replace(" ", "");
    //        string copyPath = "Assets/IDScripts/" + name + ".cs";
    //        Debug.Log("Creating Classfile: " + copyPath);
    //        if (File.Exists(copyPath) == false)
    //        { // do not overwrite
    //            using (StreamWriter outfile =
    //                new StreamWriter(copyPath))
    //            {
    //                outfile.WriteLine("using UnityEngine;");
    //                outfile.WriteLine("using System.Collections;");
    //                outfile.WriteLine("");
    //                outfile.WriteLine("public class " + name + " : MonoBehaviour {");
    //                outfile.WriteLine(" ");
    //                outfile.WriteLine("    public static int Index =  " + Index + ";");
    //                outfile.WriteLine("}");
    //            }//File written
    //        }
    //        Index++;
    //    }
    //    AssetDatabase.Refresh();
    //}
    void AddIDScript()
    {
        FindArmature();
        foreach (GameObject bone in Bones)
        {
            //string Bname = bone.name;
            //Type MyScriptType = Type.GetType(Bname + ",Assembly-CSharp");
            //Debug.Log(MyScriptType);
            //bone.AddComponent(MyScriptType);
            //Debug.Log("Scripts Added To Bone");

        }

    }
    void GetCustomListValues()
    {

    }
}
