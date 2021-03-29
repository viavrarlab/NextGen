using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoxController : MonoBehaviour
{
    [SerializeField]
    GameObject BoxObject;
    [SerializeField]
    GameObject MotorRoot;

    GameObject m_controller;
    [SerializeField]
    bool Generated;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            GenerateObject(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            Generated = false;
        }
    }
    void GenerateObject(Collider otherObject)
    {
        if (!Generated && otherObject.GetComponent<ControllerScript>() != null && otherObject.GetComponent<ControllerScript>().TriggerPush && otherObject.GetComponent<ControllerScript>().objectinhand == null)
        {
            Generated = true;
            m_controller = otherObject.gameObject;
            GameObject TempObject =  Instantiate(BoxObject,gameObject.transform.position,gameObject.transform.rotation,MotorRoot.transform);
            TempObject.name = BoxObject.name;
            m_controller.GetComponent<ControllerScript>().collidingObjectToBePickedUp = TempObject;
            m_controller.GetComponent<ControllerScript>().objectinhand = TempObject;
            m_controller.GetComponent<FixedJoint>().connectedBody = TempObject.GetComponent<Rigidbody>();
        }
    }
}
