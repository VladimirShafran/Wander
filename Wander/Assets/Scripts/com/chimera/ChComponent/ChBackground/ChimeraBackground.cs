using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Chimera
{
	public class ChimeraBackground {
		private static ChimeraBackground instance;

		private List<Sprite> textureStorage;
		private Dictionary<string, Texture2D> texturePool;

		List<GameObject> nodes;

		private float wTex;
		private float hTex;

		private ChimeraBackground()
		{
			texturePool = new Dictionary<string, Texture2D>();
			textureStorage = new List<Sprite>();

			nodes = new List<GameObject>();
		}

		public static ChimeraBackground Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new ChimeraBackground();
				}
				return instance;
			}
		}

		public void CreateTextureList()
		{
			Sprite[] spriteList = Resources.LoadAll<Sprite>("Chimera");
			textureStorage.AddRange(spriteList);
		}

		public void BgAsTextureCollection(GameObject backgroundContainer, Dictionary<int, List<Vector2>> data)
		{
			int wCount;
			int hCount;
			int counter;

			foreach(KeyValuePair<int, List<Vector2>> set in data)
			{
				for (int i = 0; i < set.Value.Count; i++)
				{
					Vector2 pos = ChimeraWorld.ChimeraToGlobal(set.Value[i]);

					if (pos != null)
					{
						GameObject node = new GameObject();
						node.name = "grassElement";
						node.transform.parent = backgroundContainer.transform;
						node.AddComponent<SpriteRenderer>().sprite = textureStorage[set.Key];

						pos = ChimeraWorld.camera.ScreenToWorldPoint(pos) * ChimeraWorld.rescaler;
	                    node.transform.position = new Vector3(pos.x, pos.y, 0f);
						
						nodes.Add(node);
					}
				}
			}
		}

		public void BuildTileMapByTexture(Dictionary<int, Dictionary<int, Chimera.ChimeraNode>> gridNodes)
		{
//			Sprite sp = Resources.Load<Sprite>("Chimera/TileTexture");
//
//			foreach(KeyValuePair<int, Dictionary<int, Chimera.ChimeraNode>> pair in gridNodes)
//			{
//				foreach(KeyValuePair<int, Chimera.ChimeraNode> pair2 in pair.Value)
//				{
//					Chimera.ChimeraNode node = pair2.Value;
//					
//					GameObject nodeobj = new GameObject();
//					nodeobj.name = "node";
////					nodeobj.transform.parent = ChimeraWorld.Instance.GetChimeraCamera.transform;
//					
//					nodeobj.AddComponent<SpriteRenderer>().sprite = sp;
//					
//					Vector3 position = Vector3.zero;
//					nodeobj.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(node.x, node.y, +10));
//					nodeobj.transform.localScale = new Vector3(100/60,100/60,100/60);
//				}
//			}
		}

		public Vector3 GetSize()
		{
			return nodes[0].GetComponent<SpriteRenderer>().bounds.extents;
		}
	}
}