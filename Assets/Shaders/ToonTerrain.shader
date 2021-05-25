Shader "Toon/Terrain"
{
    Properties
    {
        //_Color ("Color", Color) = (0.4,0.4,0.4,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Emission ("Emission", Range (0,1)) = 0.5
    }
    SubShader
    {
        Tags { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
        }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        //fixed4 _Color;
        fixed _Emission;

        float4 _StartCols[3];
        float4 _EndCols[3];

        void surf (Input IN, inout SurfaceOutput o)
        {
            int colIndex = round(IN.uv_MainTex.x);
            float t = IN.uv_MainTex.y;

            o.Albedo = lerp(tex2D (_MainTex, IN.uv_MainTex) * _StartCols[colIndex], tex2D (_MainTex, IN.uv_MainTex) * _EndCols[colIndex], t);
            o.Emission = lerp(tex2D (_MainTex, IN.uv_MainTex) * _StartCols[colIndex] * _Emission, tex2D (_MainTex, IN.uv_MainTex) * _EndCols[colIndex] * _Emission, t);
            o.Alpha = lerp(_StartCols[colIndex].a, _EndCols[colIndex].a, t);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
