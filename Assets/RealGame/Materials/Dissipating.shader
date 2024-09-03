Shader "Unlit/SimplexNoise2D"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Scale ("Noise scale", Float) = 1.0
        _Offset ("Noise offset", Float) = 0.0
        _Trim ("Noise trim", Float) = 0.0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}
        LOD 100

        CGPROGRAM
        #pragma surface surf NoLighting alpha:fade vertex:vert noforwardadd

        sampler2D _MainTex;
        float _Scale;
        float _Offset;
        float _Trim;

        struct Input
        {
            float2 uv_MainTex;
            float4 color : COLOR;
            float3 worldPos;
            float3 objectScale;
        };

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            o.objectScale = float3(
                length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)),
                length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)),
                length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z))
            );
        }

        float hash(float2 n)
        {
            return frac(sin(dot(n, float2(12.9898, 78.233))) * 43758.5453);
        }

        float noise(float2 p)
        {
            float2 i = floor(p);
            float2 f = frac(p);
            f = f * f * (3.0 - 2.0 * f);
            float2 n = float2(0, 1);
            return lerp(lerp(hash(i + n.xx), hash(i + n.yx), f.x),
                        lerp(hash(i + n.xy), hash(i + n.yy), f.x), f.y);
        }

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            return fixed4(s.Albedo, s.Alpha);
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            float2 uv = IN.uv_MainTex;
            
            
            // Use projected position and object scale for noise calculation
            float noiseInput = (uv + _Offset) * _Scale;
            float noiseval = noise(noiseInput);
            
            fixed4 texColor = tex2D(_MainTex, uv);
            o.Albedo = texColor.rgb * IN.color.rgb;
            
            // if noiseval < _Trim, alpha is zero. otherwise it's texColor.a
            float alpha = noiseval < _Trim ? 0 : 1;
            

            o.Alpha = alpha * IN.color.a;
        }
        ENDCG
    }
    FallBack "Transparent/VertexLit"
}