using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectConfig : MonoBehaviour
{
    public GameObject obj;
    GameObject armature;
    public List<GameObject> Bones;
    public void FindArmature()
    {
        foreach(Transform child in obj.transform)
        {
            if(child.name == "Armature")
            { 
                armature = child.gameObject;
                Debug.Log("Armatura atrasta");
            }
        }
        foreach (Transform child in armature.transform)
        {
                Bones.Add(child.gameObject);
                print("Ir kauli");
        }

    }
    public void AddComponents()
    {
        FindArmature();
        foreach(GameObject obj in Bones)
        {
            obj.AddComponent<Rigidbody>();
            obj.AddComponent<MeshCollider>();
        }

    }
}
