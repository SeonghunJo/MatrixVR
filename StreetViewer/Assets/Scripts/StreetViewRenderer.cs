using UnityEngine;
using System.Collections;

using System.IO; // For File Class
using System;
using LitJson; // http://lbv.github.io/litjson/

public class StreetViewRenderer : MonoBehaviour
{
	public string panoramaID = "2C2MIWjhrZUAAAQfCVLQkw";
	// zMrHSTO0GCYAAAQINlCkXg
	// 2C2MIWjhrZUAAAQfCVLQkw

    public Texture2D panoramaTexture; // for merged street view
    public string saveTextureFileName;
	public Material skyboxMaterial;

	// SET BY GOOGLE
	private string cbkURL = "http://maps.google.com/cbk?";
	private int imageWidth = 0;
	private int imageHeight = 0;
	private int tileWidth = 0;
	private int tileHeight = 0;
	private int zoomLevels;

	// SET BY SCRIPT & CALCULATE
	private int textureWidth = 0; // imageWidth / (( zoomLevels - zoom ) * 2)
	private int textureHeight = 0;// imageHeight / (( zoomLevels - zoom ) * 2)
	private int rowTilesNum = 0;
	private int colTilesNum = 0;
	private int totalTilesNum; // rowTileNums * colTileNums;

	// SET BY DEVELOPER
	private int zoom = 3;

	Texture2D[,] tiles;
    
    int count;

	// Panorama To Cubemap
	public const int FACE_FRONT  = 0;
	public const int FACE_BACK   = 1;
	public const int FACE_LEFT   = 2;
	public const int FACE_RIGHT  = 3;
	public const int FACE_UP     = 4;
	public const int FACE_DOWN   = 5;

	private float m_direction = 0.0f;
	private string [] m_textureSize = {"64", "128", "256", "512", "1024", "2048"};
	private int m_textureSizeIndex = 4;

	public Texture2D cubeTextureFront  = null;
	public Texture2D cubeTextureBack   = null;
	public Texture2D cubeTextureLeft   = null;
	public Texture2D cubeTextureRight  = null;
	public Texture2D cubeTextureUp     = null;
	public Texture2D cubeTextureDown   = null;

	// For Generate Buttons
	void OnGUI()
	{
		if(GUI.Button(new Rect(0,0,100,20), "Material 1"))
		{
			print("Current Skybox Shader Name " + RenderSettings.skybox.shader.name);
		}
		if(GUI.Button(new Rect(0, 30, 100, 20), "Material 2"))
		{
			print("Current Skybox Shader Name " + RenderSettings.skybox.shader.name);
		}
	}

    //http://cbk0.google.com/cbk?output=tile&panoid=Q_7cCDOIMymvWZcLQoOTjQ&zoom=3&x=2&y=1
    IEnumerator GoogleStreetViewTiled(string output, string pano_id, int zum, int x, int y)
    {
		string url = cbkURL
            + "output=" + output
			+ "&panoid=" + pano_id
            + "&zoom=" + zum
            + "&x=" + x
            + "&y=" + y;

        print("Download : " + url);

        WWW www = new WWW(url);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
            Debug.Log("Panorama " + name + ": " + www.error);
        else
            print("Panorama " + name + " loaded url " + url);

        print(y + "," + x + " image Downloaded");
        tiles[y, x] = www.texture;

        count++;
		if (count == totalTilesNum)
        {
            count = 0;
            MergeTiles();
        }

        if (saveTextureFileName != "")
        {
            string realSavePath = Application.persistentDataPath + "/" + saveTextureFileName + "_" + y + "_" + x + ".png";
            Debug.Log("Encode Texture : " + realSavePath);

            byte[] png = tiles[y, x].EncodeToPNG();
            File.WriteAllBytes(realSavePath, png);
        }
    }


    void SetSkybox(Material material)
    {
        GameObject camera = Camera.main.gameObject;
        Skybox skybox = camera.GetComponent<Skybox>();
        if (skybox == null)
            skybox = camera.AddComponent<Skybox>();
        skybox.material = material;
    }

