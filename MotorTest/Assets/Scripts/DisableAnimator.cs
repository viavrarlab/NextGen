using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAnimator : MonoBehaviour
{
    private Animator m_Animator;
    public List<GameObject> gameObjList;
    public GameObject _armature;
    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        GetBones();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("dismantle"))
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !m_Animator.IsInTransition(0))
            {
                m_Animator.enabled = false;
                EnablePhysics();
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(gameObjList.ToString());
        }
    }
    void GetBones()
    {
        foreach (Transform child in _armature.transform)
        {
            if (child.tag == "Grab")
            {
                Debug.Log("add");
                gameObjList.Add(child.gameObject);
            }
        }
    }
    void EnablePhysics()
    {
        foreach (GameObject obj in gameObjList)
        {
            obj.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
