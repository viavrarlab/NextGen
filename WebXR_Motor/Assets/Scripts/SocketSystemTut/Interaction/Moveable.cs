using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveable : Interactable
{
    public int m_ID = -1;
    private Socket m_ActiveSocket = null;

    public bool IsSocket;
    public Socket CurrentCollidingSocket;

    public override void StartInteraction(Hand hand)
    {
        //hand.Pickup(this);
        AttachNewSocket(hand.GetSocket());
    }

    public override void Interaction(Hand hand)
    {
        print("Interaction");
    }

    public override void EndInteraction(Hand hand)
    {
        //hand.Drop();
        ReleaseOldSocket();

    }
    //chekcs if other objec is socket, checks if the id's match, if match Issocket true changes coliding obj to socket
    //IsSocket bool value is given to GRABTESTS script
    //CurrentColliding socket is socket value given to GRABTESTS script
    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("MotorCollider"))
        //{
        //    //Physics.IgnoreCollision(this.transform.GetChild(0).gameObject.GetComponent<Collider>(), other.transform.GetChild(0).gameObject.GetComponent<Collider>());
        //}
        if (other.CompareTag("Socket"))
        {
            if (other.GetComponent<Slot>().m_PlaceableID == m_ID)
            {
                IsSocket = true;
                CurrentCollidingSocket = other.gameObject.GetComponent<Socket>();
            }
        }
 
    }
    //keeps keeps the value is socket true if conditions are met
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Socket"))
        {
            if(other.GetComponent<Slot>().m_PlaceableID == m_ID)
            {
                IsSocket = true;
            }

            //CurrentCollidingSocket = other.gameObject.GetComponent<Socket>();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Socket"))
        {
            ReleaseOldSocket();
            IsSocket = false;
            CurrentCollidingSocket = null;
        }

    }
    public void AttachNewSocket(Socket newSocket)
    {
        if (newSocket.GetStoredObject())
        {
            return;
        }

        ReleaseOldSocket();
        m_ActiveSocket = newSocket;

        m_ActiveSocket.Attach(this);

        isAvailable = false;
    }

    public void ReleaseOldSocket()
    {
        if (!m_ActiveSocket)
        {
            return;
        }

        m_ActiveSocket.Detach();
        m_ActiveSocket = null;
        isAvailable = true;
    }
}
