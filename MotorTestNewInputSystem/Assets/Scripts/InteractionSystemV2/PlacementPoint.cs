using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Animations;
using System.Linq;

/// <summary>
/// This script is added to the generated sockets.
/// This script is the socket behaviour script.
/// </summary>
public class PlacementPoint : MonoBehaviour
{
    public int m_PlaceableID;   // This must correspond to the Placeable objects' ID that you want to place here
    public Vector3 m_CorrectPlacementAngle; // TODO: make this automated somehow. Maybe when generating placement points / ObjectConfig.. IDK
    public bool m_IsOccupied = false; // If this socket/point has something socketed 
    public float m_CorrectAngleThreshold = 7.5f; // How precise does the Placeable objects' rotation has to be in order to allow snapping into socket. Used in CheckAngle(). Measures as angle degrees.
    [Header("Disable order check")]
    public bool m_CheckOrder = true;
    [Header("Disable angle checks")]
    public bool m_CheckForCorrectAngle = false;
    public bool m_CheckOnlyXAngle = false;
    public bool m_CheckOnlyYAngle = false;
    public bool m_CheckOnlyZAngle = false;
    private bool m_GameControllerAngleCheck = true;
    private OrderCheck m_OrderCheck;

    public float snapspeed = .4f;

    public List<SocketPair> m_IntersectingSockets = new List<SocketPair>();


    public Color m_DefaultStateColor = Color.white;
    public Color m_ValidObjectColor = Color.green;
    public Color m_InvalidObjectColor = Color.red;
    public Color m_ValidRotationColor = Color.cyan;
    public Color m_InvalidRotationColor = Color.yellow;

    public enum SocketState { None, Empty, Snapped, IntersectingValidObject, IntersectingInvalidObject, IntersectingValidRotation, IntersectingInvalidRotation }
    public SocketState m_CurrentSocketState = SocketState.None;

    private SocketState m_LastSocketState;
    private MeshRenderer m_MeshRenderer; // PlacementPoints' graphics. For now they are on the same object and the MR is assigned automatically
    private MaterialPropertyBlock m_MaterialPropertyBlock; // Allows to use a single material, but be able to change each objects' parameters separately (ex., Object1 - blue, Object2 - orange)

    public Placeable m_SnappableObject;    // The object that is currently available to snap and/or is snapped.

    [SerializeField]
    private ControllerScript m_ControllerScript; // TODO:  MAKE A UNIFIED GRAB SCRIPT THAT IS ADDED TO THE SCENE ONCE!

    public ControllerScript[] m_BothControllers;

    GameControllerSC m_GameController;

