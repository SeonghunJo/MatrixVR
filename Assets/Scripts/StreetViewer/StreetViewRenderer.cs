#define ENABLE_CACHE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.IO; // For File Class
using System.Text;

using LitJson; // http://lbv.github.io/litjson/

// 0. 지역의 메타 데이터 정보는 캐싱 여부와 상관없이 확인할 필요가 있음
// 1. 다운로드된 파노라마 및 큐브맵 이미지 확인
// 2-1. 기존 파노라마가 없으면 기존 방식대로 다운로드
// 2-2. 기존 파노라마가 있으면 큐브맵을 기존 텍스쳐를 로딩하는 방식으로 대체한다.

public class StreetViewRenderer : MonoBehaviour
{
    public string panoramaID = "";
    public string defaultID = "IUmXlW6pRu1w9QnUdQk4vw"; // 성산일출봉
    // "LbmCZ1nt-bgAAAQINlMIVQ"; // 경희궁
    // "zMrHSTO0GCYAAAQINlCkXg"; // 경희궁
    // 2C2MIWjhrZUAAAQfCVLQkw

    public Texture2D panoramaTexture; // for merged street view
    public string saveTextureFileName;
    public Material skyboxMaterial;

    // DRAW Buttons
    public GameObject buttonModel;
    private GameObject[] buttonModelList;

    // GOOGLE RETRIVE VARIABLES
    private string cbkURL = "http://maps.google.com/cbk?";
    private int imageWidth = 0;
    private int imageHeight = 0;
    private int tileWidth = 0;
    private int tileHeight = 0;
    private int zoomLevels;

    private string description;
    private string country;
    private string region;

    // SCRIPT & CALCULATE VARIABLES
    private int textureWidth = 0; // imageWidth / (( zoomLevels - zoom ) * 2)
    private int textureHeight = 0;// imageHeight / (( zoomLevels - zoom ) * 2)
    private int rowTilesNum = 0;
    private int colTilesNum = 0;
    private int totalTilesNum; // rowTileNums * colTileNums;
    
    public float pivotYaw = 0.0f; // Panorama Pivot Yaw Degree ; GetMetaData

    // CBK CUSTOM VARIABLES
    private const int zoom = 3; // Default Panorama Zoom Size
    private Texture2D[,] tiles;
    private int downloadedTilesCount;

    // NEED TO INITIATE VALUE BEFORE START RENDERING
    private bool retrieveMetaData = false;
    private int retryCounter = 0;

    // Panorama To Cubemap
    private const int FACE_FRONT = 0;
    private const int FACE_BACK = 1;
    private const int FACE_LEFT = 2;
    private const int FACE_RIGHT = 3;
    private const int FACE_UP = 4;
    private const int FACE_DOWN = 5;

    private float m_direction = 0.0f;
    private int[] m_textureSize = { 64, 128, 256, 512, 1024, 2048 };
    private int m_textureSizeIndex = 4;
    
    public bool enableCacheDebugging = true; // true 일 경우 캐시된 곳의 Panorama ID 버튼이 노란색으로 표시된다.

    public Texture2D cubeTextureFront = null;
    public Texture2D cubeTextureBack = null;
    public Texture2D cubeTextureLeft = null;
    public Texture2D cubeTextureRight = null;
    public Texture2D cubeTextureUp = null;
    public Texture2D cubeTextureDown = null;

    OVRLoadingScreen screen = null;

	// CACHE
    Queue<string> queue;

    static bool enableStackTrace = false;
    static Stack<string> panoIDStack = null;

    private const int MAX_RETRY = 10;
    private bool isDebugMode = false;
    // Use this for initialization
    void Start()
    {
#if UNITY_EDITOR
        isDebugMode = true;
        InitStackTrace();
#endif
        InitAutoImageCache();
        StartRenderStreetView();
    }

    void InitStackTrace()
    {
        if(panoIDStack == null)
            panoIDStack = new Stack<string>();
        else
            panoIDStack.Clear();
    }

    void InitAutoImageCache()
    {
        if (Manager.Instance.enableAutoImageCache)
        {
            if (queue == null)
                queue = new Queue<string>();
            else
                queue.Clear();

            queue.Enqueue(Manager.Instance.GetPlaceID());
        }
    }

