using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour {

    public bool isRagdoll = false;
	// Use this for initialization
	void Start () {
        if (isRagdoll == true)
        {
            Ragdolled();
        }
	}
	
	// Update is called once per frame
	void Update () {
	}

    void Ragdolled() {
        GetComponent<Rigidbody>().isKinematic = false;
        foreach(Rigidbody rb in GetComponentsInChildren<Rigidbody>()) {
            rb.isKinematic = false;
        }
        GetComponent<Animator>().enabled = false;
    }
}
