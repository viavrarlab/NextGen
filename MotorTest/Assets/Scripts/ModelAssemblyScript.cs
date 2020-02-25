using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelAssemblyScript : MonoBehaviour
{
    public float offset = .5f;
    public GameObject ReferenceModel;
    List<GameObject> ArmatureBones = new List<GameObject>();
    GameObject Armature;
    bool isWithInX;
    bool isWithInY;
    bool isWithInZ;

    // Start is called before the first frame update
    void Start()
    {
        GetAllArmature();
    }

    // Update is called once per frame
    void Update()
    {
        Assembly();   
    }
    void GetAllArmature()
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.tag == "Armature")
            {
                Armature = child.gameObject;
                print("Ir armatura");
            }
        }
        print(Armature.tag);
        foreach (Transform child in Armature.transform)
        {
            if (child.tag == "Grab")
            {
                ArmatureBones.Add(child.gameObject);
                print("Ir kauli");
            }
        }
        foreach(GameObject bone in ArmatureBones)
        {
            string Bname = bone.name;
            Type MyScriptType = Type.GetType(Bname + ",Assembly-CSharp");
        }
    }
    void Assembly()
    {
        //if (gameObject.transform.localPosition.x - offset > ReferenceModel.transform.localPosition.x && ReferenceModel.transform.localPosition.x > gameObject.transform.localPosition.x + offset)
        //{
        //    isWithInX = true;
        //    print("ir X");
        //}
        //if (gameObject.transform.localPosition.y - offset > ReferenceModel.transform.localPosition.y && ReferenceModel.transform.localPosition.y > gameObject.transform.localPosition.y + offset)
        //{
        //    isWithInY = true;
        //    print("ir Y");
        //}
        //if (gameObject.transform.localPosition.z - offset > ReferenceModel.transform.localPosition.z && ReferenceModel.transform.localPosition.z > gameObject.transform.localPosition.z + offset)
        //{
        //    isWithInZ = true;
        //    print("ir Z");
        //}
    }
}
