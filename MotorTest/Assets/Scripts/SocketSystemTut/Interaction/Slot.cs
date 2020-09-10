using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Valve.VR.InteractionSystem;

public class Slot : Interactable
{
    public int m_PlaceableID = -1;
    public bool m_CheckForCorrectAngle;

    public bool m_IncomingCanBePlaced = false;

    public enum SocketState { None, Empty, Snapped, IntersectingValidObject, IntersectingInvalidObject, IntersectingValidRotation, IntersectingInvalidRotation }
    public SocketState m_CurrentSocketState = SocketState.None;
    private SocketState m_LastSocketState;

    public Color m_DefaultStateColor = Color.white;
    public Color m_ValidObjectColor = Color.green;
    public Color m_InvalidObjectColor = Color.red;
    public Color m_ValidRotationColor = Color.cyan;
    public Color m_InvalidRotationColor = Color.yellow;

    private MeshRenderer m_MeshRenderer; // PlacementPoints' graphics. For now they are on the same object and the MR is assigned automatically
    private MaterialPropertyBlock m_MaterialPropertyBlock; // Allows to use a single material, but be able to change each objects' parameters separately (ex., Object1 - blue, Object2 - orange)

    public List<Slot> m_IntersectingSlots = new List<Slot>();

    private Socket m_Socket = null;

    private CorrectOrderTests m_CorrOrder;
    private bool canPlace;

    SocketGenerator m_SG;
    public Slot[] SlotArray;

    private void Awake()
    {
        m_Socket = GetComponent<Socket>();
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_MaterialPropertyBlock = new MaterialPropertyBlock();
    }
    private void Start()
    {
        SwitchSocketState(SocketState.Empty);
        m_SG = GameObject.FindObjectOfType<SocketGenerator>();
        SlotArray = m_SG.m_AllSlots.ToArray();
    }

    public override void StartInteraction(Hand hand)
    {
        Socket handSocket = hand.GetSocket();
        Moveable heldObject = handSocket.GetStoredObject();

        if (heldObject)
        {
            TryStore(heldObject);
        }
        else
        {
            TryRetrieve(handSocket);
        }
    }

    private void TryStore(Moveable newMoveable)
    {
        if (!m_Socket.GetStoredObject())
        {
            //this is where you check if the incoming object is correct for this slot
            if (newMoveable.m_ID == m_PlaceableID)
            {
                newMoveable.AttachNewSocket(m_Socket);
                SwitchSocketState(SocketState.Snapped);
            }
        }
    }

    private void TryRetrieve(Socket newSocket)
    {
        Moveable objectToRetrieve = m_Socket.GetStoredObject();

        if (objectToRetrieve)
            objectToRetrieve.AttachNewSocket(newSocket);
    }


    void SwitchSocketState(SocketState _state)
    {
        m_CurrentSocketState = _state;

        if (m_CurrentSocketState != m_LastSocketState)
        {
            switch (m_CurrentSocketState)
            {
                case SocketState.Empty:
                    ToggleSocketMeshRenderer(true);
                    UpdateMaterial(m_DefaultStateColor);
                    FadeAlphaTo(0.2f);
                    m_LastSocketState = m_CurrentSocketState;
                    return;
                case SocketState.IntersectingValidObject:
                    ToggleSocketMeshRenderer(true);
                    //ToggleIntersectingSockets(false);
                    UpdateMaterial(m_ValidObjectColor);
                    FadeAlphaTo(0.5f);
                    m_LastSocketState = m_CurrentSocketState;
                    return;
                case SocketState.IntersectingInvalidObject:
                    ToggleSocketMeshRenderer(true);
                    UpdateMaterial(m_InvalidObjectColor);
                    FadeAlphaTo(0.5f);
                    m_LastSocketState = m_CurrentSocketState;
                    return;
                case SocketState.IntersectingValidRotation:
                    ToggleSocketMeshRenderer(true);
                    UpdateMaterial(m_ValidRotationColor);
                    FadeAlphaTo(0.5f);
                    m_LastSocketState = m_CurrentSocketState;
                    return;
                case SocketState.IntersectingInvalidRotation:
                    ToggleSocketMeshRenderer(true);
                    UpdateMaterial(m_InvalidRotationColor);
                    FadeAlphaTo(0.5f);
                    m_LastSocketState = m_CurrentSocketState;
                    return;
                case SocketState.Snapped:
                    //ToggleIntersectingSockets(true);
                    UpdateMaterial(m_DefaultStateColor);
                    //FadeAlphaTo(0f);
                    ToggleSocketMeshRenderer(false);
                    ResetIntersectingSlots();
                    m_LastSocketState = m_CurrentSocketState;
                    return;
                default:
                    return;
            }
        }
    }

