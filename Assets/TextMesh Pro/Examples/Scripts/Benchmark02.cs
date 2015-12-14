using UnityEngine;
using System.Collections;
using TMPro;


public class Benchmark02 : MonoBehaviour 
{

    public int SpawnType = 0;
    public int NumberOfNPC = 12;

    private TextMeshProFloatingText floatingText_Script;

    
    void Start()
    {
        
        for (int i = 0; i < NumberOfNPC; i++)
        {
            if (SpawnType == 0) 
            { 
                // TextMesh Pro Implementation     
                //go.transform.localScale = new Vector3(2, 2, 2);
                GameObject go = new GameObject(); //"NPC " + i);
                go.transform.position = new Vector3(Random.Range(-95f, 95f), 0.25f, Random.Range(-95f, 95f));
                
                //go.transform.position = new Vector3(0, 1.01f, 0);
                //go.renderer.castShadows = false;
                //go.renderer.receiveShadows = false;
                //go.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

                TextMeshPro textMeshPro = go.AddComponent<TextMeshPro>();
                //textMeshPro.FontAsset = Resources.Load("Fonts/ARIAL SDF 16", typeof(TextMeshProFont)) as TextMeshProFont;
                textMeshPro.anchor = AnchorPositions.Bottom;
                textMeshPro.fontSize = 96;
                
              
                textMeshPro.color = new Color32(255, 255, 0, 255);
                textMeshPro.text = "!";

               
                // Spawn Floating Text
                floatingText_Script = go.AddComponent<TextMeshProFloatingText>();
                floatingText_Script.SpawnType = 0;
            }
            else
            {
                // TextMesh Implementation
                GameObject go = new GameObject(); //"NPC " + i);
                go.transform.position = new Vector3(Random.Range(-95f, 95f), 0.25f, Random.Range(-95f, 95f));
               
                //go.transform.position = new Vector3(0, 1.01f, 0);
                
                TextMesh textMesh = go.AddComponent<TextMesh>();
                textMesh.font = Resources.Load("Fonts/ARIAL", typeof(Font)) as Font;        
                textMesh.renderer.sharedMaterial = textMesh.font.material;
                      
                textMesh.anchor = TextAnchor.LowerCenter;
                textMesh.fontSize = 96;

                textMesh.color = new Color32(255, 255, 0, 255);
                textMesh.text = "!";

                // Spawn Floating Text
                floatingText_Script = go.AddComponent<TextMeshProFloatingText>();
                floatingText_Script.SpawnType = 1;
            }
        }
    }
	
}
