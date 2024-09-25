using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CicloGiornoNotte : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    float DurataGiorno = 0f;
    float DurataNotte = 1200f;
    void Update()
    {
        if (DurataGiorno <= 360)
        {
            DurataGiorno += 0.01f;
            transform.eulerAngles = new Vector3(0, 0, DurataGiorno);
            DurataNotte = 1200f;
        }
        else
        {

            DurataGiorno = 0f;

        }

    }
}
