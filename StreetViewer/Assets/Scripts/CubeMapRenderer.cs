using UnityEngine;
using System.Collections;

public class CubeMapRenderer : MonoBehaviour {


    /*
    using UnityEngine;
    using System.Collections;

    public class ExampleClass : MonoBehaviour {
        public Cubemap c;
        private Color[] CubeMapColors;
        public Texture2D t;
        void Example() {
            CubeMapColors = t.GetPixels();
            c.SetPixels(CubeMapColors, CubemapFace.PositiveX);
            c.Apply();
        }
    }
    s*/


    public Cubemap cubemap;
    public Texture2D t;

    private Color[] CubeMapColors;

	// Use this for initialization
	void Start () {

        Debug.Log("CubeMapRender Start");

        CubeMapColors = t.GetPixels();
        cubemap.SetPixels(CubeMapColors, CubemapFace.PositiveX);
        cubemap.SetPixels(CubeMapColors, CubemapFace.PositiveY);
        cubemap.SetPixels(CubeMapColors, CubemapFace.PositiveZ);
        cubemap.SetPixels(CubeMapColors, CubemapFace.NegativeX);
        cubemap.SetPixels(CubeMapColors, CubemapFace.NegativeY);
        cubemap.SetPixels(CubeMapColors, CubemapFace.NegativeZ);
        cubemap.Apply();        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
