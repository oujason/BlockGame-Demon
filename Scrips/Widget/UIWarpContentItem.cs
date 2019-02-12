using UnityEngine;
using System.Collections;

/***
 *@des:warp下Element对应标记
 */
[DisallowMultipleComponent]
public class UIWarpContentItem : MonoBehaviour
{
    private int index;
    private UIWarpContent warpContent;

    void OnDestroy()
    {
        warpContent = null;
    }

    public UIWarpContent WarpContent
    {
        set
        {
            warpContent = value;
        }
    }

    public int Index
    {
        set
        {
            index = value;
            transform.localPosition = warpContent.getLocalPositionByIndex(index);
            //gameObject.name = (index < 10) ? ("0" + index) : ("" + index);
            gameObject.name = "ScrollItem" + index;
            if (warpContent.onInitializeItem != null && index >= 0)
            {
				Debug.Assert (null != gameObject, "gameObject is null原因不明");//20181023
                warpContent.onInitializeItem(gameObject, index);
                
            }
        }
        get
        {
            return index;
        }
    }

}