//Шейдер я так и не понял какой вы хотите, сделал на свое усмотрение
//Есть и моргание и маска и даже заполнение)

Shader "Custom/Progress" {
	Properties {
		_MainTex ("Main Tex", 2D) = "white" {}
		_MaskTex("Mask Tex", 2D)=""{}
		_Progress("Progress", Range(-0.01,1)) = 0.1
		_BlinkSpeed("Blink Speed", Range(0,3)) = 1
		_MinBlinkOpacity("Minimal opasity for blink", Range(0,1)) = 0.1
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	sampler2D _MaskTex;
	float _Progress;
	float _BlinkSpeed;
	float _MinBlinkOpacity;

	struct v2f{
		float4 pos:SV_POSITION;
		float2 uv:TEXCOORD;
	};

	v2f vert(appdata_base i){
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP,i.vertex);
		o.uv = i.texcoord.xy;
		return o;
	}

	float4 frag(v2f i):COLOR{
		float  animtime = _Time.w * _BlinkSpeed;
		float4 c = tex2D(_MainTex, i.uv);
		if(_Progress - tex2D(_MaskTex,i.uv).a > 0){
			c.a = tex2D(_MainTex,i.uv).a * (_MinBlinkOpacity + abs(sin(animtime)));
		}
		else{
			c.a = 0;
		}
		return c;
	}

	ENDCG
	
	SubShader {
		Tags {"Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		Pass{
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}

FallBack "Diffuse"
}