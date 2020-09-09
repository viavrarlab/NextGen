using UnityEngine;

public class Interactable : MonoBehaviour
{
    protected bool isAvailable = true;

    public virtual void StartInteraction(Hand hand)
    {

    }

    public virtual void Interaction(Hand hand)
    {

    }
    public virtual void EndInteraction(Hand hand)
    {

    }

    public bool GetAvailability()
    {
        return isAvailable;
    }
}
