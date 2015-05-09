using UnityEngine;
using System.Collections;

public class SkyboxRenderer : MonoBehaviour
{
    public static Material CreateSkyboxMaterial(SkyboxManifest manifest)
    {
        Material result = new Material(Shader.Find("RenderFX/Skybox"));
        result.SetTexture("_FrontTex", manifest.textures[0]);
        result.SetTexture("_BackTex", manifest.textures[1]);
        result.SetTexture("_LeftTex", manifest.textures[2]);
        result.SetTexture("_RightTex", manifest.textures[3]);
        result.SetTexture("_UpTex", manifest.textures[4]);
        result.SetTexture("_DownTex", manifest.textures[5]);
        return result;
    }

	public static Material CreateSkyboxMaterial(Texture2D front, Texture2D back, Texture2D left, Texture2D right, Texture2D up, Texture2D down )
	{
		Material result = new Material(Shader.Find("RenderFX/Skybox"));
		result.SetTexture("_FrontTex", front);
		result.SetTexture("_BackTex", back);
		result.SetTexture("_LeftTex", left);
		result.SetTexture("_RightTex", right);
		result.SetTexture("_UpTex", up);
		result.SetTexture("_DownTex", down);
		return result;
	}

    public Texture2D[] textures;

    void OnEnable()
    {
        SkyboxManifest manifest = new SkyboxManifest(textures[0], textures[1], textures[2], textures[3], textures[4], textures[5]);
        Material material = CreateSkyboxMaterial(manifest);
        SetSkybox(material);
        enabled = false;
    }

    void SetSkybox(Material material)
    {
        GameObject camera = Camera.main.gameObject;
        Skybox skybox = camera.GetComponent<Skybox>();
        if (skybox == null)
            skybox = camera.AddComponent<Skybox>();
        skybox.material = material;
    }


    public struct SkyboxManifest
    {
        public Texture2D[] textures;

        public SkyboxManifest(Texture2D front, Texture2D back, Texture2D left, Texture2D right, Texture2D up, Texture2D down)
        {
            textures = new Texture2D[6]
            {
                 front,
                 back,
                 left,
                 right,
                 up,
                 down
             };
        }
    }

}

