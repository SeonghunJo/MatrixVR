using UnityEngine;
using System.Collections;

namespace KKR
{
	public class ZoomAnim : MonoBehaviour 
	{
		public static bool first;
		public static bool second;

		void Start () 
		{
			first = false;
			second = false;
		}

		void Update () 
		{
			if(first)
			{
				transform.Translate(Vector3.forward * Time.deltaTime * 7);
			}
			else if(second)
			{
				transform.Translate(Vector3.forward * Time.deltaTime * -7);
			}
		}
	}
}