	private int m_GetCubemapTextureSize() {
		int size = 512;
		switch (m_textureSizeIndex) {
		case 0:
			size = 64;
			break;
		case 1:
			size = 128;
			break;
		case 2:
			size = 256;
			break;
		case 3:
			size = 512;
			break;
		case 4:
			size = 1024;
			break;
		case 5:
			size = 2048;
			break;
		}
		return size;
	}

	private void m_ConvertPanoramaToCubemap() {

		int texSize = m_GetCubemapTextureSize();
		
		cubeTextureFront  = m_CreateCubemapTexture(texSize, StreetViewRenderer.FACE_FRONT,  panoramaID + "_front.png");
		cubeTextureBack   = m_CreateCubemapTexture(texSize, StreetViewRenderer.FACE_BACK,   panoramaID + "_back.png");
		cubeTextureLeft   = m_CreateCubemapTexture(texSize, StreetViewRenderer.FACE_LEFT,   panoramaID + "_left.png");
		cubeTextureRight  = m_CreateCubemapTexture(texSize, StreetViewRenderer.FACE_RIGHT,  panoramaID + "_right.png");
		cubeTextureUp     = m_CreateCubemapTexture(texSize, StreetViewRenderer.FACE_UP,     panoramaID + "_up.png");
		cubeTextureDown   = m_CreateCubemapTexture(texSize, StreetViewRenderer.FACE_DOWN,   panoramaID + "_down.png");

		RenderSettings.skybox = SkyboxRenderer.CreateSkyboxMaterial(cubeTextureFront, cubeTextureBack, cubeTextureLeft, cubeTextureRight, cubeTextureUp, cubeTextureDown);
	}

