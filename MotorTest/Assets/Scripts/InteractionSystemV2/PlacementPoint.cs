using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Valve.VR;
using HTC.UnityPlugin.Vive;
using UnityEngine.Animations;
using TMPro;

/// <summary>
/// This could be automated when creating armature/object IDs - to each ID'd object we add this and configure it accordingly (everything, including mesh, its transforms, trigger bounds, etc)
/// </summary>
public class PlacementPoint : MonoBehaviour
{
    public int m_PlaceableID;   // This must correspond to the Placeable objects' ID that you want to place here
    public Vector3 m_CorrectPlacementAngle; // TODO: make this automated somehow. Maybe when generating placement points / ObjectConfig.. IDK
    public bool m_IsOccupied = false; // If this socket/point has something socketed 
    public float m_CorrectAngleThreshold = 7.5f; // How precise does the Placeable objects' rotation has to be in order to allow snapping into socket. Used in CheckAngle(). Measures as angle degrees.
    public bool m_CheckForCorrectAngle = false;

    public OrderCheck m_OrderCheck;

    public float snapspeed = .4f;

    public List<SocketPair> m_IntersectingSockets = new List<SocketPair>();
    

    public Color m_DefaultStateColor = Color.white;
    public Color m_ValidObjectColor = Color.green;
    public Color m_InvalidObjectColor = Color.red;
    public Color m_ValidRotationColor = Color.cyan;
    public Color m_InvalidRotationColor = Color.yellow;

    public enum SocketState { None, Empty, Snapped, IntersectingValidObject, IntersectingInvalidObject, IntersectingValidRotation, IntersectingInvalidRotation}
    public SocketState m_CurrentSocketState = SocketState.None;

    private SocketState m_LastSocketState;


    private MeshRenderer m_MeshRenderer; // PlacementPoints' graphics. For now they are on the same object and the MR is assigned automatically
    private MaterialPropertyBlock m_MaterialPropertyBlock; // Allows to use a single material, but be able to change each objects' parameters separately (ex., Object1 - blue, Object2 - orange)

    private Placeable m_SnappableObject;    // The object that is currently available to snap and/or is snapped.

    private ControllerScript m_ControllerScript; // TODO:  MAKE A UNIFIED GRAB SCRIPT THAT IS ADDED TO THE SCENE ONCE!

    void Start()
    {
        m_ControllerScript = GameObject.FindObjectOfType<ControllerScript>();
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_MaterialPropertyBlock = new MaterialPropertyBlock();
        m_OrderCheck = GetComponent<OrderCheck>();

        SwitchSocketState(SocketState.Empty);
    }

