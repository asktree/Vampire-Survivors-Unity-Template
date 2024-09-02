Shader "Unlit/SimplexNoise2D"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Scale ("Noise scale", Float) = 1.0
        _Offset ("Noise offset", Float) = 0.0
        _DissipationAmount ("Dissipation Amount", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade vertex:vert

        sampler2D _MainTex;
        float _Scale;
        float _Offset;
        float _DissipationAmount;

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

        void surf (Input IN, inout SurfaceOutput o)
        {
            float2 uv = IN.uv_MainTex;
            
            // Calculate the object's local x-axis direction in world space
            float3 localXAxis = normalize(mul(unity_ObjectToWorld, float4(1, 0, 0, 0)).xyz);
            
            // Project the world position onto the local x-axis
            float projectedPos = dot(IN.worldPos, localXAxis);
            
            // Use projected position and object scale for noise calculation
            float noiseInput = (projectedPos * IN.objectScale.x + _Offset) * _Scale;
            float noiseval = noise(float2(noiseInput, 0));
            
            fixed4 texColor = tex2D(_MainTex, uv);
            o.Albedo = texColor.rgb;
            
            // Lerp between original alpha and 0 based on noise and dissipation amount
            float alpha = lerp(texColor.a, 0, noiseval * _DissipationAmount);
            o.Alpha = alpha * IN.color.a;
        }
        ENDCG
    }
    FallBack "Transparent/VertexLit"
}