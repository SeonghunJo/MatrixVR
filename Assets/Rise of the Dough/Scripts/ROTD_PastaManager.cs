using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manager that instantiates and recycles the pasta objects
/// </summary>
public class ROTD_PastaManager : MonoBehaviour 
{
    /// <summary>
    /// Internal list of pasta objects
    /// </summary>
    private List<ROTD_Pasta> _pastas = new List<ROTD_Pasta>();

    /// <summary>
    /// The number of live pizza objects
    /// </summary>
	private int _livePizzaCount;

    /// <summary>
    /// The current score value of killing a single pasta
    /// </summary>
    private int _currentPastaScore;

    /// <summary>
    /// Reference to the game manager
    /// </summary>
    public ROTD_GameManager gameManager;

    /// <summary>
    /// The maximum number of live pizzas active at any time
    /// </summary>
	public int maxLivePizzas;

    /// <summary>
    /// Prefab of the pizza animation
    /// </summary>
	public GameObject pizzaPrefab;

    /// <summary>
    /// The location of where the pasta should be spawned from the oven
    /// </summary>
    public Vector3 ovenSpawnPoint;

    /// <summary>
    /// A random offset from the oven spawn point so that overlapping doesn't get too bad
    /// </summary>
    public Vector2 ovenSpawnPointRandomOffset;

    /// <summary>
    /// An array of spawn points around the kitchen
    /// </summary>
    public Vector3 [] foregroundSpawnPoints;

    /// <summary>
    /// Offset along the Y axis for the score FX above each pasta kill
    /// </summary>
    public float scoreYOffset;

    /// <summary>
    /// The amount of score value to increase after each kill
    /// </summary>
    public int pastaScoreIncrement;

    /// <summary>
    /// The current score value of a pasta kill
    /// </summary>
    public int CurrentPastaScore
    {
        get
        {
            // increment the pasta score value, making sure it doesn't get to 10 million (too large for our score animation)
            _currentPastaScore = Mathf.Clamp(_currentPastaScore + pastaScoreIncrement, 0, 9999999);

            return _currentPastaScore;
        }
    }

    /// <summary>
    /// Called once at the start of the scene
    /// </summary>
    void Start()
    {
		GameObject go;
        ROTD_Pasta pasta;

		// instantiate the pizzas
		for (int i=0; i<maxLivePizzas; i++)
		{
            // create the pizza object
			go = (GameObject)Instantiate(pizzaPrefab, Vector3.zero, Quaternion.identity);
			go.transform.parent = this.transform;
			
            // grab the pasta class component and initialize
            pasta = go.GetComponent<ROTD_Pasta>();
			pasta.Initialize(gameManager);

			// add the pizza to the list
			_pastas.Add (pasta);
		}
    }

    /// <summary>
    /// Called every frame from the game manager
    /// </summary>
	public void FrameUpdate () 
    {	
        // update each pasta in the list if it is alive
		_livePizzaCount = 0;

        foreach (ROTD_Pasta pasta in _pastas)
        {
            if (pasta.State != ROTD_Pasta.STATE.Dead)
			{
            	pasta.FrameUpdate();

                switch (pasta.pastaType)
                {
                    case ROTD_Pasta.TYPE.Pizza:
                        _livePizzaCount++;
                        break;
                }
			}
        }
		
        // loop through the pastas to see if any new ones need to be spawned
        foreach (ROTD_Pasta pasta in _pastas)
		{
            if (pasta.State == ROTD_Pasta.STATE.Dead)
			{
                switch (pasta.pastaType)
                {
                    case ROTD_Pasta.TYPE.Pizza:

                        // check to see if we need to add more pizzas.
                        // if the pizza count is less than the game manager's difficulty level
                        // and also less than the maximum pizza amount allowed, then we add a new pizza.

                        if (_livePizzaCount < Mathf.Min(maxLivePizzas, gameManager.GetCurrentPastaTypeCount(ROTD_Pasta.TYPE.Pizza)))
                        {
                            // pasta is dead, so recycle
    				        pasta.ReSpawn();
                            _livePizzaCount++;
                        }
                        break;
                }
			}
		}
	}
	
    /// <summary>
    /// Deactivates a pasta so that it can be reused
    /// </summary>
    /// <param name="pasta">Pasta object to "kill"</param>
    public void KillPasta(ROTD_Pasta pasta)
	{
        // set the pasta state to Dead
        pasta.State = ROTD_Pasta.STATE.Dead;
	}
	
    /// <summary>
    /// Gets a random spawn position
    /// </summary>
    /// <param name="fromOven">Whether the pasta should come from the oven</param>
    /// <returns>Spawn Position</returns>
	public Vector3 GetRandomRespawnPosition(out bool fromOven)
	{
        fromOven = false;

        if (UnityEngine.Random.Range(0, 3) == 0 || gameManager.TotalPastaKills == 0)
        {
            // 1 in 3 chance the pasta will come from the oven (or it is the first pasta)

            fromOven = true;

            // open the oven door
            gameManager.room.OpenOven();

            // get the oven spawn point with some random offset to avoid overlap
            Vector3 spawnPoint = ovenSpawnPoint;
            spawnPoint.x += UnityEngine.Random.Range(0, ovenSpawnPointRandomOffset.x);
            spawnPoint.y -= UnityEngine.Random.Range(0, ovenSpawnPointRandomOffset.y);
            spawnPoint.z = spawnPoint.y;

            return spawnPoint;
        }
        else
        {
            // not from the oven, so we get one of the other spawn points in the kitchen

            return foregroundSpawnPoints[UnityEngine.Random.Range(0, foregroundSpawnPoints.Length)];
        }
	}

    /// <summary>
    /// Resets the pastas back to their starting states
    /// </summary>
    public void ResetToStart()
    {
        foreach (ROTD_Pasta pasta in _pastas)
        {
            pasta.State = ROTD_Pasta.STATE.Dead;
        }

        _currentPastaScore = 2;
    }
}
