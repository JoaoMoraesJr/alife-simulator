using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyControll : MonoBehaviour {

    [SerializeField]
    private float energy = 0f;

    [SerializeField]
    private float startEnergy = 10f;
    [SerializeField]
    private float energyCapacity = 50f;
    [SerializeField]
    private float energyLose = 2f;

    private void Start()
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

        if (energy <= 0) Die();
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }
}
