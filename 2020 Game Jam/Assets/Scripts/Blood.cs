using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.parent = null;
        transform.Rotate(0, Random.Range(0, 360), 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
