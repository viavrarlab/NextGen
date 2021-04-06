using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using System.Collections.Generic;
using NaughtyAttributes;

//Input for unity built in xr manager
public class InputManager : MonoBehaviour
{
    public InputDeviceCharacteristics deviceRole;

    [SerializeField]
    public List<InputDevice> inputDevices = new List<InputDevice>();

    [Header("Trigger")]
    [Foldout("Trigger Inputs")]
    public UnityEvent OnTriggerDown = new UnityEvent();
    [Foldout("Trigger Inputs")]
    public UnityEvent OnTriggerUp = new UnityEvent();
    [Foldout("Trigger Inputs")]
    public UnityEvent OnTriggerHold = new UnityEvent();
    bool m_TriggerLastStateDown;
    bool m_TriggerLastStateUp;


    [Header("Primary Button Inputs")]
    [Foldout("Primary Button Inputs")]
    public UnityEvent OnADown = new UnityEvent();
    [Foldout("Primary Button Inputs")]
    public UnityEvent OnAUp = new UnityEvent();
    [Foldout("Primary Button Inputs")]
    public UnityEvent OnAHold = new UnityEvent();
    bool m_PrimaryButtonLastStateDown;
    bool m_PrimaryButtonLastStateUp;

    [Header("Secondary Button Inputs")]
    [Foldout("Secondary Button Inputs")]
    public UnityEvent OnBDown = new UnityEvent();
    [Foldout("Secondary Button Inputs")]
    public UnityEvent OnBUp = new UnityEvent();
    [Foldout("Secondary Button Inputs")]
    public UnityEvent OnBHold = new UnityEvent();
    bool m_SecondaryButtonLastStateDown;
    bool m_SecondaryButtonLastStateUp;

    [Header("Grip")]
    [Foldout("Grip Button Inputs")]
    public UnityEvent OnGripDown = new UnityEvent();
    [Foldout("Grip Button Inputs")]
    public UnityEvent OnGripUp = new UnityEvent();
    [Foldout("Grip Button Inputs")]
    public UnityEvent OnGripHold = new UnityEvent();
    bool m_GripButtonLastStateDown;
    bool m_GripButtonLastStateUp;

    [Header("JoyStick")]
    [Foldout("Joystick Inputs")]
    [SerializeField]
    float JoyStickXMinThreshold = .1f;
    [Foldout("Joystick Inputs")]
    [SerializeField]
    float JoyStickYMinThreshold = .1f;
    [Foldout("Joystick Inputs")]
    [SerializeField]
    float m_JoystickMaxThreshold = 0.9f;
    [Foldout("Joystick Inputs")]
    public UnityEvent<Vector2> JoyStickMove = new UnityEvent<Vector2>();
    [Foldout("Joystick Inputs")]
    public UnityEvent OnJoyStickDown = new UnityEvent();
    [Foldout("Joystick Inputs")]
    public UnityEvent OnJoyStickUp = new UnityEvent();
    bool m_JoyStickLastStateDown;
    bool m_JoyStickLastStateUp;


    private void Start()
    {
        //m_Pose = GetComponent<VivePoseTracker>();
        InputDevices.GetDevicesWithCharacteristics(deviceRole, inputDevices);
        foreach (InputDevice device in inputDevices)
        {
            Debug.Log(string.Format("Device found with name '{0}' and role '{1}'", device.name, device.characteristics.ToString()));
        }
    }

