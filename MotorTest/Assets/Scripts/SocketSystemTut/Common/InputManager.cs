using UnityEngine;
using UnityEngine.Events;
// using Valve.VR;
using HTC.UnityPlugin.Vive;

public class InputManager : MonoBehaviour
{

    [Header("Trigger")]
    public ControllerButton m_TriggerButton;
    public UnityEvent OnTriggerDown = new UnityEvent();
    public UnityEvent OnTriggerUp = new UnityEvent();
    

    [Header("A Button")]
    public ControllerButton m_AButton;
    public UnityEvent OnADown = new UnityEvent();
    public UnityEvent OnAUp = new UnityEvent();

    [Header("Grip")]
    public ControllerButton m_GripButton;
    public UnityEvent OnGripDown = new UnityEvent();
    public UnityEvent OnGripUp = new UnityEvent();

    [Header("Touchpad")]
    public ControllerButton m_TouchpadButton;
    public UnityEvent OnTouchpadDown = new UnityEvent();
    public UnityEvent OnTouchpadUp = new UnityEvent();

    [Header("Joystick X")]
    public ControllerAxis m_JoystickXAxis;
    public UnityEvent OnJoystickXPositive = new UnityEvent();
    public UnityEvent OnJoystickXNegative = new UnityEvent();
    public float m_JoystickThreshold = 0.9f;
    private bool m_JoystickXInUse = false;

    private VivePoseTracker m_Pose = null;

    private void Awake()
    {
        m_Pose = GetComponent<VivePoseTracker>();
    }

    private void Update()
    {
        // Trigger
        if(ViveInput.GetPressDown(m_Pose.viveRole, m_TriggerButton))
            OnTriggerDown.Invoke();

        if (ViveInput.GetPressUp(m_Pose.viveRole, m_TriggerButton))
            OnTriggerUp.Invoke();

        // A Button
        if (ViveInput.GetPressDown(m_Pose.viveRole, m_AButton))
            OnADown.Invoke();

        if (ViveInput.GetPressUp(m_Pose.viveRole, m_AButton))
            OnAUp.Invoke();

        // Grip Button
        if (ViveInput.GetPressDown(m_Pose.viveRole, m_GripButton))
            OnGripDown.Invoke();

        if (ViveInput.GetPressUp(m_Pose.viveRole, m_GripButton))
            OnGripUp.Invoke();

        // Touchpad Button
        if (ViveInput.GetPressDown(m_Pose.viveRole, m_TouchpadButton))
            OnTouchpadDown.Invoke();

        if (ViveInput.GetPressUp(m_Pose.viveRole, m_TouchpadButton))
            OnTouchpadUp.Invoke();


       
        float value = ViveInput.GetAxis(m_Pose.viveRole, m_JoystickXAxis);

        if (value > m_JoystickThreshold && m_JoystickXInUse == false)
        {
            m_JoystickXInUse = true;
            OnJoystickXPositive.Invoke();
        }
        else if (value < -m_JoystickThreshold && m_JoystickXInUse == false)
        {
            m_JoystickXInUse = true;
            OnJoystickXNegative.Invoke();
        }

        if((value < m_JoystickThreshold || value > -m_JoystickThreshold) && m_JoystickXInUse == true)
        {
            m_JoystickXInUse = false;
        }

    }

}
