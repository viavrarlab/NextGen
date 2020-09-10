using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.EditorCoroutines.Editor;
using System.Linq;

public class SocketGenerator : MonoBehaviour
{
    public ConcaveCollider.EAlgorithm algorithm = ConcaveCollider.EAlgorithm.VHACD;

    public float volumeThreshold = 0.021f;

    public bool generateConcave = true;
    public bool collidersAreTriggers = true;

    [Space]

    [Range(10000, 16000000)] public int resolution = 100000; // 100,000; 10,000 - 16,000,000;  	maximum number of voxels generated during the voxelization stage
    [Range(0f, 1f)] public float concavity = 0.0025f; // 0.0025; 0-1; maximum concavity
    [Range(4, 1024)] public int maxNumVerticesPerCH = 64; // 64; 4-1024; controls the maximum number of triangles per convex-hull
    [Range(0f, 1f)] public float minVolumePerCH = 0.0001f; // 0.0001; 0-1; controls the adaptive sampling of the generated convex-hulls




    private bool fin = true;

    private float currentProgress = 0f;


    [Space(5f)]
    CorrectOrderTests m_CorrOrder;
    public Material m_SocketMaterial;

    public List<MeshRenderer> m_MeshList;
    public List<Slot> m_AllSlots;
    //public List<GameObject> m_Sockets;


    public List<SocketPair> m_SocketPairs;
    public string m_LayerName = "Placeable";
    public string m_TagName = "Grab";

    public Vector3 m_PostGenerationScale = Vector3.one;

    public void GenerateColliders()
    {
        EditorCoroutineUtility.StartCoroutine(Generate(), this);
    }

    public IEnumerator Generate()
    {
        foreach (Transform t in transform)
        {
            if (t.gameObject.activeInHierarchy)
            {
                Mesh mesh = t.GetComponent<MeshFilter>().sharedMesh;
                float volume = VolumeOfMesh(mesh);
                //print($"Mesh: {t.gameObject.name}, Volume: {volume}, Threshold: {volumeThreshold}");
                if (volume <= volumeThreshold)
                {
                    fin = false;
                    //print("Generating BOX COLLIDER.");

                    GameObject colliderParent = new GameObject("Generated Colliders");
                    colliderParent.transform.parent = t;
                    GameObject col = new GameObject("Hull 0");
                    col.transform.parent = colliderParent.transform;

                    BoxCollider boxColComp = col.AddComponent<BoxCollider>();
                    MeshRenderer meshRenderer = t.GetComponent<MeshRenderer>();

                    boxColComp.center = meshRenderer.bounds.center;
                    boxColComp.size = meshRenderer.bounds.size;
                    boxColComp.isTrigger = collidersAreTriggers;

                    yield return null;
                    fin = true;
                }
                else
                {
                    if (generateConcave)
                    {
                        EditorCoroutineUtility.StartCoroutine(ColliderTask(t), this);
                    }
                }
            }

            while (fin == false)
            {
                yield return null;
            }
        }
    }

    private IEnumerator ColliderTask(Transform t)
    {
        currentProgress = 0f;
        fin = false;
        //print("Generating CONCAVE COLLIDER.");

        if (t.gameObject.GetComponent<ConcaveCollider>() == null)
        {
            t.gameObject.AddComponent<ConcaveCollider>();
        }

        ConcaveCollider colliderGen = t.gameObject.GetComponent<ConcaveCollider>();

        colliderGen.Algorithm = algorithm;

        colliderGen.VHACD_Concavity = concavity;
        colliderGen.VHACD_MaxVerticesPerCH = maxNumVerticesPerCH;
        colliderGen.VHACD_MinVolumePerCH = minVolumePerCH;
        colliderGen.VHACD_NormalizeMesh = false;
        colliderGen.VHACD_NumVoxels = resolution;
        colliderGen.CreateMeshAssets = false;

        colliderGen.IsTrigger = collidersAreTriggers;


        colliderGen.ComputeHulls(new ConcaveCollider.LogDelegate(Message), new ConcaveCollider.ProgressDelegate(Progress));


        //while (currentProgress < 100f){
        //    yield return null;
        //}
        yield return null;
        //yield return new EditorWaitForSeconds(5f);
        fin = true;
    }


    public void Clear()
    {
        foreach (Transform t in transform)
        {
            if (t.name == "PlacementSocket_Root")
            {
                DestroyImmediate(t.gameObject);
                return;
            }
            if (t.gameObject.GetComponent<ConcaveCollider>() == null)
            {
                t.gameObject.AddComponent<ConcaveCollider>();
            }

            ConcaveCollider colliderGen = t.gameObject.GetComponent<ConcaveCollider>();

            colliderGen.DestroyHulls();
        }
        if (m_MeshList.Count > 0) m_MeshList.Clear();
        if (m_SocketPairs.Count > 0) m_SocketPairs.Clear();
        if (m_AllSlots.Count > 0) m_AllSlots.Clear();

        transform.localScale = Vector3.one;
    }

    public void SetPostGenerationOptions()
    {
        transform.localScale = m_PostGenerationScale;
    }

    void Progress(string message, float fPercent)
    {
        currentProgress = fPercent;
        //Debug.Log($"Current progress: {currentProgress}");
    }

    void Message(string message)
    {
        //Debug.Log(message);
    }

