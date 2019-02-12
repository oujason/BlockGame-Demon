using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using LPlan;
public class RandomMove : MonoBehaviour {

    public GameObject AnchorPos;

    public GameObject[] Cubes;

    private bool Moving = false;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RandomMoveOnceX(int targetNum,bool forward)
    {
        var curValue = AnchorPos.transform.localRotation;
        float value = curValue.x + (forward ? 90 : -90);
        var tween = AnchorPos.transform.DOLocalRotate(new Vector3(value, curValue.y, curValue.z), 1f);
        tween.onComplete = () => MoveComplete(targetNum,forward);
    }

    public void RandomMoveOnceY(int targetNum,bool forward)
    {
        
        var curValue = AnchorPos.transform.localRotation;
        float value = curValue.y + (!forward ? 90 : -90);
        var tween = AnchorPos.transform.DOLocalRotate(new Vector3(curValue.x, value, curValue.z), 1f);
        tween.onComplete = () => MoveComplete(targetNum,forward);
    }

    public void RandomMoveOnceZ(int targetNum,bool forward)
    {
        var curValue = AnchorPos.transform.localRotation;
        float value = curValue.z + (!forward ? 90 : -90);
        var tween = AnchorPos.transform.DOLocalRotate(new Vector3(curValue.x, curValue.y, value), 1f);
        tween.onComplete = () => MoveComplete(targetNum,forward);
    }

    private void MoveComplete(int targetNum, bool forward)
    {
        ChangeLocationNum(targetNum,forward);
        

        //复原夫节点
        List<Transform> plst = new List<Transform>();
        foreach(Transform cube in AnchorPos.transform)
        {
            plst.Add(cube);
        }
        foreach(var data in plst)
        {
            data.SetParent(AnchorPos.transform.parent);
        }

        //还原轴物体角度
        AnchorPos.transform.localRotation = new Quaternion(0, 0, 0,0);

        Moving = false;
    }

    public void RandomOrder()
    {
        if (Moving == true)
            return;

        int targetNum = Random.Range(1, 10);

        bool forward = Random.Range(0, 2)==0?true:false;

        //int targetNum = 9;
        //bool forward = false;
        Debug.Log("plan" + (forward == false ? 9 + targetNum : targetNum));
        SetCubesParent(targetNum, forward);
        Moving = true;

        
        if (targetNum < 4)
        {
            RandomMoveOnceX(targetNum,forward);
        }
        else if(targetNum<7)
        {
            RandomMoveOnceY(targetNum,forward);
        }
        else
        {
            RandomMoveOnceZ(targetNum,forward);
        }
        
    }


    public void SetCubesParent(int targetNum, bool forward)
    {
        int NUM = 0;
        foreach(var cube in Cubes)
        {            
            CubeLocaNum data = cube.GetComponent<CubeLocaNum>();

            string locaData = string.Empty;
            for (int i = 0; i < data.LocationNum.Length; i++)
            {
                if (!string.IsNullOrEmpty(locaData))
                    locaData += data.LocationNum[i];
                else
                    locaData = data.LocationNum[i].ToString();
            }
            string plan = LocationMap.GetInstance.GetPlan(locaData, targetNum, forward);

            if (plan == "999")
                Debug.LogError("999"+ locaData);

            if(plan != "")
            {
                //Debug.Log("儿子"+locaData);
                cube.transform.SetParent(AnchorPos.transform);
                NUM++;
            }
        }
        Debug.Log("总数:" + NUM);

    }


    public void ChangeLocationNum(int targetNum, bool forward)
    {
        foreach (Transform cube in AnchorPos.transform)
        {
            CubeLocaNum data = cube.GetComponent<CubeLocaNum>();
            data.ChangeLocaction(targetNum,forward);
        }
    }
}
