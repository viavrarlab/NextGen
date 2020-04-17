using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class DoorInteractionController : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Boolean grabAction;

    public SteamVR_Behaviour_Pose controllerPose;

    private GameObject collidingObject;
    private GameObject objectinhand;
    private Rigidbody thisRB;

    private void SetCollidingObject(Collider col)
    {
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        collidingObject = col.gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        thisRB = GetComponent<Rigidbody>();
    }

    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }
    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }
    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }
        collidingObject = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (grabAction.GetLastStateDown(handType))
        {
            if (collidingObject.tag == "Door")
            {
                //print("i collided with " + collidingObject.name);
                GrabObject();
            }
        }
        if (grabAction.GetLastStateUp(handType))
        {
            if (objectinhand)
            {
                ReleaseObject();
            }
        }
    }

    private void GrabObject()
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
    private void ReleaseObject()
    {
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());

            // FIXME: The velocity values shouldn't be negative. This is probably something to do with how the door armature is made.
            objectinhand.GetComponent<Rigidbody>().velocity = -controllerPose.GetVelocity();
            objectinhand.GetComponent<Rigidbody>().angularVelocity = -controllerPose.GetAngularVelocity();

        }
        objectinhand = null;
    }
}
