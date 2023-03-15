// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FORGE3D/Battleships/Ship"
{
	Properties
	{
		_Tint("Tint", Color) = (0.5294118,0.5294118,0.5294118,1)
		[Header(Blinn Phong Light)]
		_Shininess("Shininess", Range( 0.01 , 1)) = 0.1
		_DiffuseSpec("DiffuseSpec", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Specular("Specular", Range( 0.03 , 1)) = 0.03
		_NormalScale("Normal Scale", Range( 0 , 1)) = 0
		_Mask("Mask", 2D) = "black" {}
		_Windows("Windows", Color) = (1,0,0,1)
		_TeamColor("TeamColor", Color) = (1,0,0,1)
		_Lights("Lights", Color) = (1,0,0,1)
		_Engines("Engines", Color) = (1,0,0,1)
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

		uniform sampler2D _DiffuseSpec;
		uniform float4 _DiffuseSpec_ST;
		uniform float _Specular;
		uniform float _NormalScale;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _Shininess;
		uniform float4 _Tint;
		uniform float4 _TeamColor;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;
		uniform float4 _Windows;
		uniform float4 _Engines;
		uniform float4 _Lights;

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
			float2 uv_DiffuseSpec = i.uv_texcoord * _DiffuseSpec_ST.xy + _DiffuseSpec_ST.zw;
			float4 tex2DNode64 = tex2D( _DiffuseSpec, uv_DiffuseSpec );
			float4 temp_cast_0 = (( tex2DNode64.a * _Specular )).xxxx;
			float4 temp_output_43_0_g1 = temp_cast_0;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			float3 normalizeResult4_g2 = normalize( ( ase_worldViewDir + ase_worldlightDir ) );
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 normalizeResult64_g1 = normalize( WorldNormalVector( i , UnpackScaleNormal( tex2D( _Normal, uv_Normal ) ,_NormalScale ) ) );
			float dotResult19_g1 = dot( normalizeResult4_g2 , normalizeResult64_g1 );
			float3 temp_output_40_0_g1 = ( _LightColor0.rgb * ase_lightAtten );
			float dotResult14_g1 = dot( normalizeResult64_g1 , ase_worldlightDir );
			UnityGI gi34_g1 = gi;
			float3 diffNorm34_g1 = normalizeResult64_g1;
			gi34_g1 = UnityGI_Base( data, 1, diffNorm34_g1 );
			float3 indirectDiffuse34_g1 = gi34_g1.indirect.diffuse + diffNorm34_g1 * 0.0001;
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float4 tex2DNode47 = tex2D( _Mask, uv_Mask );
			float4 lerpResult76 = lerp( ( _Tint * tex2DNode64 ) , _TeamColor , tex2DNode47.g);
			float4 temp_output_42_0_g1 = lerpResult76;
			c.rgb = ( float4( ( ( (temp_output_43_0_g1).rgb * (temp_output_43_0_g1).a * pow( max( dotResult19_g1 , 0.0 ) , ( _Shininess * 128.0 ) ) * temp_output_40_0_g1 ) + ( ( ( temp_output_40_0_g1 * max( dotResult14_g1 , 0.0 ) ) + indirectDiffuse34_g1 ) * (temp_output_42_0_g1).rgb ) ) , 0.0 ) + ( ( tex2DNode47.a * _Windows * 1.0 ) + ( tex2DNode47.r * _Engines * 100.0 ) + ( tex2DNode47.b * _Lights * 1.0 ) ) ).rgb;
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
1927;83;1266;827;734.3747;-805.9631;1;True;False
Node;AmplifyShaderEditor.SamplerNode;47;-953.7123,405.892;Float;True;Property;_Mask;Mask;9;0;Create;None;b06cd176823445348822dda5f86f52e2;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;64;-351.9222,-363.0039;Float;True;Property;_DiffuseSpec;DiffuseSpec;5;0;Create;None;c99d9cd3a18bca44185fb35cc952f1fd;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;63;-301.9222,-536.0038;Float;False;Property;_Tint;Tint;0;0;Create;0.5294118,0.5294118,0.5294118,1;1,1,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;52;-480.9221,711.8959;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-253.7231,1129.496;Float;False;Constant;_Float0;Float 0;12;0;Create;100;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;53;-330.7554,955.2301;Float;False;Property;_Engines;Engines;13;0;Create;1,0,0,1;0.338235,0.1835249,0.06466235,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;51;-324.2222,733.6962;Float;False;Property;_Windows;Windows;10;0;Create;1,0,0,1;0.2720585,0.7289044,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;54;-324.6652,1283.262;Float;False;Property;_Lights;Lights;12;0;Create;1,0,0,1;1,1,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;49;-498.5209,879.897;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-712.9225,-9.004034;Float;False;Property;_NormalScale;Normal Scale;8;0;Create;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;70;95.82211,-277.9173;Float;False;Property;_TeamColor;TeamColor;11;0;Create;1,0,0,1;0.7132353,0.2359967,0.2359967,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;90.07779,-386.0039;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;62;67.07785,221.9959;Float;False;Property;_Specular;Specular;7;0;Create;0.03;1;0.03;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;48;-578.5188,1142.294;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;68;-215.5508,439.0953;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;80;-257.7548,1491.243;Float;False;Constant;_Float1;Float 1;11;0;Create;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-69.51955,1205.196;Float;False;3;3;0;FLOAT;0,0,0,0;False;1;COLOR;0;False;2;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-39.02211,712.2961;Float;False;3;3;0;FLOAT;0.0;False;1;COLOR;0;False;2;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;482.0779,151.996;Float;False;2;2;0;FLOAT;1.0;False;1;FLOAT;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;76;663.9244,-374.4785;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-47.75551,936.4301;Float;False;3;3;0;FLOAT;0.0;False;1;COLOR;0;False;2;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;66;-360.9222,-48.00403;Float;True;Property;_Normal;Normal;6;0;Create;None;d5b3df48c82488e4c92acca15c235191;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;94;895.8942,-320.6624;Float;False;Blinn-Phong Light;1;;1;cf814dba44d007a4e958d2ddd5813da6;3;42;COLOR;0,0,0,0;False;52;FLOAT3;0,0,0;False;43;COLOR;0,0,0,0;False;2;FLOAT3;0;FLOAT;57
Node;AmplifyShaderEditor.SimpleAddOpNode;60;304.6761,953.4951;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;95;1251.323,-75.00826;Float;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1506.896,-347.3248;Float;False;True;0;Float;ASEMaterialInspector;0;0;CustomLighting;FORGE3D/Battleships/Ship;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Absolute;0;Legacy Shaders/Bumped Specular;-1;-1;-1;-1;0;0;0;False;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0.0,0,0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;52;0;47;4
WireConnection;49;0;47;1
WireConnection;72;0;63;0
WireConnection;72;1;64;0
WireConnection;48;0;47;3
WireConnection;68;0;47;2
WireConnection;57;0;48;0
WireConnection;57;1;54;0
WireConnection;57;2;80;0
WireConnection;58;0;52;0
WireConnection;58;1;51;0
WireConnection;58;2;80;0
WireConnection;67;0;64;4
WireConnection;67;1;62;0
WireConnection;76;0;72;0
WireConnection;76;1;70;0
WireConnection;76;2;68;0
WireConnection;56;0;49;0
WireConnection;56;1;53;0
WireConnection;56;2;50;0
WireConnection;66;5;61;0
WireConnection;94;42;76;0
WireConnection;94;52;66;0
WireConnection;94;43;67;0
WireConnection;60;0;58;0
WireConnection;60;1;56;0
WireConnection;60;2;57;0
WireConnection;95;0;94;0
WireConnection;95;1;60;0
WireConnection;0;13;95;0
ASEEND*/
//CHKSM=8DC4B925543A2E821B193DEAB328CC605B9F58A9