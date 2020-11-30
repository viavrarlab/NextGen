using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerScript : MonoBehaviour
{
    public GameObject collidingObject;
    public GameObject objectinhand;
    private Placeable CurrentPickUpOBJ;
    private bool m_triggerPull;
    private bool m_GrabPush;


    public bool TriggerPull { get => m_triggerPull; set => m_triggerPull = value; }
    public bool GrabPush { get => m_GrabPush; set => m_GrabPush = value; }

    private void SetCollidiongObject(Transform col)
    {
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        collidingObject = col.gameObject;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Socket"))
        {
            Physics.IgnoreCollision(this.gameObject.GetComponent<Collider>(), other);
        }
        if (other.CompareTag("MotorCollider"))
        {
            SetCollidiongObject(other.transform.parent.transform.parent.transform);
        }
        if (other.CompareTag("PlacementRoot"))
        {
            SetCollidiongObject(other.transform);
        }

    }
    public void OnTriggerStay(Collider other)
    {

    }
    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }
        collidingObject = null;
    }
    public void PickUPSet()
    {
        if (collidingObject != null)
        {
            if (!collidingObject.CompareTag("MotorPart"))
            {
                if (collidingObject.CompareTag("SetGrab") || !collidingObject.CompareTag("PlacementRoot"))
                {
                    GrabObject();
                }
            }
        }
    }
    public void PickUpObj()
    {
        if (collidingObject != null)
        {
            if (!collidingObject.CompareTag("PlacementRoot") || !collidingObject.CompareTag("SetGrab"))
            {
                if (collidingObject.CompareTag("MotorPart"))
                {
                    GrabObject();
                }
            }
        }
    }
    public void GrabObject()
    {
        objectinhand = collidingObject;
        FixedJoint joint = GetComponent<FixedJoint>();
        joint.connectedBody = objectinhand.GetComponent<Rigidbody>();
        CurrentPickUpOBJ = objectinhand.GetComponent<Placeable>();
    }
    public void ReleaseObject()
    {
        FixedJoint fj = GetComponent<FixedJoint>();
        if (fj)
        {
            fj.connectedBody = null;
        }
        objectinhand = null;
        CurrentPickUpOBJ = null;
        TriggerPull = false;
    }
}
