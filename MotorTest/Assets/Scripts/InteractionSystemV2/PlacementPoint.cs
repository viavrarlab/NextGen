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
    public Vector3 m_CorrectPlacementAngle = Vector3.zero; // TODO: make this automated somehow. Maybe when generating placement points / ObjectConfig.. IDK
    public bool m_IsOccupied = false; // If this socket/point has something socketed 
    public float m_CorrectAngleThreshold = 5f; // How precise does the Placeable objects' rotation has to be in order to allow snapping into socket. Used in CheckAngle(). Measures as angle degrees.
    public bool m_CheckForCorrectAngle = false;

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

        //UpdateMaterial(0.2f); // Hide the graphics
        //UpdateMaterial(m_DefaultStateColor);
        //ToggleIntersectingSockets(false);
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
        //print(other.gameObject.name);
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
                    }
                    //m_IsOccupied = true;
                }
                else
                {
                    //SwitchSocketState(SocketState.IntersectingInvalidObject);
                    //Timer.Register(1f, () => SwitchSocketState(SocketState.Empty));
                }
            }
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "PlayerHand" || m_IsOccupied) // If the object is the player hand object OR the socket is already occupied, then just return and ignore the resto of the function
        {
            return;
        }
        if (other.CompareTag("Socket"))
        {
            Physics.IgnoreCollision(other, this.gameObject.GetComponent<BoxCollider>());
        }
        if (m_SnappableObject != null)
        {
            if (m_PlaceableID == m_SnappableObject.m_ID)
            {
                if (m_CheckForCorrectAngle)
                {
                    if (CheckAngle(m_SnappableObject.transform.rotation))   // Check if user has rotated the object correctly
                    {
                        SwitchSocketState(SocketState.IntersectingValidObject);
                        if (m_ControllerScript.m_TriggerPull == false)
                        {
                            StartCoroutine(SnapWithAnimation());
                        }
                    }
                    else
                    {
                        SwitchSocketState(SocketState.IntersectingInvalidRotation);
                    }
                }
            }
        }
        else
        {
            return;
        }

        //if (m_SnappableObject.isHeld == false && m_PlaceableID == m_SnappableObject.m_ID)
        //{
        //    StartCoroutine(SnapWithAnimation());
        //}

        //if (m_SnappableObject != null && !m_SnappableObject.m_IsPlaced)  // If we have a valid Placeable object and it isn't already placed
        //{
        //    if (m_CheckForCorrectAngle)
        //    {
        //        if (CheckAngle(m_SnappableObject.transform.rotation))   // Check if user has rotated the object correctly
        //        {
        //            SwitchSocketState(SocketState.IntersectingValidRotation);
        //            //if (ViveInput.GetPressUp(m_GrabScript.m_HandRole, m_GrabScript.m_ControllerButton) && !m_IsOccupied) // If user has released the grab button and the socket is not already occupied
        //            //{
        //            //    //SnapObject();   // snap the object to the socket
        //            //    StartCoroutine(SnapWithAnimation());
        //            //}
        //        }
        //        else
        //        {
        //            SwitchSocketState(SocketState.IntersectingInvalidRotation);
        //        }
        //    }
        //    else
        //    {
        //        SwitchSocketState(SocketState.IntersectingValidObject);
        //        //if (ViveInput.GetPressUp(m_GrabScript.m_HandRole, m_GrabScript.m_ControllerButton) && !m_IsOccupied) // If user has released the grab button and the socket is not already occupied
        //        //{
        //        //    //SnapObject();   // snap the object to the socket
        //        //    StartCoroutine(SnapWithAnimation());
        //        //}
        //    }
        //}
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
        if (other.GetComponentInParent<Placeable>() != null)
        {
            SwitchSocketState(SocketState.Empty);
            m_IsOccupied = false;
            //m_SnappableObject.GetComponentInParent<ParentConstraint>().constraintActive = false;
            m_SnappableObject = null;
        }
        //if (!other.CompareTag("Socket"))
        //{
        //        if (other.CompareTag("MotorCollider"))
        //        {
        //            SwitchSocketState(SocketState.Empty);
        //            m_SnappableObject.m_IsPlaced = false;
        //            m_IsOccupied = false;
        //            m_SnappableObject.GetComponent<ParentConstraint>().constraintActive = false;
        //            m_SnappableObject = null;
        //        }
        //}



        //if (m_IsOccupied)
        //{

        //bool otherIsNull = other.gameObject != null;
        //bool snappableIsNull = m_SnappableObject != null;
        //bool otherIsNotSnappable = other.gameObject != m_SnappableObject.gameObject;

        //if ()
        //{
        //    return;
        //}
        //if (m_SnappableObject != null)
        //{
        //    m_SnappableObject.m_IsPlaced = false;
        //    m_IsOccupied = false;
        //    m_SnappableObject = null;
        //    SwitchSocketState(SocketState.Empty);
        //}
        //else
        //{
        //    SwitchSocketState(SocketState.Empty);
        //}
    }

    // TODO: Check if this is still needed! And figure out what exactly it was supposed to fix...
    //void ToggleIntersectingSockets(bool _state)
    //{
    //    foreach (SocketPair sp in m_IntersectingSockets)
    //    {
    //        sp.m_Collider.enabled = _state;
    //        if (!_state)
    //        {
    //            if (sp.m_PlacementPoint.m_SnappableObject != null && sp.m_PlacementPoint.m_SnappableObject.m_IsPlaced)
    //            {
    //                sp.m_PlacementPoint.SwitchSocketState(SocketState.Snapped);
    //            }
    //            else
    //            {
    //                sp.m_PlacementPoint.SwitchSocketState(SocketState.Empty);
    //            }
    //        }
    //    }
    //}

    void SnapObject()
    {
        SwitchSocketState(SocketState.Snapped);
        m_SnappableObject.transform.position = transform.position;
        m_SnappableObject.transform.rotation = Quaternion.Euler(m_CorrectPlacementAngle);
        m_SnappableObject.m_IsPlaced = true;
        m_IsOccupied = true;
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