	private Texture2D m_CreateCubemapTexture(int texSize, int faceIndex, string fileName = null) {
		Texture2D tex = new Texture2D(texSize, texSize, TextureFormat.RGB24, false);
		
		Vector3 [] vDirA = new Vector3[4];
		if (faceIndex == StreetViewRenderer.FACE_FRONT) {
			vDirA[0] = new Vector3(-1.0f, -1.0f, -1.0f);
			vDirA[1] = new Vector3( 1.0f, -1.0f, -1.0f);
			vDirA[2] = new Vector3(-1.0f,  1.0f, -1.0f);
			vDirA[3] = new Vector3( 1.0f,  1.0f, -1.0f);
		}
		if (faceIndex == StreetViewRenderer.FACE_BACK) {
			vDirA[0] = new Vector3( 1.0f, -1.0f, 1.0f);
			vDirA[1] = new Vector3(-1.0f, -1.0f, 1.0f);
			vDirA[2] = new Vector3( 1.0f,  1.0f, 1.0f);
			vDirA[3] = new Vector3(-1.0f,  1.0f, 1.0f);
		}
		if (faceIndex == StreetViewRenderer.FACE_LEFT) {
			vDirA[0] = new Vector3( 1.0f, -1.0f, -1.0f);
			vDirA[1] = new Vector3( 1.0f, -1.0f,  1.0f);
			vDirA[2] = new Vector3( 1.0f,  1.0f, -1.0f);
			vDirA[3] = new Vector3( 1.0f,  1.0f,  1.0f);
		}
		if (faceIndex == StreetViewRenderer.FACE_RIGHT) {
			vDirA[0] = new Vector3(-1.0f, -1.0f,  1.0f);
			vDirA[1] = new Vector3(-1.0f, -1.0f, -1.0f);
			vDirA[2] = new Vector3(-1.0f,  1.0f,  1.0f);
			vDirA[3] = new Vector3(-1.0f,  1.0f, -1.0f);
		}
		if (faceIndex == StreetViewRenderer.FACE_UP) {
			vDirA[0] = new Vector3(-1.0f,  1.0f, -1.0f);
			vDirA[1] = new Vector3( 1.0f,  1.0f, -1.0f);
			vDirA[2] = new Vector3(-1.0f,  1.0f,  1.0f);
			vDirA[3] = new Vector3( 1.0f,  1.0f,  1.0f);
		}
		if (faceIndex == StreetViewRenderer.FACE_DOWN) {
			vDirA[0] = new Vector3(-1.0f, -1.0f,  1.0f);
			vDirA[1] = new Vector3( 1.0f, -1.0f,  1.0f);
			vDirA[2] = new Vector3(-1.0f, -1.0f, -1.0f);
			vDirA[3] = new Vector3( 1.0f, -1.0f, -1.0f);
		}
		
		Vector3 rotDX1 = (vDirA[1] - vDirA[0]) / (float)texSize;
		Vector3 rotDX2 = (vDirA[3] - vDirA[2]) / (float)texSize;
		
		float dy = 1.0f / (float)texSize;
		float fy = 0.0f;
		
		Color [] cols = new Color[texSize];
		for (int y = 0; y < texSize; y++) {
			Vector3 xv1 = vDirA[0];
			Vector3 xv2 = vDirA[2];
			for (int x = 0; x < texSize; x++) {
				Vector3 v = ((xv2 - xv1) * fy) + xv1;
				v.Normalize();
				cols[x] = m_CalcProjectionSpherical(v);
				xv1 += rotDX1;
				xv2 += rotDX2;
			}
			tex.SetPixels(0, y, texSize, 1, cols);
			fy += dy;
		}
		tex.wrapMode = TextureWrapMode.Clamp;		// cubemapの場合は、wrapModeでClampしないと境界が見えてしまう.
		tex.Apply();

		if (saveTextureFileName != "")
		{
			string realSavePath = Application.persistentDataPath + "/" + fileName;
			Debug.Log("Encode Texture : " + realSavePath);
			
			byte[] png = tex.EncodeToPNG();
			File.WriteAllBytes(realSavePath, png);
		}

		return tex;
	}

	
	private Color m_CalcProjectionSpherical(Vector3 vDir) {
		float theta = Mathf.Atan2(vDir.z, vDir.x);		// -π ～ +π（水平方向の円周上の回転）.
		float phi   = Mathf.Acos(vDir.y);				//  0  ～ +π（垂直方向の回転）.
		
		theta += m_direction * Mathf.PI / 180.0f;
		while (theta < -Mathf.PI) theta += Mathf.PI + Mathf.PI;
		while (theta > Mathf.PI) theta -= Mathf.PI + Mathf.PI;
		
		float dx = theta / Mathf.PI;		// -1.0 ～ +1.0.
		float dy = phi / Mathf.PI;			//  0.0 ～ +1.0.
		
		dx = dx * 0.5f + 0.5f;
		int px = (int)(dx * (float)panoramaTexture.width);
		if (px < 0) px = 0;
		if (px >= panoramaTexture.width) px = panoramaTexture.width - 1;
		int py = (int)(dy * (float)panoramaTexture.height);
		if (py < 0) py = 0;
		if (py >= panoramaTexture.height) py = panoramaTexture.height - 1;
		
		Color col = panoramaTexture.GetPixel(px, panoramaTexture.height - py - 1);
		return col;
	}


    void MergeTiles()
    {
		int penPosX;
		int penPosY;

		for (int i = 0; i < rowTilesNum; i++)
        {
            for (int j = 0; j < colTilesNum; j++)
            {
                Texture2D cp = tiles[i, j];
                if (cp == null)
                {
                    print("cp is null " + i + "," + j);
                    continue;
                }

                for (int y = 0; y < cp.height; y++)
                {
                    for (int x = 0; x < cp.width; x++)
                    {
						penPosX = (tileWidth * j) + x;
						penPosY = textureHeight - (tileHeight * (i + 1)) + y;
						if(penPosX < textureWidth && penPosY > 0)
							panoramaTexture.SetPixel(penPosX, penPosY, cp.GetPixel(x, y));
                    }
                }
            }
        }
        panoramaTexture.Apply();

        skyboxMaterial = new Material(Shader.Find("RenderFX/Skybox Cubed"));
        if (skyboxMaterial == null)
            print("Material is null");

		print ("Draw Skybox Texture");

		m_ConvertPanoramaToCubemap();
    }

