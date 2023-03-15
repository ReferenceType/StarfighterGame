// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Battleships/Nebula"
{
	Properties
	{
		_Base("Base", 2D) = "black" {}
		_DistFadeLength("DistFadeLength", Range( 0 , 15000)) = 0
		_DistFadeOffset("DistFadeOffset", Range( 0 , 10000)) = 0
		_FadeViewAngleExp("FadeViewAngleExp", Range( 0.01 , 10)) = 0
		_FadeViewAngleMult("FadeViewAngleMult", Float) = 0
		_TintB("TintB", Color) = (0,0,0,0)
		_TintA("TintA", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" }
		Cull Off
		ZWrite Off
		Blend One One
		BlendOp Add
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf StandardCustomLighting keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			float eyeDepth;
			float4 vertexColor : COLOR;
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

		uniform float4 _TintA;
		uniform float4 _TintB;
		uniform sampler2D _Base;
		uniform float4 _Base_ST;
		uniform float _FadeViewAngleExp;
		uniform float _FadeViewAngleMult;
		uniform float _DistFadeLength;
		uniform float _DistFadeOffset;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float3 desaturateVar95 = lerp( _TintA.rgb,dot(_TintA.rgb,float3(0.299,0.587,0.114)).xxx,_TintA.a);
			float3 desaturateVar96 = lerp( _TintB.rgb,dot(_TintB.rgb,float3(0.299,0.587,0.114)).xxx,_TintB.a);
			float2 uv_Base = i.uv_texcoord * _Base_ST.xy + _Base_ST.zw;
			float4 tex2DNode1 = tex2D( _Base, uv_Base );
			float3 lerpResult109 = lerp( desaturateVar95 , desaturateVar96 , tex2DNode1.r);
			float3 worldSpaceViewDir63 = WorldSpaceViewDir( float4( 0,0,0,0 ) );
			float3 normalizeResult65 = normalize( worldSpaceViewDir63 );
			float dotResult66 = dot( i.worldNormal , normalizeResult65 );
			float cameraDepthFade48 = (( i.eyeDepth -_ProjectionParams.y - _DistFadeOffset ) / _DistFadeLength);
			float3 temp_output_74_0 = saturate( ( saturate( lerpResult109 ) * saturate( ( saturate( ( saturate( pow( abs( dotResult66 ) , _FadeViewAngleExp ) ) * _FadeViewAngleMult ) ) * saturate( cameraDepthFade48 ) * i.vertexColor.a ) ) * tex2DNode1.r ) );
			c.rgb = temp_output_74_0;
			c.a = temp_output_74_0.x;
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
1927;83;1266;827;263.7226;993.8845;1.604223;True;False
Node;AmplifyShaderEditor.CommentaryNode;62;-1006.032,206.1882;Float;False;1471.986;307.5362;Fade over angle;10;68;72;69;67;66;64;65;63;100;101;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldSpaceViewDirHlpNode;63;-970.3584,434.9476;Float;False;1;0;FLOAT4;0,0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;64;-940.4575,260.7478;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;65;-688.2576,376.446;Float;False;1;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;66;-511.4875,257.8348;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-479.5751,387.821;Float;False;Property;_FadeViewAngleExp;FadeViewAngleExp;4;0;Create;0;0.01;0.01;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;67;-338.5555,256.8476;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;51;-361.1012,650.4072;Float;False;670.0616;355.8266;Camera Distance Fade;3;49;50;48;;1,1,1,1;0;0
Node;AmplifyShaderEditor.PowerNode;69;-79.56703,255.7528;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;-103.8766,395.6974;Float;False;Property;_FadeViewAngleMult;FadeViewAngleMult;5;0;Create;0;61.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-311.1011,891.2334;Float;False;Property;_DistFadeOffset;DistFadeOffset;3;0;Create;0;0;0;10000;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-306.6631,801.3672;Float;False;Property;_DistFadeLength;DistFadeLength;1;0;Create;0;7120;0;15000;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;100;102.8202,260.8007;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;93;200.3203,-222.8002;Float;False;Property;_TintB;TintB;6;0;Create;0,0,0,0;0.7573529,0.9196754,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CameraDepthFade;48;94.96077,700.4069;Float;False;2;0;FLOAT;500.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;101;312.1201,258.2007;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;2;-78.80003,-25.60006;Float;True;Property;_Base;Base;0;0;Create;None;3edaaef4c29c4de459c75a4d2279b8e2;False;black;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.ColorNode;78;196.8186,-400.8986;Float;False;Property;_TintA;TintA;7;0;Create;0,0,0,0;0,0.7103448,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;55;415.5925,616.0889;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;88;520.4167,718.4021;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DesaturateOpNode;95;559.1154,-404.7993;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;1;193.1999,-21.60005;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DesaturateOpNode;96;434.3194,-241.0002;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;102;526.6205,260.8;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;763.0311,305.7753;Float;False;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;109;885.1402,-352.2425;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;91;989.7158,289.001;Float;False;1;0;FLOAT;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;104;1247.694,-225.5082;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;1126.416,-22.99891;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0,0,0;False;2;FLOAT;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FresnelNode;85;-692.1833,-42.99963;Float;False;World;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;5.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;61;-831.2791,26752.2;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalVertexDataNode;87;-1178.384,-41.69937;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;74;1362.719,0.800203;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1802.306,60.59363;Float;False;True;2;Float;ASEMaterialInspector;0;0;CustomLighting;Battleships/Nebula;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;True;True;Off;2;0;False;111;111;Custom;-5.58;True;False;0;True;TransparentCutout;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;0;4;10;25;False;0.5;False;4;One;One;0;SrcAlpha;OneMinusSrcAlpha;Add;Add;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;2;-1;-1;-1;0;0;0;False;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0.0,0,0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;65;0;63;0
WireConnection;66;0;64;0
WireConnection;66;1;65;0
WireConnection;67;0;66;0
WireConnection;69;0;67;0
WireConnection;69;1;68;0
WireConnection;100;0;69;0
WireConnection;48;0;49;0
WireConnection;48;1;50;0
WireConnection;101;0;100;0
WireConnection;101;1;72;0
WireConnection;55;0;48;0
WireConnection;95;0;78;0
WireConnection;95;1;78;4
WireConnection;1;0;2;0
WireConnection;96;0;93;0
WireConnection;96;1;93;4
WireConnection;102;0;101;0
WireConnection;52;0;102;0
WireConnection;52;1;55;0
WireConnection;52;2;88;4
WireConnection;109;0;95;0
WireConnection;109;1;96;0
WireConnection;109;2;1;1
WireConnection;91;0;52;0
WireConnection;104;0;109;0
WireConnection;79;0;104;0
WireConnection;79;1;91;0
WireConnection;79;2;1;1
WireConnection;85;0;87;0
WireConnection;74;0;79;0
WireConnection;0;9;74;0
WireConnection;0;13;74;0
ASEEND*/
//CHKSM=DCE8BED4B80A221F269C41CBAC77FAA182D60DE7