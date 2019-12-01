using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveBuilding : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 10f;
    public Rigidbody rg;

    void Update()
    {
        rg.velocity = transform.right * speed;
    }
}
