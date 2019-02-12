using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class WWWController : MonoBehaviour {

    private Button closeBtn;
    private Button showBtn;
    private Text infoText;
    private Text urlText;
    private Text picText;
    private Image Img;

    /// <summary>
    /// 网络下载的图片
    /// </summary>
    private Texture2D img = null;
    /// <summary>
    /// 本地图片
    /// </summary>
    private Texture2D img2 = null;

    enum GetPicType
    {
        DownLoad = 0,
        LocalLoad,
    }

    // Use this for initialization
    void Start () {
        showBtn = transform.Find("Button").GetComponent<Button>();
        closeBtn = transform.Find("CloseBtn").GetComponent<Button>();
        Img = transform.Find("Image").GetComponent<Image>();

        infoText = transform.Find("Text").GetComponent<Text>();
        picText = transform.Find("PicUrl").GetComponent<Text>();
        urlText = transform.Find("URL").GetComponent<Text>();
        urlText.text = goodUrl;

        closeBtn.onClick.AddListener(delegate() { GameEnvironment.GetInstance.LoadScene("Menu", true); });



        showBtn.onClick.AddListener(OnShowInfo);


        //设置图片路径
        if (!Directory.Exists(Application.streamingAssetsPath + "/1.png"))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    string goodUrl = "http://www.lssdjt.com/";

    //图片地址
    string imageUrl = string.Empty;

    public void OnShowInfo()
    {
        infoText.text = WriteStream(goodUrl);

        picText.text = "PicUrl:"+imageUrl;

        if(imageUrl != string.Empty)
            StartCoroutine(this.DownLoadTexture(imageUrl, GetPicType.DownLoad));
    }    
    

    public string WriteStream(string url)
    {
        
        WebRequest wr = WebRequest.Create(url);
        Stream s = wr.GetResponse().GetResponseStream();
        StreamReader sr = new StreamReader(s, Encoding.Default);

        string all = sr.ReadToEnd(); 

        int start = 0;
        int end = 0;

        //选一个图片地址
        if (all.Contains(".jpg"))
        {
            string startStr = "http://img";
            string endStr = ".jpg";
            start = all.IndexOf(startStr);
            end = all.IndexOf(endStr);
            string content = all.Substring(start, end - start + endStr.Length);
            imageUrl = content;
        }

        //文字
        string sa = string.Empty; ;
        while (all.Contains("<i>"))
        {           
            start = all.IndexOf("<i>");
            end = all.IndexOf("</i>");
            string content = all.Substring(start + 3, end - start - 3);
            all = all.Substring(end + 1, all.Length - (end + 1));
            sa += content + "。";
        }

        return sa;

    }


    IEnumerator DownLoadTexture(string url, GetPicType getType)
    {
        WWW www = new WWW(url);
        Texture2D tempImage;
        yield return www;
        if (www.isDone && www.error == null)
        {
            switch (getType)
            {
                case GetPicType.DownLoad:
                    {
                        this.img = www.texture;
                        tempImage = this.img;
                        break;
                    }
                case GetPicType.LocalLoad:
                    this.img2 = www.texture;
                    tempImage = this.img;
                    Debug.Log(tempImage.width + "  " + tempImage.height);
                    break;
                default:
                    tempImage = null;
                    break;
            }
            if (tempImage != null)
            {
                byte[] data = tempImage.EncodeToPNG();
                File.WriteAllBytes(Application.streamingAssetsPath + "/1.png", data);
                Debug.Log(Application.streamingAssetsPath + "/1.png");
                StartCoroutine(LoadLocalImage(Application.streamingAssetsPath + "/1.png"));
            }


                
        }
    }

    IEnumerator LoadLocalImage(string filePath)
    {
        //string filePath = "file:///" + path + url.GetHashCode();

        Debug.Log("getting local image:" + filePath);
        WWW www = new WWW(filePath);
        yield return www;

        Texture2D texture = www.texture;
        Sprite m_sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
        Img.sprite = m_sprite;
    }
}


