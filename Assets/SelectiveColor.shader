Shader "Custom/ScreenFilter" {
    Properties {
        _RedMultiplier("Red Multiplier", Range(0,1)) = 0
        _GreenMultiplier("Green Multiplier", Range(0,1)) = 0
        _BlueMultiplier("Blue Multiplier", Range(0,1)) = 0
        _Saturation("Saturation", Range(0,1)) = 0.3
    }
    SubShader {
        // Transparent queue so it renders AFTER the scene
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline"}
        
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest Always // Makes sure it's always visible

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float _RedMultiplier;
                float _GreenMultiplier;
                float _BlueMultiplier;
                float _Saturation;
            CBUFFER_END

            Varyings vert (Attributes input) {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.screenPos = ComputeScreenPos(output.positionCS);
                return output;
            }

            half4 frag (Varyings input) : SV_Target {
                // 1. Grab the scene color from behind the cube
                float2 uv = input.screenPos.xy / input.screenPos.w;
                
                // SampleSceneColor requires "Opaque Texture" to be enabled in your URP Asset settings
                half3 sceneColor = SampleSceneColor(uv); 

                // 2. Apply your grayscale/desaturation logic to sceneColor
                float gray = dot(sceneColor, float3(0.299, 0.587, 0.114));
                half3 desaturated = lerp(half3(gray, gray, gray), sceneColor, _Saturation);

                // 3. Apply multipliers to blend between dark desaturation and full color
                half3 final;
                final.r = lerp(desaturated.r * 0.1, sceneColor.r, _RedMultiplier);
                final.g = lerp(desaturated.g * 0.1, sceneColor.g, _GreenMultiplier);
                final.b = lerp(desaturated.b * 0.1, sceneColor.b, _BlueMultiplier);

                return half4(final, 1.0);
            }
            ENDHLSL
        }
    }
}