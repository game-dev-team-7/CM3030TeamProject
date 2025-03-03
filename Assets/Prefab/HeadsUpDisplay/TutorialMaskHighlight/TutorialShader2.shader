Shader "Custom/HardcodedTwoRectangularHighlightShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Overlay Color", Color) = (0, 0, 0, 0.7) // Semi-transparent black
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "RenderType"="Transparent"
        }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
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
            float4 _Color;

            // Hardcoded rectangle centers and sizes
            static const float2 rect1Center = float2(0.5, 0.65); // Center of Highlight 1
            static const float2 rect1Size = float2(0.075, 0.04); // Size of Highlight 1
            static const float2 rect2Center = float2(0.5, 0.345); // Center of Highlight 2
            static const float2 rect2Size = float2(0.2, 0.04); // Size of Highlight 2

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Calculate bounds for Rectangle 1
                float2 rect1Min = rect1Center - rect1Size * 0.5;
                float2 rect1Max = rect1Center + rect1Size * 0.5;

                // Calculate bounds for Rectangle 2
                float2 rect2Min = rect2Center - rect2Size * 0.5;
                float2 rect2Max = rect2Center + rect2Size * 0.5;

                // Check if the current pixel is inside either rectangle
                bool insideRect1 = all(i.uv >= rect1Min) && all(i.uv <= rect1Max);
                bool insideRect2 = all(i.uv >= rect2Min) && all(i.uv <= rect2Max);

                // If inside any rectangle, make it fully transparent
                if (insideRect1 || insideRect2)
                {
                    return fixed4(0, 0, 0, 0); // Transparent
                }

                // Otherwise, use the overlay color
                return _Color;
            }
            ENDCG
        }
    }
}