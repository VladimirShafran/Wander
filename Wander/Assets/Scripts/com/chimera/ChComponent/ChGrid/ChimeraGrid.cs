using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Chimera
{
	public class ChimeraGrid 
	{
		static private ChimeraGrid instance;

		private int wCell;
		private int hCell;
		private float edgeCell;
		private Vector2 cPoint;

		private float wGrid;
		private float hGrid;
		private float edgeGrid;
		private Bounds rangeGrid;

		private Dictionary<int, Dictionary<int, ChimeraNode>> gridNodes;
		private ChimeraNodeRender render;

		private Texture2D renderTexture;
		private ChimeraNode topLeftNode;

		private ChimeraGrid()
		{
			wGrid = 0;
			hGrid = 0;
			edgeGrid = 0;
			gridNodes = new Dictionary<int, Dictionary<int, ChimeraNode>>();
			render = ChimeraNodeRender.Instance;
		}

		public static ChimeraGrid Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new ChimeraGrid();
				}

				return instance;
			}
		}

		private void SetParameters(float gridWidth, float gridHeight)
		{
			wGrid = gridWidth;
			hGrid = gridHeight;
			edgeGrid = Mathf.Sqrt((wGrid/2 * wGrid/2) + (hGrid/2 * hGrid));
		}

		public void Create(int cellWidth, int cellHeight, Vector2 centralPoint, Bounds range)
		{
			wCell = cellWidth;
			hCell = cellHeight;
			cPoint = centralPoint;
			rangeGrid = range;

			edgeCell = Mathf.Sqrt((wCell/2 * wCell/2) + (hCell/2 * hCell/2));

			SetParameters(rangeGrid.size.x, rangeGrid.size.y);
			render.CreateTexture(wCell, hCell);

			ComputationVertex();
		}

		private void ComputationVertex()
		{
			int g = (Mathf.CeilToInt((rangeGrid.size.x/2 + cPoint.x) / wCell));
			int f = (Mathf.CeilToInt((rangeGrid.size.y/2 + cPoint.y) / hCell));

			topLeftNode = new ChimeraNode((int)(Screen.width/2 - g * wCell), (int)(Screen.height/2 - f * hCell) + hCell/3);
			topLeftNode.a = g + f;
			topLeftNode.b = f - g;

			g = topLeftNode.a;
			f = topLeftNode.b;

			int wCount = Mathf.CeilToInt(wGrid / wCell);
			int hCount = Mathf.CeilToInt(hGrid / (hCell/2));

			int cx = topLeftNode.x;
			int cy = topLeftNode.y;
			int cg = g;
			int cf = f;

			for (int i = 0; i < hCount ; i++)
			{
				for (int j = 0; j < wCount ; j++)
				{
					cx = topLeftNode.x + j * wCell;
					cy = topLeftNode.y + i * hCell/2;

					if (i % 2 != 0)
					{
						cx = cx - wCell/2;
					}

					cg = g - j;
					cf = f + j;

					ChimeraNode vertex = new ChimeraNode(cx, cy);
					vertex.a = cg;
					vertex.b = cf;

					Debug.Log(cg + "   " + cf);

					if (!gridNodes.ContainsKey(cg))
					{
						gridNodes.Add(cg, new Dictionary<int, ChimeraNode>());
					}

					if (gridNodes.ContainsKey(cg) && !gridNodes[cg].ContainsKey(cf))
					{
						gridNodes[cg].Add(cf, vertex);
					}
				}

				if (i % 2 != 0)
				{
					g -= 1;
				}
				else
				{
					f -= 1;
				}
			}

			ChimeraEventDispather.Instance.DispatchEvent(ChimeraEvent.CalculateComplete);
		}

		public void Draw(GameObject parent)
		{
			ChimeraWorld.camera.gameObject.GetComponent<GridRendering>().StartRendering(gridNodes, wCell, hCell);
		}

		public Vector3 GlobalToChimera(Vector3 point)
		{
			Vector3 chPoint = new Vector3().error();

			int g = Mathf.CeilToInt((point.x/2 + point.y/1) /(hCell));
			int f = Mathf.CeilToInt(-(point.x/2 - point.y/1)/(hCell));

			int cg = topLeftNode.a - g;
			int cf = topLeftNode.b - f;

			try
			{
				Chimera.ChimeraNode node = gridNodes[cg][cf];
				chPoint = new Vector3(cg, cf);

				ChimeraWorld.camera.GetComponent<GridRendering>().DrawNode(node);
			}
			catch
			{
				Debug.LogError ("GlobalToChimera => in => (" + point.x + ", " +point.y + ") out => (" + cg + ", " + cf + ")");
			}
			return chPoint;
		}

		public Vector3 ChimeraToGlobal(Vector3 chPoint)
		{
			Vector3 point = Vector3.zero.error();

			try
			{
				Chimera.ChimeraNode node = gridNodes[(int)chPoint.x][(int)chPoint.y];
				point = new Vector3(node.x, node.y);
			}
			catch
			{
				Debug.LogError ("ChimeraToGlobal => in => (" + chPoint.x + ", " +chPoint.y + ")");
			}
			return point;
		}

		public Vector3 WorldToChimera(Vector3 worldPoint)
		{
			Vector3 chPoint = Vector3.zero.error();
			Vector3 bufferPoint = Camera.main.WorldToScreenPoint(worldPoint);


			float WorldWidth = Mathf.CeilToInt(ChimeraWorld.Instance.GetWorldSize().x / ChimeraWorld.Instance.GetWCell) * (int)ChimeraWorld.Instance.GetWCell;
			float WorldHeight = Mathf.CeilToInt(ChimeraWorld.Instance.GetWorldSize().y / (ChimeraWorld.Instance.GetHCell/2)) * ((int)ChimeraWorld.Instance.GetHCell/2);
			
			Vector2 mouseClick = bufferPoint;
			
			WorldWidth *= ChimeraWorld.rescaler;
			WorldHeight *= ChimeraWorld.rescaler;
			
			Vector3 corectPos = new Vector3(){x = mouseClick.x - (Screen.width/2 - WorldWidth/2), y = mouseClick.y - (Screen.height/2 - WorldHeight/2), z = 0};
			Vector3 pos = Camera.main.WorldToScreenPoint(Camera.main.ScreenToWorldPoint(corectPos) + Camera.main.transform.position);
			pos /= ChimeraWorld.rescaler; 

			chPoint = GlobalToChimera(pos);

			return chPoint;
		}

		public Chimera.ChimeraNode GetNode(int chX, int chY)
		{
			Chimera.ChimeraNode cNode = null;
			try
			{
				cNode = gridNodes[chX][chY];
			}
			catch
			{
				Debug.LogError ("GetNode => in => (" + chX + ", " + chY + ")");
			}
			return cNode;
		}

		public int CellWidth
		{
			get
			{
				return wCell;
			}
		}

		public int CellHeight
		{
			get
			{
                return hCell;
            }
        }

		/**
		 * AStar - поиск пути
		 */

		List<Chimera.ChimeraNode> openList = new List<Chimera.ChimeraNode>();
		List<Chimera.ChimeraNode> closeList = new List<Chimera.ChimeraNode>();

		public List<Chimera.ChimeraNode> SearchWay(Chimera.ChimeraNode start, Chimera.ChimeraNode end)
		{
			openList.Clear();
			closeList.Clear();

			if(start.isWall || end.isWall)
			{
				return null;
			}

			start.parent = null;
			start.g = 0;
			openList.Add(start);

			while(openList.Count > 0)
			{
				Chimera.ChimeraNode currentNode = openList[0];

				for (int i = 0; i < openList.Count; i++)
				{
					if(openList[i].f < currentNode.f)
					{
						currentNode = openList[i];
					}
				}

				if(currentNode.a == end.a && currentNode.b == end.b)
				{
					List<ChimeraNode> result = new List<Chimera.ChimeraNode>();
					while(currentNode!=null)
					{
						result.Add(currentNode);
						currentNode = currentNode.parent;
					}

					result.Reverse();
					return result;
				}

				openList.Remove(currentNode);
				closeList.Add(currentNode);

				List<Chimera.ChimeraNode> neighbors = GetNeighbors(currentNode);

				for (int j = 0; j < neighbors.Count; j++)
				{
					Chimera.ChimeraNode neighbor = null;
					try
					{
						neighbor = neighbors[j];
					}
					catch
					{
					}

					if(neighbor == null || closeList.IndexOf(neighbor) != -1 || neighbor.isWall)
					{
						continue;
					}

					int curG = currentNode.g + 1;
					bool isNew = openList.IndexOf(neighbor) == -1;

					if (isNew)
					{
						neighbor.h = Heuristic(neighbor, end);
						openList.Add(neighbor);
					}

					if(isNew || curG < neighbor.g)
					{
						neighbor.parent = currentNode;
						neighbor.g = curG;
						neighbor.f = neighbor.g + neighbor.h;
					}
				}
			}
			return null;
		}

		private int Heuristic(Chimera.ChimeraNode pos0, Chimera.ChimeraNode pos1)
		{
			int d1 = pos1.x - pos0.x;
			int d2 = pos1.y - pos0.y;

			d1 = d1 < 0 ? -d1 : d1;
			d2 = d2 < 0 ? -d2 : d2;

			return d1 + d2;
		}

		private int _widthMax;
		private int _heightMax;
		private int _widthMin;
		private int _heightMin;
		private List<Chimera.ChimeraNode> _neighbors = new List<Chimera.ChimeraNode>();

		private List<Chimera.ChimeraNode> GetNeighbors(Chimera.ChimeraNode currentNode)
		{
			_neighbors.Clear();

			int x = currentNode.x;
			int y = currentNode.y;

			_widthMax = (int)wGrid - wCell/2;
			_heightMax = (int)hGrid - hCell/2;

			_widthMin = wCell;
			_heightMin = (int)hCell/2;

			ChimeraNode top = null ,bottom = null ,left = null ,right = null;

			if(y > _heightMin)
			{
				top = GetNode(currentNode.a, currentNode.b - 1);
				_neighbors.Add(top);
			}

			if(y < _heightMax)
			{
				bottom = GetNode(currentNode.a, currentNode.b + 1);
				_neighbors.Add(bottom);
			}

			if (x > _widthMin)
			{
				left = GetNode(currentNode.a - 1, currentNode.b);
				_neighbors.Add(left);
//				try
//				{
//					if(!left.isWall)
//					{
//						if (y > _heightMin && !top.isWall)
//						{
//							_neighbors.Add(GetNode(currentNode.a - 1, currentNode.b - 1));
//						}
//						if (y < _heightMax && !bottom.isWall)
//						{
//							_neighbors.Add(GetNode(currentNode.a - 1, currentNode.b + 1));
//						}
//					}
//				}
//				catch
//				{
//					if (_neighbors[_neighbors.Count - 1] == null)
//					{
//						_neighbors.Remove(_neighbors[_neighbors.Count - 1]);
//					}
//				}
			}

			if (x < _widthMax)
			{
				right = GetNode(currentNode.a + 1, currentNode.b);
				_neighbors.Add (right);
//				try
//				{
//					if (!right.isWall)
//					{
//						if (y > _heightMin && !top.isWall)
//						{
//							_neighbors.Add (GetNode(currentNode.a + 1, currentNode.b - 1));
//						}
//						if(y < _heightMax && !bottom.isWall)
//						{
//							_neighbors.Add(GetNode(currentNode.a + 1, currentNode.b + 1));
//						}
//					}
//				}
//                catch
//				{
//					if (_neighbors[_neighbors.Count - 1] == null)
//					{
//						_neighbors.Remove(_neighbors[_neighbors.Count - 1]);
//                    }
//				}
			}
			return _neighbors;
		}

		public Dictionary<int, Dictionary<int, ChimeraNode>> GetNodes
		{
			get
			{
				if(gridNodes == null)
				{
					ComputationVertex();
				}
				return gridNodes;
			}
		}

		public List<Vector3> ChimeraNodeToVector3(List<ChimeraNode> chVector)
		{
			List<Vector3> result = new List<Vector3>();

			for (int i = 0; i < chVector.Count; i++)
			{
				ChimeraNode cNode = chVector[i];
				Vector3 cPos = new Vector3(){x = cNode.x, y = cNode.y, z = 0f};

				Vector3 newPos = Camera.main.ScreenToWorldPoint(cPos) * ChimeraWorld.rescaler;
				newPos.z = -5;

				result.Add(newPos);
			}
			return result;
		}
	}
}