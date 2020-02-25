using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SocketPair
{
    public BoxCollider m_Collider;
    public PlacementPoint m_PlacementPoint;

    public SocketPair(BoxCollider _collider, PlacementPoint _placementPoint)
    {
        m_Collider = _collider;
        m_PlacementPoint = _placementPoint;
    }
}

public class PlacementSocketGenerator : MonoBehaviour
{


    public Material m_SocketMaterial;

    public List<SkinnedMeshRenderer> m_MeshList;

    public List<GameObject> m_Sockets;

    public List<GameObject> m_ArmatureBones;

    public List<SocketPair> m_SocketPairs;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void GenerateSockets()
    {
        foreach(Transform child in this.transform)
        {
            SkinnedMeshRenderer rend = child.GetComponent<SkinnedMeshRenderer>();
            if (rend != null)
            {
                m_MeshList.Add(rend);
            }
        }

        // create empty object for holding sockets
        GameObject socketRoot = new GameObject("PlacementSocket_Root");
        socketRoot.transform.SetParent(transform,false);
        // create objects for sockets
        int id = 0;
        foreach(SkinnedMeshRenderer mesh in m_MeshList)
        {
            GameObject socket = new GameObject(mesh.gameObject.name + "_Socket");
            socket.transform.SetParent(socketRoot.transform, false);
            socket.transform.position = mesh.transform.position;
            socket.transform.rotation = mesh.transform.rotation;
            // mesh filter
            MeshFilter mf = socket.AddComponent<MeshFilter>() as MeshFilter;
            mf.mesh = mesh.sharedMesh;
            // mesh renderer with same mesh as expected object (from m_MeshList)
            MeshRenderer mr = socket.AddComponent<MeshRenderer>() as MeshRenderer;
            mr.material = m_SocketMaterial;

            BoxCollider col = socket.gameObject.AddComponent<BoxCollider>() as BoxCollider;
            Bounds bounds = new Bounds(mesh.transform.position, Vector3.zero);
            bounds.Encapsulate(mesh.bounds);
            Debug.Log($"Teksts - {mesh.bounds}");
            Vector3 localCenter = bounds.center - socket.transform.position;
            bounds.center = localCenter;
            col.center = bounds.center;
            col.size = bounds.size;
            col.isTrigger = true;

            // PlacementPoint.cs
            PlacementPoint pp = socket.AddComponent<PlacementPoint>() as PlacementPoint;
            pp.m_PlaceableID = id;
            pp.m_CheckForCorrectAngle = false;

            SocketPair sp = new SocketPair(col,pp);
            m_SocketPairs.Add(sp);

            id++;
        }

        foreach(SocketPair sp in m_SocketPairs)
        {
            foreach(SocketPair checkPair in m_SocketPairs)
            {
                if(sp != checkPair)
                {
                    bool doesIntersect = sp.m_Collider.bounds.Intersects(checkPair.m_Collider.bounds);
                    if (doesIntersect)
                    {
                        sp.m_PlacementPoint.m_IntersectingSockets.Add(checkPair);
                        //print($"{sp.m_Collider.name} intersects with {checkPair.m_Collider.name}");
                    }
                }
            }
        }
    }

    public void GenerateGrabbableObjects()
    {
        GameObject armatureRoot = transform.Find("Armature").gameObject;
        int id = 0;
        foreach (Transform bone in armatureRoot.transform)
        {
            Rigidbody rb = bone.gameObject.AddComponent<Rigidbody>() as Rigidbody;
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            BoxCollider col = bone.gameObject.AddComponent<BoxCollider>() as BoxCollider;
            Renderer rend = GetMeshRenderer(bone.name);
            Debug.Log($"Mesh - {bone.name}");
            Bounds bounds = new Bounds(rend.transform.position, Vector3.zero);
            bounds.Encapsulate(rend.bounds);
            Vector3 localCenter = bounds.center - bone.position;
            bounds.center = localCenter;

            col.center = bounds.center;
            col.size = bounds.size;

            Placeable placeable = Placeable.CreateComponentReturn(bone.gameObject, id);
            id++;
        }
    }

    public void CleanGrabbableObjects()
    {
        GameObject armatureRoot = transform.Find("Armature").gameObject;
        foreach (Transform bone in armatureRoot.transform)
        {
            DestroyImmediate(bone.gameObject.GetComponent<Placeable>());
            DestroyImmediate(bone.gameObject.GetComponent<Rigidbody>());
            DestroyImmediate(bone.gameObject.GetComponent<Collider>());
        }
    }

    SkinnedMeshRenderer GetMeshRenderer(string _boneName)
    {
        if (m_MeshList.Count <= 0)
        {
            Debug.LogError("Mesh List is empty. Cannot get mesh for collider.");
            return null;
        }
        foreach (SkinnedMeshRenderer mesh in m_MeshList)
        {
            if(_boneName == mesh.gameObject.name + "_Bone")
            {
                return mesh;
            }
        }
        return null;
    }

    public void ClearAll()
    {
        if (m_MeshList.Count > 0) m_MeshList.Clear();
        if (m_Sockets.Count > 0) m_Sockets.Clear();
        if (m_ArmatureBones.Count > 0) m_ArmatureBones.Clear();
        if (m_SocketPairs.Count > 0) m_SocketPairs.Clear();
        if (transform.Find("PlacementSocket_Root") != null)
            DestroyImmediate(transform.Find("PlacementSocket_Root").gameObject);
        CleanGrabbableObjects();
    }

}
