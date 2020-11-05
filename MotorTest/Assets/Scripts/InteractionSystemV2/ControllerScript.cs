using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerScript : MonoBehaviour
{
    public GameObject collidingObject;
    public GameObject objectinhand;
    private Placeable CurrentPickUpOBJ;
    private bool checkSocketState;
    private bool triggerPull;

    public bool TriggerPull { get => triggerPull; set => triggerPull = value; }

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
        if (collidingObject != null && collidingObject.CompareTag("PlacementRoot") && !collidingObject.CompareTag("MotorPart"))
        {
            GrabObject();
        }
    }
    public void PickUpObj()
    {
        if (collidingObject != null && collidingObject.CompareTag("MotorPart") && !collidingObject.CompareTag("PlacementRoot"))
        {
            GrabObject();
            //TriggerPull = true;
        }
    }
    public void GrabObject()
    {
        objectinhand = collidingObject;
        //collidingObject = null;
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

    void Update()
    {
        print(TriggerPull);
    }
}
