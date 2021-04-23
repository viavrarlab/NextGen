using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using System.Text;

public class LeftHandUIController : MonoBehaviour
{
    [SerializeField]
    Canvas m_ControllerUI;
    [SerializeField]
    Image m_HintImage = null;
    [SerializeField]
    Text PartName = null;

    UnityEngine.Object[] m_Thumbnails;

    [SerializeField]
    PlacementPoint[] PP_Array;

    CorrectOrderTests m_CorrOrder;
    GameControllerSC m_Gcontroller;
    private void Awake()
    {
        //m_ControllerUI = gameObject.GetComponentInChildren<Canvas>();
        m_ControllerUI.enabled = false;
        m_CorrOrder = FindObjectOfType<CorrectOrderTests>();
        string[] NameArray = m_CorrOrder.Parts.Select(x => x.obj.name+"_Socket").ToArray();
        m_Gcontroller = FindObjectOfType<GameControllerSC>();
        PlacementPoint[] TempArray = FindObjectsOfType<PlacementPoint>();
        PP_Array = TempArray.OrderBy(x => Array.IndexOf(NameArray, x.name)).ToArray();
        //PP_Array = TempArray.OrderBy()
        //PP_Array = TempArray;
        Array.Clear(TempArray, 0, TempArray.Length);
    }
    public void EnableUI()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            if (m_Gcontroller != null)
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

        for (int i = 0; i < PP_Array.Length; i++)
        {
            if (PP_Array[i].m_IsOccupied == true && i < PP_Array.Length - 1)
            {
                foreach (Sprite img in m_Thumbnails)
                {
                    if (img.name == PP_Array[i+1].name.Replace("_Socket", "_Thumbnail"))
                    {
                        m_HintImage.sprite = img;
                        if (PP_Array[i + 1].name == "Set0_Socket")
                        {
                            PartName.text = PP_Array[i + 1].name.Replace("Set0_Socket", "Assembled Front End shield");
                        }
                        if (PP_Array[i + 1].name == "Set1_Socket")
                        {
                            PartName.text = PP_Array[i + 1].name.Replace("Set1", "Back shield");
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder(PP_Array[i+1].name);
                            sb.Replace("_", " ");
                            sb.Replace("Socket", " ");
                            PartName.text = sb.ToString();
                            //PartName.text = PP_Array[i + 1].name.Replace("_", " ");
                        }
                    }
                }
            }
            else
            {
                foreach (Sprite img in m_Thumbnails)
                {
                    if (img.name == PP_Array[i].name.Replace("_Socket", "_Thumbnail"))
                    {
                        m_HintImage.GetComponentInChildren<Image>().sprite = img;
                        if (PP_Array[i].name == "Set0_Socket")
                        {
                            PartName.text = PP_Array[i].name.Replace("Set0_Socket", "Assembled Front End shield");
                        }
                        if (PP_Array[i].name == "Set1_Socket")
                        {
                            PartName.text = PP_Array[i].name.Replace("Set1_Socket", "Assembled Back end shield");
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder(PP_Array[i].name);
                            sb.Replace("_", " ");
                            sb.Replace("Socket", " ");
                            PartName.text = sb.ToString();
                            //PartName.text = PP_Array[i].name.Replace("_", " ");
                        }
                    }
                }
                break;
            }
        }

    }

}
