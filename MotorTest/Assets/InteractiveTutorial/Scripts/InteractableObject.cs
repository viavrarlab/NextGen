using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractableObject : MonoBehaviour
{

    public SocketObject m_IntersectingSocket;

    public SocketObject m_ExpectedSocket;
    public bool m_IsCurrentlyIntersecting = false;

    public MotorPart m_MotorPartInformation;

    private Rigidbody m_Rigidbody;
    private Collider m_Collider;
    private ModelOutline.Outline m_Outline;

    void Start ()
    {
        m_Rigidbody = gameObject.GetComponent<Rigidbody> ();
        m_Collider = gameObject.GetComponent<Collider> ();
        m_Outline = gameObject.GetComponent<ModelOutline.Outline>();

        ResetComponents();
    }

    public void ResetComponents ()
    {
        m_Outline.enabled = false;
        m_Rigidbody.isKinematic = false;
        m_Collider.enabled = true;
    }

    public void PutIntoSocket (XRBaseInteractor interactor)
    {
        if (m_IntersectingSocket != null && m_IntersectingSocket == m_ExpectedSocket)
        {
            transform.DOMove (m_ExpectedSocket.transform.position, 0.5f);
            transform.DORotateQuaternion (m_ExpectedSocket.transform.rotation, 0.5f).OnComplete (() =>
            {
                OnFinishPlaceInSocket ();
                InteractiveTutorialController.i.ActivateNextSocket ();
            });
        }
    }
    public void PutIntoSocket ()
    {
        if (m_IntersectingSocket != null && m_IntersectingSocket == m_ExpectedSocket)
        {
            transform.DOMove (m_ExpectedSocket.transform.position, 0.5f);
            transform.DORotateQuaternion (m_ExpectedSocket.transform.rotation, 0.5f).OnComplete (() =>
            {
                OnFinishPlaceInSocket ();
                InteractiveTutorialController.i.ActivateNextSocket ();
            });
        }
    }

    public void LocalMove (Vector3 position, float delay)
    {
        transform.DOBlendableLocalMoveBy (position, 0.5f).SetDelay(delay);
    }

    public void OnFinishPlaceInSocket ()
    {
        m_Outline.enabled = false;
        m_Rigidbody.isKinematic = true;
        m_Collider.enabled = false;
    }

    private void OnTriggerEnter (Collider other)
    {
        SocketObject incomingTest = other.gameObject.GetComponent<SocketObject> ();
        if (incomingTest != null)
        {
            if (incomingTest == m_ExpectedSocket)
            {
                m_IsCurrentlyIntersecting = true;
                m_IntersectingSocket = incomingTest;
            }
        }
    }

    private void OnTriggerExit (Collider other)
    {
        if (m_IntersectingSocket != null)
        {
            m_IsCurrentlyIntersecting = false;
            m_IntersectingSocket = null;
        }
    }

}