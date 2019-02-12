using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShaderController : MonoBehaviour {

    public Image targetImg;
    public Button AlphaBtn;
    public Button closeBtn;
    // Use this for initialization
    void Start () {
        AlphaBtn.onClick.AddListener(delegate () {
            var alphaEffect = targetImg.gameObject.GetComponent<SetFlowTexMaterial>();
            if(alphaEffect == null)
                alphaEffect = targetImg.gameObject.AddComponent<SetFlowTexMaterial>();

            if (alphaEffect.CanRun)
                alphaEffect.Stop();
            else
                alphaEffect.Begin();


        });

        closeBtn.onClick.AddListener(delegate () { GameEnvironment.GetInstance.LoadScene("Menu", true); });
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
