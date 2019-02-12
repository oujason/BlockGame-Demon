using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Menu : MonoBehaviour {


    public UIWarpContent titleListUIWarpContent;
    List<string> titleList;

    public Text PathText;

    // Use this for initialization
    void Start() {
        var envir = GameObject.Find("GameEnvironment");
        if (envir == null)
        {
            GameObject gameEnvironment = new GameObject("GameEnvironment");
            gameEnvironment.AddComponent<GameEnvironment>();
        }
        else
        {
            envir.GetComponent<GameEnvironment>().UiRoot = GameObject.Find("UiRoot");
        }
        titleList = new List<string>();
        titleList.Add("爬虫 Demon");
        titleList.Add("Shader Demon");
        titleList.Add("屏幕后处理特效");
        //初始化 
        titleListUIWarpContent.onInitializeItem = OnInitMenuListItem;
        titleListUIWarpContent.Init(titleList.Count);

        //显示平台路径
        PathText.DOText(GameEnvironment.DataPath, 0.8f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        titleList.Clear();
        //清空
        titleListUIWarpContent.ClearAll();
        titleListUIWarpContent.Init(titleList.Count);
        titleListUIWarpContent.setScrollItem(0);
    }

    private void OnInitMenuListItem(GameObject go, int dataIndex)
    {
        go.transform.Find("Text").GetComponent<Text>().text = titleList[dataIndex];
        go.GetComponent<Image>().color = new Color(dataIndex * 0.1f, dataIndex * 0.1f, dataIndex * 0.1f);

        go.transform.Find("Button").GetComponent<Button>().onClick.AddListener(delegate () {
            EnterFunction(go,dataIndex);
        });
    }

    public void EnterFunction(GameObject go, int dataIndex)
    {
        switch (dataIndex)
        {
            case 0:
                Debug.Log("爬虫 Demon");
                GameEnvironment.GetInstance.LoadScene("WWWDemon",true);
                break;
            case 1:
                Debug.Log("Shader Demon");
                GameEnvironment.GetInstance.LoadScene("ShaderDemon", true);
                break;
            case 2:
                Debug.Log("屏幕后处理特效");
                //KUBIKOS - World/Demon/Scenes/
                UnityEngine.SceneManagement.SceneManager.LoadScene(@"Demo7");
                break;
            default:
                Debug.Log(" ");
                break;
        }
    }

}