    void Start()
    {
        m_ControllerScript = FindObjectOfType<ControllerScript>();
        m_BothControllers = FindObjectsOfType<ControllerScript>();
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_MaterialPropertyBlock = new MaterialPropertyBlock();
        m_OrderCheck = GetComponent<OrderCheck>();

        SwitchSocketState(SocketState.Empty);
        m_GameController = FindObjectOfType<GameControllerSC>();
        if (m_GameController != null)
        {
            m_GameControllerAngleCheck = m_GameController.SetAngleCheck;
        }
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
                    //FadeAlphaTo(0.2f);
                    m_LastSocketState = m_CurrentSocketState;
                    return;
                case SocketState.IntersectingValidObject:
                    ToggleSocketMeshRenderer(true);
                    //ToggleIntersectingSockets(false);
                    UpdateMaterial(m_ValidObjectColor);
                    //FadeAlphaTo(0.5f);
                    m_LastSocketState = m_CurrentSocketState;
                    return;
                case SocketState.IntersectingInvalidObject:
                    ToggleSocketMeshRenderer(true);
                    UpdateMaterial(m_InvalidObjectColor);
                    //FadeAlphaTo(0.5f);
                    m_LastSocketState = m_CurrentSocketState;
                    return;
                case SocketState.IntersectingValidRotation:
                    ToggleSocketMeshRenderer(true);
                    UpdateMaterial(m_ValidRotationColor);
                    //FadeAlphaTo(0.5f);
                    m_LastSocketState = m_CurrentSocketState;
                    return;
                case SocketState.IntersectingInvalidRotation:
                    ToggleSocketMeshRenderer(true);
                    UpdateMaterial(m_InvalidRotationColor);
                    //FadeAlphaTo(0.5f);
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
        if (other.CompareTag("PlayerHand") || m_IsOccupied) // If the object is the player hand object OR the socket is already occupied, then just return and ignore the resto of the function
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

        if (m_BothControllers.FirstOrDefault(x => x.TriggerPush || x.GrabPush))
        {
            m_ControllerScript = m_BothControllers.FirstOrDefault(x => x.TriggerPush || x.GrabPush);
        }
        //If object is placed and want to take it out of the socket
        //if (m_ControllerScript != null && m_ControllerScript.TriggerPush == true && m_IsOccupied && m_ControllerScript.collidingObjectToBePickedUp != null && m_SnappableObject.CanTakeOut) // If the object is the player hand object OR the socket is already occupied, then just return and ignore the resto of the function
        //{
        //    if (m_ControllerScript.objectinhand == m_SnappableObject.gameObject)
        //    {
        //        m_SnappableObject.m_IsPlaced = false;
        //        m_SnappableObject.GetComponentInParent<ParentConstraint>().constraintActive = false;             
        //        m_IsOccupied = false;
        //    }
        //}
        //else
        if (m_IsOccupied)
        {
            return;
        }
        if (other.CompareTag("Socket"))
        {
            Physics.IgnoreCollision(other, this.gameObject.GetComponent<BoxCollider>());
        }
        if (!m_IsOccupied && GameControllerSC.Instance != null && !GameControllerSC.Instance.ObjectIsSnapping)
        {
            if (m_SnappableObject != null)
            {
                if (m_PlaceableID == m_SnappableObject.m_ID)
                {                    
                    if (m_OrderCheck.m_OrderCorrect)
                    {
                        if (m_CheckForCorrectAngle && m_GameControllerAngleCheck) // if option is check
                        {
                            if (CheckAngle(m_SnappableObject.transform.rotation.eulerAngles))   // Check if user has rotated the object correctly
                            {
                                SwitchSocketState(SocketState.IntersectingValidObject);
                                if (!m_ControllerScript.TriggerPush && !m_ControllerScript.GrabPush)
                                {
                                    if (GameControllerSC.Instance.SetOrderCheck)
                                    {
                                        GradingController.Instance.PointCounter(m_OrderCheck.m_OrderCorrect);
                                        GradingController.Instance.WhiteBoardPointUpdate();
                                    }
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
                                if (!m_ControllerScript.TriggerPush && !m_ControllerScript.GrabPush)
                                {
                                    if (GameControllerSC.Instance.SetOrderCheck)
                                    {
                                        GradingController.Instance.PointCounter(m_OrderCheck.m_OrderCorrect);
                                        GradingController.Instance.WhiteBoardPointUpdate();
                                    }
                                    StartCoroutine(SnapWithAnimation());
                                }
                            }
                        }
                    }
                    else
                    {
                        SwitchSocketState(SocketState.IntersectingInvalidObject);
                        if (GameControllerSC.Instance.SetOrderCheck)
                        {
                            GradingController.Instance.PointCounter(m_OrderCheck.m_OrderCorrect);
                            GradingController.Instance.WhiteBoardPointUpdate();
                        }
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

        if (other.CompareTag("PlayerHand")) // If the object is the player hand object, then just return and ignore the resto of the function
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
            if(m_SnappableObject != null)
            {
                GradingController.Instance.ObjectExitedSocket = true;
            }
            m_SnappableObject = null;

        }
        //if (other.GetComponentInParent<Placeable>() != null)
        //{
        //    //SwitchSocketState(SocketState.Empty);
        //}
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

    //float GetMaterialAlpha()
    //{
    //    if (m_MeshRenderer == null)
    //    {
    //        Debug.LogWarning("No MeshRenderer component assigned.");
    //        return 0f;
    //    }
    //    m_MeshRenderer.GetPropertyBlock(m_MaterialPropertyBlock);
    //    return m_MaterialPropertyBlock.GetFloat("_AlphaValue");
    //}

    bool CheckAngle(Vector3 _incomingRotation)
    {
        bool isSameRotation = false;
        //angle check on x axis
        if (m_CheckOnlyXAngle)
        {
            isSameRotation = CorectRotationX(_incomingRotation, m_CorrectPlacementAngle, true);
        }
        //angle check on y axis
        if (m_CheckOnlyYAngle)
        {
            isSameRotation = CorectRotationY(_incomingRotation, m_CorrectPlacementAngle, true);
        }
        //angle check on z axis
        if (m_CheckOnlyZAngle)
        {
            isSameRotation = CorectRotationZ(_incomingRotation, m_CorrectPlacementAngle, true);
        }
        //Check all angles
        if (!m_CheckOnlyXAngle && !m_CheckOnlyYAngle && !m_CheckOnlyZAngle)
        {
            isSameRotation = CorectRotationAllAngles(_incomingRotation, m_CorrectPlacementAngle, true);
        }
        //bool isSameRotation = Mathf.Abs(angle) < m_CorrectAngleThreshold;   // If the difference is less than a threshold then we accept that it's the same rotation
        return isSameRotation;
    }
    bool CorectRotationAllAngles(Vector3 objRotation, Vector3 currentRot, bool useThisFunc)
    {
        if (!useThisFunc)
            return true;
        if (IsInRange(objRotation.x - m_CorrectAngleThreshold, objRotation.x + m_CorrectAngleThreshold, currentRot.x)
            &&
            IsInRange(objRotation.y - m_CorrectAngleThreshold, objRotation.y + m_CorrectAngleThreshold, currentRot.y)
            &&
            IsInRange(objRotation.z - m_CorrectAngleThreshold, objRotation.z + m_CorrectAngleThreshold, currentRot.z))
            return true;
        else
            return false;
    }
    bool CorectRotationX(Vector3 objRotation, Vector3 currentRot, bool useThisFunc)
    {
        if (!useThisFunc)
            return true;
        if (IsInRange(objRotation.x - m_CorrectAngleThreshold, objRotation.x + m_CorrectAngleThreshold, currentRot.y))
            return true;
        else
            return false;
    }
    bool CorectRotationY(Vector3 objRotation, Vector3 currentRot, bool useThisFunc)
    {
        if (!useThisFunc)
            return true;
        if (IsInRange(objRotation.y - m_CorrectAngleThreshold, objRotation.y + m_CorrectAngleThreshold, currentRot.y))
            return true;
        else
            return false;
    }
    bool CorectRotationZ(Vector3 objRotation, Vector3 currentRot, bool useThisFunc)
    {
        if (!useThisFunc)
            return true;
        if (IsInRange(objRotation.z - m_CorrectAngleThreshold, objRotation.z + m_CorrectAngleThreshold, currentRot.z))
            return true;
        else
            return false;
    }
    bool IsInRange(float min, float max, float value)
    {
        if (value >= min && value <= max)
            return true;
        else
            return false;
    }
    private IEnumerator SnapWithAnimation()
    {
        GameControllerSC.Instance.ObjectIsSnapping = true;
        float t = 0f;
        Vector3 StartPosition = m_SnappableObject.transform.position;
        Quaternion StartRotation = m_SnappableObject.transform.localRotation;
        while (t <= 1f)
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
        GameControllerSC.Instance.ObjectIsSnapping = false;
        GradingController.Instance.pointAdded = false;
        yield return null;
    }
}
