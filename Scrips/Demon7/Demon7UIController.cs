using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using LPlan;
using UnityEngine.UI;
public class Demon7UIController : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler {

    public GameObject m_Menu;

    public Text alText;

    private bool Moving = false;
    private bool IsOpen = false;
    // Use this for initialization
    void Start () {
        LocationMap.GetInstance.Initialize();
        alText.DOText("右划菜单", 2.5f);
        alText.DOFade(0.5f, 1).SetLoops(-1, LoopType.Yoyo);
    }
	

    private Vector2 startPos;
    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 endPos = eventData.position;
        if(endPos.x > startPos.x)//右
        {
            OpenMenu();
        }
        else//左
        {
            CloseMenu();
        }
    }


    private void OpenMenu()
    {
        if (Moving == true || IsOpen == true)
            return;
        Moving = true;
        Vector2 pos = m_Menu.GetComponent<RectTransform>().localPosition;
        Rect rect = m_Menu.GetComponent<RectTransform>().rect;
        var tween = m_Menu.transform.DOLocalMoveX(pos.x + rect.width, 0.5f);
        IsOpen = true;

        tween.onComplete = () => MoveComplete();
    }

    private void CloseMenu()
    {
        if (Moving == true || IsOpen == false)
            return;
        Moving = true;
        Vector2 pos = m_Menu.GetComponent<RectTransform>().localPosition;
        Rect rect = m_Menu.GetComponent<RectTransform>().rect;
        var tween = m_Menu.transform.DOLocalMoveX(pos.x - rect.width, 0.85f);
        IsOpen = false;

        tween.onComplete= () => MoveComplete();
    }
    private void MoveComplete()
    {
        Moving = false;
    }

    // Update is called once per frame
    void Update () {
		
	}
    
}
