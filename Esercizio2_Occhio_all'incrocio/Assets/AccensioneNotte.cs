using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccensioneNotte : MonoBehaviour
{
    public Light coso;
    public Light luce;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(coso.transform.position.z < 170)
        {
            luce.intensity = 3;
        }
        else
        {
            luce.intensity = 0;
        }
    }
}