    void Update()
    {
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
                    m_LastSocketState = m_CurrentSocketState;
                    return;
                default:
                    return;
            }
        }
    }

    void SwitchSocketState(SocketState _state)
    {
        m_CurrentSocketState = _state;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerHand" || m_IsOccupied) // If the object is the player hand object OR the socket is already occupied, then just return and ignore the resto of the function
        {
            return;
        }
        if (other.CompareTag("Socket"))
        {
            Physics.IgnoreCollision(other, this.gameObject.GetComponent<BoxCollider>());
            return;
        }
        if (other.CompareTag("PlacementRoot"))
        {
            Physics.IgnoreCollision(other, this.gameObject.GetComponent<BoxCollider>());
            return;
        }
        // If the socket is not occupied, allow the entered Placeable object to be processed.
        if (!m_IsOccupied)
        {
            Placeable p = other.GetComponentInParent<Placeable>(); // Get the Placeable component on incoming object.
            if (p != null)
            {
                if (p.m_ID == m_PlaceableID) // Check if the incoming Placeable objects ID is what this socket wants
                {
                    if (m_SnappableObject != p) // Kind of redundant check, but whatever - just keep it as extra security. Just setting the member variable to incoming objects Placeable script reference.
                    {
                        m_SnappableObject = p;
                        m_CorrectPlacementAngle = transform.rotation.eulerAngles; // update socket angle
                    }
                    //m_IsOccupied = true;
                }
            }
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        //If object is placed and want to take it out of the socket
        if (m_ControllerScript.TriggerPush == true && m_IsOccupied && m_ControllerScript.collidingObjectToBePickedUp != null && m_SnappableObject.CanTakeOut) // If the object is the player hand object OR the socket is already occupied, then just return and ignore the resto of the function
        {
            if(m_ControllerScript.objectinhand == m_SnappableObject.gameObject)
            {
                m_SnappableObject.GetComponentInParent<ParentConstraint>().constraintActive = false;
                m_SnappableObject.m_IsPlaced = false;
                m_IsOccupied = false;
            }

        }
        else
        if (m_IsOccupied)
        {
            return;
        }

        if (other.CompareTag("Socket"))
        {
            Physics.IgnoreCollision(other, this.gameObject.GetComponent<BoxCollider>());
        }
        if (!m_IsOccupied)
        {
            if (m_SnappableObject != null)
            {
                if (m_PlaceableID == m_SnappableObject.m_ID)
                {
                    if (m_OrderCheck.m_OrderCorrect) 
                    {
                        if (m_CheckForCorrectAngle) // if option is check
                        {
                            if (CheckAngle(m_SnappableObject.transform.rotation))   // Check if user has rotated the object correctly
                            {
                                SwitchSocketState(SocketState.IntersectingValidObject);
                                if (m_ControllerScript.TriggerPush == false)
                                {
                                    StartCoroutine(SnapWithAnimation()); //snap object in
                                }
                            }
                            else
                            {
                                SwitchSocketState(SocketState.IntersectingInvalidRotation);
                            }
                        }
                        else
                        {
                            {
                                SwitchSocketState(SocketState.IntersectingValidObject);
                                if (m_ControllerScript.TriggerPush == false)
                                {
                                    StartCoroutine(SnapWithAnimation());
                                }
                            }
                        }
                    }
                    else
                    {
                        SwitchSocketState(SocketState.IntersectingInvalidObject);
                    }
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
        
        if(other.CompareTag("PlayerHand")) // If the object is the player hand object, then just return and ignore the resto of the function
        {
            return;
        }
        if (other.CompareTag("Socket") || other.CompareTag("PlacementRoot"))
        {
            return;
        }
        if (!m_IsOccupied)
        {
            SwitchSocketState(SocketState.Empty);
            m_SnappableObject = null;
        }
        if (other.GetComponentInParent<Placeable>() != null)
        {
            //SwitchSocketState(SocketState.Empty);
        }
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
        //float val = 1f - to;
        float val = to;
        DOTween.To(() => val, x => val = x, to, 0.25f).OnUpdate(() =>
        {
            UpdateMaterial(val);
        });
    }

    void UpdateMaterial(float alphaValue)
    {
        if(m_MeshRenderer == null)
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

    bool CheckAngle(Quaternion _incomingRotation)
    {
        Quaternion angleA = Quaternion.Euler(m_CorrectPlacementAngle);  // Convert Euler angles to Quaternion for the correct/expected rotation and store it in a variable.
        Quaternion angleB = _incomingRotation;  // Store incoming Placeable objects' rotation in variable

        float angle = Quaternion.Angle(angleA, angleB); // Calculate difference between the 2 angles

        bool isSameRotation = Mathf.Abs(angle) < m_CorrectAngleThreshold;   // If the difference is less than a threshold then we accept that it's the same rotation
        return isSameRotation;
    }
    private IEnumerator SnapWithAnimation()
    {
        float t = 0f;
        Vector3 StartPosition = m_SnappableObject.transform.position;
        Quaternion StartRotation = m_SnappableObject.transform.rotation;
        while(t <= 1f)
        {
            t += Time.deltaTime / snapspeed;
            Vector3 currentposition = Vector3.Lerp(StartPosition, transform.position, t);
            Quaternion Currentroatation = Quaternion.Lerp(StartRotation, Quaternion.Euler(m_CorrectPlacementAngle), t);
            m_SnappableObject.transform.rotation = Currentroatation;
            m_SnappableObject.transform.position = currentposition;
            yield return null;
        }
        m_SnappableObject.m_IsPlaced = true;
        m_IsOccupied = true;
        SwitchSocketState(SocketState.Snapped);
        m_SnappableObject.GetComponent<ParentConstraint>().constraintActive = true;
        yield return null;
    }
}