    void OnGUI()
    {
        if(isDebugMode)
        {
            // 연결된 파노라마가 있으면 해당 위치의 파노라마 아이디를 버튼으로 표현한다.
            // 해당 파노라마 아이디가 이미 방문된 상태라면 노란색으로 표시한다.
            if (Manager.Instance.nextIDs != null)
            {
                if (Manager.Instance.nextIDs.Length > 0)
                {
                    for (int i = 0; i < Manager.Instance.nextIDs.Length; i++)
                    {
                        if (enableCacheDebugging)
                        {
                            if (Utility.FindCachedImageFromID(Manager.Instance.nextIDs[i]))
                            {
                                GUI.color = Color.yellow;
                            }
                            else
                            {
                                GUI.color = Color.white;
                            }
                        }

                        if (GUI.Button(new Rect(0, i * 30, 250, 20), Manager.Instance.nextIDs[i]))
                        {
                            print("Direction ID : " + Manager.Instance.nextIDs[i]);
                            Manager.Instance.SetPlaceID(Manager.Instance.nextIDs[i]);
                            StartRenderStreetView();
                        }
                    }
                }
            }

            // 뒤로가기 버튼을 만든다.
            if (panoIDStack != null)
            {
                string backID = "Earth";

                string[] stackList = panoIDStack.ToArray();

                if (panoIDStack.Count > 1)
                {
                    backID = stackList[1];
                    if (enableCacheDebugging)
                    {
                        if (Utility.FindCachedImageFromID(backID))
                        {
                            GUI.color = Color.yellow;
                        }
                        else
                        {
                            GUI.color = Color.white;
                        }
                    }
                }

                if (GUI.Button(new Rect(400, 0, 250, 20), backID))
                {
                    print("Direction ID : " + backID);

                    panoramaID = backID;
                    panoIDStack.Pop();

                    StartRenderStreetView();
                }
            }
        }
    }

    void Update()
    {
		// StreetView 도중 E키를 누르면 지구로 돌아간다
        if(Input.GetKeyDown(KeyCode.E))
        {
            Application.LoadLevel("SceneEarth");
        }
    }

    public void StartRenderStreetView()
    {
        /* 스트리트뷰 정보 확인 */
        print("StreetViewer Start");
#if ENABLE_CACHE
        if( Manager.Instance.enableAutoImageCache )
        {
            StartCoroutine(AutoImageCaching());
        }
        else 
        {   
            StartCoroutine(RenderStreetView());
        }
#endif
		//StartCoroutine(RenderStreetView());

        print("StreetViewer Start End");
    }

    private void GetCachedImageFromID(string id)
    {
        LoadTexture(Utility.cacheFolderPath + "/" + id + "_front.png", ref cubeTextureFront);
		LoadTexture(Utility.cacheFolderPath + "/" + id + "_back.png", ref cubeTextureBack);
		LoadTexture(Utility.cacheFolderPath + "/" + id + "_left.png", ref cubeTextureLeft);
		LoadTexture(Utility.cacheFolderPath + "/" + id + "_right.png", ref cubeTextureRight);
		LoadTexture(Utility.cacheFolderPath + "/" + id + "_up.png", ref cubeTextureUp);
		LoadTexture(Utility.cacheFolderPath + "/" + id + "_down.png", ref cubeTextureDown);
    }

    void Initialize()
    {
        Manager.Instance.SetProgress(0); // 진행률 초기화

        panoramaID = Manager.Instance.GetPlaceID(); // 파노라마 아이디 설정
        print("Panorama ID : " + panoramaID);

        if (panoramaID == null)
            panoramaID = defaultID;

        saveTextureFileName = panoramaID;

        cubeTextureFront = new Texture2D(512, 512);
        cubeTextureBack = new Texture2D(512, 512);
        cubeTextureLeft = new Texture2D(512, 512);
        cubeTextureRight = new Texture2D(512, 512);
        cubeTextureUp = new Texture2D(512, 512);
        cubeTextureDown = new Texture2D(512, 512);

        retrieveMetaData = false;
        retryCounter = 0; // 메타데이터 재시도 횟수
        downloadedTilesCount = 0;
    }

