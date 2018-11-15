using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyControll : MonoBehaviour {

    public float energy = 0;

    [SerializeField]
    private float startEnergy = 10f;
    [SerializeField]
    private float energyCapacity = 50f;
    [SerializeField]
    private float energyLose = 2f;

    private void Awake()
    {
        energy = startEnergy;
    }

    public void Restart ()
    {
        energy = startEnergy;
    }
    public void gainEnergy (float gain)
    {
        energy += gain;

        if (energy > energyCapacity) energy = energyCapacity;
    }

    private void Update()
    {
        energy -= energyLose * Time.deltaTime;

        float resize = energy * 0.1f;
        //this.gameObject.transform.localScale = new Vector3 (resize, resize, 0);

    }

}
