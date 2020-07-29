using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Valve.VR;
using HTC.UnityPlugin.Vive;

public class DoorInteractionController : MonoBehaviour
{
    //public SteamVR_Input_Sources handType;
    //public SteamVR_Action_Boolean grabAction;

    //public SteamVR_Behaviour_Pose controllerPose;

    //public HandRole m_HandRole;
    //public ControllerButton m_ControllerButton;

    private GameObject collidingObject;
    private GameObject objectinhand;
    private Rigidbody thisRB;

    private void SetCollidingObject(Collider col)
    {
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        //print("colliding with:: " + collidingObject);
        if (col.tag == "Door")
        {
            collidingObject = col.gameObject;
        }
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
    //void Update()
    //{

    //    if(ViveInput.GetPressDown(m_HandRole, m_ControllerButton))
    //    {
    //        if (collidingObject != null)
    //        {
    //            GrabObject();
    //        }
    //    }

    //    if (ViveInput.GetPressUp(m_HandRole, m_ControllerButton))
    //    {
    //        if (objectinhand)
    //        {
    //            ReleaseObject();
    //        }
    //    }
    //}

    public void TryGrabDoor()
    {
        if (collidingObject != null)
        {
            GrabObject();
        }
    }

    public void ReleaseDoor()
    {
        if (objectinhand)
        {
            ReleaseObject();
        }
    }

    private void GrabObject()
    {

        objectinhand = collidingObject;

        //collidingObject = null;
        var joint = AddFixedJoint();
        joint.connectedBody = objectinhand.GetComponent<Rigidbody>();
        // objectinhand.GetComponent<DoorHinge>().GrabDoor(transform);
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
            //objectinhand.GetComponent<Rigidbody>().velocity = -controllerPose.GetVelocity();
            //objectinhand.GetComponent<Rigidbody>().angularVelocity = -controllerPose.GetAngularVelocity();

        }

        // objectinhand.GetComponent<DoorHinge>().ReleaseDoor();
        objectinhand = null;
    }
}