    void ToggleSocketMeshRenderer(bool _state)
    {
        if (m_MeshRenderer.enabled != _state)
        {
            m_MeshRenderer.enabled = _state;
        }

        //if (_state == true)
        //{
        //    m_MeshRenderer.enabled = true;
        //    gameObject.layer = LayerMask.NameToLayer("Default");
        //}
        //else
        //{
        //    m_MeshRenderer.enabled = false;
        //    gameObject.layer = LayerMask.NameToLayer("NoCollision");
        //}

        //switch (_state)
        //{
        //    case true:
        //        m_MeshRenderer.enabled = true;
        //        gameObject.layer = LayerMask.NameToLayer("Default");
        //        break;
        //    case false:
        //        m_MeshRenderer.enabled = false;
        //        gameObject.layer = LayerMask.NameToLayer("NoCollision");
        //        break;
        //}
    }
    void ResetIntersectingSlots()
    {
        foreach(Slot s in m_IntersectingSlots)
        {
            if (s.m_CurrentSocketState != SocketState.Snapped)
            {
                s.SwitchSocketState(SocketState.Empty);
            }
            else
            {
                s.SwitchSocketState(SocketState.Snapped);
            }
        }
    }

    public void FadeAlphaTo(float to)
    {
        //float val = 1f - to;
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
        m_MaterialPropertyBlock.SetColor("_Color", _col);
        m_MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.tag == "MotorCollider")
        //{
        //    Moveable obj = other.transform.parent.transform.parent.gameObject.GetComponent<Moveable>();
        //    if (obj.m_ID == m_PlaceableID)
        //    {
        //        SwitchSocketState(SocketState.IntersectingValidObject);
        //    }
        //    else
        //    {
        //        //SwitchSocketState(SocketState.IntersectingInvalidObject);
        //    }
        //}
        if (other.CompareTag("PlayerHand") )
        {
            Debug.Log("It's a hand you fool!");
        }
        if (other.CompareTag("PlacementRoot") || other.CompareTag("Socket"))
        {
            Physics.IgnoreCollision(this.gameObject.GetComponent<Collider>(), other.gameObject.GetComponent<Collider>());
        }
        else
        {
            if (other.transform.parent.transform.parent.gameObject.GetComponent<Moveable>() != null)
            {
                Moveable obj = other.transform.parent.transform.parent.gameObject.GetComponent<Moveable>();
                if (obj.m_ID != m_PlaceableID)
                {
                    SwitchSocketState(SocketState.IntersectingInvalidObject);
                }
                for(int i =0; i<SlotArray.Length;i++)
                {
                    //Debug.Log(slt.name.ToString());
                    if (obj.m_ID == 0)
                    {
                        canPlace = true;
                        print("pirmais objekts");
                    }
                    else
                    {
                        if (SlotArray[obj.m_ID-1].m_CurrentSocketState == SocketState.Snapped)
                        {
                            print("Bus istais");
                            canPlace = true;
                        }
                        else
                        {
                            print("Nevar likt");
                            canPlace = false;
                        }
                    }

                }
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            Debug.Log("It's a hand you fool! or a socket?");
        }
        if (other.CompareTag("PlacementRoot") || other.CompareTag("Socket"))
        {
            Physics.IgnoreCollision(this.gameObject.GetComponent<Collider>(), other.gameObject.GetComponent<Collider>());
        }
        else
        {
                if (other.transform.parent.transform.parent.gameObject.GetComponent<Moveable>() != null)
                {
                    Moveable obj = other.transform.parent.transform.parent.gameObject.GetComponent<Moveable>();
                    //Check if object and socet id's match, if not then change to socket to invalid object

                    //check if object id's match and if fixed joint is connected
                    if (obj.m_ID == m_PlaceableID && GetComponent<FixedJoint>().connectedBody == null)
                    {
                        Debug.Log("Valid Intersect");
                        SwitchSocketState(SocketState.IntersectingValidObject);
                    }
                    //if not null change to snapped
                    if (GetComponent<FixedJoint>().connectedBody != null)
                    {
                        //Debug.Log("Snapped");
                        SwitchSocketState(SocketState.Snapped);
                    }
                    if (m_CurrentSocketState == SocketState.Snapped)
                    {

                    }
                }    
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "MotorCollider")
        {
            //Moveable obj = other.transform.parent.transform.parent.gameObject.GetComponent<Moveable>();
            //if (obj.m_ID == m_PlaceableID)
            //{
            //    SwitchSocketState(SocketState.IntersectingValidObject);
            //}
            //else
            //{
            //    SwitchSocketState(SocketState.IntersectingInvalidObject);
            //}
            if (m_CurrentSocketState != SocketState.Snapped)
            {
                SwitchSocketState(SocketState.Empty);
            }
        }
    }
}
