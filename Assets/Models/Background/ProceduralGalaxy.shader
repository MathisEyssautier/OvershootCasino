Shader "Custom/ProceduralGalaxy"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _StarSizeMin ("Star Size Min", Range(0, 0.002)) = 0.0005
        _StarSizeMax ("Star Size Max", Range(0, 0.01)) = 0.005
        _StarBrightnessMin ("Star Brightness Min", Range(0, 2)) = 0.5
        _StarBrightnessMax ("Star Brightness Max", Range(0, 2)) = 1.5
        _GalaxySpinSpeed ("Galaxy Spin Speed", Range(5, 20)) = 10
        _GalaxyPosition ("Galaxy position" , Vector) = (0, 0, 0)
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #define TAU 6.2382

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 viewDir : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 viewDir : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                float _StarSizeMin;
                float _StarSizeMax;
                float _StarBrightnessMin;
                float _StarBrightnessMax;
                float _GalaxySpinSpeed;
                float3 _GalaxyPosition;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.viewDir = IN.viewDir;
                return OUT;
            }

            float sdbEllipsoid( in float3 p, in float3 r )
            {
                float k1 = length(p/r);
                return (k1-1.0)*min(min(r.x,r.y),r.z);
            }
            
            float3 HashDir(float seed)
            {
                float n1 = frac(sin(seed * 12.9898) * 43758.5453);
                float n2 = frac(sin(seed * 78.233) * 43758.5453);
                float n3 = frac(sin(seed * 37.719) * 43758.5453);
                float3 dir = normalize(float3(n1 * 2 - 1, n2 * 2 - 1, n3 * 2 - 1));
                return dir;
            }
            
            float3 addGalaxy(float3 center, float3 ellipseDir, float2 ab, float3 dir, float s)
            {
                // ellipse orientation
                float3 centerDir = normalize(center.xyz);
                float3 up = normalize(ellipseDir.xyz);
                float3 right = normalize(cross(up, centerDir));
                up = normalize(cross(centerDir, right));

                // local coordinates
                float3 local = float3(dot(dir, right), dot(dir, up), dot(dir, centerDir));

                // ellipse parameters
                float2 radii = float2(ab.x, ab.y);
                float2 p = local.xy / radii;
                float ellipDist = length(p);
                float phi = atan2(p.y, p.x);

                // falloff intensity
                float falloff = exp(-pow(ellipDist * s, 2)) * saturate(local.z);

                float3 ellipseColor = float3(0, 0, 0);
                
                // color contribution
                float3 color = float3(0.1, 0.15, 0.4); 
                ellipseColor += color.rgb * falloff;

                ///////////////////////////////
                //center ellipse
                color = float3(0.15, 0.1, 0.3);
                // ellipse parameters
                radii = float2(ab.x/2, ab.y/2);
                p = local.xy / radii;
                ellipDist = length(p);

                // falloff intensity
                falloff = exp(-pow(ellipDist * s, 2)) * saturate(local.z);

                float spiralSpin = - _Time*_GalaxySpinSpeed;
                float spiral = cos(phi * 4 + ellipDist * 80 + spiralSpin);
                float armMask = smoothstep(0.2, 1.0, pow(abs(spiral), 1.5));
                falloff = falloff * armMask * saturate(local.z);

                spiral = cos(phi * 4 + ellipDist * 50 + spiralSpin);
                armMask = smoothstep(0.2, 1.0, pow(abs(spiral), 1.5));
                falloff = falloff * armMask * saturate(local.z);

                // color contribution
                ellipseColor += 2.5 * (color * falloff);

                ///////////////////////////////
                //center middle ellipse
                color = float3(1, 0.3, 0.1);
                // ellipse parameters
                radii = float2(ab.x/3, ab.y/4);
                p = local.xy / radii;
                ellipDist = length(p);

                // falloff intensity
                falloff = exp(-pow(ellipDist * s, 2)) * saturate(local.z);

                spiral = cos(phi * 4 + ellipDist * 70 + spiralSpin);
                armMask = smoothstep(0.2, 1.0, pow(abs(spiral), 1));
                falloff = falloff * armMask * saturate(local.z);

                spiral = cos(phi * 4 + ellipDist * 50 + spiralSpin);
                armMask = smoothstep(0.2, 1.0, pow(abs(spiral), 1));
                falloff = falloff * armMask * saturate(local.z);

                // color contribution
                ellipseColor += 4 * (color * falloff);

                ///////////////////////////////
                //center center ellipse
                color = float3(0.7, 0.95, 0.3)*2;
                // ellipse parameters
                radii = float2(ab.x/3, ab.y/5);
                p = local.xy / radii;
                ellipDist = length(p);

                // falloff intensity
                falloff = exp(-pow(ellipDist * s, 2)) * saturate(local.z);

                // color contribution
                ellipseColor += 3 * (color * falloff);
                
                return ellipseColor;
            }
            
            half4 frag(Varyings IN) : SV_Target
            {
                float4 color;

                float3 dir = normalize(IN.viewDir);
                float brightness = 0;
                
                for (uint s = 0; s < 500; s++)
                {
                    float3 starDir = HashDir(s);
                    float angle = acos(saturate(dot(dir, starDir)));

                    float seed = Hash(s * 0.7);
                    
                    seed = abs(sin(seed + 30 * _Time + 2) + sin(seed + 25 * _Time) - sin(4 * seed + 20 * _Time) - sin(seed + 25 * _Time))/3;
                    
                    float size = lerp(_StarSizeMin, _StarSizeMax, seed);

                    // Star center
                    float bright = lerp(_StarBrightnessMin, _StarBrightnessMax, pow(seed, 0.5));

                    // Rays
                    

                    brightness += exp(-pow(angle / size, 2)) * bright;
                }

                // --- Directional gradient ---
                float3 baseA = float3(0.01, 0.02, 0.04); // deep blue-black
                float3 baseB = float3(0.015, 0.025, 0.05)*0.6; // slightly lighter tint
                float3 baseC = float3(0.00, 0.00, 0.00); // near black
                float3 baseD = float3(0.015, 0.007, 0.02);
                
                // Color directions
                float3 gradDir1 = normalize(float3(0.7, 0.4, 0.2));
                float3 gradDir2 = normalize(float3(-0.3, 0.8, -0.5));
                float3 gradDir3 = normalize(float3(-0.1, 0.5, -0.9));

                float t1 = saturate(dot(dir, gradDir1) * 0.5 + 0.5);
                float t2 = saturate(dot(dir, gradDir2) * 0.5 + 0.5);
                float t3 = saturate(dot(dir, gradDir3) * 0.5 + 0.5);

                // Blend colors
                float3 color1 = lerp(baseA, baseB, smoothstep(0.0, 1.0, t1));
                float3 color2 = lerp(baseB, baseC, smoothstep(0.0, 1.0, t2));
                float3 color3 = lerp(baseC, baseD, smoothstep(0.0, 1.0, t2));
                float3 color4 = lerp(baseA, baseD, smoothstep(0.0, 1.0, t3));
                float3 background1 = lerp(color1, color2, 0.5);
                float3 background2 = lerp(color3, color4, 0.5);

                // Galaxy stuff
                float3 ellipseCenter = float3(1, 0, 0);
                float3 ellipseUp = float3(0, 0.4, 1);
                float ellipseRadiusX = 1;
                float ellipseRadiusY = 3;
                float ellipseSharpness = 10;
                
                float3 ellipseColor = addGalaxy(_GalaxyPosition, ellipseUp, float2(ellipseRadiusX, ellipseRadiusY), dir, ellipseSharpness);
                
                color.rgb = brightness + background1 + background2 + ellipseColor;
                color.a = 1;
                
                return color;
            }
            
            ENDHLSL
        }
    }
}