    public void GenerateGrabbables()
    {
        int id = 0;
        foreach (CustomListClass t in m_CorrOrder.Parts)
        {

            if(t.obj.name == "PlacementSocket_Root")
            {
                continue;
            }

            if (t.obj.gameObject.GetComponent<Rigidbody>() == null)
            {
                t.obj.gameObject.AddComponent<Rigidbody>();
            }


            Rigidbody rb = t.obj.gameObject.GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = false;
            //rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            //if (t.gameObject.GetComponent<Placeable>() == null)
            //{
            //    Placeable placeable = Placeable.CreateComponentReturn(t.gameObject, id);
            //}
            if (t.obj.gameObject.GetComponent<Moveable>() == null)
            {
                Moveable moveable = t.obj.gameObject.AddComponent<Moveable>();
            }

            Moveable m = t.obj.gameObject.GetComponent<Moveable>();
            m.m_ID = id;

            //t.gameObject.tag = m_TagName;
            t.obj.gameObject.layer = LayerMask.NameToLayer(m_LayerName);

            Transform[] childrenHullObjects = FindChildren(t.obj.transform, "Hull");
            foreach (Transform child in childrenHullObjects)
            {
                child.gameObject.tag = m_TagName;
                child.gameObject.layer = LayerMask.NameToLayer(m_LayerName);

                //if (child.gameObject.GetComponent<Moveable>() == null)
                //{
                //    Moveable moveable = child.gameObject.AddComponent<Moveable>();
                //}
            }

            id++;
        }
    }

    public Transform[] FindChildren(Transform transform, string name)
    {
        return transform.GetComponentsInChildren<Transform>().Where(t => t.name.Contains(name)).ToArray();
    }

    float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float v321 = p3.x * p2.y * p1.z;
        float v231 = p2.x * p3.y * p1.z;
        float v312 = p3.x * p1.y * p2.z;
        float v132 = p1.x * p3.y * p2.z;
        float v213 = p2.x * p1.y * p3.z;
        float v123 = p1.x * p2.y * p3.z;
        return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
    }

    float VolumeOfMesh(Mesh mesh)
    {
        float volume = 0;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i + 0]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];
            volume += SignedVolumeOfTriangle(p1, p2, p3);
        }
        return Mathf.Abs(volume);
    }


    public void GenerateSockets()
    {
        m_CorrOrder = GetComponent<CorrectOrderTests>();
        foreach (Transform child in this.transform)
        {
            MeshRenderer rend = child.GetComponent<MeshRenderer>();
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
        socketRoot.transform.SetParent(transform, false);
        socketRoot.AddComponent<BoxCollider>();
        socketRoot.tag = "PlacementRoot";

        List<GameObject> setObjectRoots = new List<GameObject>();

        //Create sets
        int count = 0;
        while (count <= maxValue)
        {
            GameObject set = new GameObject("set" + count.ToString());
            set.transform.parent = socketRoot.transform;
            set.gameObject.AddComponent<SetComplete>().SetID = count;
            setObjectRoots.Add(set);
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
                    socket.transform.SetParent(FindSetRoot(setObjectRoots, setname), false);
                    socket.transform.position = obj.obj.transform.position;
                    socket.transform.rotation = obj.obj.transform.rotation;
                    // mesh filter
                    MeshFilter mf = socket.AddComponent<MeshFilter>() as MeshFilter;
                    mf.mesh = obj.obj.GetComponent<MeshFilter>().sharedMesh;

                    // mesh renderer with same mesh as expected object (from m_MeshList)
                    MeshRenderer mr = socket.AddComponent<MeshRenderer>() as MeshRenderer;
                    mr.material = m_SocketMaterial;

                    BoxCollider col = socket.gameObject.AddComponent<BoxCollider>();
                    Bounds bounds = new Bounds(obj.obj.transform.position, Vector3.zero);
                    bounds.Encapsulate(obj.obj.GetComponent<MeshRenderer>().bounds);
                    //Debug.Log($"Teksts - {mesh.bounds}");
                    Vector3 localCenter = bounds.center - socket.transform.position;
                    bounds.center = localCenter;
                    col.center = bounds.center;
                    col.size = bounds.size;
                    col.isTrigger = true;

                    // PlacementPoint.cs
                    //PlacementPoint pp = socket.AddComponent<PlacementPoint>();
                    //pp.m_PlaceableID = id;
                    //pp.m_CheckForCorrectAngle = false;

                    socket.layer = LayerMask.NameToLayer(m_LayerName);

                    Socket socketScript = socket.AddComponent<Socket>();
                                        
                    Rigidbody rigidbody = socket.AddComponent<Rigidbody>();
                    rigidbody.isKinematic = true;
                    rigidbody.useGravity = false;

                    FixedJoint joint = socket.AddComponent<FixedJoint>();

                    Slot slot = socket.AddComponent<Slot>();
                    slot.m_PlaceableID = id;
                    slot.m_CheckForCorrectAngle = false;

                    //SocketPair sp = new SocketPair(col, pp);
                    //print($"socket pair. col: {sp.m_Collider}, point: {sp.m_PlacementPoint}");
                    m_AllSlots.Add(slot);
                    //m_SocketPairs.Add(sp);

                    socket.gameObject.tag = "Socket";
                    socket.gameObject.layer = LayerMask.NameToLayer(m_LayerName);

                    id++;
                }
            }
            setID++;
        }
        foreach (Slot slot in m_AllSlots)
        {
            foreach (Slot slotOther in m_AllSlots)
            {
                if (slot != slotOther)
                {
                    bool doesIntersect = slot.gameObject.GetComponent<BoxCollider>().bounds.Intersects(slotOther.gameObject.GetComponent<BoxCollider>().bounds);
                    if (doesIntersect)
                    {
                        slot.m_IntersectingSlots.Add(slotOther);
                        //print($"{sp.m_Collider.name} intersects with {checkPair.m_Collider.name}");
                    }
                }
            }
        }
    }

    Transform FindSetRoot(List<GameObject> allSetRootObjects, string setName)
    {
        foreach (GameObject setObj in allSetRootObjects)
        {
            if (setObj.name == setName)
            {
                return setObj.transform;
            }
        }
        return null;
    }
}
