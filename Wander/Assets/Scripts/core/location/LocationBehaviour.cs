using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LocationBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		ChimeraWorld chimera = ChimeraWorld.Instance;
		chimera.SetWorldSize (1000, 1000);

		chimera.SetBGList (GetBGDataFromServer);
		chimera.CreateGrid(true, 32, 16, new Vector2(){x = 0, y = 0}, new Bounds(){size = new Vector2(){x = 1000, y = 1000}});

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private Dictionary<int, List<Vector2>> GetBGDataFromServer
	{
		get
		{
			Dictionary<int, List<Vector2>> data = new Dictionary<int, List<Vector2>>();
			
			List<Vector2> row0 = new List<Vector2>();
			
			row0.Add(new Vector2(-4, 4));
			row0.Add(new Vector2(4, 4));
			row0.Add(new Vector2(4, -12));
			row0.Add(new Vector2(-12, -4));
			data[0] = row0;
			
			List<Vector2> row1 = new List<Vector2>();
			
			row1.Add(new Vector2(4, -4));
			row1.Add(new Vector2(-4, -4));
			row1.Add(new Vector2(12, -4));
			row1.Add(new Vector2(-12, 4));
			data[1] = row1;
			
			List<Vector2> row2 = new List<Vector2>();
			
			row2.Add(new Vector2(-12, 12));
			row2.Add(new Vector2(12, 12));
			row2.Add(new Vector2(12, 4));
			row2.Add(new Vector2(4, 12));
			data[2] = row2;
			
			List<Vector2> row3 = new List<Vector2>();
			
			row3.Add(new Vector2(12, -12));
			row3.Add(new Vector2(-12, -12));
			row3.Add(new Vector2(-4, -12));
			row3.Add(new Vector2(-4, 12));
			data[3] = row3;
			
			return data;
		}
	}
}
