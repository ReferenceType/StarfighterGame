// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FORGE3D/Battleships/Station"
{
	Properties
	{
		_NormalMap("NormalMap", 2D) = "bump" {}
		[Header(Blinn Phong Light)]
		_Shininess("Shininess", Range( 0.01 , 1)) = 0.1
		_SpecularMap("SpecularMap", 2D) = "white" {}
		_Diffuse("Diffuse", 2D) = "white" {}
		_Emissive("Emissive", 2D) = "white" {}
		_Tint("Tint", Color) = (0,0,0,0)
		_EmissiveColor("EmissiveColor", Color) = (0,0,0,0)
		_EmissiveGlow("EmissiveGlow", Float) = 1
		_NormalScale("Normal Scale", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "Lighting.cginc"
		#pragma target 2.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		struct SurfaceOutputCustomLightingCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			fixed Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform sampler2D _SpecularMap;
		uniform float4 _SpecularMap_ST;
		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float _NormalScale;
		uniform float _Shininess;
		uniform sampler2D _Diffuse;
		uniform float4 _Diffuse_ST;
		uniform float4 _Tint;
		uniform sampler2D _Emissive;
		uniform float4 _Emissive_ST;
		uniform float4 _EmissiveColor;
		uniform float _EmissiveGlow;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#if DIRECTIONAL
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			float2 uv_SpecularMap = i.uv_texcoord * _SpecularMap_ST.xy + _SpecularMap_ST.zw;
			float4 temp_cast_0 = (tex2D( _SpecularMap, uv_SpecularMap ).a).xxxx;
			float4 temp_output_43_0_g1 = temp_cast_0;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			float3 normalizeResult4_g2 = normalize( ( ase_worldViewDir + ase_worldlightDir ) );
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float3 normalizeResult64_g1 = normalize( WorldNormalVector( i , UnpackScaleNormal( tex2D( _NormalMap, uv_NormalMap ) ,_NormalScale ) ) );
			float dotResult19_g1 = dot( normalizeResult4_g2 , normalizeResult64_g1 );
			float3 temp_output_40_0_g1 = ( _LightColor0.rgb * ase_lightAtten );
			float dotResult14_g1 = dot( normalizeResult64_g1 , ase_worldlightDir );
			UnityGI gi34_g1 = gi;
			float3 diffNorm34_g1 = normalizeResult64_g1;
			gi34_g1 = UnityGI_Base( data, 1, diffNorm34_g1 );
			float3 indirectDiffuse34_g1 = gi34_g1.indirect.diffuse + diffNorm34_g1 * 0.0001;
			float2 uv_Diffuse = i.uv_texcoord * _Diffuse_ST.xy + _Diffuse_ST.zw;
			float4 temp_output_42_0_g1 = ( tex2D( _Diffuse, uv_Diffuse ) * _Tint );
			float2 uv_Emissive = i.uv_texcoord * _Emissive_ST.xy + _Emissive_ST.zw;
			c.rgb = ( float4( ( ( (temp_output_43_0_g1).rgb * (temp_output_43_0_g1).a * pow( max( dotResult19_g1 , 0.0 ) , ( _Shininess * 128.0 ) ) * temp_output_40_0_g1 ) + ( ( ( temp_output_40_0_g1 * max( dotResult14_g1 , 0.0 ) ) + indirectDiffuse34_g1 ) * (temp_output_42_0_g1).rgb ) ) , 0.0 ) + ( ( tex2D( _Emissive, uv_Emissive ) * _EmissiveColor ) * _EmissiveGlow ) ).rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Legacy Shaders/Bumped Specular"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14201
1927;83;1266;827;306.1851;523.2415;1.3;True;False
Node;AmplifyShaderEditor.CommentaryNode;20;-1304,-437;Float;False;2057;593;Albedo;5;1;3;19;18;44;Albedo / Specular;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;3;-1247,-370;Float;True;Property;_Diffuse;Diffuse;6;0;Create;None;63563af1db66c3a4baf4050a89012d26;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.CommentaryNode;21;274.5004,223.4014;Float;False;642.9993;286;Comment;1;7;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;40;-1290.099,236.9992;Float;False;1348.197;396.7004;Comment;5;27;30;28;29;24;Emissive;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;1;-970,-371;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;18;230.5004,-270.3999;Float;False;Property;_Tint;Tint;9;0;Create;0,0,0,0;0.09926467,0.1122211,0.125,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;7;614.7003,285.9015;Float;True;Property;_NormalMap;NormalMap;0;0;Create;None;fbdf5454571142545b5af6bd133d69d5;True;0;True;bump;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;24;-881.3009,401.3998;Float;False;Property;_EmissiveColor;EmissiveColor;10;0;Create;0,0,0,0;1,1,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;27;-1233.699,286.9991;Float;True;Property;_Emissive;Emissive;7;0;Create;None;5717885ce6937eb469a754f7cd0916ec;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;43;448.8076,656.7984;Float;False;Property;_NormalScale;Normal Scale;12;0;Create;1;1.17;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;584,-387;Float;False;2;2;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.UnpackScaleNormalNode;42;992.206,292.799;Float;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0.0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;47;-35.68934,2.797776;Float;True;Property;_SpecularMap;SpecularMap;5;0;Create;None;76d2cf5d820fe1c4786a1e954cbae022;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;29;-577.101,415.6988;Float;False;Property;_EmissiveGlow;EmissiveGlow;11;0;Create;1;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-578.4009,323.3991;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;48;969.4899,-393.8071;Float;False;Blinn-Phong Light;1;;1;cf814dba44d007a4e958d2ddd5813da6;3;42;COLOR;0,0,0,0;False;52;FLOAT3;0,0,0;False;43;COLOR;0,0,0,0;False;2;FLOAT3;0;FLOAT;57
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-371.7018,322.0989;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;49;1213.442,-166.6824;Float;False;2;2;0;FLOAT3;0.0,0,0,0;False;1;COLOR;0.0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;44;448.0068,-99.5022;Float;False;Property;_Specular;Specular;8;0;Create;0.5;0.0805;0.003;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1472.797,-230.7002;Float;False;True;0;Float;ASEMaterialInspector;0;0;CustomLighting;FORGE3D/Battleships/Station;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Absolute;0;Legacy Shaders/Bumped Specular;-1;-1;-1;-1;0;0;0;False;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0.0,0,0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;1;0;3;0
WireConnection;19;0;1;0
WireConnection;19;1;18;0
WireConnection;42;0;7;0
WireConnection;42;1;43;0
WireConnection;28;0;27;0
WireConnection;28;1;24;0
WireConnection;48;42;19;0
WireConnection;48;52;42;0
WireConnection;48;43;47;4
WireConnection;30;0;28;0
WireConnection;30;1;29;0
WireConnection;49;0;48;0
WireConnection;49;1;30;0
WireConnection;0;13;49;0
ASEEND*/
//CHKSM=D228761F711F22812878306124589E6EF2408617