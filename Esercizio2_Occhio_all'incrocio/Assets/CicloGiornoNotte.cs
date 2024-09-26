using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class CicloGiornoNotte : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    float DurataGiorno = 0f;
    void Update()
    {
        if (DurataGiorno <= 360)
        {
            DurataGiorno += 0.01f;
            transform.eulerAngles = new Vector3(0, 0, DurataGiorno);

            //if(DurataGiorno >= 200)
            //{
            //    Sole.orientation = Quaternion.Euler(0, 90, 0);
            //}
            //else
            //{
            //    Sole.orientation = Quaternion.Euler(0, 270, 0);
            //}
        }
        else
        {

            DurataGiorno = 0f;

        }

    }
}
