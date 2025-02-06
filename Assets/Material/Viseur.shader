Shader "Custom/ScopeShader"
{
    Properties
    {
        _CircleSize ("Circle Size", Range(0, 1)) = 0.3
        _SoftEdge ("Soft Edge", Range(0, 0.2)) = 0.05
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Pass
        {
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

            float _CircleSize;
            float _SoftEdge;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);

                // Transition douce entre le centre et les bords
                float mask = smoothstep(_CircleSize, _CircleSize + _SoftEdge, dist);

                // Appliquer la transparence aux bords
                return float4(0, 0, 0, mask);
            }
            ENDCG
        }
    }
}
