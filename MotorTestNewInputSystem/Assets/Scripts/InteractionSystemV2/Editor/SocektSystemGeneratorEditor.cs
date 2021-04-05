﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Animations;
using Unity.EditorCoroutines.Editor;
using System.Linq;
using ModelOutline;

[CustomEditor(typeof(SystemGeneratorScript))]
public class SocektSystemGeneratorEditor : Editor
{
    public ConcaveCollider.EAlgorithm algorithm = ConcaveCollider.EAlgorithm.VHACD;

    public float volumeThreshold = 0.021f;

    public bool generateConcave = true;
    public bool collidersAreTriggers = true;

    public ConstraintSource source;
    [Space]

    [Range(10000, 16000000)] public int resolution = 100000; // 100,000; 10,000 - 16,000,000;  	maximum number of voxels generated during the voxelization stage
    [Range(0f, 1f)] public float concavity = 0.93f; // 0.0025; 0-1; maximum concavity
    [Range(4, 1024)] public int maxNumVerticesPerCH = 64; // 64; 4-1024; controls the maximum number of triangles per convex-hull
    [Range(0f, 1f)] public float minVolumePerCH = 0.0001f; // 0.0001; 0-1; controls the adaptive sampling of the generated convex-hulls

    private bool fin = true;

    SystemGeneratorScript m_SysGen;
    [Space(5f)]

    //public Material m_SocketMaterial;


    public List<Slot> m_AllSlots;
    //public List<GameObject> m_Sockets;

    public string m_LayerName = "Placeable";

    public Vector3 m_PostGenerationScale = Vector3.one;

    public bool m_GenerateSetBoundaryCollider = true;
    public bool m_GenerateSocketRootBoundaryCollider = true;


    public List<GameObject> m_AllSets = new List<GameObject>();

    public float currentProgress;

    private void OnEnable()
    {
        m_SysGen = (SystemGeneratorScript)target;

    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Generate Sockets"))
        {
            GenerateSockets();
        }
        if (GUILayout.Button("Add Parent Constraint + source"))
        {
            AddParentConstraint();
        }
        if (GUILayout.Button("ObjectID"))
        {
            //AddScriptValues();
            AddObjectID();
        }
        if (GUILayout.Button("Generate Colliders and grabables"))
        {
            GenerateColliders();
        }
        if (GUILayout.Button("Place Set Sockets"))
        {
            PlaceSetSocekts();
        }
        if(GUILayout.Button("Add outlines"))
        {
            AddOutlines();
        }
        if (GUILayout.Button("Clear"))
        {
            Clear();
        }
    }

