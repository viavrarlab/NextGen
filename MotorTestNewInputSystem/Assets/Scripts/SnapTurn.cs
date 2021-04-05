using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using HTC.UnityPlugin.Vive;

public class SnapTurn : MonoBehaviour
{
    public Transform m_VRRig;
    public Transform m_VRCamera;
    public int m_TurnAngle = 45;

    //public HandRole m_HandRole;
    //public ControllerAxis m_ControllerAxis;
    public float m_AxisThreshold = 0.8f;

    private bool m_IsRotating = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //float value = ViveInput.GetAxisEx(m_HandRole, m_ControllerAxis);

        //if ((value > m_AxisThreshold || value < -m_AxisThreshold)/* && !m_IsRotating*/)
        //{
        //    if (m_IsRotating == false)
        //    {
        //        Rotate(Mathf.Sign(value));
        //    }
        //}
        //else
        //{
        //    if (m_IsRotating == true)
        //    {
        //        m_IsRotating = false;
        //    }
        //}
    }

    public void Rotate(float direction)
    {
        m_IsRotating = true;
        //Quaternion rotationAmount = Quaternion.Euler(0f,(m_TurnAngle * direction), 0f);
        //Quaternion postRotation = m_VRRig.rotation * rotationAmount;
        //m_VRRig.rotation = postRotation;

        m_VRRig.RotateAround(m_VRCamera.position, Vector3.up, m_TurnAngle * direction);

        //transform.RotateAround(target.transform.position, Vector3.up, 20 * Time.deltaTime);

        //m_VRRig.rotation = Quaternion.Euler(newRotation);
        //m_VRRig.Rotate(m_VRRig.up, m_TurnAngle * direction);
    }
}
