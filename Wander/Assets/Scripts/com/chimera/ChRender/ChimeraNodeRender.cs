using UnityEngine;
using System.Collections;

namespace Chimera
{
	public class ChimeraNodeRender {
		private static ChimeraNodeRender instance;

		private Material lineMaterial;

		private ChimeraNodeRender()
		{
			lineMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
			                            "SubShader { Pass { " +
			                            "    Blend SrcAlpha OneMinusSrcAlpha " +
			                            "    ZWrite Off Cull Off Fog { Mode Off } " +
			                            "    BindChannels {" +
			                            "      Bind \"vertex\", vertex Bind \"color\", color }" +
			                            "} } }" );

			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;

//			Camera.main.gameObject.AddComponent<GridRendering>().lineMaterial = lineMaterial;
//			Camera.main.GetComponent<GridRendering>().lineMaterial = lineMaterial;
		}

		public static ChimeraNodeRender Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new ChimeraNodeRender();
				}
				return instance;
			}
		}

		public void CreateTexture(int cellWidth, int cellHeight)
		{
			lineMaterial.SetPass( 0 );
			GL.Begin(GL.LINES);
			GL.Color ( new Color(1,1,1,0.5f));
			GL.Vertex3(0,0,0);
			GL.Vertex3(1,0,0);
			GL.Vertex3(0,1,0);
			GL.Vertex3(1,1,0);
			GL.Color(new Color(0,0,0,0.5f));
			GL.Vertex3(0,0,0);
			GL.Vertex3(0,1,0);
			GL.Vertex3(1,0,0);
			GL.Vertex3(1,1,0);
			GL.End();
		}
	}
}
