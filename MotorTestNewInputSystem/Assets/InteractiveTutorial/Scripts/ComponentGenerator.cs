using System;
using System.Collections;
using System.Collections.Generic;
using ModelOutline;
using UnityEngine;
using UnityEngine.Animations;

public class ComponentGenerator : MonoBehaviour
{
    public Transform m_TargetObject;
    public string m_InteractableTag = "MotorCollider";
    public LayerMask m_InteractableLayerMask = -1;
    public LayerMask m_SocketLayerMask = -1;
    public Material m_SocketMaterial;

    public string m_SocketRootObject_Name = "Sockets";
    public string m_SocketObject_Prefix = "Socket_";

    public bool m_InteractableRigidbodyUseGravity = false;
    public bool m_InteractableRigidbodyIsKinematic = false;

    public InteractiveTutorialController m_InteractiveTutorialController;

    public List<SocketObject> m_SocketObjects = new List<SocketObject> ();
    public List<InteractableObject> m_InteractableObjects = new List<InteractableObject> ();

    public void Generate ()
    {
        if (m_TargetObject == null)
        {
            Debug.LogError ("No socket target object specified!");
            return;
        }

        m_SocketObjects.Clear ();
        m_InteractableObjects.Clear ();

        GenerateInteractables ();
        GenerateSockets ();
        SetExpectedObjects ();
        SetUpConstraintForInteractables();
    }

    private void GenerateSockets ()
    {
        if (m_TargetObject.Find (m_SocketRootObject_Name) != null)
        {
            GameObject.DestroyImmediate (m_TargetObject.Find (m_SocketRootObject_Name).gameObject);
        }

        GameObject socketRoot = new GameObject (m_SocketRootObject_Name);
        socketRoot.transform.parent = m_TargetObject;
        socketRoot.transform.localPosition = Vector3.zero;
        socketRoot.transform.localRotation = Quaternion.identity;
        socketRoot.transform.localScale = Vector3.one;

        socketRoot.tag = "SetGrab";
        socketRoot.layer = LayerMask.NameToLayer ("Sets");

        BoxCollider rootCollider = (socketRoot.gameObject.GetComponent<BoxCollider> () != null) ? socketRoot.gameObject.GetComponent<BoxCollider> () : socketRoot.gameObject.AddComponent<BoxCollider> ();
        rootCollider.isTrigger = true;

        Rigidbody rootRB = (socketRoot.gameObject.GetComponent<Rigidbody> () != null) ? socketRoot.gameObject.GetComponent<Rigidbody> () : socketRoot.gameObject.AddComponent<Rigidbody> ();
        rootRB.useGravity = false;

        string socketLayer = LayerMask.LayerToName ((int) Mathf.Log (m_SocketLayerMask.value, 2)); // https://answers.unity.com/questions/472886/use-layer-name-for-layermask-instead-of-layer-numb.html  Answer by Vahradrim

        Bounds tempBounds = new Bounds (socketRoot.transform.position, Vector3.zero);

        foreach (Transform t in m_TargetObject)
        {
            if (t == socketRoot.transform)
            {
                continue;
            }

            GameObject socketObj = new GameObject (m_SocketObject_Prefix + t.gameObject.name);
            socketObj.transform.parent = socketRoot.transform;
            socketObj.transform.SetParent (socketRoot.transform, false);

            socketObj.transform.rotation = t.rotation;
            socketObj.transform.position = t.position;
            socketObj.transform.localScale = t.localScale;

            socketObj.layer = LayerMask.NameToLayer (socketLayer);

            BoxCollider boxCollider = socketObj.AddComponent<BoxCollider> ();

            MeshRenderer meshRendererObject = t.gameObject.GetComponent<MeshRenderer> ();

            MeshRenderer meshRendererSocket = socketObj.AddComponent<MeshRenderer> ();
            meshRendererSocket.material = m_SocketMaterial;
            meshRendererSocket.receiveShadows = false;
            meshRendererSocket.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            tempBounds.Encapsulate (meshRendererSocket.bounds);

            MeshFilter meshFilterSocket = socketObj.AddComponent<MeshFilter> ();
            meshFilterSocket.sharedMesh = t.gameObject.GetComponent<MeshFilter> ().sharedMesh;

            boxCollider.center = meshRendererObject.bounds.center - socketObj.transform.position;
            boxCollider.size = meshRendererObject.bounds.size;
            boxCollider.isTrigger = true;

            //---------------- Parent Constraint Setup
            ParentConstraint parentConstraint = socketObj.gameObject.AddComponent<ParentConstraint> ();

            ConstraintSource cs = new ConstraintSource ();
            cs.sourceTransform = socketRoot.transform;
            cs.weight = 1f;

            for (int x = parentConstraint.sourceCount - 1; x >= 0; x--)
            {
                parentConstraint.RemoveSource (x);
            }

            parentConstraint.translationAxis = Axis.None;

            parentConstraint.AddSource (cs);
            var posDelta = t.transform.localPosition - socketRoot.transform.localPosition;

            parentConstraint.SetTranslationOffset (0, posDelta);

            parentConstraint.constraintActive = true;
            parentConstraint.locked = true;
            //----------------

            SocketObject socketObjScript = socketObj.AddComponent<SocketObject> ();
            socketObjScript.m_Collider = boxCollider;

            m_SocketObjects.Add (socketObjScript);
        }

        Vector3 localCenter = tempBounds.center - socketRoot.transform.position;
        tempBounds.center = localCenter;
        // socketRoot.transform.position = tempBounds.center;
        rootCollider.size = tempBounds.size;
        rootCollider.center = tempBounds.center;
    }

