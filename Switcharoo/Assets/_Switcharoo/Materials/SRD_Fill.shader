Shader "Custom/Fill"
{

	///<Summary>
	/// This shader allows you to fill in all the pixels of a sprite, and lerp between that and the actual colors, using the effect amount
	///The effect amount has to be changed through script
	///A summary of how to do that is here: 
	///http://thomasmountainborn.com/2016/05/25/materialpropertyblocks/
	///<Summary>

	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_FillColor ("Fill Color",Color ) = (1,1,1,1)
		[PerRendererData]_EffectAmount("Effect Amount", Range(0, 1)) = 0.0

		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha

			Pass
			{
			CGPROGRAM
				#pragma vertex SpriteVert
				#pragma fragment FillSpriteFrag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile_local _ PIXELSNAP_ON
				#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
				#include "UnitySprites.cginc"


				fixed4 _FillColor;
		float _EffectAmount;

				fixed4 FillSpriteFrag(v2f IN) : SV_Target
				{
					fixed4 texel = SampleSpriteTexture(IN.texcoord);
				//IN.color.a = (IN.color.a - _EffectAmount) / (1.0 - _BlinkLimit);
				fixed4 c = texel * IN.color * (1.0-_EffectAmount) + _FillColor * (_EffectAmount) * sign(texel.a);
					c.rgb *= c.a;
					return c;
				}
			ENDCG
			}
		}
}