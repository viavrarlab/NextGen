using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPickUp : MonoBehaviour
{
    private GameObject collidingObject;
    private GameObject objectinhand;
    private GameObject ThrownObject;
     
    private void SetCollidiongObject(Collider col)
    {
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        collidingObject = col.gameObject;
    }
    public void OnTriggerEnter(Collider other)
    {
        print("i collided with " + other.gameObject.name);
        if (other.CompareTag("PlacementRoot"))
        {
            SetCollidiongObject(other);         
        }
    }
    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PlacementRoot"))
        {
            SetCollidiongObject(other);
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
    public void SetGrab()
    {
        objectinhand = collidingObject;
        //collidingObject = null;
        var joint = AddFixedJoint();
        joint.connectedBody = objectinhand.GetComponent<Rigidbody>();
    }
    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }
    public void ReleaseObject()
    {
        FixedJoint fj = GetComponent<FixedJoint>();
        if (fj)
        {
            fj.connectedBody = null;
            Destroy(fj);
            //objectinhand.GetComponent<Rigidbody>().velocity = controllerPose.GetVelocity();
            //objectinhand.GetComponent<Rigidbody>().angularVelocity = controllerPose.GetAngularVelocity();

        }
        objectinhand = null;
    }
}
