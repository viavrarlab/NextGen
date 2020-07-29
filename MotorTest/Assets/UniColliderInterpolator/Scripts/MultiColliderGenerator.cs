using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MultiColliderGenerator : MonoBehaviour
{
    public bool shouldMerge = true;
    public bool createColliderChildGameObject = true;
    public int boxesPerEdge = 20;
    public string colliderObjectName = "NCM-Colliders";

    public void GenerateColliders()
    {
        foreach (GameObject go in GetAllChildren())
        {
            if (!go.activeInHierarchy)
            {
                return;
            }
            NonConvexMeshCollider colGenComp;

            if (go.GetComponent<NonConvexMeshCollider>() == null)
            {
                colGenComp = go.AddComponent<NonConvexMeshCollider>();
            }
            else
            {
                colGenComp = go.GetComponent<NonConvexMeshCollider>();
            }

            Rigidbody rb;
            if(go.GetComponent<Rigidbody>() == null)
            {
                rb = go.AddComponent<Rigidbody>();
            }
            else
            {
                rb = go.GetComponent<Rigidbody>();
            }

            //if (go.GetComponent<MeshCollider>() == null)
            //{
            //    go.AddComponent<MeshCollider>();
            //}

            rb.useGravity = false;

            colGenComp.colliderObjectName = colliderObjectName;
            colGenComp.shouldMerge = shouldMerge;
            colGenComp.createColliderChildGameObject = createColliderChildGameObject;
            colGenComp.boxesPerEdge = boxesPerEdge;
            colGenComp.Calculate();


            go.tag = "Grab";
            go.layer = LayerMask.NameToLayer("Placeable");


        }
    }

    public void RemoveAllColliders()
    {
        foreach(GameObject go in GetAllChildrenByName())
        {
            DestroyImmediate(go);
        }

        foreach(Transform t in transform)
        {
            MeshCollider mc = t.gameObject.GetComponent<MeshCollider>();
            if(mc != null)
            {
                DestroyImmediate(mc);
            }
        }
    }

    private List<GameObject> GetAllChildren()
    {
        List<GameObject> childObjects = new List<GameObject>();
        foreach (Transform t in transform)
        {
            childObjects.Add(t.gameObject);
        }
        return childObjects;
    }

    private List<GameObject> GetAllChildrenByName()
    {
        List<GameObject> validChildObjects = new List<GameObject>();
        Transform[] allChildObjects = GetComponentsInChildren<Transform>();
        
        foreach (Transform t in allChildObjects)
        {

            if (t.gameObject.name == colliderObjectName)
            {
                validChildObjects.Add(t.gameObject);
            }
            
        }
        return validChildObjects;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MultiColliderGenerator))]
public class MultiColliderGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var script = (MultiColliderGenerator)target;
        if (GUILayout.Button("Generate colliders"))
        {
            script.GenerateColliders();
        }
        if (GUILayout.Button("Remove all colliders"))
        {
            script.RemoveAllColliders();
        }


    }
}
#endif
