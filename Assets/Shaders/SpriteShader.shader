// Developer : Chance Wees (Fat Cat GameWorks)
// Date : 10/29/2012
// (c)2012 Chance Wees, all rights reserved

Shader "Custom/SpriteShader2" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {	
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	Cull off
		
	CGPROGRAM
	
	#pragma surface surf Lambert vertex:vert alphatest:_Cutoff addshadow fullforwardshadows
	
	sampler2D _MainTex;
	fixed4 _Color;
	
	void vert (inout appdata_full v) {
		if(dot( normalize(ObjSpaceViewDir (v.vertex)), v.normal) < 0)
		{
      		v.normal = -v.normal;
      	}
  	}
	
	struct Input {
		float2 uv_MainTex;
	};
	
	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG
}

Fallback "Transparent/Cutout/VertexLit"
}