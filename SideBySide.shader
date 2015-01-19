Shader "SideBySide" {

	Properties {
		_MainTex ("Main Texture", 2D) = "MainTex" {}
		_Angle ("Angle of Attenuator", Float) = 18.435
		_HInterval ("Horizontal Interval of Attenuator", Float) = 8
		_Views ("Number Of Views", Float) = 2.0
		_IsParallel ("Is Parallel", Float) = 0
		_ParallelShift ("Parallel Shift", Float) = 0
		_RenderTexture1 ("Render Texture1", 2D) = "RenderTexture1" {}
		_RenderTexture2 ("Render Texture2", 2D) = "RenderTexture2" {}

}
	SubShader {
		Pass {
			GLSLPROGRAM
				uniform sampler2D _MainTex;
				uniform float	  _Angle;
				uniform float	  _Views;
				uniform float	  _HInterval;
				uniform float	   _IsParallel;
				uniform float	  _ParallelShift;
				varying vec4	  renderTexCoord;
				uniform sampler2D _RenderTexture1;
				uniform sampler2D _RenderTexture2;
				#ifdef VERTEX
				void main()
				{
					renderTexCoord = gl_MultiTexCoord0;
					gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
				}
				#endif
				#ifdef FRAGMENT
				void main()
				{
					int view = 0;
					
					if (renderTexCoord.x > 0.5)
						view = 1;
					
					vec4 outClr = vec4(0.0, 0.0, 0.0, 1.0);
					vec4 outTexCoord = renderTexCoord;
					
					if (view == 0)
					{
						outTexCoord.x = outTexCoord.x * 2.0;
						outClr = texture2D(_RenderTexture2, vec2(outTexCoord));
					}
					else if (view == 1)
					{
						outTexCoord.x = (outTexCoord.x - 0.5) * 2.0;
						outClr = texture2D(_RenderTexture1, vec2(outTexCoord));
					}
					
					gl_FragColor = outClr;
					
				}
				#endif
				ENDGLSL
		}
	}
}