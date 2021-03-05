using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class POIController : MonoBehaviour
{
    public float m_PunchStrength = 2f;

    private Vector3 m_InitScale;

    private Sequence m_Sequence;

    public Button m_Button;

    private void Awake()
    {
        //m_Rend = GetComponent<Renderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //print(gameObject.transform.parent.gameObject.name);

        //m_Button = gameObject.GetComponentInChildren<Button>();

        m_Sequence = DOTween.Sequence();
        m_InitScale = transform.localScale;
        Pop();
    }

    public void TurnOn()
    {
        //m_Rend.enabled = true;
        gameObject.SetActive(true);
        m_Sequence.Play();
    }

    public void TurnOff()
    {
        //m_Rend.enabled = false;
        gameObject.SetActive(false);
        m_Sequence.Pause();
    }

    public void Pop()
    {


        m_Sequence.Append(transform.DOScale(m_InitScale * m_PunchStrength, 0.1f)).SetDelay(1f)
           .Append(transform.DOScale(m_InitScale, 0.2f))
           .Append(transform.DOScale(m_InitScale * m_PunchStrength, 0.1f))
           .Append(transform.DOScale(m_InitScale, 0.2f));

        m_Sequence.SetLoops(-1, LoopType.Restart);
    }
}
