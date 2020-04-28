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

    CorrectOrderTests m_CorrOrder;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void GenerateSockets()
    {
        m_CorrOrder = GetComponent<CorrectOrderTests>();
        foreach(Transform child in this.transform)
        {
            SkinnedMeshRenderer rend = child.GetComponent<SkinnedMeshRenderer>();
            if (rend != null)
            {
                m_MeshList.Add(rend);
            }
        }
        // get object set count
        int maxValue = 0;
        foreach (CustomListClass obj in m_CorrOrder.Parts)
        {
            if (obj.set > maxValue)
            {
                maxValue = obj.set;
            }
        }

        // create empty object for holding sockets
        GameObject socketRoot = new GameObject("PlacementSocket_Root");
        socketRoot.transform.SetParent(transform,false);

        //Create sets
        int count = 0;
        while (count <= maxValue)
        {
            GameObject set = new GameObject("set" + count.ToString());
            set.transform.parent = socketRoot.transform;
            count++;
        }

        // create objects for sockets and add to each set
        int id = 0;
        int setID = 0;
        while (setID <= maxValue)
        {
            foreach (CustomListClass obj in m_CorrOrder.Parts)
            {
                if (obj.set == setID)
                {
                    string setname = "set" + setID.ToString();
                    GameObject socket = new GameObject(obj.obj.gameObject.name + "_Socket");
                    socket.transform.SetParent(GameObject.Find(setname).transform, false);
                    socket.transform.position = obj.obj.transform.position;
                    socket.transform.rotation = obj.obj.transform.rotation;
                    // mesh filter
                    MeshFilter mf = socket.AddComponent<MeshFilter>() as MeshFilter;
                    mf.mesh = obj.obj.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                    // mesh renderer with same mesh as expected object (from m_MeshList)
                    MeshRenderer mr = socket.AddComponent<MeshRenderer>() as MeshRenderer;
                    mr.material = m_SocketMaterial;

                    BoxCollider col = socket.gameObject.AddComponent<BoxCollider>() as BoxCollider;
                    Bounds bounds = new Bounds(obj.obj.transform.position, Vector3.zero);
                    bounds.Encapsulate(obj.obj.GetComponent<SkinnedMeshRenderer>().bounds);
                    //Debug.Log($"Teksts - {mesh.bounds}");
                    Vector3 localCenter = bounds.center - socket.transform.position;
                    bounds.center = localCenter;
                    col.center = bounds.center;
                    col.size = bounds.size;
                    col.isTrigger = true;

                    // PlacementPoint.cs
                    PlacementPoint pp = socket.AddComponent<PlacementPoint>() as PlacementPoint;
                    pp.m_PlaceableID = id;
                    pp.m_CheckForCorrectAngle = false;

                    SocketPair sp = new SocketPair(col, pp);
                    m_SocketPairs.Add(sp);

                    id++;
                }
            }
            setID++;
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
        foreach (CustomListClass obj in m_CorrOrder.Parts)
        {
            Rigidbody rb = obj.Bone.gameObject.AddComponent<Rigidbody>() as Rigidbody;
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            BoxCollider col = obj.Bone.gameObject.AddComponent<BoxCollider>() as BoxCollider;
            Renderer rend = GetMeshRenderer(obj.Bone.name);
            Debug.Log($"Mesh - {obj.Bone.name}");
            Bounds bounds = new Bounds(rend.transform.position, Vector3.zero);
            bounds.Encapsulate(rend.bounds);
            Vector3 localCenter = bounds.center - obj.Bone.transform.position;
            bounds.center = localCenter;

            col.center = bounds.center;
            col.size = bounds.size;

            Placeable placeable = Placeable.CreateComponentReturn(obj.Bone.gameObject, id);
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
