Shader "Custom/BillboardGeomShader" {
	Properties {
		_SquareTexture("SquareTexture", 2D) = "white" {}
		_CircleTexture("CircleTexture", 2D) = "white" {}
		_CrossTexture("CrossTexture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_Size("Size", Vector) = (1, 1, 0, 0)
	}

	SubShader {
		Tags{ "Queue" = "Overlay+100" "RenderType" = "Transparent" }

		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

		Cull off
		ZWrite off

		Pass {
			CGPROGRAM
			#pragma target 5.0

			#pragma vertex vert			
			#pragma geometry geom
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _SquareTexture;
			sampler2D _CircleTexture;
			sampler2D _CrossTexture;
			float4 _Color = float4(1,0.5f,0.0f,1);
			float2 _Size = float2(1,1);
			float3 _worldPos;

			int _BillboardType = 2; // 0 = static, 1 = cylinder (faces camera toward x & z axes only), 2 = spherical (always faces camera)

			struct PointData {
				float3 pos;
				float4 color;
				int shape;
			};

			//The buffer containing the points we want to draw.
			StructuredBuffer<PointData> geomCenters;

			struct VertexData {
				float4 pos : SV_POSITION;
				float4 color: COLOR0;
				float2 uv : TEXCOORD0;
				int shape : NORMAL0;
			};

			VertexData vert(uint id : SV_VertexID) {
				VertexData v;
				UNITY_INITIALIZE_OUTPUT(VertexData, v);
				v.pos = float4(geomCenters[id].pos + _worldPos, 1.0f);
				v.color = geomCenters[id].color;
				v.shape = geomCenters[id].shape;
				return v;
			}

			float4 RotPoint(float4 p, float3 offset, float3 sideVector, float3 upVector) {
				float3 finalPos = p.xyz;

				finalPos += offset.x * sideVector;
				finalPos += offset.y * upVector;

				return float4(finalPos, 1);
			}

			[maxvertexcount(4)]
			void geom(point VertexData p[1], inout TriangleStream<VertexData> triStream) {
				float2 halfS = _Size;

				float4 v[4];

				if (_BillboardType == 0) {
					v[0] = p[0].pos.xyzw + float4(-halfS.x, -halfS.y, 0, 0);
					v[1] = p[0].pos.xyzw + float4(-halfS.x, halfS.y, 0, 0);
					v[2] = p[0].pos.xyzw + float4(halfS.x,  -halfS.y, 0, 0);
					v[3] = p[0].pos.xyzw + float4(halfS.x,  halfS.y, 0, 0);
				}
				else {
					float3 up = float3(0, 1, 0);
					float3 look = _WorldSpaceCameraPos - p[0].pos.xyz;

					if (_BillboardType == 1) {
						look.y = 0;
					}

					look = normalize(look);
					float3 right = normalize(cross(look, up));
					up = normalize(cross(right, look));

					v[0] = RotPoint(p[0].pos , float3(-halfS.x,-halfS.y,0), right,up);
					v[1] = RotPoint(p[0].pos , float3(-halfS.x,halfS.y,0), right,up);
					v[2] = RotPoint(p[0].pos , float3(halfS.x,-halfS.y,0), right,up);
					v[3] = RotPoint(p[0].pos , float3(halfS.x,halfS.y,0), right,up);
				}

				VertexData pIn;
				UNITY_INITIALIZE_OUTPUT(VertexData, pIn);

				pIn.pos = mul(UNITY_MATRIX_VP, v[0]);
				pIn.uv = float2(0.0f, 0.0f);
				pIn.color = p[0].color;
				pIn.shape = p[0].shape;
				triStream.Append(pIn);

				pIn.pos = mul(UNITY_MATRIX_VP, v[1]);
				pIn.uv = float2(0.0f, 1.0f);
				pIn.color = p[0].color;
				pIn.shape = p[0].shape;
				triStream.Append(pIn);

				pIn.pos = mul(UNITY_MATRIX_VP, v[2]);
				pIn.uv = float2(1.0f, 0.0f);
				pIn.color = p[0].color;
				pIn.shape = p[0].shape;
				triStream.Append(pIn);

				pIn.pos = mul(UNITY_MATRIX_VP, v[3]);
				pIn.uv = float2(1.0f, 1.0f);
				pIn.color = p[0].color;
				pIn.shape = p[0].shape;
				triStream.Append(pIn);
			}

			float4 frag(VertexData i) : COLOR {
				float4 col;
				if (i.shape == 0) {
					col = tex2D(_CircleTexture, i.uv) * i.color;
				}
				else if (i.shape == 1) {
					col = tex2D(_SquareTexture, i.uv) * i.color;
				}
				else if (i.shape == 2) {
					col = tex2D(_CrossTexture, i.uv) * i.color;
				}				
			
				return col;
			}

			ENDCG
		}
	}
	Fallback Off
}