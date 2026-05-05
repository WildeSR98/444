// Cheap 2D sprite outline shader — draws a coloured outline around the sprite by sampling
// the alpha channel at 4 cardinal offsets. Use it for hovered enemies, bosses, or items.
//
// Built-in render pipeline. For URP, port the SubShader to a ShaderGraph or
// rewrite the pass with HLSL + URP includes.
Shader "Vymesy/SpriteOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1, 0.85, 0.4, 1)
        _OutlineThickness ("Outline Thickness (px)", Range(0, 8)) = 2
        _OutlinePulseSpeed ("Outline Pulse Speed", Range(0, 6)) = 2
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineThickness;
            float _OutlinePulseSpeed;

            v2f vert(appdata IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.uv;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.uv) * IN.color;

                // Sample neighbours to detect transparent edges and draw an outline ring.
                float2 t = _MainTex_TexelSize.xy * _OutlineThickness;
                float a0 = tex2D(_MainTex, IN.uv + float2( t.x, 0)).a;
                float a1 = tex2D(_MainTex, IN.uv + float2(-t.x, 0)).a;
                float a2 = tex2D(_MainTex, IN.uv + float2(0,  t.y)).a;
                float a3 = tex2D(_MainTex, IN.uv + float2(0, -t.y)).a;
                float neighbourAlpha = max(max(a0, a1), max(a2, a3));
                float edge = saturate(neighbourAlpha - c.a);

                float pulse = 0.6 + 0.4 * sin(_Time.y * _OutlinePulseSpeed);
                fixed4 outline = _OutlineColor * pulse * edge;

                fixed4 result = c + outline * (1 - c.a);
                result.rgb *= result.a;
                return result;
            }
            ENDCG
        }
    }
}
