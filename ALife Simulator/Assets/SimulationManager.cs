using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour {

    public GameObject foodPrefab;
    public Transform simulationArea;
    public int foodAmount = 50;

	// Use this for initialization
	void Start () {
        SpawnFood(foodAmount);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SpawnFood(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            var food = Instantiate(foodPrefab);
            food.transform.position = simulationArea.position + new Vector3(Random.Range(-1f, 1f) * simulationArea.localScale.x/2, Random.Range(-1f, 1f) * simulationArea.localScale.y/2, 0);
        }
    }
}