    private void Update()
    {
        if (inputDevices.Count > 0)
        {
            //--------Trigger Inputs --------
            if (TriggerDown(inputDevices[0]))
            {
                OnTriggerDown.Invoke();
            }
            if (TriggerUp(inputDevices[0]))
            {
                OnTriggerUp.Invoke();
            }
            if (TriggerHold(inputDevices[0]))
            {
                OnTriggerHold.Invoke();
            }

            //--------Primary button Inputs --------
            if (PrimaryButtonDown(inputDevices[0]))
            {
                OnADown.Invoke();
            }
            if (PrimaryButtonUp(inputDevices[0]))
            {
                OnAUp.Invoke();
            }
            if (PrimaryButtonHold(inputDevices[0]))
            {
                OnAHold.Invoke();
            }
            //--------Secondary button Inputs --------
            if (SecondaryButtonDown(inputDevices[0]))
            {
                OnBDown.Invoke();
            }
            if (SecondaryButtonUp(inputDevices[0]))
            {
                OnBUp.Invoke();
            }
            if (SecondaryButtonHold(inputDevices[0]))
            {
                OnBHold.Invoke();
            }
            //--------Grip button Inputs --------
            if (GripButtonDown(inputDevices[0]))
            {
                OnGripDown.Invoke();
            }
            if (GripButtonUp(inputDevices[0]))
            {
                OnGripUp.Invoke();
            }
            if (GripHold(inputDevices[0]))
            {
                OnGripHold.Invoke();
            }
            //--------JoyStick Inputs --------
            if (JoyStick().x > JoyStickXMinThreshold && JoyStick().x < m_JoystickMaxThreshold || JoyStick().y > JoyStickYMinThreshold && JoyStick().y < m_JoystickMaxThreshold || JoyStick().x < -JoyStickXMinThreshold && JoyStick().x > -m_JoystickMaxThreshold || JoyStick().y < -JoyStickYMinThreshold && JoyStick().y > -m_JoystickMaxThreshold)
            {
                Debug.Log("IsWithInThreshold");
                JoyStickMove.Invoke(JoyStick());
            }
            if (JoyStickPress(inputDevices[0]))
            {
                OnJoyStickDown.Invoke();
            }
            if (JoyStickUp(inputDevices[0]))
            {
                OnJoyStickUp.Invoke();
            }
        }
    }
    //-------- Trigger Button logic ----------
    public bool TriggerDown(InputDevice device)
    {
        if (device.TryGetFeatureValue(CommonUsages.triggerButton, out bool m_triggerpush) && m_triggerpush)
        {
            bool tempStatePrimary = m_triggerpush;

            if (tempStatePrimary != m_TriggerLastStateDown)  //Button Down
            {
                m_TriggerLastStateDown = tempStatePrimary;
                return true;
            }
        }
        else
        {
            m_TriggerLastStateDown = false;
        }
        return false;
    }
    public bool TriggerHold(InputDevice device)
    {
        if (device.TryGetFeatureValue(CommonUsages.triggerButton, out bool m_triggerpush) && m_triggerpush)
        {
            return true;
        }
        return false;
    }
    public bool TriggerUp(InputDevice device)
    {
        if (device.TryGetFeatureValue(CommonUsages.triggerButton, out bool m_triggerpush) && !m_triggerpush)
        {
            bool tempStatePrimary = m_triggerpush;

            if (tempStatePrimary != m_TriggerLastStateUp)  //Button Down
            {
                m_TriggerLastStateUp = tempStatePrimary;
                return true;
            }
        }
        else
        {
            m_TriggerLastStateUp = true;
        }
        return false;
    }
    //-------- Primary Button logic ----------
    public bool PrimaryButtonDown(InputDevice device)
    {
        if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool primarybuttonValue) && primarybuttonValue)
        {
            bool tempStatePrimary = primarybuttonValue;

            if (tempStatePrimary != m_PrimaryButtonLastStateDown)  //Button Down
            {
                m_PrimaryButtonLastStateDown = tempStatePrimary;
                return true;
            }
        }
        else
        {
            m_PrimaryButtonLastStateDown = false;
        }
        return false;
    }
    public bool PrimaryButtonUp(InputDevice device)
    {
        if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool primarybuttonValue) && !primarybuttonValue)
        {
            bool tempStatePrimary = primarybuttonValue;

            if (tempStatePrimary != m_PrimaryButtonLastStateUp)  //Button Down
            {
                m_PrimaryButtonLastStateUp = tempStatePrimary;
                return true;
            }
        }
        else
        {
            m_PrimaryButtonLastStateUp = true;
        }
        return false;
    }
    public bool PrimaryButtonHold(InputDevice device)
    {
        if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool primarybuttonValue) && primarybuttonValue)
        {
            return true;
        }
        return false;
    }
    //-------- Secondary Button logic ----------
    public bool SecondaryButtonDown(InputDevice device)
    {
        if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonValue) && secondaryButtonValue)
        {
            bool tempStatePrimary = secondaryButtonValue;

            if (tempStatePrimary != m_SecondaryButtonLastStateDown)  //Button Down
            {
                m_SecondaryButtonLastStateDown = tempStatePrimary;
                return true;
            }
        }
        else
        {
            m_SecondaryButtonLastStateDown = false;
        }
        return false;
    }
    public bool SecondaryButtonUp(InputDevice device)
    {
        if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonValue) && !secondaryButtonValue)
        {
            bool tempStatePrimary = secondaryButtonValue;

            if (tempStatePrimary != m_SecondaryButtonLastStateUp)  //Button Down
            {
                m_SecondaryButtonLastStateUp = tempStatePrimary;
                return true;
            }
        }
        else
        {
            m_SecondaryButtonLastStateUp = true;
        }
        return false;
    }
    public bool SecondaryButtonHold(InputDevice device)
    {
        if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonValue) && secondaryButtonValue)
        {
            return true;
        }
        return false;
    }
    //-------- Grip Button logic ----------
    public bool GripButtonDown(InputDevice device)
    {
        if (device.TryGetFeatureValue(CommonUsages.gripButton, out bool GripButtonValue) && GripButtonValue)
        {
            bool tempStatePrimary = GripButtonValue;

            if (tempStatePrimary != m_GripButtonLastStateDown)  //Button Down
            {
                m_GripButtonLastStateDown = tempStatePrimary;
                return true;
            }
        }
        else
        {
            m_GripButtonLastStateDown = false;
        }

        return false;
    }
    public bool GripButtonUp(InputDevice device)
    {
        if (device.TryGetFeatureValue(CommonUsages.gripButton, out bool GripButtonValue) && !GripButtonValue)
        {
            bool tempStatePrimary = GripButtonValue;

            if (tempStatePrimary != m_GripButtonLastStateUp)  //Button Down
            {
                m_GripButtonLastStateUp = tempStatePrimary;
                return true;
            }
        }
        else
        {
            m_GripButtonLastStateUp = true;
        }

        return false;
    }
    public bool GripHold(InputDevice device)
    {
        if (device.TryGetFeatureValue(CommonUsages.gripButton, out bool GripButtonValue) && GripButtonValue)
        {
            return true;
        }
        return false;
    }
    //-------JoyStick inputs-------
    public Vector2 JoyStick()
    {
        if (inputDevices.Count > 0)
        {
            InputDevice device = inputDevices[0];
            if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2Daxis))
            {
                return primary2Daxis;
            }
        }
        return new Vector2();
    }
    public bool JoyStickPress(InputDevice device)
    {
        if(device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool JoystickClick) && JoystickClick)
        {
            bool tempStatePrimary = JoystickClick;

            if (tempStatePrimary != m_JoyStickLastStateDown)  //Button Down
            {
                m_JoyStickLastStateDown = tempStatePrimary;
                return true;
            }
        }
        else
        {
            m_JoyStickLastStateDown = false;
        }

        return false;
    }
    public bool JoyStickUp(InputDevice device)
    {
        if (device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool JoyStickClick) && !JoyStickClick)
        {
            bool tempStatePrimary = JoyStickClick;

            if (tempStatePrimary != m_JoyStickLastStateUp)  //Button Down
            {
                m_JoyStickLastStateUp = tempStatePrimary;
                return true;
            }
        }
        else
        {
            m_JoyStickLastStateUp = true;
        }
        return false;
    }
}