    private void GenerateInteractables ()
    {

        string interactableLayer = LayerMask.LayerToName ((int) Mathf.Log (m_InteractableLayerMask.value, 2)); // https://answers.unity.com/questions/472886/use-layer-name-for-layermask-instead-of-layer-numb.html  Answer by Vahradrim

        foreach (Transform t in m_TargetObject)
        {
            if (t.gameObject.name.Contains (m_SocketRootObject_Name))
            {
                continue;
            }

            t.gameObject.tag = m_InteractableTag;
            t.gameObject.layer = LayerMask.NameToLayer (interactableLayer);

            Rigidbody rb = (t.gameObject.GetComponent<Rigidbody> () != null) ? t.gameObject.GetComponent<Rigidbody> () : t.gameObject.AddComponent<Rigidbody> ();
            rb.isKinematic = m_InteractableRigidbodyIsKinematic;
            rb.useGravity = m_InteractableRigidbodyUseGravity;

            BoxCollider boxCollider = (t.gameObject.GetComponent<BoxCollider> () != null) ? t.gameObject.GetComponent<BoxCollider> () : t.gameObject.AddComponent<BoxCollider> ();
            MeshRenderer meshRenderer = t.gameObject.GetComponent<MeshRenderer> ();

            boxCollider.center = meshRenderer.bounds.center - t.transform.position;
            boxCollider.size = meshRenderer.bounds.size * (1f / m_TargetObject.localScale.x);

            InteractableObject interactableObj = (t.gameObject.GetComponent<InteractableObject> () != null) ? t.gameObject.GetComponent<InteractableObject> () : t.gameObject.AddComponent<InteractableObject> ();

            ParentConstraint parentConstraint = (t.gameObject.GetComponent<ParentConstraint> () != null) ? t.gameObject.GetComponent<ParentConstraint> () : t.gameObject.AddComponent<ParentConstraint> ();

            Outline tOutline = t.gameObject.GetComponent<Outline> () != null ? t.gameObject.GetComponent<Outline> () : t.gameObject.AddComponent<Outline> ();
            tOutline.enabled = false;

            m_InteractableObjects.Add (interactableObj);
        }
    }

    private void SetUpConstraintForInteractables ()
    {
        foreach (InteractableObject interactable in m_InteractableObjects)
        {
            Transform interactableSocket = GetSocketTransform (interactable.gameObject.name);

            ConstraintSource cs = new ConstraintSource ();
            cs.sourceTransform = interactableSocket;
            cs.weight = 1f;
            if (interactable.TryGetComponent<ParentConstraint> (out ParentConstraint constraint))
            {
                for (int x = constraint.sourceCount - 1; x >= 0; x--)
                {
                    constraint.RemoveSource (x);
                }

                constraint.AddSource (cs);
                var posDelta = interactable.transform.localPosition - interactableSocket.localPosition;

                constraint.SetTranslationOffset (0, posDelta);

                constraint.constraintActive = false;
                constraint.locked = true;
            }
        }

    }

    private void SetExpectedObjects ()
    {
        foreach (InteractableObject intObj in m_InteractableObjects)
        {
            intObj.m_ExpectedSocket = GetSocketObject (intObj.gameObject.name);
        }

        foreach (SocketObject sObj in m_SocketObjects)
        {
            sObj.m_ExpectedObject = GetInteractableObject (sObj.gameObject.name);
        }
    }

    GameObject GetGroupObject (List<GameObject> objects, int groupID)
    {
        foreach (GameObject go in objects)
        {
            if (Int32.Parse ((go.name.Replace ("Group", ""))) == groupID)
            {
                return go;
            }
        }
        return null;
    }

    List<InteractableObject> FindAllInteractablesInGroup (List<InteractableObject> searchList, int groupID)
    {
        List<InteractableObject> output = new List<InteractableObject> ();
        foreach (var obj in searchList)
        {
            if (obj.m_MotorPartInformation.m_GroupID == groupID)
            {
                output.Add (obj);
            }
        }
        return output;
    }

    List<SocketObject> FindAllSocketsForInteractableList (List<InteractableObject> searchList)
    {
        List<SocketObject> output = new List<SocketObject> ();
        foreach (var obj in searchList)
        {
            output.Add (GetSocketObject (obj.gameObject.name));
        }
        return output;
    }

    private Transform GetSocketTransform (string objectName)
    {
        foreach (SocketObject t in m_SocketObjects)
        {
            if (t.gameObject.name == m_SocketObject_Prefix + objectName)
            {
                return t.gameObject.transform;
            }
        }
        return null;
    }
    private SocketObject GetSocketObject (string objectName)
    {
        foreach (SocketObject sObj in m_SocketObjects)
        {
            if (sObj.gameObject.name == m_SocketObject_Prefix + objectName)
            {
                return sObj;
            }
        }
        return null;
    }

    private InteractableObject GetInteractableObject (string objectName)
    {
        foreach (InteractableObject iObj in m_InteractableObjects)
        {
            if (iObj.gameObject.name == objectName.Replace (m_SocketObject_Prefix, ""))
            {
                return iObj;
            }
        }
        Debug.LogError ("Could not find InteractableObject with name: " + objectName);
        return null;
    }
}