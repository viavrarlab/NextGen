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
    [SerializeField]
    GameObject CorrectPlaceIndex;
    [SerializeField]
    GameObject ActualPlacementIndex;

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
