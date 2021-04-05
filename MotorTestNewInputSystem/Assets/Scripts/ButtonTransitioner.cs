using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ButtonTransitioner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public Color32 m_NormalColor = Color.white;
    public Color32 m_HoverColor = Color.grey;
    public Color32 m_DownColor = Color.white;

    private Image m_Image = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        m_Image.color = m_HoverColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_Image.color = m_DownColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_Image.color = m_HoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_Image.color = m_NormalColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_Image.color = m_NormalColor;
    }

    void Awake()
    {
        m_Image = GetComponent<Image>();
    }

}
