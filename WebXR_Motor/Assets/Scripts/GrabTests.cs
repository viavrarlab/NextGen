using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using HTC.UnityPlugin.Vive;
using UnityEngine.Events;
public class GrabTests : MonoBehaviour
{
    //------STEAMVR INPUTS-----
    //public SteamVR_Input_Sources handType;
    //public SteamVR_Action_Boolean teleportAction;
    //public SteamVR_Action_Boolean grabAction;
    //public SteamVR_Behaviour_Pose controllerPose;

    public HandRole m_HandRole;
    //public ControllerButton m_ControllerButton;

    private GameObject collidingObject;
    public GameObject objectinhand;
    private Moveable CurrentPickUpOBJ;
    private bool checkSocketState;

    //------ Controller inputs -----
    [Header("Trigger")]
    public ControllerButton m_TriggerButton;
    public UnityEvent OnTriggerDown = new UnityEvent();
    public UnityEvent OnTriggerUp = new UnityEvent();
    public UnityEvent OnTriggerHold = new UnityEvent();


    [Header("A Button")]
    public ControllerButton m_AButton;
    public UnityEvent OnADown = new UnityEvent();
    public UnityEvent OnAUp = new UnityEvent();
    public UnityEvent OnAHold = new UnityEvent();

    [Header("Grip")]
    public ControllerButton m_GripButton;
    public UnityEvent OnGripDown = new UnityEvent();
    public UnityEvent OnGripUp = new UnityEvent();
    public UnityEvent OnGripHold = new UnityEvent();

    [Header("Touchpad")]
    public ControllerButton m_TouchpadButton;
    public UnityEvent OnTouchpadDown = new UnityEvent();
    public UnityEvent OnTouchpadUp = new UnityEvent();


    [Header("Joystick X")]
    public ControllerAxis m_JoystickXAxis;
    public UnityEvent OnJoystickXPositive = new UnityEvent();
    public UnityEvent OnJoystickXNegative = new UnityEvent();
    private bool m_JoystickXInUse = false;

    [Header("Joystick Y")]
    public ControllerAxis m_JoystickYAxis;
    public UnityEvent OnJoystickYPositive = new UnityEvent();
    public UnityEvent OnJoystickYNegative = new UnityEvent();
    public UnityEvent OnJoystickYRelease = new UnityEvent();
    private bool m_JoystickYInUse = false;

    private VivePoseTracker m_Pose = null;

    public float m_JoystickThreshold = 0.9f;

    //----- controller input ends -----

    private void SetCollidiongObject(Transform col)
    {
        if(collidingObject|| !col.GetComponent<Rigidbody>()){
            return;
        }
        collidingObject = col.gameObject;
    }

    private void Awake()
    {
        m_Pose = GetComponent<VivePoseTracker>();
    }

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

        //if (ViveInput.GetPressDown(m_HandRole, m_ControllerButton))
        //{
        //    if (collidingObject != null && collidingObject.transform.GetChild(0).GetChild(0).tag == "MotorCollider")
        //    {
        //        if(checkSocketState == true)
        //        {
        //            collidingObject.GetComponent<Moveable>().ReleaseOldSocket();
        //            GrabObject();
        //        }
        //        print("i collided with " + collidingObject.name);
        //        GrabObject();
        //    }
        //}

        //if (ViveInput.GetPressUp(m_HandRole, m_ControllerButton))
        //{
        //    if (objectinhand)
        //    {
        //        PlaceInSocket();
        //        ReleaseObject();
        //    }
        //}

        //Editor inputs ------
        // Trigger----------------------------------------------------------------------------
        if (ViveInput.GetPressDown(m_HandRole, m_TriggerButton))
            OnTriggerDown.Invoke();

        if (ViveInput.GetPressUp(m_HandRole, m_TriggerButton))
            OnTriggerUp.Invoke();

        if (ViveInput.GetPress(m_HandRole, m_TriggerButton))
            OnTriggerHold.Invoke();

        // A Button----------------------------------------------------------------------------
        if (ViveInput.GetPressDown(m_Pose.viveRole, m_AButton))
            OnADown.Invoke();

