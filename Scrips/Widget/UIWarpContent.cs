using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class UIWarpContent : MonoBehaviour
{
    private int count;
    public int Count { get { return count; } set { count = value; } }

    private int maxCount;
    public int MaxCount { get { return maxCount; } set { maxCount = value; } }

	public delegate void OnInitializeItem(GameObject go, int dataIndex);

	public OnInitializeItem onInitializeItem;
	public OnInitializeItem onUpdateItem;

    public enum Arrangement
    {
        Horizontal,
        Vertical,
    }

    /// <summary>
    /// Type of arrangement -- vertical or horizontal.
    /// </summary>

    public Arrangement arrangement = Arrangement.Horizontal;

    /// <summary>
    /// Maximum children per line.
    /// If the arrangement is horizontal, this denotes the number of columns.
    /// If the arrangement is vertical, this stands for the number of rows.
    /// </summary>
    [Range(1, 50)]
    public int maxPerLine = 1;

    /// <summary>
    /// The width of each of the cells.
    /// </summary>

    public float cellWidth = 200f;

    /// <summary>
    /// The height of each of the cells.
    /// </summary>

    public float cellHeight = 200f;

    /// <summary>
    /// The Width Space of each of the cells.
    /// </summary>
    [Range(0, 50)]
    public float cellWidthSpace = 0f;

    /// <summary>
    /// The Height Space of each of the cells.
    /// </summary>
    [Range(0, 50)]
    public float cellHeightSpace = 0f;

    [Range(0, 30)]
    public int viewCount = 5;

    public ScrollRect scrollRect;

    public RectTransform content;

    public GameObject goItemPrefab;

    private int dataCount;

    private int curScrollPerLineIndex = -1;

    private List<UIWarpContentItem> listItem;


    public int ScrollIndex { get { return curScrollPerLineIndex; } }

    void Awake()
    {
        
    }

    void Start()
    {
//        if (scrollRect != null)
//            scrollRect.scrollSensitivity = 20;
    }

    public void Init(int dataCount, int maxDataCount = 0)
    {
        count = dataCount;
        maxCount = maxDataCount;

        listItem = new List<UIWarpContentItem>();

        if (scrollRect == null || content == null || goItemPrefab == null)
        {
            Debug.LogError("异常:请检测<" + gameObject.name + ">对象上UIWarpContent对应ScrollRect、Content、GoItemPrefab 是否存在值...." + scrollRect + " _" + content + "_" + goItemPrefab);
            return;
        }
        if (dataCount <= 0)
        {
            return;
        }
        setDataCount(dataCount);

        listItem.Clear();

        setUpdateRectItem(0);

    }

    /// <summary>
    /// 清空
    /// </summary>
    public void ClearAll()
    {
        if (null == listItem || 0 == listItem.Count) //20170503
            return;
        for (int i = listItem.Count - 1; i >= 0; i--)
        {
            UIWarpContentItem item = listItem[i];
            GameObject.Destroy(item.gameObject);
        }
       
        setDataCount(0);

        listItem.Clear();

        setUpdateRectItem(0);
    }

    private void setDataCount(int count)
    {
        if (dataCount == count)
        {
            return;
        }
        dataCount = count;
        setUpdateContentSize();
    }
        

    /// <summary>
    /// 设置滚动
    /// </summary>
    /// <param name="scrollIndex">item索引</param>
    public void setScrollItem(int scrollIndex)
    {
        if (maxPerLine == 1)
        {
            if ((scrollIndex < 0) || scrollIndex > dataCount)
                return;

            setUpdateRectItem(scrollIndex);
            setScrollPosition(scrollIndex);
        }
        else
        {
            if (scrollIndex <= 1  || scrollIndex > dataCount)
                return;

            setUpdateRectItem(scrollIndex / maxPerLine);
            setScrollPosition(scrollIndex);
        }
    }

    private void setScrollPosition(int index)
    {
		Debug.Assert (null != scrollRect.content, "scrollRect.content is null");
		if (null == scrollRect.content)
			return;
		
        scrollRect.content.localPosition = getScrollPosition(index);
        
    }

    private Vector3 getScrollPosition(int index)
    {
        float x = 0f;
        float y = 0f;
        float z = 0f;
        switch (arrangement)
        {

            case Arrangement.Horizontal: //水平方向
                x = -(index / maxPerLine) * (cellWidth + cellWidthSpace);
                //y = (index % maxPerLine) * (cellHeight + cellHeightSpace);
                break;
            case Arrangement.Vertical://垂着方向
                //x = -(index % maxPerLine) * (cellWidth + cellWidthSpace);
                y = (index / maxPerLine) * (cellHeight + cellHeightSpace);
                break;
        }

        return new Vector3(x, y, z);
    }

    public Vector3 offsetScrollPosition()
    {
        Vector3 vec = getScrollPosition();

        int oldIndex = 0;
        switch (arrangement)
        {
            case Arrangement.Horizontal: //水平方向
                oldIndex = System.Math.Abs((int)System.Math.Floor(vec.x / (cellHeight + cellHeightSpace) * maxPerLine));
                break;
            case Arrangement.Vertical://垂着方向
                oldIndex = System.Math.Abs((int)System.Math.Floor(vec.y / (cellHeight + cellHeightSpace) * maxPerLine));
                break;
        }

        Vector3 diff = getScrollPosition(oldIndex);
        diff = vec - diff;

        return diff;
    }

    public void fixedScrollPosition(Vector3 vec, int index)
    {
        if (dataCount <= viewCount || index <= 1)
            return;

        Vector3 pos = getScrollPosition(index);
        pos += vec;
        scrollRect.content.localPosition = pos;

        setUpdateRectItem(index);
    }

    public Vector3 getScrollPosition()
    {
        return scrollRect.content.localPosition;
    }

  

    /**
	 * @des:设置更新区域内item
	 * 功能:
	 * 1.隐藏区域之外对象
	 * 2.更新区域内数据
	 */
    private void setUpdateRectItem(int scrollPerLineIndex)
    {
        if (scrollPerLineIndex < 0)
        {
            return;
        }
        curScrollPerLineIndex = scrollPerLineIndex;
        int startDataIndex = curScrollPerLineIndex * maxPerLine;

        //显示
        for (int dataIndex = startDataIndex; dataIndex < dataCount; dataIndex++)
        {
            if (dataIndex >= dataCount)
            {
                continue;
            }
            if (isExistDataByDataIndex(dataIndex))
            {
                continue;
            }
            createItem(dataIndex);
        }
    }

    /// <summary>
    /// 更新指定索引项
    /// </summary>
    /// <param name="dataIndex">Data index.</param>
    public void UpdateItem(int dataIndex)
    {
//        System.Action action = () =>
//        {
                var oldListCount = listItem.Count;
                var oldDataCount = dataCount;
                Debug.Assert (oldListCount >= oldDataCount,"list count err");
                DelItem(dataIndex);
				Debug.Assert (oldListCount == listItem.Count+1,"逻辑错误，DelItem后item数量错误"+oldListCount + "=>" +listItem.Count + " : " + goItemPrefab.name+",\tdataIndex="+dataIndex);//20171212
				Debug.Assert (oldDataCount == dataCount+1,"逻辑错误，DelItem后dataCount错误"+oldDataCount+"=>" + dataCount  + " : " +  goItemPrefab.name+",\tdataIndex="+dataIndex);//20171212

                AddItem(dataIndex);
				Debug.Assert (oldListCount == listItem.Count,"逻辑错误，UpdateItem后item数量变化"+oldListCount + "=>"  + goItemPrefab.name+",\tdataIndex="+dataIndex);//20171212
				Debug.Assert (oldDataCount == dataCount,"逻辑错误，UpdateItem后dataCount错误"+oldDataCount+"=>"  + goItemPrefab.name+",\tdataIndex="+dataIndex);//20171212
//        };
//        GameEnvironment.GetInstance.AddFrameAction(action);
    }
	public bool UpdateItem2(int dataIndex)
	{
		if (null == onUpdateItem) {
			Debug.LogError ("onUpdateItem is null");
			return false;
		}
		
		foreach (var item in listItem ) {
			if (item.Index != dataIndex)
				continue;

			onUpdateItem (item.gameObject, dataIndex);
			return true;
		}

		Debug.LogError ("UpdateItem2 err,dataIndex="+dataIndex+",\tlistItem.Count="+listItem.Count);
		return false;
	}

    /**
	 * @des:添加当前数据索引数据
	 */
    public void AddItem(int dataIndex)
    {
        if (listItem == null || listItem.Count <= 0)
        {
            Init(1);
            return;
        }
        if (dataIndex < 0 || dataIndex > dataCount)
        {
            return;
        }
        //检测是否需添加gameObject
        bool isNeedAdd = false;
        for (int i = listItem.Count - 1; i >= 0; i--)
        {
            UIWarpContentItem item = listItem[i];
            if (item.Index >= (dataCount - 1))
            {
                isNeedAdd = true;
                break;
            }
        }
        setDataCount(dataCount + 1);

        if (isNeedAdd)
        {
            for (int i = 0; i < listItem.Count; i++)
            {
                UIWarpContentItem item = listItem[i];
                int oldIndex = item.Index;
                if (oldIndex >= dataIndex)
                {
                    item.Index = oldIndex + 1;
                }
                item = null;
            }
            //setUpdateRectItem(getCurScrollPerLineIndex());
			setUpdateRectItem(0);//解决如果第一项被滚动后没显示出来的时候，更新有可能失败的问题20171212
        }
        else
        {
            //重新刷新数据
            for (int i = 0; i < listItem.Count; i++)
            {
                UIWarpContentItem item = listItem[i];
                int oldIndex = item.Index;
                if (oldIndex >= dataIndex)
                {
                    item.Index = oldIndex;
                }
                item = null;
            }
        }
    }

    /**
	 * @des:删除当前数据索引下数据
	 */
    public void DelItem(int dataIndex)
    {
        if (dataIndex < 0 || dataIndex >= dataCount)
        {
            return;
        }
        //删除item逻辑三种情况
        //1.只更新数据，不销毁gameObject,也不移除gameobject
        //2.更新数据，且移除gameObject,不销毁gameObject
        //3.更新数据，销毁gameObject

        bool isNeedDestroyGameObject = (listItem.Count >= dataCount);
        setDataCount(dataCount - 1);

        for (int i = listItem.Count - 1; i >= 0; i--)
        {
            UIWarpContentItem item = listItem[i];
            int oldIndex = item.Index;
            if (oldIndex == dataIndex)
            {
                listItem.Remove(item);
                if (isNeedDestroyGameObject)
                {
                    GameObject.Destroy(item.gameObject);
                }
                else
                {
                    item.Index = -1;
                }
            }
            if (oldIndex > dataIndex)
            {
                item.Index = oldIndex - 1;
            }
        }
        setUpdateRectItem(getCurScrollPerLineIndex());
    }

    /**
	 * @des:获取当前index下对应Content下的本地坐标
	 * @param:index
	 * @内部使用
	*/
    public Vector3 getLocalPositionByIndex(int index)
    {
        float x = 0f;
        float y = 0f;
        float z = 0f;
        switch (arrangement)
        {
            case Arrangement.Horizontal: //水平方向
                x = (index / maxPerLine) * (cellWidth + cellWidthSpace);
                y = -(index % maxPerLine) * (cellHeight + cellHeightSpace);
                break;
            case Arrangement.Vertical://垂着方向
                x = (index % maxPerLine) * (cellWidth + cellWidthSpace);
                y = -(index / maxPerLine) * (cellHeight + cellHeightSpace);
                break;
        }
        return new Vector3(x, y, z);
    }

    /**
	 * @des:创建元素
	 * @param:dataIndex
	 */
    private void createItem(int dataIndex)
    {
        UIWarpContentItem item;
        item = addChild(goItemPrefab, content).AddComponent<UIWarpContentItem>();
        item.WarpContent = this;
        item.Index = dataIndex;
        listItem.Add(item);
    }
        

    /**
	 * @des:当前数据是否存在List中
	 */
    private bool isExistDataByDataIndex(int dataIndex)
    {
        if (listItem == null || listItem.Count <= 0)
        {
            return false;
        }
        for (int i = 0; i < listItem.Count; i++)
        {
            if (listItem[i].Index == dataIndex)
            {
                return true;
            }
        }
        return false;
    }

    /**
	 * @des:根据Content偏移,计算当前开始显示所在数据列表中的行或列
	 */
    public int getCurScrollPerLineIndex()
    {
        switch (arrangement)
        {
            case Arrangement.Horizontal: //水平方向
                return Mathf.FloorToInt(Mathf.Abs(content.anchoredPosition.x) / (cellWidth + cellWidthSpace));
            case Arrangement.Vertical://垂着方向
                return Mathf.FloorToInt(Mathf.Abs(content.anchoredPosition.y) / (cellHeight + cellHeightSpace));
        }
        return 0;
    }

    /**
	 * @des:更新Content SizeDelta
	 */
    private void setUpdateContentSize()
    {
        int lineCount = Mathf.CeilToInt((float)dataCount / maxPerLine);
        switch (arrangement)
        {
            case Arrangement.Horizontal:
                content.sizeDelta = new Vector2(cellWidth * lineCount + cellWidthSpace * (lineCount - 1), content.sizeDelta.y);
                break;
            case Arrangement.Vertical:
                content.sizeDelta = new Vector2(content.sizeDelta.x, cellHeight * lineCount + cellHeightSpace * (lineCount - 1));
                break;
        }
    }

    /**
	 * @des:实例化预设对象 、添加实例化对象到指定的子对象下
	 */
    private GameObject addChild(GameObject goPrefab, Transform parent)
    {
        if (goPrefab == null || parent == null)
        {
            Debug.LogError("异常。UIWarpContent.cs addChild(goPrefab = null  || parent = null)");
            return null;
        }
        GameObject goChild = GameObject.Instantiate(goPrefab) as GameObject;
        goChild.layer = parent.gameObject.layer;
        goChild.transform.SetParent(parent, false);
        return goChild;
    }

    void OnDestroy()
    {

        scrollRect = null;
        content = null;
        goItemPrefab = null;
        onInitializeItem = null;

        if (listItem != null)
        {
            listItem.Clear();
        }

        listItem = null;
    }




    private UIWarpContentItem findCurrentIndexItem(int index)
    {
        foreach (var item in listItem)
        {
            if (index == item.Index)
            {
                return item;
            }
        }
        return null;
    }
}