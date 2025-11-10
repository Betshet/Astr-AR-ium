Shader "Unlit/ARStarfield"
{
    Properties
    {
        _StarColor("Star Color", Color) = (1,1,1,1)
        _Density("Star Density", Range(10, 500)) = 80
        _StarSize("Star Size", Range(0.0, 0.5)) = 0.1
        _Opacity("Global Opacity", Range(0.0, 1.0)) = 0.3
        _TwinkleSpeed("Twinkle Speed", Range(0.0, 10.0)) = 2.0
    }
        SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
        LOD 100

        // On est dans un dôme autour de la caméra : on regarde l'intérieur de la sphère
        Cull Front

        // Transparent, ne pas écrire dans le ZBuffer
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

            // Vertex shader : positionne les vertices
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Petite fonction de "hash" pour générer du pseudo-aléatoire à partir d'une coordonnée 2D
            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 345.45));
                p += dot(p, p + 34.345);
                return frac(p.x * p.y);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // On découpe les UV en une grille
                float2 grid = uv * _Density;

                // Identifiant entier de la cellule
                float2 cell = floor(grid);

                // Coordonnées locales dans la cellule (0 → 1)
                float2 f = frac(grid);

                // Nombre pseudo-aléatoire par cellule
                float rnd = hash21(cell);

                // Seulement certaines cellules contiennent une étoile
                // Ici environ 0.5% des cellules
                float starMask = step(0.995, rnd);

                // Distance au centre de la cellule
                float2 center = float2(0.5, 0.5);
                float d = distance(f, center);

                // Taille de l'étoile : plus _StarSize est petit, plus l'étoile est fine
                float size = _StarSize * 0.5;
                float shape = smoothstep(size, 0.0, d); // 1 au centre, 0 à l'extérieur

                // Scintillement basé sur le temps et le random
                float twinkle = 0.5 + 0.5 * sin(_Time.y * _TwinkleSpeed + rnd * 6.2831);

                // Intensité finale de l'étoile
                float brightness = starMask * shape * twinkle;

                fixed4 col = _StarColor;
                col.rgb *= brightness;
                col.a = brightness * _Opacity;

                return col;
            }
            ENDCG
        }
    }
        FallBack Off
}
