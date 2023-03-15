// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Battleships/Skybox"
{
	Properties
	{
		_Stars("Stars", 2D) = "black" {}
		_Brightness("Brightness", Range( 0 , 10)) = 1
		_SkyTint("SkyTint", Color) = (0,0,0,0)
		_StarsTint("Stars Tint", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" }
		Cull Front
		ZWrite Off
		Offset  11111 , 11111
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#pragma target 3.0
		#pragma surface surf StandardCustomLighting keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
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

		uniform sampler2D _Stars;
		uniform float4 _Stars_ST;
		uniform half _Brightness;
		uniform float4 _StarsTint;
		uniform float4 _SkyTint;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float2 uv_Stars = i.uv_texcoord * _Stars_ST.xy + _Stars_ST.zw;
			c.rgb = saturate( ( ( tex2D( _Stars, uv_Stars ) * _Brightness * _StarsTint ) + _SkyTint ) ).rgb;
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
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14201
1927;83;1266;827;1375.5;471.5;1;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;2;-1257,-181;Float;True;Property;_Stars;Stars;0;0;Create;None;aa29b6f053e2cf64fa7a2f9b36276307;False;black;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.ColorNode;13;-953.5,-356.5;Float;False;Property;_StarsTint;Stars Tint;3;0;Create;0,0,0,0;0.6544117,0.321133,0.2550281,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-1037,-181;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;6;-1019.5,29.5;Half;False;Property;_Brightness;Brightness;1;0;Create;1;3.3;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;7;-649.5,65.5;Float;False;Property;_SkyTint;SkyTint;2;0;Create;0,0,0,0;0.09411757,0.003921569,0,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-653.5,-173.5;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;8;-372.5,-174.5;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;12;-116.5,-175.5;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;89,-222;Float;False;True;2;Float;ASEMaterialInspector;0;0;CustomLighting;Battleships/Skybox;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;True;False;Front;2;0;True;11111;11111;Opaque;0;True;False;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;0;4;10;25;False;0.5;False;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Absolute;0;;-1;-1;-1;-1;0;0;0;False;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0.0,0,0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;1;0;2;0
WireConnection;3;0;1;0
WireConnection;3;1;6;0
WireConnection;3;2;13;0
WireConnection;8;0;3;0
WireConnection;8;1;7;0
WireConnection;12;0;8;0
WireConnection;0;13;12;0
ASEEND*/
//CHKSM=2FA3F72FF529B08EE114BE5F0BE45AB62238D539