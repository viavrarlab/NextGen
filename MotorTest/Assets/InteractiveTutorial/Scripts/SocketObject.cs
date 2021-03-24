using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.XR.Interaction.Toolkit;


public class SocketObject : MonoBehaviour
{
    public bool m_IsOccupied = false;

    public enum SocketState { None, Idle, Snapped, IntersectingValidObject, IntersectingInvalidObject}
    public SocketState m_CurrentSocketState = SocketState.None;

    private SocketState m_LastSocketState;

    private MeshRenderer m_MeshRenderer; // PlacementPoints' graphics. For now they are on the same object and the MR is assigned automatically
    private MaterialPropertyBlock m_MaterialPropertyBlock; // Allows to use a single material, but be able to change each objects' parameters separately (ex., Object1 - blue, Object2 - orange)

    public Collider m_Collider;
    

    public InteractableObject m_PotentialObject;

    public InteractableObject m_ExpectedObject;

    void Start()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_MaterialPropertyBlock = new MaterialPropertyBlock();
        m_Collider = gameObject.GetComponent<Collider>();

        SwitchSocketState(SocketState.Idle);
    }

    void Update()
    {
        if (m_CurrentSocketState != m_LastSocketState)
        {
            switch (m_CurrentSocketState)
            {
                case SocketState.Idle:
                    UpdateMaterial(SocketGlobalSettings.i.m_ColorIdle);
                    FadeAlphaTo(0.2f);
                    m_LastSocketState = m_CurrentSocketState;
                    return;
                case SocketState.IntersectingValidObject:
                    UpdateMaterial(SocketGlobalSettings.i.m_ColorValid);
                    FadeAlphaTo(0.5f);
                    m_LastSocketState = m_CurrentSocketState;
                    return;
                case SocketState.IntersectingInvalidObject:
                    UpdateMaterial(SocketGlobalSettings.i.m_ColorInvalid);
                    FadeAlphaTo(0.5f);
                    m_LastSocketState = m_CurrentSocketState;
                    return;
                case SocketState.Snapped:
                    UpdateMaterial(SocketGlobalSettings.i.m_ColorIdle);
                    m_LastSocketState = m_CurrentSocketState;
                    return;
                default:
                    return;
            }
        }
    }

    public void SwitchSocketState(SocketState _state)
    {
        m_CurrentSocketState = _state;
    }

    void ToggleSocketMeshRenderer(bool _state)
    {
        if (m_MeshRenderer.enabled != _state)
        {
            m_MeshRenderer.enabled = _state;
        }
    }

    public void FadeAlphaTo(float to)
    {
        float val = to;
        DOTween.To(() => val, x => val = x, to, 0.25f).OnUpdate(() =>
        {
            UpdateMaterial(val);
        });
    }

    void UpdateMaterial(float alphaValue)
    {
        if (m_MeshRenderer == null)
        {
            Debug.LogWarning("No MeshRenderer component assigned.");
            return;
        }
        m_MeshRenderer.GetPropertyBlock(m_MaterialPropertyBlock);
        m_MaterialPropertyBlock.SetFloat("_AlphaValue", alphaValue);
        m_MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
    }
    void UpdateMaterial(Color _col)
    {
        if (m_MeshRenderer == null)
        {
            Debug.LogWarning("No MeshRenderer component assigned.");
            return;
        }
        m_MeshRenderer.GetPropertyBlock(m_MaterialPropertyBlock);
        m_MaterialPropertyBlock.SetColor("_BaseColor", _col);
        m_MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
    }

    float GetMaterialAlpha()
    {
        if (m_MeshRenderer == null)
        {
            Debug.LogWarning("No MeshRenderer component assigned.");
            return 0f;
        }
        m_MeshRenderer.GetPropertyBlock(m_MaterialPropertyBlock);
        return m_MaterialPropertyBlock.GetFloat("_AlphaValue");
    }

    public void OnPlacedInSocket()
    {
        m_IsOccupied = true;
        ToggleSocketActiveState(false);
    }

    public void PlaceInSocket(InteractableObject interactable)
    {
        if (!m_IsOccupied)
        {
            if (m_PotentialObject != null)
            {
                if (m_PotentialObject == m_ExpectedObject)
                {
                    
                }
            }
        }
    }

    public void ToggleSocketActiveState(bool state)
    {
        m_Collider.enabled = state;
        m_MeshRenderer.enabled = state;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != m_ExpectedObject.gameObject)
        {
            Physics.IgnoreCollision(other, gameObject.GetComponent<Collider>());
        }

        if (other.tag == "PlayerHand" || m_IsOccupied) // If the object is the player hand object OR the socket is already occupied, then just return and ignore the resto of the function
        {
            return;
        }

        // If the socket is not occupied, allow the entered Placeable object to be processed.
        if (!m_IsOccupied)
        {
            InteractableObject interactable = other.GetComponentInParent<InteractableObject>(); // Get the Placeable component on incoming object.
            if (interactable != null)
            {
                m_PotentialObject = interactable;
                m_PotentialObject.m_IntersectingSocket = this;

            }
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (m_IsOccupied)
        {
            return;
        }

        if (other.CompareTag("Socket") || other.CompareTag("PlayerHand"))
        {
            Physics.IgnoreCollision(other, this.gameObject.GetComponent<BoxCollider>());
        }
        if (!m_IsOccupied)
        {
            if (m_PotentialObject != null)
            {
                if (m_PotentialObject == m_ExpectedObject)
                {
                    SwitchSocketState(SocketState.IntersectingValidObject);                          
                }
                else
                {
                    SwitchSocketState(SocketState.IntersectingInvalidObject);
                }
            }
            else
            {
                return;
            }
        }
        else
        {
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("PlayerHand") || other.CompareTag("Socket")) // If the object is the player hand object, then just return and ignore the resto of the function
        {
            return;
        }
        if (!m_IsOccupied)
        {
            SwitchSocketState(SocketState.Idle);
            m_PotentialObject = null;
        }
        else
        {
            SwitchSocketState(SocketState.Idle);
            m_IsOccupied = false;
        }
    }
}
