using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveable : Interactable
{
    public int m_ID = -1;
    private Socket m_ActiveSocket = null;

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
