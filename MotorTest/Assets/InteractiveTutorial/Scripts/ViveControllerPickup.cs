using ModelOutline;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ViveControllerPickup : MonoBehaviour
{
    //public GameObject collidingObjectToBePickedUp;
    //private GameObject objectInHand;

    public GameObject currentActiveObject;

    //public List<GameObject> CollidingObj = new List<GameObject>();

    public InteractableObject m_Interactable;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Socket"))
        {
            Physics.IgnoreCollision(this.gameObject.GetComponent<Collider>(), other);
        }

        if (other.CompareTag("MotorCollider"))
        {
            if (currentActiveObject == null)
            {
                currentActiveObject = other.gameObject;
                other.gameObject.GetComponent<Outline>().enabled = true;
                
            }
        }
        
    }

    void OnTriggerExit(Collider other)
    {

        if (other.gameObject.GetComponent<Outline>() != null)
        {
            other.gameObject.GetComponent<Outline>().enabled = false;
        }
        currentActiveObject = null;

    }

    public void PickUpObj()
    {
        if (currentActiveObject != null)
        {
            GrabObject();
        }
    }
    public void GrabObject()
    {
        m_Interactable = currentActiveObject.gameObject.GetComponent<InteractableObject>();
        FixedJoint joint = gameObject.GetComponent<FixedJoint>();
        joint.connectedBody = currentActiveObject.GetComponent<Rigidbody>();
    }

    public void ReleaseObject()
    {
        FixedJoint fj = gameObject.GetComponent<FixedJoint>();
        if (fj)
        {
            fj.connectedBody = null;
        }
        currentActiveObject = null;

        if (m_Interactable != null && m_Interactable.m_IsCurrentlyIntersecting == true) // If we have a valid interactable object
        {
            m_Interactable.PutIntoSocket(); // Call the PutIntoSocket method to put the object in its socket
            ClearInteractableObject(); // Since we have already done everything with the interactable, we don't need it anymore and so that it doesn't cause any problems later, clear the variable
        }
    }

    public void ClearInteractableObject()
    {
        m_Interactable = null;
    }

}
