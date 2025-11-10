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

        _ClusterDensityBoost("Cluster Density Boost", Range(0.0, 0.05)) = 0.01
        _ClusterStarBoost("Cluster Star Brightness", Range(0.0, 3.0)) = 1.5
        _ClusterNebulaBoost("Cluster Nebula Boost", Range(0.0, 3.0)) = 1.0

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
                float3 dir    : TEXCOORD1;   // direction sur la sphère
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

            float  _ClusterDensityBoost;
            float  _ClusterStarBoost;
            float  _ClusterNebulaBoost;


            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                // Direction depuis le centre de la sphère (en objet), normalisée
                o.dir = normalize(v.vertex.xyz);

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
                float3 dir3 = normalize(i.dir);  // direction sur la sphère
                float  time = _Time.y;

                //========================
                // 0) BASE COMMUNE POUR NÉBULEUSE & CLUSTERS
                //========================
                // Axe principal de la voie lactée
                float3 bandAxis = normalize(float3(1.0, 0.3, 0.1));

                // Vecteur orthogonal pour "balayer" la bande
                float3 tmp = (abs(bandAxis.y) < 0.99) ? float3(0,1,0) : float3(1,0,0);
                float3 ortho1 = normalize(cross(bandAxis, tmp));

                float  scale = _NebulaScale;

                // Coordonnée perpendiculaire à la bande (pour répartir couleurs / clusters)
                float coordDir = dot(dir3, ortho1) * 2.0 * scale;
                // Distance par rapport au plan de la voie lactée
                float alongDir = dot(dir3, bandAxis) * scale;

                //========================
                // 1) CLUSTER MASK : où les amas sont plus denses
                //========================

                // Amas plus serrés autour du plan de la voie lactée
                float clusterThickness = exp(-alongDir * alongDir * 10.0);

                // Centres mobiles des amas (gauche / centre / droite)
                float cc1 = -0.9 + 0.9 * sin(time * 0.11 + 0.3);
                float cc2 = 0.0 + 0.9 * sin(time * 0.13 + 1.7);
                float cc3 = 0.9 + 0.9 * sin(time * 0.09 + 3.2);

                // Gaussiennes étroites : amas assez localisés
                float cl1 = exp(-(coordDir - cc1) * (coordDir - cc1) * 12.0);
                float cl2 = exp(-(coordDir - cc2) * (coordDir - cc2) * 12.0);
                float cl3 = exp(-(coordDir - cc3) * (coordDir - cc3) * 12.0);

                float clusterMask = (cl1 + cl2 + cl3) * clusterThickness;
                clusterMask = saturate(clusterMask);  // 0 → pas d’amas, 1 → plein d’amas

                //========================
                // 2) ÉTOILES (avec amas)
                //========================

                float2 grid = uv * _Density;
                float2 cell = floor(grid);
                float2 f = frac(grid);

                float rnd = hash21(cell);

                // Densité de base des étoiles
                float baseThreshold = 0.992;

                // Dans les amas : seuil plus bas → plus de cellules deviennent des étoiles
                float threshold = baseThreshold - clusterMask * _ClusterDensityBoost;

                float starMask = step(threshold, rnd);

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

                // Dans les amas : étoiles plus lumineuses
                starBrightness *= (1.0 + clusterMask * _ClusterStarBoost);

                fixed4 starCol = _StarColor;
                starCol.rgb *= starBrightness;
                starCol.a = starBrightness * _Opacity * 1.2;

                //========================
                // 3) NÉBULEUSE LARGE (dégradé de fond)
                //========================

                // Bande principale de la voie lactée : plus forte quand alongDir ≈ 0
                float thickness = exp(-alongDir * alongDir * 4.0);

                // Centres des trois grandes zones de couleur (plus larges que les amas)
                float c1 = -0.8 + 0.8 * sin(time * 0.05 + 0.0);
                float c2 = 0.0 + 0.8 * sin(time * 0.06 + 1.7);
                float c3 = 0.8 + 0.8 * sin(time * 0.04 + 3.1);

                float zone1 = exp(-(coordDir - c1) * (coordDir - c1) * 3.0);
                float zone2 = exp(-(coordDir - c2) * (coordDir - c2) * 3.0);
                float zone3 = exp(-(coordDir - c3) * (coordDir - c3) * 3.0);

                zone1 *= thickness;
                zone2 *= thickness;
                zone3 *= thickness;

                float zoneSum = zone1 + zone2 + zone3 + 1e-4;

                float3 nebRGB =
                    _NebulaColor1.rgb * zone1 +
                    _NebulaColor2.rgb * zone2 +
                    _NebulaColor3.rgb * zone3;

                nebRGB /= zoneSum;

                float nebulaMask = saturate(zoneSum);
                nebulaMask = pow(nebulaMask, 1.2);

                // Dans les amas : on renforce un peu la nébuleuse → effet "bout de voie lactée"
                nebulaMask *= (1.0 + clusterMask * _ClusterNebulaBoost);

                fixed4 nebCol;
                nebCol.rgb = nebRGB * nebulaMask * _NebulaIntensity;
                nebCol.a = nebulaMask * _NebulaOpacity;

                //========================
                // 4) COMBINAISON
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
