Shader "Custom/OutlineFlashMetallic"
{
    Properties
    {
        _Color ("Flash Color", Color) = (1, 0, 0, 1)
        _FlashIntensity ("Flash Intensity", Range(1, 10)) = 2
        _Metallic ("Metallic", Range(0,1)) = 0.5
        _Gloss ("Gloss", Range(0,1)) = 0.6
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Cull Back
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldView : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            fixed4 _Color;
            float _FlashIntensity;
            float _Metallic;
            float _Gloss;

            v2f vert (appdata v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldView = normalize(_WorldSpaceCameraPos - worldPos);
                o.worldPos = worldPos;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float fresnel = pow(1.0 - saturate(dot(i.worldNormal, i.worldView)), 5.0);
                float metallicLight = _Gloss * _Metallic * fresnel;
                float3 finalColor = (_Color.rgb * _FlashIntensity) + metallicLight;
                return fixed4(finalColor, _Color.a);
            }
            ENDCG
        }
    }
}