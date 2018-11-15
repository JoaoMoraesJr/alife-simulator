using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationArea : MonoBehaviour {

    // Use this for initialization
    [SerializeField]
    public Transform simulationArea;
	void Start () {
		
	}

    public static SimulationArea Instance
    {
        get;
        private set;
    }

    public Transform getSimulationArea()
    {
        return simulationArea;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
