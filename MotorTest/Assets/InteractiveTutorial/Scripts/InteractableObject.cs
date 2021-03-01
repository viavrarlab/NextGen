using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.XR.Interaction.Toolkit;
using DG.Tweening;

public class InteractableObject : MonoBehaviour
{


    public SocketObject m_IntersectingSocket;

    public SocketObject m_ExpectedSocket;
    public bool m_IsCurrentlyIntersecting = false;

    public MotorPart m_MotorPartInformation;

    private Rigidbody m_Rigidbody;
    private Collider m_Collider;

    void Start()
    {
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();
        m_Collider = gameObject.GetComponent<Collider>();
    }

    public void PutIntoSocket(XRBaseInteractor interactor)
    {
        if (m_IntersectingSocket != null && m_IntersectingSocket == m_ExpectedSocket)
        {
            transform.DOMove(m_ExpectedSocket.transform.position, 0.5f);
            transform.DORotateQuaternion(m_ExpectedSocket.transform.rotation, 0.5f).OnComplete(() =>
            {
                OnFinishPlaceInSocket();
                InteractiveTutorialController.i.ActivateNextSocket();
            });
        }
    }
    public void PutIntoSocket()
    {
        if (m_IntersectingSocket != null && m_IntersectingSocket == m_ExpectedSocket)
        {
            transform.DOMove(m_ExpectedSocket.transform.position, 0.5f);
            transform.DORotateQuaternion(m_ExpectedSocket.transform.rotation, 0.5f).OnComplete(() =>
            {
                OnFinishPlaceInSocket();
                InteractiveTutorialController.i.ActivateNextSocket();
            });
        }
    }

    public void LocalMove(Vector3 position)
    {
        transform.DOBlendableLocalMoveBy(position, 0.5f);
    }

    public void OnFinishPlaceInSocket()
    {
        m_Rigidbody.isKinematic = true;
        m_Collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        SocketObject incomingTest = other.gameObject.GetComponent<SocketObject>();
        if (incomingTest != null)
        {
            if (incomingTest == m_ExpectedSocket)
            {
                m_IsCurrentlyIntersecting = true;
                m_IntersectingSocket = incomingTest;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(m_IntersectingSocket != null)
        {
            m_IsCurrentlyIntersecting = false;
            m_IntersectingSocket = null;
        }
    }

}
