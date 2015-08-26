Shader "Custom/WireShader" {
Properties {
	_EmisColor ("Emissive Color", Color) = (.2,.2,.2,1)
	_MainTex ("Particle Texture", 2D) = "white" {}
}

SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Tags { "LightMode" = "Vertex" }
	Cull Off
	Lighting On
	Material { 
		Emission [_EmisColor]
				
	}
	ColorMaterial Emission
	AlphaTest Equal 1
	Pass { 
		ZWrite On
		SetTexture [_MainTex] { combine primary * texture }
	}
}
}