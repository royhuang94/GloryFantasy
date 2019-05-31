// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Sprites/2DOutline"
{
	Properties {
			//纹理颜色
			 _MainColor ("Main Color", Color) = (1,1,1,1)
			  //主纹理属性
			  _MainTex ("Texture", 2D) = "white" {}
			  //法线贴图纹理属性
			  _BumpMap ("Bumpmap", 2D) = "bump" {}
			  //边缘光颜色值
			  _RimColor ("Rim Color", Color) = (1,1,1,1)
			  //边缘光强度值
			  _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
	    }
		SubShader {
			  //标明渲染类型是不透明的物体
			  Tags { "RenderType" = "Opaque" }
			  //标明CG程序的开始
			  CGPROGRAM
			  //声明表面着色器函数
			  #pragma surface surf Lambert
			  //定义着色器函数输入的参数Input
			  struct Input {
			  	  //主纹理坐标值
			      float2 uv_MainTex;
			      //法线贴图坐标值
			      float2 uv_BumpMap;
			      //视图方向
			      float3 viewDir;
			  };
			  //声明对属性的引用
			  float4 _MainColor;
			  sampler2D _MainTex;
			  sampler2D _BumpMap;
			  float4 _RimColor;
			  float _RimPower;
			  //表面着色器函数
			  void surf (Input IN, inout SurfaceOutput o) {
			  	  fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			  	  
			  	  //赋值颜色信息
				  o.Albedo = tex.rgb * _MainColor.rgb;
			      //赋值法线信息
			      o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
			      half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
			      //赋值自发光颜色信息
			      o.Emission = _RimColor.rgb * pow (rim, _RimPower);
			  }
			  //标明CG程序的结束
			  ENDCG
		} 
	    Fallback "Diffuse"	
}
/*ASEBEGIN
Version=16700
-1913;-1919;1906;1010;3082.432;219.8402;2.419167;True;False
Node;AmplifyShaderEditor.CommentaryNode;136;-2740.2,100.2236;Float;False;1549.195;309.0018;;7;102;147;83;74;108;107;106;Base UV Scale;0.4764151,1,0.8965332,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;102;-2690.661,176.2951;Float;False;Property;_RectSize;RectSize;4;0;Create;True;0;0;False;0;1;1;1;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;147;-2397.221,177.505;Float;False;baseScale;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;106;-2086.803,293.3898;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;107;-1953.062,291.6295;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;137;-2360.323,820.246;Float;False;946.5944;371.4059;;7;96;2;97;99;98;100;101;Thickness using texel size;0.5653189,1,0.3066038,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;108;-1796.943,288.8691;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;101;-2310.323,870.246;Float;False;0;0;_MainTex;Shader;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;74;-1690.514,154.7621;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexelSizeNode;96;-2093.066,872.7103;Float;False;-1;1;0;SAMPLER2D;_Sampler096;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;2;-2054.051,1055.368;Float;False;Property;_Thickness;Thickness;0;0;Create;True;0;0;True;0;1;0.7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;-1849.728,953.6519;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-1855.228,1058.652;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;135;-1157.777,-707.0172;Float;False;1355.977;516.3763;;6;84;11;10;9;12;52;BaseColor;1,0.3632075,0.3632075,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;83;-1434.005,150.2236;Float;False;scaleUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;98;-1656.728,941.6519;Float;False;width;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;100;-1662.228,1046.652;Float;False;height;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;138;-1116.539,1131.44;Float;False;1016.522;642.0741;;8;72;14;15;71;90;31;29;21;Blur;0.5463571,0.4575472,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;84;-790.6024,-305.6408;Float;False;83;scaleUV;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;11;-1107.777,-494.599;Float;False;0;0;_MainTex;Shader;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;9;-419.0588,-657.0172;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;14;-1043.039,1292.983;Float;False;98;width;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;15;-1033.616,1385.582;Float;False;100;height;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;90;-1049.258,1181.44;Float;False;83;scaleUV;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;10;-532.9538,-449.5651;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;72;-1023.539,1658.514;Float;False;Property;_sigma;sigma;3;0;Create;True;0;0;False;0;3;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;71;-1066.539,1533.514;Float;False;Property;_BlurIterations;BlurIterations;2;0;Create;True;0;0;False;0;11;31;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-211.6069,-522.7832;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CustomExpressionNode;31;-783.8468,1361.673;Half;False; //get our base color...$     half col = tex2D(_MainTex, uv).a@$     //total width/height of our blur "grid":$     const int mSize = blurIterations@$     //this gives the number of times we'll iterate our blur on each side $     //(up,down,left,right) of our uv coordinate@$     //NOTE that this needs to be a const or you'll get errors about unrolling for loops$     const int iter = (mSize - 1) / 2@$     //run loops to do the equivalent of what's written out line by line above$     //(number of blur iterations can be easily sized up and down this way)$     for (int i = -iter@ i <= iter@ ++i) {$         for (int j = -iter@ j <= iter@ ++j) {$             col += tex2D(_MainTex, uv + float2(i * width, j * height), width, height).a * normpdf(float(i), sigma)@$            }$     }$     //return blurred color$     return col/mSize@;1;False;5;True;uv;FLOAT2;0,0;In;;Float;False;True;width;FLOAT;0;In;;Float;False;True;height;FLOAT;0;In;;Float;False;True;blurIterations;FLOAT;0;In;;Float;False;True;sigma;FLOAT;0;In;;Float;False;blur;True;False;0;5;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;52;-44.80061,-522.7152;Float;False;baseColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;29;-507.0494,1361.574;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;128;-29.25765,830.6304;Float;False;52;baseColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;21;-343.0166,1358.012;Float;False;outline;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;230.8753,667.9052;Float;False;21;outline;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;129;185.1431,832.2304;Float;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;132;414.0759,716.5252;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;113;21.50743,220.1621;Float;False;52;baseColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;18;275.45,410.6064;Float;False;Property;_OutlineColor;OutlineColor;1;1;[HDR];Create;True;0;0;True;0;1,1,1,1;0.3163285,3.776172,2.862889,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;118;242.383,201.6011;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.CommentaryNode;149;430.4563,1123.263;Float;False;650.9846;346.6472;Resize the rect;4;140;139;146;148;VertexOffset;0.990566,0.6368745,0.4719206,1;0;0
Node;AmplifyShaderEditor.WireNode;134;578.7748,905.9249;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;123;563.6385,590.2552;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;92;574.3078,438.0237;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;119;554.2123,203.0447;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;148;480.4563,1334.241;Float;False;147;baseScale;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;133;735.1743,691.825;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;124;828.6227,326.4861;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;146;688.2256,1336.91;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;73;-1055.319,130.5559;Float;False;627.4775;201.6885;;1;30;DON'T REMOVE PLEASE;1,1,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;131;861.2743,691.8251;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;139;647.4411,1173.263;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;122;1073.928,579.8384;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;140;912.441,1235.263;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CustomExpressionNode;30;-820.8956,201.3689;Half;False;return 0.39894*exp(-0.5*x*x / (sigma*sigma)) / sigma@;1;False;2;True;x;FLOAT;0;In;;Float;False;True;sigma;FLOAT;0;In;;Float;False;normpdf;False;True;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;1253.726,580.4704;Float;False;True;2;Float;ASEMaterialInspector;0;6;Sprites/2DOutline;0f8ba0101102bb14ebf021ddadce9b49;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 1;2;True;3;1;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;False;False;True;2;False;-1;False;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;0;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;147;0;102;0
WireConnection;106;0;147;0
WireConnection;107;0;106;0
WireConnection;108;0;107;0
WireConnection;74;0;147;0
WireConnection;74;1;108;0
WireConnection;96;0;101;0
WireConnection;97;0;96;1
WireConnection;97;1;2;0
WireConnection;99;0;96;2
WireConnection;99;1;2;0
WireConnection;83;0;74;0
WireConnection;98;0;97;0
WireConnection;100;0;99;0
WireConnection;10;0;11;0
WireConnection;10;1;84;0
WireConnection;12;0;9;0
WireConnection;12;1;10;0
WireConnection;31;0;90;0
WireConnection;31;1;14;0
WireConnection;31;2;15;0
WireConnection;31;3;71;0
WireConnection;31;4;72;0
WireConnection;52;0;12;0
WireConnection;29;0;31;0
WireConnection;21;0;29;0
WireConnection;129;0;128;0
WireConnection;132;0;91;0
WireConnection;132;1;129;0
WireConnection;118;0;113;0
WireConnection;134;0;129;0
WireConnection;123;0;18;4
WireConnection;123;1;132;0
WireConnection;92;0;18;1
WireConnection;92;1;18;2
WireConnection;92;2;18;3
WireConnection;119;0;118;0
WireConnection;119;1;118;1
WireConnection;119;2;118;2
WireConnection;133;0;123;0
WireConnection;133;1;134;0
WireConnection;124;0;92;0
WireConnection;124;1;119;0
WireConnection;124;2;118;3
WireConnection;146;0;148;0
WireConnection;131;0;133;0
WireConnection;122;0;124;0
WireConnection;122;3;131;0
WireConnection;140;0;139;0
WireConnection;140;1;146;0
WireConnection;0;0;122;0
WireConnection;0;1;140;0
ASEEND*/
//CHKSM=79DFA0B549EB48B275A76D593D29349547652C28