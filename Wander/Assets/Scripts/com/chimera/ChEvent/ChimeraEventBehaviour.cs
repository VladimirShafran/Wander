using System;
using UnityEngine;

namespace Chimera
{
	public class ChimeraEventBehaviour : MonoBehaviour
	{
		void Start()
		{

		}

		void Update()
		{
			if(Input.GetMouseButtonDown(0))
			{
				float WorldWidth = Mathf.CeilToInt(ChimeraWorld.Instance.GetWorldSize().x / ChimeraWorld.Instance.GetWCell) * (int)ChimeraWorld.Instance.GetWCell;
				float WorldHeight = Mathf.CeilToInt(ChimeraWorld.Instance.GetWorldSize().y / (ChimeraWorld.Instance.GetHCell/2)) * ((int)ChimeraWorld.Instance.GetHCell/2);

				Vector2 mouseClick = Input.mousePosition;

				WorldWidth *= ChimeraWorld.rescaler;
				WorldHeight *= ChimeraWorld.rescaler;

				Vector3 corectPos = new Vector3(){x = mouseClick.x - (Screen.width/2 - WorldWidth/2), y = mouseClick.y - (Screen.height/2 - WorldHeight/2), z = 0};

//				Vector3 pos = Camera.main.WorldToScreenPoint(Camera.main.ScreenToWorldPoint(corectPos) + Camera.main.transform.position);
				Vector3 pos = Camera.main.WorldToScreenPoint(Camera.main.ScreenToWorldPoint(corectPos) + Camera.main.transform.position);

//				Debug.Log (Camera.main.WorldToScreenPoint(new Vector3(ChimeraBackground.Instance.GetSize().x/4, 0f, 0f)));
				pos /= ChimeraWorld.rescaler; 

				ChimeraEventDispather.Instance.DispatchEvent(ChimeraEvent.OnMouseClick, pos);
			}
		}
	}
}

