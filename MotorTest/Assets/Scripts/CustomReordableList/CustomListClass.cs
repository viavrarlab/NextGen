using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CustomListClass
{
    public GameObject obj;
    public int set;
    public bool OrderNotMandatory;
    public bool DifferentObject;

    public override string ToString()
    {
        return string.Join(",",new string[] { obj.name.ToString(), set.ToString(), OrderNotMandatory.ToString(), DifferentObject.ToString() });
    }
    public CustomListClass()
    {

    }
    public CustomListClass(string x)
    {
        
    }
    public static CustomListClass FromCSV(string csvLine)
    {
        string[] values = csvLine.Split(',');
        CustomListClass Item = new CustomListClass();
        Item.obj = GameObject.Find(values[0]);
        Item.set = Convert.ToInt32(values[1]);
        Item.OrderNotMandatory = Convert.ToBoolean(values[2]);
        Item.DifferentObject = Convert.ToBoolean(values[3]);
        return Item;
    }
}
