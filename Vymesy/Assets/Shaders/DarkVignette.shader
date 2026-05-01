// Dark vignette overlay. Draw it on a fullscreen quad parented to the main camera
// to give the scene a creepy "shadows close in" effect.
//
// Built-in render pipeline. Works as either an OnRenderImage post effect (write a
// minimal Blit script) or as a giant transparent quad pinned to the camera.
Shader "Vymesy/DarkVignette"
{
    Properties
    {
        _Tint ("Tint", Color) = (0.05, 0.03, 0.07, 1)
        _Strength ("Vignette Strength", Range(0, 4)) = 1.6
        _Power ("Vignette Power", Range(0.5, 8)) = 2.5
        _PulseSpeed ("Pulse Speed", Range(0, 4)) = 0.6
        _PulseAmount ("Pulse Amount", Range(0, 0.6)) = 0.15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Overlay"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            fixed4 _Tint;
            float _Strength;
            float _Power;
            float _PulseSpeed;
            float _PulseAmount;

            v2f vert(appdata v) { v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.uv = v.uv; return o; }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 c = i.uv - 0.5;
                float d = saturate(length(c) * 2.0); // 0 at center, ~1 at corners
                float pulse = 1 + sin(_Time.y * _PulseSpeed) * _PulseAmount;
                float vignette = pow(d, _Power) * _Strength * pulse;
                fixed4 col = _Tint;
                col.a = saturate(vignette);
                return col;
            }
            ENDCG
        }
    }
}
