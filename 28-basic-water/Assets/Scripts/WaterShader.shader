Shader "Minecraft/WaterShader" {

	Properties {
		_MainTex ("First Texture", 2D) = "white" {}
		_SecondaryTex("Second Texture", 2D) = "white" {}
	}

	SubShader {
		
		Tags {"Queue"="Transparent" "RenderType"="Transparent"}
		LOD 100
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
		
			CGPROGRAM
				#pragma vertex vertFunction
				#pragma fragment fragFunction
				#pragma target 2.0

				#include "UnityCG.cginc"

				struct appdata {
				
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					float4 color : COLOR;

				};

				struct v2f {
				
					float4 vertex : SV_POSITION;
					float2 uv : TEXCOORD0;
					float4 color : COLOR;

				};

				sampler2D _MainTex;
				sampler2D _SecondaryTex;
				float GlobalLightLevel;
				float minGlobalLightLevel;
				float maxGlobalLightLevel;

				v2f vertFunction (appdata v) {
				
					v2f o;

					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					o.color = v.color;

					return o;

				}

				fixed4 fragFunction (v2f i) : SV_Target {
				
					i.uv.x += (_SinTime.x * 0.5);

					fixed4 tex1 = tex2D (_MainTex, i.uv);
					fixed4 tex2 = tex2D(_SecondaryTex, i.uv);

					fixed4 col = lerp(tex1, tex2, 0.5 + (_SinTime.w * 0.5));

					float shade = (maxGlobalLightLevel - minGlobalLightLevel) * GlobalLightLevel + minGlobalLightLevel;
					shade *= i.color.a;
					shade = clamp (1 - shade, minGlobalLightLevel, maxGlobalLightLevel);

					clip(col.a - 1);
					col = lerp(col, float4(0, 0, 0, 1), shade);

					col.a = 0.5f;

					return col;

				}

				ENDCG

		}


	}

}