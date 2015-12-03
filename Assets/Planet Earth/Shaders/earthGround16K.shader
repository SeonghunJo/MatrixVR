Shader "Custom/EarthGround16K" {
	Properties { 
		//Top Left Texture
        _MainTex0 ("Top Left Texture", 2D) = "white" {}  
        //Top Right --
        _MainTex1 ("Top Right Texture", 2D) = "white" {}  
        //Bottom Left
        _MainTex2 ("Bottom Left Texture", 2D) = "white" {}  
        //Bottom Right
        _MainTex3 ("Bottom Right Texture", 2D) = "white" {} 
        _Brightness ("Brightness", Range (0.1, 1.5)) = 1
        _Normals0("Normal Map Top Left", 2D) = "black" {}
        _Normals1("Normal Map Top Right", 2D) = "black" {}
        _Normals2("Normal Map Bottom Left", 2D) = "black" {}
        _Normals3("Normal Map Bottom Right", 2D) = "black" {}
        _NormalStrength ("Normal Strength", Range (0, 2)) = 0.6
        _Lights0("Light Map Top Left", 2D) = "black" {}
        _Lights1("Light Map Top Right", 2D) = "black" {}
        _Lights2("Light Map Bottom Left", 2D) = "black" {}
        _Lights3("Light Map Bottom Right", 2D) = "black" {}
        _LightScale("Night Lights Intensity", Range (0, 2)) = 1
        _LightRed("Night Lights Red", Range (0, 1)) = 1
        _LightGreen("Night Lights Green", Range (0, 1)) = 0.75
        _LightBlue("Night Lights Blue", Range (0, 1)) = 0.35
        _SpecMap ("Specular Map", 2D) = "black" {}
        
        
   	 	
   	 	_Shininess ("Reflection Shininess", Range (0.03, 2)) = 1
   	 	_ReflectionColor("Reflection Color", Color)= (0.5,0.5,0.34,1)
   	 	
   	 	_AtmosNear("_AtmosNear", Color) = (0.1686275,0.7372549,1,1)
		_AtmosFar("_AtmosFar", Color) = (0.4557808,0.5187039,0.9850746,1)
		_AtmosFalloff("_AtmosFalloff", Float) = 1.8
    }  
    SubShader {  
        Tags { "RenderType"="Opaque" }  
        LOD 200  
  
        CGPROGRAM
    #pragma surface surf BlinnPhongEditor exclude_path:prepass
    #pragma target 3.0
  
        sampler2D _MainTex0;  
        //Added three more 2D samplers, one for each additional texture  
        sampler2D _MainTex1;  
        sampler2D _MainTex2;  
        sampler2D _MainTex3;  
  		sampler2D _Normals0;
  		sampler2D _Normals1;
  		sampler2D _Normals2;
  		sampler2D _Normals3;
  		half _NormalStrength;
  		sampler2D _Lights0;
  		sampler2D _Lights1;
  		sampler2D _Lights2;
  		sampler2D _Lights3;
  		sampler2D _SpecMap;
  		float _LightScale;
  		float _LightRed;
  		float _LightGreen;
  		float _LightBlue;
  		half _Shininess;
  		half _Brightness;
  		float4 _ReflectionColor;
  		float4 _AtmosNear;
	    float4 _AtmosFar;
	    float _AtmosFalloff;
  		
  		struct EditorSurfaceOutput {
           
           	half3 Albedo;
            half3 Normal;
            half3 Emission;
            half3 Gloss;
            half Specular;
            half Alpha;
            half4 Custom;
         };
          
          
         inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
          {
		    half3 spec = light.a * s.Gloss;
		    half4 c;
		    c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
		    c.g -= .01 * s.Alpha;
		    c.r -= .03 * s.Alpha;
		    c.r += _LightRed * min(s.Custom, s.Alpha);
		    c.g += _LightGreen * min(s.Custom, s.Alpha);
		    c.b += _LightBlue * min(s.Custom, s.Alpha);
		    //c.b = saturate(c.b + s.Alpha * .02);
		    c.a = 1.0;
		    return c;

          }

          inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
          {
            half3 h = normalize (lightDir + viewDir);

            half diff = max (0, dot ( lightDir, s.Normal ));

            float nh = max (0, dot (s.Normal, h));
            float spec = pow (nh, s.Specular*128.0);

            half4 res;
            res.rgb = _LightColor0.rgb * diff;
            res.w = spec * Luminance (_LightColor0.rgb);
            res *= atten * 2.0;

        //s.Alpha is set to 1 where the earth is dark.  The value of night lights has been saved to Custom
        half invdiff = 1 - saturate(16 * diff);
        s.Alpha = invdiff;

            return LightingBlinnPhongEditor_PrePass( s, res );
          } 
          
        struct Input {  
        	float3 viewDir;
            float2 uv_MainTex0;  
            float2 uv_Normals0;
            float2 uv_Lights0;
            float2 uv_SpecMap;
        };  
        
        
  
        //this variable stores the current texture coordinates multiplied by 2  
        float2 dbl_uv_MainTex0;
  		float2 dbl_uv_Normals0;
  		float2 dbl_uv_Lights0;
        void surf (Input IN, inout EditorSurfaceOutput o) {  
  			o.Gloss = 0.0;
            o.Specular = 0.5;
            o.Custom = 0.0;
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
                    c0.rgb = c1.rgb = c2.rgb = 0;  
                }  
                else  
                {  
                    c0.rgb = c2.rgb = c3.rgb = 0;  
                }  
            }  
            else  
            {  
                if(IN.uv_MainTex0.y <= 0.5)  
                {  
                    c0.rgb = c1.rgb = c3.rgb = 0;  
                }  
                else  
                {  
                    c1.rgb = c2.rgb = c3.rgb = 0;  
                }  
            }  
            
            //8K Normals
            
            dbl_uv_Normals0 = IN.uv_Normals0*2;
            half4 n0 = tex2D (_Normals0, dbl_uv_Normals0 - float2(0.0, 1.0));  
            half4 n1 = tex2D (_Normals1, dbl_uv_Normals0 - float2(1.0, 1.0));  
            half4 n2 = tex2D (_Normals2, dbl_uv_Normals0);  
            half4 n3 = tex2D (_Normals3, dbl_uv_Normals0 - float2(1.0, 0.0));
            if(IN.uv_Normals0.x >= 0.5)  
            {  
                if(IN.uv_Normals0.y <= 0.5)  
                {  
                    n0 = n1 = n2 = 0;  
                }  
                else  
                {  
                    n0 = n2 = n3 = 0;  
                }  
            }  
            else  
            {  
                if(IN.uv_Normals0.y <= 0.5)  
                {  
                    n0 = n1 = n3 = 0;  
                }  
                else  
                {  
                    n1 = n2 = n3 = 0;  
                }  
            }  
            
            // 8K Lights
            dbl_uv_Lights0 = IN.uv_Lights0*2;
            half4 l0 = tex2D (_Lights0, dbl_uv_Lights0 - float2(0.0, 1.0));  
            half4 l1 = tex2D (_Lights1, dbl_uv_Lights0 - float2(1.0, 1.0));  
            half4 l2 = tex2D (_Lights2, dbl_uv_Lights0);  
            half4 l3 = tex2D (_Lights3, dbl_uv_Lights0 - float2(1.0, 0.0));
            if(IN.uv_Lights0.x >= 0.5)  
            {  
                if(IN.uv_Lights0.y <= 0.5)  
                {  
                    l0 = l1 = l2 = 0;  
                }  
                else  
                {  
                    l0 = l2 = l3 = 0;  
                }  
            }  
            else  
            {  
                if(IN.uv_Lights0.y <= 0.5)  
                {  
                    l0 = l1 = l3 = 0;  
                }  
                else  
                {  
                    l1 = l2 = l3 = 0;  
                }  
            }  
            
            float4 Fresnel0_1_NoInput = float4(0,0,1,1);
	        float4 Fresnel0=(1.0 - dot( normalize( float4( IN.viewDir.x, IN.viewDir.y,IN.viewDir.z,1.0 ).xyz), normalize( Fresnel0_1_NoInput.xyz ) )).xxxx;
	        float4 Pow0=pow(Fresnel0,_AtmosFalloff.xxxx);
	        float4 Saturate0=saturate(Pow0);
	        float4 Lerp0=lerp(_AtmosNear,_AtmosFar,Saturate0);
	        float4 Multiply1=Lerp0 * Saturate0;
            
  			//float4 Sampled2D0=tex2D(_Normals,IN.uv_Normals.xy);
        	float4 UnpackNormal0=float4(UnpackNormal(n0+n1+n2+n3), 1.0);

        	fixed4 specTex = tex2D(_SpecMap, IN.uv_SpecMap);

            //sum the colors and the alpha, passing them to the Output Surface 'o'  
            o.Albedo = (c0.rgb + c1.rgb + c2.rgb + c3.rgb+ Multiply1)*_Brightness;
            
            UnpackNormal0.xy = UnpackNormal0.xy*_NormalStrength;
            UnpackNormal0.x = -UnpackNormal0.x;
            o.Normal = UnpackNormal0;//+UnpackNormal1;//+UnpackNormal2+UnpackNormal3;
            
            o.Emission = 0.0;
			o.Gloss = specTex.r * _ReflectionColor.rgb;
            o.Specular = _Shininess;
        	o.Custom = (l0.r + l1.r + l2.r + l3.r) * _LightScale;
            o.Alpha = 1;// c0.a + c1.a + c2.a + c3.a ;  
            o.Normal = normalize(o.Normal);
        }  
        ENDCG  
    }  
    FallBack "Specular"
}  