Shader "Custom/Particles/SmokeCloud"
{
    Properties
    {
        _CellDensity("Cell Density", Range(0, 100)) = 2.0
        _ScrollX("Scroll X", Float) = 0.0
        _ScrollY("Scroll Y", Float) = 0.0
    }
        SubShader
    {
        Tags { "RenderType" = "TransparentCutout" }
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
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 particleInfo : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 particleInfo : TEXCOORD1;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = v.uv;
                o.particleInfo = v.particleInfo;
                return o;
            }
            
            float random(float2 st, float2 vec) 
            {
                return frac(sin(dot(st.xy, vec)) * 43758.5453123);
            }

            float2 random2D(float2 st)
            {
                return float2(random(st, float2(39.08232, 21.12345)), random(st, float2(12.25274, 52.32668)));
            }
                        
            float voronoiNoise(float2 value) 
            {
                float2 baseCell = floor(value);
                float minDistToCell = 10.0;

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        float2 cell = baseCell + float2(x, y);
                        float2 cellPosition = cell + random2D(cell);
                        float2 toCell = cellPosition - value;
                        float distToCell = length(toCell);
                        minDistToCell = min(distToCell, minDistToCell);
                    }
                }

                return minDistToCell;
            }

            float _CellDensity;
            float _ScrollX;
            float _ScrollY;

            float4 frag(v2f i) : SV_Target
            {
                float2 st = i.uv.xy + i.particleInfo.yz + float2((_Time.x * _ScrollX), (_Time.x * _ScrollY));
                st *= _CellDensity;

                float4 voronoi = voronoiNoise(st);
                clip(voronoi - i.particleInfo.x);

                float4 col = i.color;
                return col;
            }

            ENDCG
        }
    }
}