        if (ViveInput.GetPressUp(m_Pose.viveRole, m_AButton))
            OnAUp.Invoke();
        if (ViveInput.GetPress(m_Pose.viveRole, m_AButton))
            OnAHold.Invoke();

        // Grip Button----------------------------------------------------------------------------
        if (ViveInput.GetPressDown(m_Pose.viveRole, m_GripButton))
            OnGripDown.Invoke();

        if (ViveInput.GetPressUp(m_Pose.viveRole, m_GripButton))
            OnGripUp.Invoke();
        if (ViveInput.GetPress(m_Pose.viveRole, m_GripButton))
            OnGripHold.Invoke();

        // Touchpad Button----------------------------------------------------------------------------
        if (ViveInput.GetPressDown(m_Pose.viveRole, m_TouchpadButton))
            OnTouchpadDown.Invoke();

        if (ViveInput.GetPressUp(m_Pose.viveRole, m_TouchpadButton))
            OnTouchpadUp.Invoke();

        // Joystick X AXIS----------------------------------------------------------------------------
        float valueX = ViveInput.GetAxis(m_Pose.viveRole, m_JoystickXAxis);

        if (valueX > m_JoystickThreshold && m_JoystickXInUse == false)
        {
            m_JoystickXInUse = true;
            OnJoystickXPositive.Invoke();
        }
        else if (valueX < -m_JoystickThreshold && m_JoystickXInUse == false)
        {
            m_JoystickXInUse = true;
            OnJoystickXNegative.Invoke();
        }

        if ((valueX < m_JoystickThreshold || valueX > -m_JoystickThreshold) && m_JoystickXInUse == true)
        {
            m_JoystickXInUse = false;
        }

        // Joystick Y AXIS----------------------------------------------------------------------------
        float valueY = ViveInput.GetAxis(m_Pose.viveRole, m_JoystickYAxis);
        print($"is value between m_JoystickThreshold: {IsBetween(valueY, -m_JoystickThreshold, m_JoystickThreshold)}");

        if (valueY > m_JoystickThreshold && m_JoystickYInUse == false)
        {
            m_JoystickYInUse = true;
            print("y axis positive action");
            OnJoystickYPositive.Invoke();
        }
        else if (valueY < -m_JoystickThreshold && m_JoystickYInUse == false)
        {
            m_JoystickYInUse = true;
            print("y axis negative action");
            OnJoystickYNegative.Invoke();
        }

        if ((IsBetween(valueY, -m_JoystickThreshold, m_JoystickThreshold)) && m_JoystickYInUse == true)
        {
            m_JoystickYInUse = false;
            print("y axis release action");
            OnJoystickYRelease.Invoke();
        }
    }
    public bool IsBetween(double testValue, double bound1, double bound2)
    {
        if (bound1 > bound2)
            return testValue >= bound2 && testValue <= bound1;
        return testValue >= bound1 && testValue <= bound2;
    }
    //This method is called form inspector
    public void pickUpOBJ()
    {
        if (collidingObject != null && collidingObject.transform.GetChild(0).GetChild(0).tag == "MotorCollider")
        {
            if (checkSocketState == true)
            {
                collidingObject.GetComponent<Moveable>().ReleaseOldSocket();
                GrabObject();
            }
            print("i collided with " + collidingObject.name);
            GrabObject();
        }
    }
    //This method is called from inspector
    public void releaseOBJ()
    {
        if (objectinhand)
        {
            PlaceInSocket();
            ReleaseObject();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        SetCollidiongObject(other.transform.parent.transform.parent.transform);
        if (other.CompareTag("MotorCollider"))
        {
            if (other.transform.parent.transform.parent.GetComponent<Moveable>().IsSocket == true)
            {
                checkSocketState = true;
            }
            else
            {
                checkSocketState = false;
            }
        }

    }
    public void OnTriggerStay(Collider other)
    {
        SetCollidiongObject(other.transform.parent.transform);
        print("socketstate = " + checkSocketState.ToString());
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
