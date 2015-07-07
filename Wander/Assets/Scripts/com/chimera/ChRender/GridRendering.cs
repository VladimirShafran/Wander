using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridRendering : MonoBehaviour {
	public Material lineMaterial{get;set;}

	public delegate Vector3 ChimeraPixelsToWorldPoint(float x, float y, float z);

	private Dictionary<int, Dictionary<int, Chimera.ChimeraNode>> gridNodes;
	private int wCell;
	private int hCell;

	void Start () 
	{
	}
	
	void Update ()
	{
		Vector3 pos = GetComponent<Camera>().transform.parent.position;
		pos = Camera.main.transform.position + Camera.main.transform.position / ChimeraWorld.scaler;
		pos.z = -10;
		GetComponent<Camera>().transform.position = pos;
		GetComponent<Camera>().orthographicSize = Camera.main.orthographicSize;
	}

	public void StartRendering(Dictionary<int, Dictionary<int, Chimera.ChimeraNode>> gridNodes, int wCell, int hCell)
	{
		this.gridNodes = gridNodes;
		this.wCell = wCell;
		this.hCell = hCell;

		lineMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
		                            "SubShader { Pass { " +
		                            "    Blend SrcAlpha OneMinusSrcAlpha " +
		                            "    ZWrite Off Cull Off Fog { Mode Off } " +
		                            "    BindChannels {" +
		                            "      Bind \"vertex\", vertex Bind \"color\", color }" +
		                            "} } }"
		                            );
		
		lineMaterial.hideFlags = HideFlags.HideAndDontSave;
		lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;

	}

	void OnPostRender()
	{
		if (lineMaterial != null)
		{
			if (gridNodes == null)
			{
				gridNodes = ChimeraWorld.GetChimeraGrid;
			}

			foreach(KeyValuePair<int, Dictionary<int, Chimera.ChimeraNode>> pair in gridNodes)
			{
				foreach(KeyValuePair<int, Chimera.ChimeraNode> pair2 in pair.Value)
                {
					Chimera.ChimeraNode node = pair2.Value;
					RenderTile(node);
				}
			}

			Chimera.ChimeraEventDispather.Instance.DispatchEvent(Chimera.ChimeraEvent.DrawComplete);


		}
	}
        
	private void RenderTile(Chimera.ChimeraNode node)
	{
		float cX = node.x;
		float cY = node.y;

	    lineMaterial.SetPass( 0 );
	    GL.Begin( GL.LINES );

		if (node.a == 0 && node.b == 0)
		{
			GL.Color(Color.red);
		}
		else
		{
			GL.Color( Color.green );
		}
		/////////////////
		ChimeraPixelsToWorldPoint InternalPoint = (float x, float y, float z) =>
		{
			Vector3 point = new Vector3(x,y,z);
			Vector3 transformPoint = Camera.main.ScreenToWorldPoint(point);
			transformPoint *= ChimeraWorld.rescaler;
			transformPoint.z = 0;
			return transformPoint;
		};


		GL.Vertex(InternalPoint(cX - wCell/2, cY, 0));
		GL.Vertex(InternalPoint(cX, cY - hCell/2, 0));
		
		GL.Vertex(InternalPoint(cX, cY - hCell/2, 0));
		GL.Vertex(InternalPoint(cX + wCell/2, cY, 0));
		
		GL.Vertex(InternalPoint(cX + wCell/2, cY, 0));
		GL.Vertex(InternalPoint(cX, cY + hCell/2, 0));
		
		GL.Vertex(InternalPoint(cX, cY + hCell/2, 0));
		GL.Vertex(InternalPoint(cX - wCell/2, cY, 0));
        
        GL.End();
	}

	public void DrawNode(Chimera.ChimeraNode node)
	{

	}
}