	// For WWWHelper Class
	void OnHttpRequest(int id, WWW www) {
		print ("WWW URL : " + www.url);
		if (www.error != null) {
			Debug.Log ("[Error] " + www.error);
			return;
		}

		JsonData json = JsonMapper.ToObject (www.text);
		JsonData data = json["Data"];
		
		imageWidth = Convert.ToInt32(data["image_width"].ToString());
		imageHeight = Convert.ToInt32(data["image_height"].ToString());
		tileWidth = Convert.ToInt32(data["tile_width"].ToString());
		tileHeight = Convert.ToInt32(data["tile_height"].ToString());

		print ("image width : " + imageWidth + " image height : " + imageHeight);
		print ("tile width : " + tileWidth + " tile height : " + tileHeight);


		JsonData location = json["Location"];
		zoomLevels = Convert.ToInt32(location["zoomLevels"].ToString());

		// 현재 파노라마 위치에서 갈 수 있는 방향 및 파노라마 ID 정보를 파싱한다.
		JsonData links = json["Links"];
		int count = links.Count;
		
		for (int i=0; i<count; i++) {
			JsonData item = links[i];
			string linkID = item["panoId"].ToString();
			string yawDeg = item["yawDeg"].ToString();

			print ("Link ID : " + linkID + " yawDeg : " + yawDeg);
		}

		textureWidth = imageWidth / (( zoomLevels - zoom ) * 2);
		textureHeight = imageHeight / (( zoomLevels - zoom) * 2);
		// 타일과 더불어 통합된 파노라마 텍스쳐 이미지를 얻는다.
		GetPanoramaImage(panoramaID, textureWidth, textureHeight); 
	}

	void GetPanoramaImage(string pano_id, int width, int height)
	{
		print ("Get Panorama Image - ID : " + pano_id + " width : " + width + " height : " + height);

		panoramaTexture = new Texture2D(width, height);

		string output = "tile";
		int x = 0;
		int y = 0;
		count = 0;

		rowTilesNum = height/tileHeight;
		if((height % tileHeight) > 0)
			rowTilesNum += 1;

		colTilesNum = width/tileWidth;
		if((width % tileHeight) > 0)
			colTilesNum += 1;

		totalTilesNum = rowTilesNum * colTilesNum;

		tiles = new Texture2D[rowTilesNum, colTilesNum];
		
		for (y = 0; y < rowTilesNum; y++)
		{
			for (x = 0; x < colTilesNum; x++)
			{
				StartCoroutine(GoogleStreetViewTiled(output, panoramaID, zoom, x, y));
			}
		}
	}

	/* JSON Format
	Data:{
	image_width:
	image_height:
	tile_width:
	tile_height:
	image_date
	}
	Projection:{
		"projection_type":"spherical","pano_yaw_deg":"302.11","tilt_yaw_deg":"-180","tilt_pitch_deg":"0"
	}
	Location:{
		panoId:
		level_id:
		zoomLevels:
		lat:
		lng:
		original_lat:
		original_lng:
		description:
		region:
		country:
	}
	Links:[{"yawDeg":"162.08","panoId":"lU_IGRhMM3oAAAQINlMI5w","road_argb":"0x80fdf872","description":"","scene":"1"},
			{"yawDeg":"358.9","panoId":"iP_znBxAS-IAAAQINlCqtA","road_argb":"0x80fdf872","description":"","scene":"1"},
			{"yawDeg":"230.09","panoId":"LbmCZ1nt-bgAAAQINlMIVQ","road_argb":"0x80fdf872","description":"","scene":"1"}]	
	}
	 */
	
	// Use this for initialization
	void Start()
	{
		/* 스트리트뷰 정보 확인 */
		WWWHelper helper = WWWHelper.Instance;
		helper.OnHttpRequest += OnHttpRequest;
		helper.get (100, cbkURL + "output=json" + "&panoid=" + panoramaID);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

