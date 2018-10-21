using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour {

    public List<AgentController> agentList = new List<AgentController>();
    public GameObject agentPrefab;
    public GameObject foodPrefab;
    public Transform simulationArea;
    public int foodAmount = 50;
    public int populationSize = 1;
    public bool simulationFinished = false;

	// Use this for initialization
	void Start () {
        SpawnAgents(populationSize);
        SpawnFood(foodAmount);
        GetComponent<EvolutionManager>().StartEvolution();
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(agentList.Count);
        foreach (AgentController a in agentList)
        {
            if (a.isDead == true)
            {
                agentList.Remove(a);
            }
        }
		if (agentList.Count == 0)
        {
            simulationFinished = true;
            Debug.Log("All Agents Died");
        }
	}

    public void SpawnAgents(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            var agent = Instantiate(agentPrefab);
            agent.transform.position = simulationArea.position + new Vector3(Random.Range(-1f, 1f) * simulationArea.localScale.x / 2, Random.Range(-1f, 1f) * simulationArea.localScale.y / 2, 0);
            agent.GetComponent<AgentMovement>().walkableArea = simulationArea;
            agentList.Add(agent.GetComponent<AgentController>());
        }
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
