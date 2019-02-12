using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SetImageAlpha : PostEffectsBase
{
    [Range(0, 1)]
    public float leftX = 0;
    [Range(0, 1)]
    public float rightX = 0;
    [Range(0, 1)]
    public float topY = 0;
    [Range(0, 1)]
    public float bottomY = 0;
    [Range(-2, 0)]
    public float alphaSmooth = 0;
    // Use this for initialization
    public Shader alphaShader;
    private Material _materal;
    public Material _Material
    {
        get
        {
            _materal = CheckShaderAndCreateMaterial(alphaShader, _materal);
            return _materal;
        }
    }
    private void Awake()
    {
        alphaShader = Shader.Find("Unlit/ImageAlpha");
    }
    // Update is called once per frame
    void Update()
    {
        _Material.SetFloat("_AlphaLX", leftX * 2);
        _Material.SetFloat("_AlphaRX", ((1 - rightX) - 0.5f) * 2);
        _Material.SetFloat("_AlphaTY", ((1 - topY) - 0.5f) * 2);
        _Material.SetFloat("_AlphaBY", bottomY * 2);
        _Material.SetFloat("_AlphaPower", alphaSmooth);
        //变量的计算只是为了映射范围
        GetComponent<Image>().material = _Material;
    }
}
