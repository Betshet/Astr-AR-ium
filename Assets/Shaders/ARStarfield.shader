Shader "Unlit/ARStarfield"
{
    Properties
    {
        _StarColor("Star Color", Color) = (1,1,1,1)
        _Density("Star Density", Range(10, 500)) = 200
        _StarSize("Star Size", Range(0.0, 0.5)) = 0.3
        _Opacity("Global Opacity", Range(0.0, 1.0)) = 0.7
        _TwinkleSpeed("Twinkle Speed", Range(0.0, 10.0)) = 2.0
    }

        SubShader
    {
        // Transparent pour passer au-dessus de la vidéo AR
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
        LOD 100

        // On regarde l'intérieur de la sphère
        Cull Front

        // On ne modifie pas le ZBuffer et on mélange en alpha
        ZWrite Off
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
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _StarColor;
            float  _Density;
            float  _StarSize;
            float  _Opacity;
            float  _TwinkleSpeed;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Petit "hasard" 2D → 1 valeur pseudo-aléatoire
            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 345.45));
                p += dot(p, p + 34.345);
                return frac(p.x * p.y);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                float2 grid = uv * _Density;
                float2 cell = floor(grid);
                float2 f = frac(grid);

                float rnd = hash21(cell);

                // Beaucoup moins d'étoiles : seulement quand rnd > 0.995
                float starMask = step(0.995, rnd);

                float2 center = float2(0.5, 0.5);
                float d = distance(f, center);

                float size = _StarSize * 0.5;
                float shape = smoothstep(size, 0.0, d);

                float twinkle = 0.5 + 0.5 * sin(_Time.y * _TwinkleSpeed + rnd * 6.2831);

                float brightness = starMask * shape * twinkle;

                fixed4 col = _StarColor;
                col.rgb *= brightness;
                col.a = brightness * _Opacity;

                // Pas de fond : totalement transparent là où il n’y a pas d’étoiles
                return col;
            }
            ENDCG
        }
    }

        FallBack Off
}
