// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Edu/GaussianBlur" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BlurRadius("Blur Radius",Range(0,10)) = 5
        _Color("Color Tint",Color)= (1,1,1,1)
    }
    
    SubShader {
	CGINCLUDE

        #include "unityCG.cginc"
        sampler2D _MainTex;
        float4 _MainTex_TexelSize;
        half _BlurRadius;

        struct a2v
        {
            float4 vertex:POSITION;
            float2 texcoord:TEXCOORD0;
        };

        struct v2f
        {
            float4 pos:SV_POSITION;
            half2 uv[5]:TEXCOORD0;
        };

        v2f vertBlurVertical(a2v v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv[0] = v.texcoord ;
            o.uv[1] = v.texcoord + _BlurRadius * float2(0,1) * _MainTex_TexelSize;
            o.uv[2] = v.texcoord + _BlurRadius * float2(0,-1) * _MainTex_TexelSize;
            o.uv[3] = v.texcoord + _BlurRadius * float2(0,2) * _MainTex_TexelSize;
            o.uv[4] = v.texcoord + _BlurRadius * float2(0,-2) * _MainTex_TexelSize;
            return o;
        }

        v2f vertBlurHorizontal(a2v v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv[0] = v.texcoord ;
            o.uv[1] = v.texcoord + _BlurRadius * float2(1,0) * _MainTex_TexelSize;
            o.uv[2] = v.texcoord + _BlurRadius * float2(-1,0) * _MainTex_TexelSize;
            o.uv[3] = v.texcoord + _BlurRadius * float2(2,0) * _MainTex_TexelSize;
            o.uv[4] = v.texcoord + _BlurRadius * float2(-2,0) * _MainTex_TexelSize;
            return o;
        }

        fixed4 fragBlur(v2f i):SV_Target
        {
            float weight[3] = {0.4026,0.2442,0.0545};

            fixed3 sum = tex2D(_MainTex,i.uv[0]).rgb * weight[0];

            for(int it=1; it<3; it++){
                sum += tex2D(_MainTex, i.uv[2*it]).rgb * weight[it];
                sum += tex2D(_MainTex, i.uv[2*it-1]).rgb * weight[it];
            }

            return fixed4(sum,1.0);
        }
    ENDCG

        Tags { "RenderType"="Opaque" }
        ZTest Always
		Cull Off
        ZWrite Off

		Pass 
        {
            CGPROGRAM
            #pragma vertex vertBlurVertical
            #pragma fragment fragBlur
            ENDCG
        }
        
        Pass 
        {
           CGPROGRAM
            #pragma vertex vertBlurHorizontal
            #pragma fragment fragBlur
            ENDCG
        }

		
    }
    FallBack "Diffuse"
}
