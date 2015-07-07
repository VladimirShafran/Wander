using UnityEngine;
using System.Collections;

namespace Chimera
{
	public class ChimeraContainer : MonoBehaviour {
		private GameObject container = new GameObject();
		private float _width = 0;
		private float _height = 0;

		public float x
		{
			get
			{
				return container.transform.localPosition.x;
			}

			set
			{
				Vector3 cPos = container.transform.localPosition;
				cPos.x = value;
				container.transform.localPosition = cPos;
			}
		}

		public float y
		{
			get
			{
				return container.transform.localPosition.y;
			}
			
			set
			{
				Vector3 cPos = container.transform.localPosition;
				cPos.y = value;
				container.transform.localPosition = cPos;
			}
		}

		public Vector3 Position
		{
			get
			{
				return container.transform.localPosition;
			}
			
			set
			{
				container.transform.localPosition = value;
			}
		}

		public Vector3 Scale
		{
			get
			{
				return container.transform.localScale;
			}
			
			set
			{
				container.transform.localScale = value;
			}
		}

		public float Width
		{
			get
			{
				return _width;
			}
			set
			{
				_width = value;
			}
		}

		public float Height
		{
			get
			{
				return _height;
			}
			set
			{
				_height = value;
			}
		}
	}
}
