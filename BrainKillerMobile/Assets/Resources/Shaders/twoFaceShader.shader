Shader "Custom/DoubleSided" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" { }
    }
    
    SubShader {
        Tags { "Queue" = "Overlay" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o) {
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
        }
        ENDCG
    }

    // Two-sided rendering for back faces
    SubShader {
        Tags { "Queue" = "Overlay" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Front
        ZWrite On

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o) {
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
        }
        ENDCG
    }
}