    public void GenerateColliders()
    {
        EditorCoroutineUtility.StartCoroutine(Generate(), this);
    }
    public IEnumerator Generate()
    {
        foreach (Transform t in m_SysGen.transform)
        {
            if (t.gameObject.activeInHierarchy)
            {
                Mesh mesh = t.GetComponent<MeshFilter>().sharedMesh;
                float volume = VolumeOfMesh(mesh);
                t.gameObject.tag = "MotorPart";
                //print($"Mesh: {t.gameObject.name}, Volume: {volume}, Threshold: {volumeThreshold}");
                if (volume <= volumeThreshold)
                {
                    fin = false;
                    //print("Generating BOX COLLIDER.");
                    GameObject colliderParent = new GameObject("Generated Colliders");
                    colliderParent.transform.parent = t;
                    if (t.name.StartsWith("Set"))
                    {
                        Debug.Log("Setgrab tags");
                        GameObject col = new GameObject("Hull 0");
                        col.gameObject.tag = "SetGrab";
                        col.gameObject.layer = 16;
                        col.transform.parent = colliderParent.transform;
                        BoxCollider boxColComp = col.AddComponent<BoxCollider>();
                        MeshRenderer meshRenderer = t.GetComponent<MeshRenderer>();

                        boxColComp.center = meshRenderer.bounds.center;
                        boxColComp.size = meshRenderer.bounds.size;
                        boxColComp.isTrigger = collidersAreTriggers;
                    }
                    else
                    {
                        GameObject col = new GameObject("Hull 0");
                        col.gameObject.tag = "MotorCollider";
                        col.gameObject.layer = 16;
                        col.transform.parent = colliderParent.transform;
                        BoxCollider boxColComp = col.AddComponent<BoxCollider>();
                        MeshRenderer meshRenderer = t.GetComponent<MeshRenderer>();

                        boxColComp.center = meshRenderer.bounds.center;
                        boxColComp.size = meshRenderer.bounds.size;
                        boxColComp.isTrigger = collidersAreTriggers;
                    }
   
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
    public void AddScriptValues()
    {
        //Sometimes didn't chache the script
        m_SysGen.m_CorrOrder = m_SysGen.GetComponent<CorrectOrderTests>();
    }
    public void AddObjectID()
    {
        int id = 0;
        CustomListClass[] CLCarray;
        CLCarray = m_SysGen.m_CorrOrder.Parts.ToArray();
        
        for(int i = 0; i < CLCarray.Count();i++)
        {
            if (CLCarray[i].OrderNotMandatory == true)
            {
                Placeable.CreateComponentReturn(CLCarray[i].obj.gameObject, id);
                if (CLCarray[i + 1].OrderNotMandatory == false)
                {
                    id++;
                }
                if (CLCarray[i + 1].DifferentObject == true)
                {
                    id++;
                }
            }
            else
            {
                Placeable.CreateComponentReturn(CLCarray[i].obj.gameObject, id);
                id++;
            }
        }
    }
    public void AddOutlines()
    {
        foreach(Transform t in m_SysGen.transform)
        {
            if (t.gameObject.CompareTag("SetGrab") || t.gameObject.CompareTag("PlacementRoot"))
            {
                t.gameObject.AddComponent<Outline>();
                t.gameObject.GetComponent<Outline>().OutlineColor = Color.blue;
                t.gameObject.GetComponent<Outline>().enabled = false;
            }
            else
            {
                t.gameObject.AddComponent<Outline>();
                t.gameObject.GetComponent<Outline>().enabled = false;
            }
        }
    }


    public void PlaceSetSocekts()
    {
        GameObject PlacementRoot = GameObject.Find("PlacementSocket_Root");
        List<GameObject> SetList = new List<GameObject>();
        for(int i = 0; i < PlacementRoot.transform.childCount; i++)
        {
            SetList.Add(PlacementRoot.transform.GetChild(i).gameObject);
        }
        foreach(Transform t in m_SysGen.transform)
        {
            foreach(GameObject Set in SetList)
            {
                if (Set.name == t.gameObject.name && t.gameObject.activeInHierarchy) 
                {
                    Set.transform.parent = t;
                    t.gameObject.tag = "SetGrab";
                }
            }
        }
    }
    public void AddParentConstraint()
    {
        List<GameObject> TempObj = new List<GameObject>();
        List<ConstraintSource> TempSources = new List<ConstraintSource>();
        foreach (CustomListClass Obj in m_SysGen.m_CorrOrder.Parts)
        {
            if (Obj.obj.name != Obj.obj.name + "_Socket")
            {
                if (Obj.obj.gameObject.GetComponent<ParentConstraint>() == null)
                {
                    if (Obj.OrderNotMandatory == true)
                    {
                        if(Obj.DifferentObject == true)
                        {
                            TempObj.Clear();
                            TempSources.Clear();
                        }
                        TempObj.Add(Obj.obj);
                        TempObj.ToArray();
                        for (int i = 0; i < TempObj.Count; i++)
                        {
                            if (TempObj[i].gameObject.GetComponent<ParentConstraint>() == null)
                            {
                                TempObj[i].gameObject.AddComponent<ParentConstraint>();
                                GameObject ConstraintTemp = GameObject.Find(TempObj[i].gameObject.name + "_Socket");
                                source.sourceTransform = ConstraintTemp.transform;
                                source.weight = 1;
                                TempSources.Add(source);
                            }
                        }
                        for (int i = 0; i < TempObj.Count; i++)
                        {
                            TempObj[i].gameObject.GetComponent<ParentConstraint>().SetSources(TempSources);
                        }
                        Debug.Log(TempObj.Count);
                    }
                    else
                    {
                        TempObj.Clear();
                        TempSources.Clear();
                    }
                }
                if (Obj.obj.GetComponent<ParentConstraint>() == null)
                {
                    Obj.obj.gameObject.AddComponent<ParentConstraint>();
                    GameObject tempGo = GameObject.Find(Obj.obj.name + "_Socket");
                    source.sourceTransform = tempGo.transform;
                    source.weight = 1;
                    Obj.obj.gameObject.GetComponent<ParentConstraint>().AddSource(source);
                }
                if (Obj.obj.gameObject.GetComponent<Rigidbody>() == null)
                {
                    Obj.obj.gameObject.AddComponent<Rigidbody>();
                    Obj.obj.gameObject.GetComponent<Rigidbody>().useGravity = false;
                }
                else
                {
                    Obj.obj.gameObject.GetComponent<Rigidbody>().useGravity = false;
                }
            }
        }
    }
    public void GenerateSockets()
    {
        m_SysGen.m_CorrOrder = m_SysGen.GetComponent<CorrectOrderTests>();
        foreach (Transform child in m_SysGen.transform)
        {
            MeshRenderer rend = child.GetComponent<MeshRenderer>();
            if (rend != null)
            {
                m_SysGen.m_MeshList.Add(rend);
            }
        }
        // get object set count
        int maxValue = 0;
        foreach (CustomListClass obj in m_SysGen.m_CorrOrder.Parts)
        {
            if (obj.set > maxValue)
            {
                maxValue = obj.set;
            }
        }

        // create empty object for holding sockets
        GameObject socketRoot = new GameObject("PlacementSocket_Root");
        socketRoot.transform.SetParent(m_SysGen.transform, false);
        socketRoot.AddComponent<Rigidbody>();
        socketRoot.GetComponent<Rigidbody>().isKinematic = false;
        socketRoot.GetComponent<Rigidbody>().useGravity = false;
        m_SysGen.transform.gameObject.AddComponent<SetEnable>();
        //socketRoot.AddComponent<BoxCollider>();
        socketRoot.tag = "PlacementRoot";
        socketRoot.layer = 16;

        List<GameObject> setObjectRoots = new List<GameObject>();

        //Create sets
        int count = 0;
        while (count <= maxValue)
        {
            GameObject set = new GameObject("Set" + count.ToString());
            set.transform.parent = socketRoot.transform;
            set.layer = 15;
            set.gameObject.AddComponent<SetComplete>().SetID = count;

            //------Adding Mesh for Sets ---------
            //if(GameObject.Find(set.name).GetComponent<MeshRenderer>() != null)
            //{
            //    //set.transform.position = GameObject.Find(set.name).transform.position;
            //    //set.transform.rotation = GameObject.Find(set.name).transform.rotation;
            //    //MeshFilter SetMF = set.AddComponent<MeshFilter>() as MeshFilter;
            //    //SetMF.mesh = GameObject.Find(set.name).GetComponent<MeshFilter>().sharedMesh;
            //    //MeshRenderer SetMR = set.AddComponent<MeshRenderer>() as MeshRenderer;
            //    //SetMR.materials = GameObject.Find(set.name).GetComponent<MeshRenderer>().sharedMaterials;
            //    //for (int i = 0; i < GameObject.Find(set.name).GetComponent<MeshRenderer>().sharedMaterials.Length; i++)
            //    //{
            //    //    SetMR.sharedMaterials[i] = m_SysGen.m_SocketMaterial;
            //    //}
            //    //Bounds bounds = new Bounds(GameObject.Find(set.name).transform.position, Vector3.zero);
            //    //bounds.Encapsulate(GameObject.Find(set.name).GetComponent<MeshRenderer>().bounds);
            //    //Vector3 localCenter = bounds.center - set.transform.position;
            //    //bounds.center = localCenter;
            //}

            setObjectRoots.Add(set);
            m_AllSets.Add(set);
            count++;
        }
        //Convert List To array
        CustomListClass[] CLCarray;
        CLCarray = m_SysGen.m_CorrOrder.Parts.ToArray();
        // create objects for sockets and add to each set
        int id = 0;
        int setID = 0;
        while (setID <= maxValue)
        {
            for (int i = 0; i < CLCarray.Count(); i++)
            {
                if (CLCarray[i].set == setID)
                {
                    string setname = "Set" + setID.ToString();
                    Debug.Log(setname);
                    if (CLCarray[i].obj.name == setname)
                    {
                        return;
                    }
                    else
                    {
                        GameObject socket = new GameObject(CLCarray[i].obj.gameObject.name + "_Socket");
                        socket.transform.SetParent(FindSetRoot(setObjectRoots, setname), false);
                        socket.transform.position = CLCarray[i].obj.transform.position;
                        socket.transform.rotation = CLCarray[i].obj.transform.rotation;
                        // mesh filter
                        MeshFilter mf = socket.AddComponent<MeshFilter>() as MeshFilter;
                        mf.mesh = CLCarray[i].obj.GetComponent<MeshFilter>().sharedMesh;

                        // mesh renderer with same mesh as expected object (from m_MeshList)
                        MeshRenderer mr = socket.AddComponent<MeshRenderer>() as MeshRenderer;
                        if(CLCarray[i].obj.GetComponent<MeshRenderer>().sharedMaterials.Length > 1)
                        {
                            List<Material> TempList = new List<Material>();
                            foreach (Material mat in CLCarray[i].obj.GetComponent<MeshRenderer>().sharedMaterials)
                            {
                                //mr.material = m_SysGen.m_SocketMaterial;
                                TempList.Add(m_SysGen.m_SocketMaterial);
                            }
                            mr.materials = TempList.ToArray();
                        }
                        else
                        {
                            mr.material = m_SysGen.m_SocketMaterial;
                        }

                        BoxCollider col = socket.gameObject.AddComponent<BoxCollider>();
                        Bounds bounds = new Bounds(CLCarray[i].obj.transform.position, Vector3.zero);
                        bounds.Encapsulate(CLCarray[i].obj.GetComponent<MeshRenderer>().bounds);
                        //Debug.Log($"Teksts - {mesh.bounds}");
                        Vector3 localCenter = bounds.center - socket.transform.position;
                        bounds.center = localCenter;
                        col.center = bounds.center;
                        col.size = bounds.size;
                        col.isTrigger = true;

                        socket.layer = LayerMask.NameToLayer(m_LayerName);

                        PlacementPoint PP = socket.AddComponent<PlacementPoint>();
                        socket.AddComponent<OrderCheck>();
                        PP.m_CheckForCorrectAngle = true;

                        Rigidbody rigidbody = socket.AddComponent<Rigidbody>();
                        rigidbody.isKinematic = true;
                        rigidbody.useGravity = false;

                        //m_AllSlots.Add(slot);

                        socket.gameObject.tag = "Socket";
                        socket.gameObject.layer = LayerMask.NameToLayer(m_LayerName);

                        SocketPair sp = new SocketPair(col, PP);
                        m_SysGen.m_SocketPairs.Add(sp);
                        if (CLCarray[i].OrderNotMandatory == true)
                        {
                            PP.m_PlaceableID = id;
                            if (CLCarray[i + 1].OrderNotMandatory == false)
                            {
                                id++;
                            }
                            if (CLCarray[i + 1].DifferentObject == true)
                            {
                                id++;
                            }
                        }
                        else
                        {
                            PP.m_PlaceableID = id;
                            id++;
                        }
                    }
                }
            }
            setID++;
        }
        //if (m_GenerateSetBoundaryCollider)
        //{
        //    foreach (GameObject set in m_AllSets)
        //    {
        //        //CalculateSetBounds(set);
        //    }
        //}

        if (m_GenerateSocketRootBoundaryCollider)
        {
            CalculateRootBounds(socketRoot.transform);
        }

        foreach (SocketPair sp in m_SysGen.m_SocketPairs)
        {
            foreach (SocketPair checkPair in m_SysGen.m_SocketPairs)
            {
                if (sp != checkPair)
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

    public void CalculateRootBounds(Transform rootObj)
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        List<MeshFilter> mr = new List<MeshFilter>();

        foreach (Transform t1 in rootObj)
        {
            foreach (Transform t2 in t1)
            {
                if (t2.GetComponent<MeshFilter>() == null)
                {
                    continue;
                }
                mr.Add(t2.GetComponent<MeshFilter>());
            }
        }

        foreach (MeshFilter rend in mr)
        {
            bounds.Encapsulate(rend.sharedMesh.bounds);
        }

        Vector3 newCenter = new Vector3(bounds.center.x, bounds.size.y / 2f, bounds.center.z);
        BoxCollider collider = rootObj.gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.center = newCenter;
        collider.size = bounds.size;
    }
    public void CalculateSetBounds(GameObject set)
    {
        Bounds allObjectBounds = new Bounds(Vector3.zero, Vector3.zero);
        Vector3 allPositions = Vector3.zero;
        int childCount = 0;

        foreach (Transform t in set.transform)
        {
            Bounds childRendererBounds = t.GetComponent<MeshFilter>().sharedMesh.bounds;

            allObjectBounds.Encapsulate(childRendererBounds);
            allPositions += t.localPosition;
            childCount++;
        }

        Vector3 averagedPosition = new Vector3(allPositions.x / childCount, allPositions.y / childCount, allPositions.z / childCount);
        Vector3 localCenter = allObjectBounds.center - averagedPosition;
        allObjectBounds.center = localCenter;

        BoxCollider collider = set.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.center = averagedPosition;
        collider.size = allObjectBounds.size;
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
    private IEnumerator ColliderTask(Transform t)
    {
        fin = false;

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


        //colliderGen.ComputeHulls(new ConcaveCollider.LogDelegate(Message), new ConcaveCollider.ProgressDelegate(Progress));

        yield return null;

        fin = true;
    }


    public void Clear()
    {
        foreach (Transform t in m_SysGen.transform)
        {
            if (t.name == "PlacementSocket_Root")
            {
                DestroyImmediate(t.gameObject);
                return;
            }
            if(t.childCount >=1)
            {
                for(int i = 0; i < t.childCount; i++)
                {
                    DestroyImmediate(t.GetChild(i).gameObject);
                }
            }
            if (t.tag != "Untagged")
            {
                t.tag = "Untagged";
            }
            if (t.gameObject.layer != 0)
            {
                t.gameObject.layer = 0;
            }
            if (t.gameObject.GetComponent<ConcaveCollider>() == null)
            {
                t.gameObject.AddComponent<ConcaveCollider>();
            }
            if (t.gameObject.GetComponent<ParentConstraint>())
            {
                var Componenet = t.gameObject.GetComponent<ParentConstraint>();
                DestroyImmediate(Componenet);
            }
            if (t.gameObject.GetComponent<Outline>())
            {
                var Outline = t.gameObject.GetComponent<Outline>();
                DestroyImmediate(Outline);
            }
            if (t.gameObject.GetComponent<Placeable>())
            {
                var plDestroy = t.gameObject.GetComponent<Placeable>();
                DestroyImmediate(plDestroy);
            }
            if (t.gameObject.GetComponent<Rigidbody>())
            {
                var RBDestroy = t.gameObject.GetComponent<Rigidbody>();
                DestroyImmediate(RBDestroy);
            }

            ConcaveCollider colliderGen = t.gameObject.GetComponent<ConcaveCollider>();

            colliderGen.DestroyHulls();
        }
        if(m_SysGen.gameObject.GetComponent<SetEnable>() != null)
        {
            var SetEnb = m_SysGen.gameObject.GetComponent<SetEnable>();
            DestroyImmediate(SetEnb);
        }
        if (m_SysGen.m_MeshList.Count > 0) m_SysGen.m_MeshList.Clear();
        if (m_SysGen.m_SocketPairs.Count > 0) m_SysGen.m_SocketPairs.Clear();
        if (m_AllSlots.Count > 0)
        {
            m_AllSlots.Clear();
        }
        if (m_AllSets.Count > 0)
        {
            m_AllSets.Clear();
        }
        m_SysGen.transform.localScale = Vector3.one;
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

    //void Progress(string message, float fPercent)
    //{
    //    currentProgress = fPercent;
    //    //Debug.Log($"Current progress: {currentProgress}");
    //}

    //void Message(string message)
    //{
    //    //Debug.Log(message);
    //}
}

