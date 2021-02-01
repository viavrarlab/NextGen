using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListEntryValues : MonoBehaviour
{
    [SerializeField]
    GameObject Image;
    [SerializeField]
    GameObject PartNameText;
    [SerializeField]
    GameObject PartCountText;
    [SerializeField]
    GameObject PartTimerText;

    public void UpdateTextsAndImage(Sprite thumbnail,string PartName, string PartPicCount, string Timer)
    {
        Image.GetComponent<Image>().sprite = thumbnail;
        PartNameText.GetComponent<Text>().text = PartName;
        PartCountText.GetComponent<Text>().text = PartPicCount;
        PartTimerText.GetComponent<Text>().text = Timer;
    }
    public void UpdateTexts(string PartName, string PartPicCount, string Timer)
    {
        PartNameText.GetComponent<Text>().text = PartName;
        PartCountText.GetComponent<Text>().text = PartPicCount;
        PartTimerText.GetComponent<Text>().text = Timer;
    }

}
