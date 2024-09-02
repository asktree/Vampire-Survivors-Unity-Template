// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/SimplexNoise"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Scale ("Noise scale", Vector) = (1.0, 1.0, 1.0)
		_Offset ("Noise offset", Vector) = (1.0, 1.0, 1.0)
        [HDR] _Color1 ("Color 1", Color) = (1,0,0,1)
        [HDR] _Color2 ("Color 2", Color) = (0,1,0,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata {			
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float3 noise_uv : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float3 _Scale;
			float3 _Offset;
            float4 _Color1;
            float4 _Color2;

			float hash( float n )
			{
			    return frac(sin(n)*43758.5453);
			}

			float noise( float3 x )
			{
			    // The noise function returns a value in the range -1.0f -> 1.0f

			    float3 p = floor(x);
			    float3 f = frac(x);

			    f       = f*f*(3.0-2.0*f);
			    float n = p.x + p.y*57.0 + 113.0*p.z;

			    return lerp(lerp(lerp( hash(n+0.0), hash(n+1.0),f.x),
			                   lerp( hash(n+57.0), hash(n+58.0),f.x),f.y),
			               lerp(lerp( hash(n+113.0), hash(n+114.0),f.x),
			                   lerp( hash(n+170.0), hash(n+171.0),f.x),f.y),f.z);
			}

			v2f vert (appdata v)
			{
				v2f o;

				o.normal = v.normal;
				float noiseval = noise(_Scale*(_Offset+v.vertex));
				v.vertex += 0.1*float4(noiseval * o.normal, 0.0);

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.noise_uv = v.vertex.xyz/v.vertex.w;


				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
            {
                float noiseval = noise(_Scale * (_Offset + (i.noise_uv)));
                
                fixed4 col = lerp(_Color1, _Color2, noiseval);
                
                return col;
            }
			ENDCG
		}
	}
}