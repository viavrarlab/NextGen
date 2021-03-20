using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LeftHandUIController : MonoBehaviour
{
    [SerializeField]
    Canvas m_ControllerUI;
    [SerializeField]
    Image m_HintImage = null;
    [SerializeField]
    Text PartName = null;

    Object[] m_Thumbnails;

    CorrectOrderTests m_CorrOrder;
    GameControllerSC m_Gcontroller;
    private void Awake()
    {
        //m_ControllerUI = gameObject.GetComponentInChildren<Canvas>();
        m_ControllerUI.enabled = false;
        m_CorrOrder = FindObjectOfType<CorrectOrderTests>();
        m_Gcontroller = FindObjectOfType<GameControllerSC>();
    }
    public void EnableUI()
    {
        if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            if(m_Gcontroller != null)
            {
                if (m_Gcontroller.EnableHint)
                {
                    if (m_ControllerUI.enabled)
                    {
                        m_ControllerUI.enabled = false;
                    }
                    else
                    {
                        m_ControllerUI.enabled = true;
                        NextPartHint();
                    }
                }
            }
            else
            {
                if (m_ControllerUI.enabled)
                {
                    m_ControllerUI.enabled = false;
                }
                else
                {
                    m_ControllerUI.enabled = true;
                    NextPartHint();
                }
            }
        }
    }
    void NextPartHint()
    {
        m_Thumbnails = Resources.LoadAll("Thumbnails", typeof(Sprite));

        for(int i = 0; i < m_CorrOrder.Parts.Count; i++)
        {
            if(m_CorrOrder.Parts[i].obj.GetComponent<Placeable>().m_IsPlaced == true && i < m_CorrOrder.Parts.Count - 1)
            {
                foreach (Sprite img in m_Thumbnails)
                {
                    if (img.name == m_CorrOrder.Parts[i+1].obj.name + "_Thumbnail")
                    {
                        m_HintImage.sprite = img;
                        if(m_CorrOrder.Parts[i + 1].obj.name == "Set0")
                        {
                            PartName.text = m_CorrOrder.Parts[i + 1].obj.name.Replace("Set0", "Front shield");
                        }
                        if (m_CorrOrder.Parts[i + 1].obj.name == "Set1")
                        {
                            PartName.text = m_CorrOrder.Parts[i + 1].obj.name.Replace("Set1", "Back shield");
                        }
                        else
                        {
                            PartName.text = m_CorrOrder.Parts[i + 1].obj.name.Replace('_', ' ');
                        }
                    }
                }
            }
            else
            {
                foreach (Sprite img in m_Thumbnails)
                {
                    if (img.name == m_CorrOrder.Parts[i].obj.name + "_Thumbnail")
                    {
                        m_HintImage.GetComponentInChildren<Image>().sprite = img;
                        if (m_CorrOrder.Parts[i + 1].obj.name == "Set0")
                        {
                            PartName.text = m_CorrOrder.Parts[i].obj.name.Replace("Set0", "Front shield");
                        }
                        if (m_CorrOrder.Parts[i + 1].obj.name == "Set1")
                        {
                            PartName.text = m_CorrOrder.Parts[i].obj.name.Replace("Set1", "Back shield");
                        }
                        else
                        {
                            PartName.text = m_CorrOrder.Parts[i].obj.name.Replace('_', ' ');
                        }
                    }
                }
                break;
            }
        }

    }

}
