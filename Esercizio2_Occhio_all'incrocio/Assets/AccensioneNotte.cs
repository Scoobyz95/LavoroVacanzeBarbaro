using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class AccensioneNotte : MonoBehaviour
{
    public GameObject solare;
    Light luce;


    // Start is called before the first frame update
    void Start()
    {
        luce = transform.Find("Luce").GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (solare.transform.eulerAngles.z > 165 || solare.transform.eulerAngles.z < 8)
        {
            luce.intensity = 2.5f;
        }
        else
        {
            luce.intensity = 0;
        }
    }
}
