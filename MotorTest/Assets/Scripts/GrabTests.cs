using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using HTC.UnityPlugin.Vive;
public class GrabTests : MonoBehaviour
{
    //public SteamVR_Input_Sources handType;
    //public SteamVR_Action_Boolean teleportAction;
    //public SteamVR_Action_Boolean grabAction;

    //public SteamVR_Behaviour_Pose controllerPose;

    public HandRole m_HandRole;
    public ControllerButton m_ControllerButton;

    private GameObject collidingObject;
    public GameObject objectinhand;
    private Moveable CurrentPickUpOBJ;

    private void SetCollidiongObject(Transform col)
    {
        if(collidingObject|| !col.GetComponent<Rigidbody>()){
            return;
        }
        collidingObject = col.gameObject;
    }
    // Update is called once per frame
    void Update()
    {
        //if (GetTeleportDown())
        //{
        //    print("Teleport" + handType);
        //}
        //if (GetGrab())
        //{
        //    print("Grab" + handType);
        //}

        //if (grabAction.GetLastStateDown(handType))
        //{
        //    if (collidingObject.tag == "Grab")
        //    {
        //        print("i collided with "+collidingObject.name);
        //        GrabObject();
        //    }
        //}
        //if (grabAction.GetLastStateUp(handType))
        //{
        //    if (objectinhand)
        //    {
        //        ReleaseObject();
        //    }
        //}

        if (ViveInput.GetPressDown(m_HandRole, m_ControllerButton))
        {
            if (collidingObject != null && collidingObject.transform.GetChild(0).GetChild(0).tag == "MotorCollider")
            {
                print("i collided with " + collidingObject.name);
                GrabObject();
            }
        }

        if (ViveInput.GetPressUp(m_HandRole, m_ControllerButton))
        {
            if (objectinhand)
            {
                PlaceInSocket();
                ReleaseObject();
            }
        }
    }
    //public bool GetTeleportDown()
    //{
    //    return teleportAction.GetStateDown(handType);
    //}
    //public bool GetGrab()
    //{
    //    return grabAction.GetState(handType);
    //}
    public void OnTriggerEnter(Collider other)
    {
        SetCollidiongObject(other.transform.parent.transform.parent.transform);
    }
    public void OnTriggerStay(Collider other)
    {
        SetCollidiongObject(other.transform.parent.transform);
    }
    public void OnTriggerExit(Collider other)
    {   
        if(!collidingObject){
            return;
        }
        collidingObject = null;
    }
    private void GrabObject()
    {
        objectinhand = collidingObject;
        //collidingObject = null;
        FixedJoint joint = GetComponent<FixedJoint>();
        joint.connectedBody = objectinhand.GetComponent<Rigidbody>();
        CurrentPickUpOBJ = objectinhand.GetComponent<Moveable>();
    }
    //Checks for values coming form movable script and uses attachnewsocket method form movable script
    private void PlaceInSocket()
    {
        print(CurrentPickUpOBJ.IsSocket.ToString());
        if(CurrentPickUpOBJ.IsSocket == true)
        {
            CurrentPickUpOBJ.AttachNewSocket(CurrentPickUpOBJ.CurrentCollidingSocket);
        }
    }

    private void ReleaseObject()
    {
        FixedJoint fj = GetComponent<FixedJoint>();
        if (fj)
        {
            fj.connectedBody = null;
        }
        objectinhand = null;
        CurrentPickUpOBJ = null;
    }


}
