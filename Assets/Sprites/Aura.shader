Shader "Custom/SpriteAura"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Visibility("Visibility", Range(0,1)) = 1
        _AuraColor ("Aura Color", Color) = (0, 1, 1, 1)
        _AuraIntensity ("Aura Intensity", Range(0, 3)) = 1.5
        _AuraThickness ("Aura Thickness (pixels)", Range(1, 20)) = 5
        _Speed ("Pulse Speed", Range(0, 10)) = 2
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

            float4 _AuraColor;
            float _AuraIntensity;
            float _AuraThickness;
            float _Speed;
            float _Visibility;
            float4 _MainTex_TexelSize;

            fixed4 frag(v2f IN) : SV_Target
            {
                // Amostra o sprite principal
                fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
                float alpha = c.a;
                
                // Calcula o offset baseado no tamanho do texel
                float2 texelSize = _MainTex_TexelSize.xy;
                
                // Verifica pixels adjacentes em múltiplas camadas
                float outline = 0.0;
                int samples = 16; // Mais direções para suavidade
                
                // Múltiplas camadas de distância
                for(float layer = 1.0; layer <= _AuraThickness; layer += 1.0)
                {
                    float layerStrength = 1.0 - (layer / _AuraThickness) * 0.5; // Gradiente suave
                    
                    for(int i = 0; i < samples; i++)
                    {
                        float angle = (float(i) / float(samples)) * 6.28318530718;
                        float2 dir = float2(cos(angle), sin(angle));
                        float2 offset = dir * texelSize * layer;
                        
                        float sampleAlpha = SampleSpriteTexture(IN.texcoord + offset).a;
                        outline = max(outline, sampleAlpha * layerStrength);
                    }
                }
                
                // A aura só aparece onde o sprite é transparente mas há pixels próximos
                float auraStrength = outline * (1.0 - alpha);
                
                // Suaviza a borda da aura
                auraStrength = smoothstep(0.0, 0.5, auraStrength);
                
                // Animação de pulsação
                float pulse = sin(_Time.y * _Speed) * 0.3 + 0.7;
                auraStrength *= pulse * _AuraIntensity;
                
                // Combina a cor do sprite com a aura
                c.rgb = c.rgb * c.a + _AuraColor.rgb * auraStrength;
                c.a = saturate(c.a + auraStrength * _AuraColor.a);
                
                c.rgb *= c.a;
                c.a *= _Visibility;
                c.rgb *= c.a;
                
                return c;
            }
            ENDCG
        }
    }
}