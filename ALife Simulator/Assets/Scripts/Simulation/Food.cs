﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour {

    [SerializeField]
    private float energy = 10;

    // Use this for initialization
    private void Awake()
    {
        this.gameObject.active = true;
    }

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public float Kill()
    {
        Destroy(this.gameObject);
        return energy;
    }
}
