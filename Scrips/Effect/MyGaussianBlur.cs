using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
//using System.Security.Cryptblurography;

public class MyGaussianBlur : PostEffectsBase
{

    public Shader GaussianBlurShader;
    private Material GaussianBlurMat;
    public Material mat
    {
        get
        {
            if (GaussianBlurMat == null)
            {
                GaussianBlurMat = CheckShaderAndCreateMaterial(GaussianBlurShader, GaussianBlurMat);
            }
            return GaussianBlurMat;
        }
    }

    [Range(0, 8)]
    public int downSample;

    /// <summary>
    /// 迭代次数
    /// </summary>
    [Range(0, 4)]
    public int iterations;

    /// <summary>
    /// 斑点范围
    /// </summary>
    [Range(0, 10)]
    public float blurRadius;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (mat != null)
        {

            mat.SetFloat("_BlurRadius", blurRadius);
            int rtW = src.width / downSample;
            int rtH = src.height / downSample;
            
            //获取临时渲染纹理
            //最后一个参数depth，当为0时，表示不产生深度Z的缓冲
            RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
            buffer0.filterMode = FilterMode.Bilinear;

            Graphics.Blit(src, buffer0);
            for (int i = 0; i < iterations; i++)
            {

                //pass 0 ，在水平方向上进行模糊
                RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
                Graphics.Blit(buffer0, buffer1, mat, 0);

                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;

                buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

                Graphics.Blit(buffer0, buffer1, mat, 1);

                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
                


                //RenderTexture.ReleaseTemporary (buffer1);
            }

            Graphics.Blit(buffer0, dest);
            RenderTexture.ReleaseTemporary(buffer0);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
