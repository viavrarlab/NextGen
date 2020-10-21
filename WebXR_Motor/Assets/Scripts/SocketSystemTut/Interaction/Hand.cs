using System.Collections.Generic;
using UnityEngine;
// using Valve.VR;
using HTC.UnityPlugin.Vive;

public class Hand : MonoBehaviour
{
    private Socket m_Socket = null;
    private VivePoseTracker m_Pose = null;

    public List<Interactable> m_ContactInteractables = new List<Interactable>();

    private void Awake()
    {
        m_Socket = GetComponent<Socket>();
        m_Pose = GetComponent<VivePoseTracker>();
    }

    private void OnTriggerEnter(Collider other)
    {
        AddInteractable(other.gameObject);
    }

    private void AddInteractable(GameObject newObject)
    {
        Interactable newInteractable;
        if (newObject.tag == "MotorCollider")
        {
            newInteractable = newObject.transform.parent.transform.parent.gameObject.GetComponent<Interactable>();
        }
        else
        {
            newInteractable = newObject.GetComponent<Interactable>();
        }
        m_ContactInteractables.Add(newInteractable);
    }

    private void OnTriggerExit(Collider other)
    {
        RemoveInteractable(other.gameObject);
    }

    private void RemoveInteractable(GameObject newObject)
    {
        //Interactable existingInteractable = newObject.GetComponent<Interactable>();
        Interactable existingInteractable;
        if (newObject.tag == "MotorCollider")
        {
            existingInteractable = newObject.transform.parent.transform.parent.gameObject.GetComponent<Interactable>();
        }
        else
        {
            existingInteractable = newObject.GetComponent<Interactable>();
        }
        m_ContactInteractables.Remove(existingInteractable);
    }

    public void TryInteraction()
    {
        if (NearestInteraction())
        {
            return;
        }
        HeldInteraction();
    }
    public void PlaceIneraction()
    {
        if (checkForSocket())
        {
            return;
        }
        else
        {
            StopInteraction();
        }

    }
    private bool checkForSocket()
    {
        Interactable nearestobj = Utility.GetNearestInteractable(transform.position, m_ContactInteractables);
        if(nearestobj)
        {
            nearestobj.StartInteraction(this);
        }
        return nearestobj;
    }

    private bool NearestInteraction()
    {
        //m_ContactInteractables.Remove(m_Socket.GetStoredObject());
        Interactable nearestObject = Utility.GetNearestInteractable(transform.position, m_ContactInteractables);

        if (nearestObject)
        {
            nearestObject.StartInteraction(this);
        }

        return nearestObject;
    }

    private void HeldInteraction()
    {
        //if (!HasHeldObject())
        //{
        //    return;
        //}

        Moveable heldObject = m_Socket.GetStoredObject();
        if(heldObject)
            heldObject.Interaction(this);
    }

    public void StopInteraction()
    {
        //if (!HasHeldObject())
        //{
        //    return;
        //}

        Moveable heldObject = m_Socket.GetStoredObject();
        if(heldObject)
            heldObject.EndInteraction(this);
    }

    //public void Pickup(Moveable moveable)
    //{
    //    moveable.AttachNewSocket(m_Socket);
    //}

    //public Moveable Drop()
    //{
    //    if (!HasHeldObject())
    //    {
    //        return null;
    //    }

    //    Moveable detachedObject = m_Socket.GetStoredObject();
    //    detachedObject.ReleaseOldSocket();

    //    //Rigidbody rigidbody = detachedObject.gameObject.GetComponent<Rigidbody>();
    //    //rigidbody.velocity = m_Pose.GetVelocity();

    //    return detachedObject;
    //}

    //public bool HasHeldObject()
    //{
    //    return m_Socket.GetStoredObject();
    //}

    public Socket GetSocket()
    {
        return m_Socket;
    }

    public VivePoseTracker GetPose()
    {
        return m_Pose;
    }
}