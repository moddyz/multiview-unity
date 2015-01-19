Shader "PixelMultiplexer" {

	Properties {
		_MainTex ("Main Texture", 2D) = "MainTex" {}
		_Angle ("Angle of Attenuator", Float) = 18.435
		_HInterval ("Horizontal Interval of Attenuator", Float) = 5
		_Views ("Number Of Views", Float) = 5.0
		_IsParallel ("Is Parallel", Float) = 0
		_ParallelShift ("Parallel Shift", Float) = 0
		_RenderTexture1 ("Render Texture1", 2D) = "RenderTexture1" {}
		_RenderTexture2 ("Render Texture2", 2D) = "RenderTexture2" {}
		_RenderTexture3 ("Render Texture3", 2D) = "RenderTexture3" {}
		_RenderTexture4 ("Render Texture4", 2D) = "RenderTexture4" {}
		_RenderTexture5 ("Render Texture5", 2D) = "RenderTexture5" {}
}
	SubShader {
		Pass {
			GLSLPROGRAM
				uniform sampler2D _MainTex;
				uniform float	  _Angle;
				uniform float     _Views;
				uniform float	  _HInterval;
				uniform float	   _IsParallel;
				uniform float	  _ParallelShift;
				varying vec4	  renderTexCoord;
				uniform sampler2D _RenderTexture1;
				uniform sampler2D _RenderTexture2;
				uniform sampler2D _RenderTexture3;
				uniform sampler2D _RenderTexture4;
				uniform sampler2D _RenderTexture5;
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
					float PI = 3.14159265358979323846264;
					float v_interval = _HInterval / (tan(_Angle * PI / 180.0) * 3.0);
					float omega = -gl_FragCoord.y;
					float v_view = mod(omega, v_interval) + 1.0;
					v_view = v_view * _HInterval / v_interval;
					float h_view = mod((gl_FragCoord.x * 3.0) + _HInterval - 1.0 - v_view, _HInterval);
					int r_view, g_view, b_view;
					int _HIntervalInt = int(_HInterval);
					int _ViewsInt = int(_Views);
					r_view = int(h_view);
					g_view = r_view + 1;
					b_view = g_view + 1;
					if (r_view >= _HIntervalInt)
						r_view = r_view - _HIntervalInt;
					if (g_view >= _HIntervalInt)
						g_view = g_view - _HIntervalInt;
					if (b_view >= _HIntervalInt)
						b_view = b_view - _HIntervalInt;
					if (b_view >= _HIntervalInt)
						b_view = b_view - _HIntervalInt;
					r_view = int(float(r_view) * (_Views / _HInterval)) + 1;
					g_view = int(float(g_view) * (_Views / _HInterval)) + 1;
					b_view = int(float(b_view) * (_Views / _HInterval)) + 1;
					vec4 r, g, b;
					vec4 renderTexCoordR, renderTexCoordG, renderTexCoordB;
					renderTexCoordR = renderTexCoord;
					renderTexCoordG = renderTexCoord;
					renderTexCoordB = renderTexCoord;
					if (_IsParallel >= 1.0)
					{
						renderTexCoordR.x = renderTexCoord.x + (((float(r_view)-1.0)/(_HInterval - 1.0))*_ParallelShift);
						renderTexCoordG.x = renderTexCoord.x + (((float(g_view)-1.0)/(_HInterval - 1.0))*_ParallelShift);
						renderTexCoordB.x = renderTexCoord.x + (((float(b_view)-1.0)/(_HInterval - 1.0))*_ParallelShift);
					}
					if ((renderTexCoordR.x) < 1.0 && (renderTexCoordG.x < 1.0) && (renderTexCoordB.x < 1.0))
					{
						if (r_view == 1)
							r = texture2D(_RenderTexture1, vec2(renderTexCoordR));
						else if (r_view == 1)
							r = texture2D(_RenderTexture1, vec2(renderTexCoordR));
						else if (r_view == 2)
							r = texture2D(_RenderTexture2, vec2(renderTexCoordR));
						else if (r_view == 3)
							r = texture2D(_RenderTexture3, vec2(renderTexCoordR));
						else if (r_view == 4)
							r = texture2D(_RenderTexture4, vec2(renderTexCoordR));
						else if (r_view == 5)
							r = texture2D(_RenderTexture5, vec2(renderTexCoordR));
						if (g_view == 1)
							g = texture2D(_RenderTexture1, vec2(renderTexCoordG));
						else if (g_view == 1)
							g = texture2D(_RenderTexture1, vec2(renderTexCoordG));
						else if (g_view == 2)
							g = texture2D(_RenderTexture2, vec2(renderTexCoordG));
						else if (g_view == 3)
							g = texture2D(_RenderTexture3, vec2(renderTexCoordG));
						else if (g_view == 4)
							g = texture2D(_RenderTexture4, vec2(renderTexCoordG));
						else if (g_view == 5)
							g = texture2D(_RenderTexture5, vec2(renderTexCoordG));
						if (b_view == 1)
							b = texture2D(_RenderTexture1, vec2(renderTexCoordB));
						else if (b_view == 1)
							b = texture2D(_RenderTexture1, vec2(renderTexCoordB));
						else if (b_view == 2)
							b = texture2D(_RenderTexture2, vec2(renderTexCoordB));
						else if (b_view == 3)
							b = texture2D(_RenderTexture3, vec2(renderTexCoordB));
						else if (b_view == 4)
							b = texture2D(_RenderTexture4, vec2(renderTexCoordB));
						else if (b_view == 5)
							b = texture2D(_RenderTexture5, vec2(renderTexCoordB));
					}
					else
					{
						r = vec4(0, 0, 0, 1);
						g = vec4(0, 0, 0, 1);
						b = vec4(0, 0, 0, 1);
					}
					gl_FragColor = vec4(r[0], g[1], b[2], 1.0);
				}
				#endif
				ENDGLSL
		}
	}
}