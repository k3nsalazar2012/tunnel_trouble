using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private TunnelSystem tunnelSystem;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetAxisRaw("Vertical") != 0)
            transform.Translate(transform.forward * Input.GetAxisRaw("Vertical") * 10f * Time.deltaTime);
    }

    void OnTriggerEnter(Collider collider)
    {
        transform.LookAt(new Vector3(collider.transform.position.x, transform.position.y, collider.transform.position.z));
    }
}
