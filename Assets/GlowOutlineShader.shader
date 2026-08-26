// Shader Graph ではなく ShaderLab で、輪郭が光る 2D Sprite 用の Shader を出力します
// URP 用、Sprite Lit Shader をベースにした発光輪郭エフェクト

Shader "Custom/GlowOutlineShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineThickness ("Outline Thickness", Float) = 1
        _EmissionPower ("Emission Power", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 200

        Pass
        {
            Name "OUTLINE"
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineThickness;
            float _EmissionPower;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 offset = _OutlineThickness / _ScreenParams.xy;
                float alpha = 0.0;
                // 8方向に拡張して境界線を抽出
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        float2 uvOffset = float2(x, y) * offset;
                        alpha = max(alpha, tex2D(_MainTex, i.uv + uvOffset).a);
                    }
                }

                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                float centerAlpha = col.a;
                float outlineOnly = saturate(alpha - centerAlpha);

                fixed4 result = col;
                result.rgb += _OutlineColor.rgb * outlineOnly * _EmissionPower;
                result.a = max(result.a, outlineOnly);
                return result;
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}
