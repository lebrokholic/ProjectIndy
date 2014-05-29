Shader "Transparent/Mask" {

Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _Mask ("Mixing Mask (A)", 2D) = "gray" {}
}

Category {
    ZWrite Off
    Alphatest Greater 0
    Tags {Queue=Transparent}
    Blend SrcAlpha OneMinusSrcAlpha
    ColorMask RGB
    Lighting Off
    SubShader {
        Pass {

CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
#pragma exclude_renderers gles
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members uv)
#pragma exclude_renderers d3d11 xbox360
#pragma fragment frag
 
float4 _Color;
sampler2D _MainTex;
sampler2D _Mask;
 
struct v2f {
    float4 uv;
};
 
half4 frag( v2f i ) : COLOR
{
    half4 color = tex2D( _MainTex, i.uv.xy );
    half4 mask = tex2D( _Mask, i.uv.xy );
    return half4(color.r, color.g, color.b, mask.a) * _Color;
}

ENDCG
        }
    }
}

}