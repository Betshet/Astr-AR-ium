Shader "Unlit/ARStarfield"
{
    Properties
    {
        _StarColor("Star Color", Color) = (1,1,1,1)
        _Density("Star Density", Range(10, 500)) = 150
        _StarSize("Star Size", Range(0.0, 0.5)) = 0.15
        _Opacity("Star Opacity", Range(0.0, 1.0)) = 0.3
        _TwinkleSpeed("Twinkle Speed", Range(0.0, 10.0)) = 2.0

        // Ancien :
        // _NebulaColor("Nebula Color", Color) = (0.4,0.6,1.0,1.0)

        // Nouveau : 3 couleurs + vitesse de déplacement
        _NebulaColor1("Nebula Color 1", Color) = (0.6,0.5,1.0,1.0)
        _NebulaColor2("Nebula Color 2", Color) = (0.3,0.8,1.0,1.0)
        _NebulaColor3("Nebula Color 3", Color) = (1.0,0.6,0.9,1.0)
        _NebulaIntensity("Nebula Intensity", Range(0.0, 2.0)) = 0.6
        _NebulaOpacity("Nebula Opacity", Range(0.0, 1.0)) = 0.22
        _NebulaScale("Nebula Scale", Range(0.1, 5.0)) = 1.5
        _NebulaScrollSpeed("Nebula Scroll Speed", Range(0.0, 2.0)) = 0.2

        //_NebulaIntensity("Nebula Intensity", Range(0.0, 2.0)) = 0.6
        //_NebulaOpacity("Nebula Opacity", Range(0.0, 1.0)) = 0.25
        //_NebulaScale("Nebula Scale", Range(0.1, 5.0)) = 1.5
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

            // Nébuleuse multicolore
            fixed4 _NebulaColor1;
            fixed4 _NebulaColor2;
            fixed4 _NebulaColor3;
            float  _NebulaIntensity;
            float  _NebulaOpacity;
            float  _NebulaScale;
            float  _NebulaScrollSpeed;


            //fixed4 _NebulaColor;
            //float  _NebulaIntensity;
            //float  _NebulaOpacity;
            //float  _NebulaScale;

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
                float  time = _Time.y;

                //========================
                // 1) ÉTOILES
                //========================
                float2 grid = uv * _Density;
                float2 cell = floor(grid);
                float2 f = frac(grid);

                float rnd = hash21(cell);

                // Plus la valeur est basse, plus il y a d'étoiles
                float starMask = step(0.992, rnd);

                float2 centerCell = float2(0.5, 0.5);
                float d = distance(f, centerCell);

                float size = _StarSize * 0.35;
                float shape = smoothstep(size, 0.0, d);

                // Scintillement désynchronisé
                float rndPhase = hash21(cell + float2(17.2, 9.1));
                float rndSpeed = hash21(cell + float2(-5.3, 42.7));

                float localSpeed = _TwinkleSpeed * (0.5 + rndSpeed); // 0.5x → 1.5x
                float phase = rndPhase * 6.2831;

                float twinkleRaw = sin(time * localSpeed + phase);
                float twinkle = 0.5 + 0.5 * twinkleRaw;
                twinkle = twinkle * twinkle;

                float starBrightness = starMask * shape * twinkle;

                fixed4 starCol = _StarColor;
                starCol.rgb *= starBrightness;
                starCol.a = starBrightness * _Opacity * 1.2;

                //========================
                // 2) NÉBULEUSE : 3 ZONES LOIN L'UNE DE L'AUTRE
                //========================

                // Coordonnées centrées + échelle
                float2 pCenter = (uv - 0.5) * _NebulaScale;

                // Direction générale de la voie lactée
                float2 dir = normalize(float2(1.0, 0.3));
                float2 ortho = float2(-dir.y, dir.x); // perpendiculaire

                // Coordonnée perpendiculaire à la bande, étendue sur ~[-2, 2]
                float coord = dot(ortho, pCenter) * 2.0;

                // Centres de base des 3 zones : gauche, centre, droite
                // Avec grande amplitude pour qu’elles parcourent tout le champ
                float c1 = -0.8 + 0.8 * sin(time * 0.08 + 0.0);
                float c2 = 0.0 + 0.8 * sin(time * 0.09 + 1.7);
                float c3 = 0.8 + 0.8 * sin(time * 0.07 + 3.1);

                // Profils en cloche lisses (zones assez larges)
                float zone1 = exp(-(coord - c1) * (coord - c1) * 3.0);
                float zone2 = exp(-(coord - c2) * (coord - c2) * 3.0);
                float zone3 = exp(-(coord - c3) * (coord - c3) * 3.0);

                // Radial : plus fort au centre du dôme
                float radial = 1.0 - saturate(length(pCenter) * 1.4);
                radial = pow(radial, 1.6);

                zone1 *= radial;
                zone2 *= radial;
                zone3 *= radial;

                // Somme pour normaliser
                float zoneSum = zone1 + zone2 + zone3 + 1e-4;

                // Mélange des 3 couleurs selon la zone dominante
                float3 nebRGB =
                    _NebulaColor1.rgb * zone1 +
                    _NebulaColor2.rgb * zone2 +
                    _NebulaColor3.rgb * zone3;

                nebRGB /= zoneSum;

                float nebulaMask = saturate(zoneSum);
                nebulaMask = pow(nebulaMask, 1.2);

                fixed4 nebCol;
                nebCol.rgb = nebRGB * nebulaMask * _NebulaIntensity;
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
