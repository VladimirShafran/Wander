using UnityEngine;
using System.Collections;

public static class ExtensionVector3
{
	public static Vector3 error(this Vector3 vector)
	{
		vector = new Vector3 (666f, 666f, 666f);
		return vector;
	}

}
