using Chimera;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChimeraWorld {
	static private ChimeraWorld instance;

	private GameObject background;
	private GameObject foreground;

	private GameObject backContainer;
	private GameObject mContainer;
	private GameObject gridContainer;
	private GameObject gameContainer;

	private GameObject bufferContainer;
	private GameObject windowContainer;

	private Camera chimeraCamera;

	private Bounds rangeGrid;
	private Vector2 centralPoint;

	private ChimeraGrid grid;
	private Dictionary<int, List<Vector2>> bgData;

	private bool isGridDraw = false;

	private float wCell = 32;
	private float hCell = 16;

	private float wWorld;
	private float hWorld;

	private ChimeraWorld()
	{
		wWorld = 0;
		hWorld = 0;

		mContainer = new GameObject();
		background = new GameObject();
		gameContainer = new GameObject();
		foreground = new GameObject();
		bufferContainer = new GameObject();
		gridContainer = new GameObject();
		backContainer = new GameObject();

		chimeraCamera = new GameObject().AddComponent<Camera>();

		chimeraCamera.gameObject.name = "Chimera Camera";
		chimeraCamera.gameObject.transform.parent = mContainer.transform;
		
		chimeraCamera.clearFlags = CameraClearFlags.Depth;
		chimeraCamera.cullingMask = 0;
		chimeraCamera.orthographic = true;
		chimeraCamera.orthographicSize = 170;
		chimeraCamera.nearClipPlane = 0.3f;
		chimeraCamera.farClipPlane = 1000;
		chimeraCamera.transform.position = new Vector3(0, 0, -10);
		chimeraCamera.depth = -1;
		chimeraCamera.gameObject.AddComponent<GridRendering>();

		mContainer.name = "ChimeraWorld";
		mContainer.transform.position = new Vector3(0,0,5);

		background.name = "Background";
		background.transform.parent = mContainer.transform;
		background.layer = LayerMask.NameToLayer( "Default" );

		gameContainer.name = "GameContainer";
		gameContainer.transform.parent = mContainer.transform;
		gameContainer.layer = LayerMask.NameToLayer( "Default" );

		foreground.name = "Foreground";
		foreground.transform.parent = mContainer.transform;
		foreground.layer = LayerMask.NameToLayer( "Default" );

		bufferContainer.name = "Buffer";
		bufferContainer.transform.parent = mContainer.transform;
		bufferContainer.layer = LayerMask.NameToLayer( "Default" );

		backContainer.name = "BackContainer";
		backContainer.transform.parent = mContainer.transform;
		backContainer.layer = LayerMask.NameToLayer( "Default" );

		gridContainer.name = "GridContainer";
		gridContainer.transform.parent = mContainer.transform;
		gridContainer.layer = LayerMask.NameToLayer( "Default" );

		ChimeraBackground.Instance.CreateTextureList();
	}

	static public ChimeraWorld Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new ChimeraWorld();
			}
			return instance;
		}
	}

	public void SetWorldSize(float width, float height)
	{
		wWorld = width;
		hWorld = height;

		if (wWorld < wCell || hWorld < hCell)
		{
			Debug.LogError("Tiny size of world");
		}
	}

	public void SetBGList(Dictionary<int, List<Vector2>> data)
	{
		bgData = data;
	}

	public Vector2 GetWorldSize()
	{
		return new Vector2(wWorld, hWorld);
	}

	public void CreateGrid(bool isDraw, int wCell, int hCell, Vector2 startPoint, Bounds range)
	{
		isGridDraw = isDraw;

		rangeGrid = range != null ? range : new Bounds(){size = new Vector3(){x = wWorld, y = hWorld, z = 0f}};
		centralPoint = startPoint;// != null ? startPoint : new Vector2(){x = rangeGrid.size.x/2, y = rangeGrid.size.y/2};

		grid = ChimeraGrid.Instance;

		wCell = wCell;
		hCell = hCell;

		ChimeraEventDispather.Instance.AddEventListener(ChimeraEvent.CalculateComplete, null, CalculationComplete);
		ChimeraEventDispather.Instance.AddEventListener(ChimeraEvent.DrawComplete, null, GridDrawComplete);

		grid.Create(wCell, hCell, centralPoint, rangeGrid);

	}

	private void CalculationComplete(ChimeraEventResponce responce)
	{
		ChimeraEventDispather.Instance.RemoveEventListener(ChimeraEvent.CalculateComplete, CalculationComplete);
		mContainer.AddComponent<ChimeraEventBehaviour>();
		ChimeraEventDispather.Instance.AddEventListener(ChimeraEvent.OnMouseClick, null, OnMouseClickHandler);

		if (isGridDraw)
		{
			grid.Draw(gridContainer);
		}
		else
		{
			ChimeraEventDispather.Instance.DispatchEvent(ChimeraEvent.DrawComplete);
		}
	}

	private void GridDrawComplete(ChimeraEventResponce response)
	{
		ChimeraEventDispather.Instance.RemoveEventListener(ChimeraEvent.DrawComplete, GridDrawComplete);
		CreateBackground();
	}

	private void CreateBackground()
	{
		if (bgData != null)
		{
			ChimeraBackground.Instance.BgAsTextureCollection(background, bgData);
		}
	}

	public static Camera camera
	{
		get
		{
			Camera result = instance.chimeraCamera;
			return result;
		}
	}

	public static float scaler
	{
		get
		{
			return 100 / (Screen.height/( 2 * Camera.main.orthographicSize));
		}
	}

	public static float rescaler
	{
		get
		{
			return (Screen.height/( 2 * Camera.main.orthographicSize)) / 100;
		}
	}

	public static Dictionary<int, Dictionary<int, ChimeraNode>> GetChimeraGrid
	{
		get
		{
			Dictionary<int, Dictionary<int, ChimeraNode>> result = instance.grid.GetNodes;
			return result;
		}
	}

	private void OnMouseClickHandler(ChimeraEventResponce response)
	{
		Vector3 posOnMap = grid.GlobalToChimera(response.mousePosition);
		Debug.Log(posOnMap);
		posOnMap = posOnMap == Vector3.zero.error() ? posOnMap : grid.ChimeraToGlobal(posOnMap);

		if (posOnMap != Vector3.zero.error())
		{
			Vector3 newPos = Camera.main.ScreenToWorldPoint(posOnMap) * ChimeraWorld.rescaler;
			newPos.z = -5;
			ChimeraEventDispather.Instance.DispatchEvent(ChimeraEvent.HoverOnTile, newPos);
		}
	}

	public float GetWCell
	{
		get
		{
			return wCell;
		}
	}

	public float GetHCell
	{
		get
		{
			return hCell;
		}
	}

	public GameObject GetMContainer
	{
		get
		{
			return mContainer;
		}
	}

	public static Vector2 ChimeraToGlobal(Vector2 chPoint)
	{
		Vector2 result = instance.grid.ChimeraToGlobal(chPoint);
		return result;
	}

	public static void HoverHandler(EventDelegat listener)
	{
		ChimeraEventDispather.Instance.AddEventListener(ChimeraEvent.HoverOnTile, null, listener);
	}

	public static void RemoveHoverHandler(EventDelegat listener)
	{
		ChimeraEventDispather.Instance.RemoveEventListener(ChimeraEvent.HoverOnTile, listener);
	}

	public static bool HaveHoverHandler(EventDelegat listener)
	{
		return ChimeraEventDispather.Instance.HasEventListener(ChimeraEvent.HoverOnTile, listener);
	}

	public static List<Vector3> SearchWay(Vector3 currentPosition, Vector3 destinationPosition)
	{
		Vector3 chCurrent = instance.grid.WorldToChimera(currentPosition);
		Vector3 chDestination = instance.grid.WorldToChimera(destinationPosition);

		ChimeraNode nodeCurrent = instance.grid.GetNode((int)chCurrent.x, (int)chCurrent.y);
		ChimeraNode nodeDestination = instance.grid.GetNode((int)chDestination.x, (int)chDestination.y);

		List<ChimeraNode> chWay = new List<ChimeraNode>();
		List<Vector3> result = new List<Vector3>(); 

		try
		{
			chWay = instance.grid.SearchWay(nodeCurrent, nodeDestination);
			result = instance.grid.ChimeraNodeToVector3(chWay);
		}
		catch
		{
		}

		return result;
	}
}
