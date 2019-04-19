// set as transparet in rendering mode (materials -> inspector)

Shader "Stimuli/SemiTransparentBlinking"
{
    Properties
    {
		_MainTex("Base (RGB)", 2D) = "white" {} // grey, white or black
		_BlinkRate("Blink Rate", Range(0.0, 1000.0)) = 5
		_Transparency("Transparency", Range(0.0,1.0)) = 0.25
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Opaque" }
        LOD 100

		//ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
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
			float _BlinkRate;
			float _Transparency;
			

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

			/* Gets called once per pixel */
            fixed4 frag (v2f i) : SV_Target
            {
				// fixed4 is used to represent the color in R,G,B,A. The color values range from 0-1, e.g. {0,0,0,0] = BLACK.
                fixed4 col = tex2D(_MainTex, i.uv); // gets the pixel (color) from the UV coordinate from the texture
				col = abs(sin(2 * 3.14159 * _BlinkRate * _Time.y)); // multiple col with values ranging from 0-1 at a frequency of _BlinkRate
				col.a = _Transparency; 
                return col;
            }
            ENDCG
        }
    }
}
