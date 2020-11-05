using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using HTC.UnityPlugin.Vive;

public class InputManager : MonoBehaviour
{

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

    private void Awake()
    {
        m_Pose = GetComponent<VivePoseTracker>();
    }

    private void Update()
    {
        // Trigger----------------------------------------------------------------------------
        if (ViveInput.GetPressDown(m_Pose.viveRole, m_TriggerButton))
            OnTriggerDown.Invoke();

        if (ViveInput.GetPressUp(m_Pose.viveRole, m_TriggerButton))
            OnTriggerUp.Invoke();

        if (ViveInput.GetPress(m_Pose.viveRole, m_TriggerButton))
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

        if((valueX < m_JoystickThreshold || valueX > -m_JoystickThreshold) && m_JoystickXInUse == true)
        {
            m_JoystickXInUse = false;
        }

        // Joystick Y AXIS----------------------------------------------------------------------------
        float valueY = ViveInput.GetAxis(m_Pose.viveRole, m_JoystickYAxis);
        //print($"is value between m_JoystickThreshold: {IsBetween(valueY, -m_JoystickThreshold, m_JoystickThreshold)}");

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

        //if ((valueY < m_JoystickThreshold || valueY > -m_JoystickThreshold) && m_JoystickYInUse == true)
        //{
        //    m_JoystickYInUse = false;
        //    //print("y axis release action");
        //    //OnJoystickYRelease.Invoke();
        //}
    }

    public bool IsBetween(double testValue, double bound1, double bound2)
    {
        if (bound1 > bound2)
            return testValue >= bound2 && testValue <= bound1;
        return testValue >= bound1 && testValue <= bound2;
    }

}
