Shader "Unlit/LKGSimulator"
{
    Properties
    {
//				uniform sampler2D quiltTexture;
		_MainTex("quiltTexture", 2D) = "white" {}
		tilesX("tilesX", Float)=5
		tilesY("tilesY", Float)=9
		quiltViewPortion("quiltViewPortion", Vector) = (1,1,1,1)

	}
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float4 normal: NORMAL;
            };

            struct v2f 
            {
                float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
					fixed4 color : COLOR;
				float4 normal: NORMAL;
				float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
			float4 _MainTex_ST;
			uniform float tilesX;
			uniform float tilesY;
			float2 quiltViewPortion;
			float2 vUv;

            v2f vert (appdata v)
            {
					// vUv.y = 1.0 - vUv.y; // why???
					// normal info
					// https://discourse.threejs.org/t/basic-custom-shader-camera/10065
//					gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);
//					gl_Position = projectionMatrix * modelViewMatrix * float4(position, 1.0);
				
				/*
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				//o.vertex= mul(mul(UNITY_MATRIX_P, UNITY_MATRIX_MV), v.vertex);
               // o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);  // o.uv=v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
				*/


				v2f o;
				o.normal = mul(UNITY_MATRIX_IT_MV, v.normal);
//				o.normal = normalMatrix * normal;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o, o.vertex);
				o.color = o.normal*0.5 + 0.5;
				return o;
			}

			float mod(float x, float y)
			{
				return x - y * floor(x / y);
			}

			//uvz.z is the view number
			//uvz.xy is the UV on the looking glass face 
			float2 texArr(float3 uvz) {
				// float z = floor(uvz.z * tilesX * tilesY);
				float z = uvz.z;
				float x = (mod(z, tilesX) + uvz.x) / tilesX;
				float y = (floor(z / tilesX) + uvz.y) / tilesY;
				return float2(x, y) * quiltViewPortion;
			}

			float remap(float value, float from1, float to1, float from2, float to2) {
				return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
			}


            float4 frag (v2f i): SV_Target
            {
//				fixed4 col1 = fixed4(0,0,0,0);

				/*
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
				*/

					float3 nuv = float3(i.uv, 0.0);
					nuv.z = -i.normal.x * 1.6; // now -1 to 1 is like the viewcone
					float fade = nuv.z; // how far off center it is determines fade
					nuv.z = nuv.z * .5 + .5; // 0-1
					nuv.z = clamp(nuv.z, 0.0, 1.0);

					// get some samples
					float tiles = tilesX * tilesY;
					nuv.z *= tiles;

					float3 uv_1 = nuv;
					uv_1.z = floor(uv_1.z);
					uv_1.z = clamp(uv_1.z, 0.0, tiles - 1.0);

					float3 uv_2 = nuv;
					uv_2.z = ceil(uv_2.z);
					uv_2.z = clamp(uv_2.z, 0.0, tiles - 1.0);

					float lerpAmount = clamp(mod(nuv.z + 2.0, 1.0), 0.0, 1.0);

					float4 col1=tex2D(_MainTex, texArr(uv_1));
					float4 col2 = tex2D(_MainTex, texArr(uv_2));
					
					col1 = lerp(col1, col2, lerpAmount);
					if (abs(fade) > 1.0) {
						float3 edgeUv = nuv;
						edgeUv.z = tiles - 1.0;
						if (fade > 0.0) {
							edgeUv.z = 0.0;
						}
						float4 col3 = tex2D(_MainTex, texArr(edgeUv));

						float fadeLerp = clamp((abs(fade) - 1.0) * 1.6, 0.0, 1.0);
						col1 = lerp(col1, col3, fadeLerp);
					}
					//col1 = float4(quiltViewPortion.x, quiltViewPortion.y,0,1);
//					col1 = fixed4(1,0,0,1);
//					gl_FragColor = col1;
//					col1=fixed4(
					//return fixed4(i.normal);
					//col1=fixed4(uv_1.z/tiles,0,0,1);	
					return col1;
					//return tex2D(_MainTex, );
					//return fixed4(255, 255, 255, 255);
            }
            ENDCG
        }
    }
}
