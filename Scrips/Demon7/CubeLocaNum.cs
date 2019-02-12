using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LPlan;
using System.Text.RegularExpressions;
using System;

public class CubeLocaNum : MonoBehaviour {

    public short[] LocationNum;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeLocaction(int targetNum, bool forward)
    {
        string locaData= string.Empty;
        for (int i = 0; i < LocationNum.Length; i++)
        {
            if (!string.IsNullOrEmpty(locaData))
                locaData += LocationNum[i];
            else
                locaData = LocationNum[i].ToString();
        }

        string result = LocationMap.GetInstance.GetPlan(locaData, targetNum, forward);

        string[] str= Regex.Split(result,"");

        LocationNum[0] = short.Parse(str[1]);
        LocationNum[1] = short.Parse(str[2]);
        LocationNum[2] = short.Parse(str[3]);
    }
}
