using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListEntryValues : MonoBehaviour
{
    [SerializeField]
    GameObject Image = null;
    [SerializeField]
    GameObject PartNameText = null;
    [SerializeField]
    GameObject PartCountText = null;
    [SerializeField]
    GameObject PartTimerText = null;
    [SerializeField]
    GameObject CorrectPlaceIndex = null;
    [SerializeField]
    GameObject ActualPlacementIndex = null;

    public void UpdateTextsAndImage(Sprite thumbnail,string PartName, string PartPicCount, string Timer, string corrOrder,string ActualOrder)
    {
        Image.GetComponent<Image>().sprite = thumbnail;
        PartNameText.GetComponent<Text>().text = PartName;
        PartCountText.GetComponent<Text>().text = PartPicCount;
        PartTimerText.GetComponent<Text>().text = Timer;
        CorrectPlaceIndex.GetComponent<Text>().text = corrOrder;
        ActualPlacementIndex.GetComponent<Text>().text = ActualOrder;
    }
    public void UpdateTexts(string PartName, string PartPicCount, string Timer, string corrOrder, string ActualOrder)
    {
        PartNameText.GetComponent<Text>().text = PartName;
        PartCountText.GetComponent<Text>().text = PartPicCount;
        PartTimerText.GetComponent<Text>().text = Timer;
        CorrectPlaceIndex.GetComponent<Text>().text = corrOrder;
        ActualPlacementIndex.GetComponent<Text>().text = ActualOrder;

    }

}
