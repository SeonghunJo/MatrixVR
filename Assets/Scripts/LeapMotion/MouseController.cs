using UnityEngine;
using System.Collections;

public class MouseController : MonoBehaviour {
	StreetViewPoint StreetView_Pointed = null;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit = new RaycastHit();
	
		Ray r= Camera.main.ScreenPointToRay(Input.mousePosition);

		Debug.DrawRay(r.origin, r.direction * 1000, Color.red);
		

		
		if (Physics.Raycast(r, out hit, Mathf.Infinity))
		{
			if(hit.collider != null)
			{
				if(hit.collider.gameObject.tag == "StreetviewPoint")
				{
					StreetView_Pointed = hit.collider.gameObject.GetComponent<StreetViewPoint>();
					Debug.Log (StreetView_Pointed.name);
					if(StreetView_Pointed != null)
					{
						// TODO : Mouse Enter (SHJO)
						//StreetView_Pointed.gameObject.renderer.material.color=Color.blue;
						StreetView_Pointed.Pointed() ;
						
					}

				}
				else
				{
                    if(StreetView_Pointed != null)
					    //StreetView_Pointed.gameObject.renderer.material.color=Color.red;
						StreetView_Pointed.PointedOut() ;
					StreetView_Pointed = null;
				}
			}
		}
		else
		{
			// TODO : Mouse Exit (SHJO)
			if(StreetView_Pointed != null)
			{
				//StreetView_Pointed.gameObject.renderer.material.color=Color.red;
				StreetView_Pointed.PointedOut() ;
				StreetView_Pointed = null;
			}
		}

		if (Input.GetMouseButtonDown (0)) {

			if(StreetView_Pointed != null)
			{				
				Application.LoadLevel("StreetViewer");
			}

		}





	
	}
}
