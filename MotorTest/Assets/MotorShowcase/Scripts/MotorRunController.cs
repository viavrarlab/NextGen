using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorRunController : MonoBehaviour
{
    public GameObject m_MotorWireConnectButton;

    // Start is called before the first frame update
    void Start()
    {
        //Disable_ConnectMotorWire();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Enable_ConnectMotorWire()
    {
        m_MotorWireConnectButton.SetActive(true);
    }
    public void Disable_ConnectMotorWire()
    {
        m_MotorWireConnectButton.SetActive(false);
    }

}
