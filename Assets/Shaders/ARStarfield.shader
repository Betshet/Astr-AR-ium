Shader "Unlit/ARStarfield"
{
    Properties
    {
        _StarColor("Star Color", Color) = (1,1,1,1)
        _Density("Star Density", Range(10, 500)) = 150
        _StarSize("Star Size", Range(0.0, 0.5)) = 0.15
        _Opacity("Star Opacity", Range(0.0, 1.0)) = 0.3
        _TwinkleSpeed("Twinkle Speed", Range(0.0, 10.0)) = 2.0

        _NebulaColor("Nebula Color", Color) = (0.4,0.6,1.0,1.0)
        _NebulaIntensity("Nebula Intensity", Range(0.0, 2.0)) = 0.6
        _NebulaOpacity("Nebula Opacity", Range(0.0, 1.0)) = 0.25
        _NebulaScale("Nebula Scale", Range(0.1, 5.0)) = 1.5
    }

        SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }
        LOD 100

        Cull Front
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

            fixed4 _NebulaColor;
            float  _NebulaIntensity;
            float  _NebulaOpacity;
            float  _NebulaScale;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Random simple 2D
            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 345.45));
                p += dot(p, p + 34.345);
                return frac(p.x * p.y);
            }

            // Bruit "fbm" simple pour la nébuleuse (3 octaves)
            float fbm(float2 p)
            {
                float v = 0.0;
                float a = 0.5;
                float2 shift = float2(37.0, 17.0);

                for (int i = 0; i < 3; i++)
                {
                    v += hash21(p) * a;
                    p = p * 2.0 + shift;
                    a *= 0.5;
                }
                return v;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                //========================
                // 1) ÉTOILES (plus visibles)
                //========================
                float2 grid = uv * _Density;
                float2 cell = floor(grid);
                float2 f = frac(grid);

                float rnd = hash21(cell);

                // Plus d'étoiles : valeur plus basse que 0.997
                float starMask = step(0.99, rnd);

                float2 centerCell = float2(0.5, 0.5);
                float d = distance(f, centerCell);

                // Taille raisonnable mais pas énorme
                float size = _StarSize * 0.35;
                float shape = smoothstep(size, 0.0, d);

                // Scintillement un peu marqué
                // rnd = hash21(cell) existe déjà plus haut

                // Deux randoms indépendants pour chaque étoile
                float rndPhase = hash21(cell + float2(17.2, 9.1));   // phase différente
                float rndSpeed = hash21(cell + float2(-5.3, 42.7));  // vitesse différente

                // Vitesse locale : entre 0.5x et 1.5x la vitesse globale
                float localSpeed = _TwinkleSpeed * (0.5 + rndSpeed);

                // Phase aléatoire sur 0 → 2π
                float phase = rndPhase * 6.2831;

                // Scintillement brut
                float twinkleRaw = sin(_Time.y * localSpeed + phase);

                // Normalisation 0 → 1
                float twinkle = 0.5 + 0.5 * twinkleRaw;

                // Optionnel : on rend le pic un peu plus marqué, mais doux
                twinkle = twinkle * twinkle;

                // Et on applique
                float starBrightness = starMask * shape * twinkle;


                fixed4 starCol = _StarColor;
                starCol.rgb *= starBrightness;
                // On booste un peu l'opacité des étoiles
                starCol.a = starBrightness * _Opacity * 1.2;

                //========================
                // 2) NÉBULEUSE ULTRA LISSE
                //   (pas de bruit, juste des dégradés)
                //========================

                // Coordonnées centrées
                float2 p = uv - 0.5;

                // Dégradé radial doux (plus fort au centre du dôme)
                float radial = 1.0 - saturate(length(p) * 1.4);
                radial = pow(radial, 1.8); // >1 = plus doux, moins brutal

                // Bande diagonale type "voie lactée"
                float2 dir = normalize(float2(1.0, 0.3));   // direction de la bande
                float bandCoord = dot(dir, p);              // position le long de cette direction
                // Profil en cloche (gaussienne) → super lisse
                float band = exp(-bandCoord * bandCoord * 8.0);

                // Masque final de la nébuleuse
                float nebulaMask = radial * band;
                nebulaMask = pow(saturate(nebulaMask), 1.2);  // encore adouci

                fixed4 nebCol = _NebulaColor;
                nebCol.rgb *= nebulaMask * _NebulaIntensity;
                nebCol.a = nebulaMask * _NebulaOpacity;

                //========================
                // 3) COMBINAISON
                //========================
                fixed4 finalCol;
                finalCol.rgb = nebCol.rgb + starCol.rgb;
                finalCol.a = saturate(nebCol.a + starCol.a);

                return finalCol;
            }


            ENDCG
        }
    }

        FallBack Off
}
