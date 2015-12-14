Shader "Custom/Clouds_Normal_16K" {
Properties {
	//Top Left Texture
	_MainTex0 ("Top Left Texture", 2D) = "white" {}  
	//Top Right --
	_MainTex1 ("Top Right Texture", 2D) = "white" {}  
	//Bottom Left
	_MainTex2 ("Bottom Left Texture", 2D) = "white" {}  
	//Bottom Right
	_MainTex3 ("Bottom Right Texture", 2D) = "white" {} 
	
    _Brightness ("Brightness", Range (0.5, 5)) = 1.5
	_Normals("Normal Map tl", 2D) = "black" {}
    _NormalStrength ("Normal Strength", Range (0, 2)) = 1
}
 
SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200
	Offset -1, -2
 
CGPROGRAM
#pragma surface surf Lambert finalcolor:nightcolor alpha
#pragma target 3.0

sampler2D _MainTex0;
sampler2D _MainTex1;
sampler2D _MainTex2;
sampler2D _MainTex3;
sampler2D _Normals;
half _NormalStrength;
half _Brightness;

struct EditorSurfaceOutput {
           
           	half3 Albedo;
            half3 Normal;
            half3 Emission;
            half3 Gloss;
            half Specular;
            half Alpha;
            half4 Custom;
         };

struct Input {
	float2 uv_MainTex0;  
	float2 uv_Normals;
};
float2 dbl_uv_MainTex0;
void surf (Input IN, inout SurfaceOutput o) {

	//multiply the current vertex texture coordinate by two  
    dbl_uv_MainTex0 = IN.uv_MainTex0*2;  

    //add an offset to the texture coordinates for each of the input textures  
    half4 c0 = tex2D (_MainTex0, dbl_uv_MainTex0 - float2(0.0, 1.0));  
    half4 c1 = tex2D (_MainTex1, dbl_uv_MainTex0 - float2(1.0, 1.0));  
    half4 c2 = tex2D (_MainTex2, dbl_uv_MainTex0);  
    half4 c3 = tex2D (_MainTex3, dbl_uv_MainTex0 - float2(1.0, 0.0));  
			


    //this if statement assures that the input textures won't overlap  
    if(IN.uv_MainTex0.x >= 0.5)  
    {  
        if(IN.uv_MainTex0.y <= 0.5)  
        {  
            c0 = c1 = c2 = 0;  
        }  
        else  
        {  
            c0 = c2 = c3 = 0;  
        }  
    }  
    else  
    {  
        if(IN.uv_MainTex0.y <= 0.5)  
        {  
            c0 = c1 = c3 = 0;  
        }  
        else  
        {  
            c1 = c2 = c3 = 0;  
        }  
    }  
    
    

    
	float4 Sampled2D0=tex2D(_Normals,IN.uv_Normals.xy);
   	float4 UnpackNormal0=float4(UnpackNormal(Sampled2D0).xyz, 1.0);
	o.Albedo = (c0.rgb + c1.rgb + c2.rgb + c3.rgb)*_Brightness;
	UnpackNormal0.xy = UnpackNormal0.xy*_NormalStrength;
	o.Normal = UnpackNormal0;
	o.Alpha = c0.a + c1.a + c2.a + c3.a;
	o.Normal = normalize(o.Normal);
}
 
void nightcolor (Input IN, SurfaceOutput o, inout fixed4 color)
{
	color.r = saturate(o.Albedo.r - (o.Albedo.r - color.r) * 1.02);
	color.g = saturate(o.Albedo.g - (o.Albedo.g - color.g) * 1.028);
	color.b = saturate(o.Albedo.b - (o.Albedo.b - color.b) * 1.03);
	//color.rgb = saturate(o.Albedo - (o.Albedo - color.rgb) * 1.1);
}
 
ENDCG
}
 
Fallback "Transparent/VertexLit"
}