    // Step 1
    IEnumerator RenderStreetView() // INIT VARIABLES AND DOWNLOAD START
    {
        /* 파노라마 렌더링 시작 */
        // 파노라마 아이디를 통해 파노라마 이미지에 대한 부가 정보를 받는다.
        Initialize();
        LoadingScreen.Show();

        if(enableStackTrace)
        {
            panoIDStack.Push(panoramaID);
        }

        if(Manager.Instance.enableAutoImageCache)
        {
            if (queue.Count == 0)
            {
                print("queue empty");
                yield break;
            }
            print("dequeue : " + panoramaID);
            panoramaID = queue.Dequeue();
        }

        string[] stackList = panoIDStack.ToArray();
        for(int i=0; i<stackList.Length; i++)
        {
            Debug.LogWarning("stack " + i.ToString() + " : " + stackList[i]);
        }

        // FIND OVR CAMERA
        GameObject OVRCameraRig = GameObject.Find("LeapOVRCameraRig");
        if(OVRCameraRig != null)
            screen = OVRCameraRig.GetComponent<OVRLoadingScreen>();
        
        if (screen != null)
            screen.ShowScreen();

        Debug.Log("ID -> META DATA");
        do
        {
            yield return StartCoroutine(GetMetaData());
            retryCounter++;
        } while (retrieveMetaData == false && retryCounter < MAX_RETRY);
        Debug.Log("GetMetaData End : Retry Count is " + retryCounter.ToString());
        
        if(retryCounter == MAX_RETRY)
        {
            Debug.LogError("Get Meta Data Failed");
            retryCounter = 0;
        }

        if(Utility.FindCachedImageFromID(panoramaID)) // 해당 데이터가 캐시폴더에 있을 경우
        {
            Debug.Log("Image data is exist");
            // 6방향 이미지를 모두 로드하고
            GetCachedImageFromID(panoramaID);
            if(!enableCacheDebugging)
                yield return new WaitForSeconds(1.0f); // 1초간 대기
            Manager.Instance.SetProgress(20);
            if (!enableCacheDebugging)
                yield return new WaitForSeconds(1.0f); // 1초간 대기
            Manager.Instance.SetProgress(40);
        }
        else
        {
            yield return StartCoroutine(GetPanoramaImage(panoramaID, textureWidth, textureHeight));
            yield return StartCoroutine(MergeTiles());
            yield return StartCoroutine(ConvertPanoramaToCubemap());
        }

        // 메타데이터 받고
        // 기존에 다운로드 받은 큐브맵이 있는지 확인한다.
        // 있다면 바로 스카이박스에 적용시키고 끝낸다.

        SetSkybox();
        DrawButtons();

        if (screen != null)
            screen.HideScreen();

        LoadingScreen.Hide();
		
        if (Manager.Instance.enableAutoImageCache)
        {
            for (int i = 0; i < Manager.Instance.nextIDs.Length; i++)
            {
                if (!Utility.FindCachedImageFromID(Manager.Instance.nextIDs[i]))
                {
                    print("enque : " + Manager.Instance.nextIDs[i]);
                    queue.Enqueue(Manager.Instance.nextIDs[i]);
                }
            }
        }
    }

    IEnumerator AutoImageCaching()
    {
        while(queue.Count != 0)
        {
            yield return StartCoroutine(RenderStreetView());
        }
        Application.LoadLevel("SceneEarth");
    }

    // Step 2
    IEnumerator GetMetaData()
    {
        string metaURL = cbkURL + "output=json" + "&panoid=" + panoramaID;
        WWW www = new WWW(metaURL);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log("WWW Error [Meta Data] : " + www.error);
            retrieveMetaData = false;
            yield break;
        }
        else
        {
            retrieveMetaData = true;
        }

        JsonData json = JsonMapper.ToObject(www.text);
        JsonData data = json["Data"];

        imageWidth = Convert.ToInt32(data["image_width"].ToString());
        imageHeight = Convert.ToInt32(data["image_height"].ToString());
        tileWidth = Convert.ToInt32(data["tile_width"].ToString());
        tileHeight = Convert.ToInt32(data["tile_height"].ToString());

        print("image width : " + imageWidth + " image height : " + imageHeight);
        print("tile width : " + tileWidth + " tile height : " + tileHeight);

        JsonData projection = json["Projection"];
        pivotYaw = Convert.ToSingle(projection["pano_yaw_deg"].ToString());

        JsonData location = json["Location"];
        zoomLevels = Convert.ToInt32(location["zoomLevels"].ToString());

