using UnityEngine;
using System.Collections;

namespace Chimera
{
	public class ChimeraNode {
		public int x {get;set;}
		public int y {get;set;}

		public int a {get;set;}
		public int b {get;set;}

		public int g {get;set;}
		public int f {get;set;}

		public int h {get;set;}

		public ChimeraNode parent {get;set;}
		public bool isWall {get;set;}
		public bool isUsed {get;set;}

		public ChimeraNode(int x, int y)
		{
			this.x = x;
			this.y = y;

			a = b = g = f = 0;
			parent = null;
			isWall = isUsed = false;
		}
	}
}