        string locationText = "";
        if (location.Keys.Contains("description"))
        {
            description = location["description"].ToString();
            locationText += description;
        }
        if (location.Keys.Contains("country"))
        {
            country = location["country"].ToString();
            locationText += ", " + country;
        }
        if (location.Keys.Contains("region"))
        {
            region = location["region"].ToString();
            locationText += ", " + region;
        }

        LoadingScreen.SetLocationText(locationText);
        // 현재 파노라마 위치에서 갈 수 있는 방향 및 파노라마 ID 정보를 파싱한다.
        JsonData links = json["Links"];
        int linkCount = links.Count;

        Manager.Instance.nextIDs = new string[linkCount];
        Manager.Instance.nextDegrees = new string[linkCount];

        for (int i = 0; i < linkCount; i++)
        {
            JsonData item = links[i];
            string linkID = item["panoId"].ToString();
            Manager.Instance.nextIDs[i] = linkID;
            string yawDeg = item["yawDeg"].ToString();
            Manager.Instance.nextDegrees[i] = yawDeg;

            print("Link ID : " + linkID + " yawDeg : " + yawDeg);
        }

        textureWidth = imageWidth / ((zoomLevels - zoom) * 2);
        textureHeight = imageHeight / ((zoomLevels - zoom) * 2);
    }

    // Step 3
    IEnumerator GetPanoramaImage(string pano_id, int width, int height)
    {
        string output = "tile";

        if (panoramaTexture != null)
        {
            DestroyImmediate(panoramaTexture);
            panoramaTexture = null;
            
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        panoramaTexture = new Texture2D(width, height);

        downloadedTilesCount = 0;

        rowTilesNum = height / tileHeight;
        if ((height % tileHeight) > 0)
            rowTilesNum += 1;

        colTilesNum = width / tileWidth;
        if ((width % tileHeight) > 0)
            colTilesNum += 1;

        totalTilesNum = rowTilesNum * colTilesNum;

        tiles = new Texture2D[rowTilesNum, colTilesNum];

        for (int y = 0; y < rowTilesNum; y++)
        {
            for (int x = 0; x < colTilesNum; x++)
            {
                yield return StartCoroutine(GoogleStreetViewTiled(output, panoramaID, zoom, x, y));
            }
        }

        if (downloadedTilesCount == totalTilesNum)
            Debug.Log("All tiles downloaded!");
        else
            Debug.LogWarning("Tiles downloaed loss");
    }

    // 전체 파노라마 타일중 x, y 좌표에 위치한 타일 하나를 받아오는 함수 
    // http://cbk0.google.com/cbk?output=tile&panoid=Q_7cCDOIMymvWZcLQoOTjQ&zoom=3&x=2&y=1
    IEnumerator GoogleStreetViewTiled(string output, string pano_id, int zum, int x, int y)
    {
        string url = cbkURL
            + "output=" + output
            + "&panoid=" + pano_id
            + "&zoom=" + zum
            + "&x=" + x
            + "&y=" + y;

        WWW www = new WWW(url);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
            Debug.LogError("Panorama Download Error : " + www.error);
        else
            Debug.Log("Panorama Download : " + downloadedTilesCount);

        tiles[y, x] = www.texture;
        downloadedTilesCount++;
        Manager.Instance.IncreaseProgress();
    }

    IEnumerator MergeTiles()
    {
        int penPosX;
        int penPosY;

        for (int i = 0; i < rowTilesNum; i++)
        {
            for (int j = 0; j < colTilesNum; j++)
            {
                if (tiles[i, j] == null)
                {
                    print("tile is null " + i + "," + j);
                    continue;
                }

                for (int y = 0; y < tiles[i, j].height; y++)
                {
                    for (int x = 0; x < tiles[i, j].width; x++)
                    {
                        penPosX = (tileWidth * j) + x;
                        penPosY = textureHeight - (tileHeight * (i + 1)) + y;
                        if (penPosX < textureWidth && penPosY > 0)
                            panoramaTexture.SetPixel(penPosX, penPosY, tiles[i, j].GetPixel(x, y));
                    }
                }

                DestroyImmediate(tiles[i, j]);
            }
        }

        panoramaTexture.Apply();
        Manager.Instance.IncreaseProgress();
        yield return null;
    }

    void DrawButtons()
    {
        // 만약 기존의 화살표 프리팹이 있다면 정리해준다.
        if (buttonModelList != null && buttonModelList.Length > 0)
        {
            for (int i = 0; i < buttonModelList.Length; i++)
            {
                Destroy(buttonModelList[i]);
            }
        }

        buttonModelList = new GameObject[Manager.Instance.nextDegrees.Length];
        for (int i = 0; i < Manager.Instance.nextDegrees.Length; i++)
        {
            buttonModelList[i] = Instantiate(buttonModel, transform.position, Quaternion.identity) as GameObject;
            buttonModelList[i].GetComponent<Button>().SetDegree(Convert.ToSingle(Manager.Instance.nextDegrees[i]) - pivotYaw);
            buttonModelList[i].GetComponent<Button>().SetPanoramaID(Manager.Instance.nextIDs[i]);
        }
    }

    private IEnumerator ConvertPanoramaToCubemap()
    {
        int texSize = GetCubemapTextureSize();

        yield return StartCoroutine(CreateCubemapTexture(texSize, FACE_FRONT, panoramaID + "_front.png"));
        Manager.Instance.IncreaseProgress();
        yield return StartCoroutine(CreateCubemapTexture(texSize, FACE_BACK, panoramaID + "_back.png"));
        Manager.Instance.IncreaseProgress();
        yield return StartCoroutine(CreateCubemapTexture(texSize, FACE_LEFT, panoramaID + "_left.png"));
        Manager.Instance.IncreaseProgress();
        yield return StartCoroutine(CreateCubemapTexture(texSize, FACE_RIGHT, panoramaID + "_right.png"));
        Manager.Instance.IncreaseProgress();
        yield return StartCoroutine(CreateCubemapTexture(texSize, FACE_UP, panoramaID + "_up.png"));
        Manager.Instance.IncreaseProgress();
        yield return StartCoroutine(CreateCubemapTexture(texSize, FACE_DOWN, panoramaID + "_down.png"));
        Manager.Instance.IncreaseProgress();
    }

    private void SetSkybox()
    {
        skyboxMaterial = SkyboxRenderer.CreateSkyboxMaterial(cubeTextureFront, cubeTextureBack, cubeTextureLeft, cubeTextureRight, cubeTextureUp, cubeTextureDown);
        RenderSettings.skybox = skyboxMaterial;
    }

    private int GetCubemapTextureSize()
    {
        int size = 512;
        if (m_textureSize.Length > m_textureSizeIndex)
        {
            size = m_textureSize[m_textureSizeIndex];
        }
        return size;
    }


    private IEnumerator CreateCubemapTexture(int texSize, int faceIndex, string fileName = null)
    {
        Texture2D tex = new Texture2D(texSize, texSize, TextureFormat.RGB24, false);

        // SHJO TODO : 만약 로컬에 해당하는 Panorama ID의 텍스쳐가 있다면 해당 리소스의 텍스쳐를 사용한다.
        print("Create Cubemap texture");
        Vector3[] vDirA = new Vector3[4];
        if (faceIndex == FACE_FRONT)
        {
            vDirA[0] = new Vector3(-1.0f, -1.0f, -1.0f);
            vDirA[1] = new Vector3(1.0f, -1.0f, -1.0f);
            vDirA[2] = new Vector3(-1.0f, 1.0f, -1.0f);
            vDirA[3] = new Vector3(1.0f, 1.0f, -1.0f);
        }
        if (faceIndex == FACE_BACK)
        {
            vDirA[0] = new Vector3(1.0f, -1.0f, 1.0f);
            vDirA[1] = new Vector3(-1.0f, -1.0f, 1.0f);
            vDirA[2] = new Vector3(1.0f, 1.0f, 1.0f);
            vDirA[3] = new Vector3(-1.0f, 1.0f, 1.0f);
        }
        if (faceIndex == FACE_LEFT)
        {
            vDirA[0] = new Vector3(1.0f, -1.0f, -1.0f);
            vDirA[1] = new Vector3(1.0f, -1.0f, 1.0f);
            vDirA[2] = new Vector3(1.0f, 1.0f, -1.0f);
            vDirA[3] = new Vector3(1.0f, 1.0f, 1.0f);
        }
        if (faceIndex == FACE_RIGHT)
        {
            vDirA[0] = new Vector3(-1.0f, -1.0f, 1.0f);
            vDirA[1] = new Vector3(-1.0f, -1.0f, -1.0f);
            vDirA[2] = new Vector3(-1.0f, 1.0f, 1.0f);
            vDirA[3] = new Vector3(-1.0f, 1.0f, -1.0f);
        }
        if (faceIndex == FACE_UP)
        {
            vDirA[0] = new Vector3(-1.0f, 1.0f, -1.0f);
            vDirA[1] = new Vector3(1.0f, 1.0f, -1.0f);
            vDirA[2] = new Vector3(-1.0f, 1.0f, 1.0f);
            vDirA[3] = new Vector3(1.0f, 1.0f, 1.0f);
        }
        if (faceIndex == FACE_DOWN)
        {
            vDirA[0] = new Vector3(-1.0f, -1.0f, 1.0f);
            vDirA[1] = new Vector3(1.0f, -1.0f, 1.0f);
            vDirA[2] = new Vector3(-1.0f, -1.0f, -1.0f);
            vDirA[3] = new Vector3(1.0f, -1.0f, -1.0f);
        }

        Vector3 rotDX1 = (vDirA[1] - vDirA[0]) / texSize;
        Vector3 rotDX2 = (vDirA[3] - vDirA[2]) / texSize;

        float dy = 1.0f / texSize;
        float fy = 0.0f;

        Color[] cols = new Color[texSize];
        for (int y = 0; y < texSize; y++)
        {
            Vector3 xv1 = vDirA[0];
            Vector3 xv2 = vDirA[2];
            for (int x = 0; x < texSize; x++)
            {
                Vector3 v = ((xv2 - xv1) * fy) + xv1;
                v.Normalize();
                cols[x] = CalcProjectionSpherical(v);
                xv1 += rotDX1;
                xv2 += rotDX2;
            }
            tex.SetPixels(0, y, texSize, 1, cols);
            fy += dy;
        }
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.Apply();

        if (totalTilesNum == downloadedTilesCount) // ** 다운로드 받은 타일수와 전체 타일수가 같을 경우만 큐브맵을 저장한다.
            SaveTexture(ref tex, fileName);

        switch (faceIndex)
        {
            case FACE_FRONT:
                cubeTextureFront = tex;
                break;
            case FACE_BACK:
                cubeTextureBack = tex;
                break;
            case FACE_LEFT:
                cubeTextureLeft = tex;
                break;
            case FACE_RIGHT:
                cubeTextureRight = tex;
                break;
            case FACE_UP:
                cubeTextureUp = tex;
                break;
            case FACE_DOWN:
                cubeTextureDown = tex;
                break;
        }

        print("Create Cubemap Texture" + fileName);
        yield return null;
    }


    private Color CalcProjectionSpherical(Vector3 vDir)
    {
        float theta = Mathf.Atan2(vDir.z, vDir.x);		// -π ~ +π.
        float phi = Mathf.Acos(vDir.y);				//  0 ~ +π

        theta += m_direction * Mathf.PI / 180.0f;
        while (theta < -Mathf.PI) theta += Mathf.PI + Mathf.PI;
        while (theta > Mathf.PI) theta -= Mathf.PI + Mathf.PI;

        float dx = theta / Mathf.PI;		// -1.0 ~ +1.0.
        float dy = phi / Mathf.PI;			//  0.0 ~ +1.0.

        dx = dx * 0.5f + 0.5f;
        int px = (int)(dx * panoramaTexture.width);
        if (px < 0) px = 0;
        if (px >= panoramaTexture.width) px = panoramaTexture.width - 1;
        int py = (int)(dy * panoramaTexture.height);
        if (py < 0) py = 0;
        if (py >= panoramaTexture.height) py = panoramaTexture.height - 1;

        Color col = panoramaTexture.GetPixel(px, panoramaTexture.height - py - 1);
        return col;
    }

    public void SaveTexture(ref Texture2D tex, string saveFileName)
    {
        string realSavePath = Utility.cacheFolderPath + "/" + saveFileName;
        File.WriteAllBytes(realSavePath, tex.EncodeToPNG());
    }

    public bool LoadTexture(string filePath, ref Texture2D tex)
    {
        if (File.Exists(filePath))
        {
            return tex.LoadImage(File.ReadAllBytes(filePath));
        }
        return false;
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
