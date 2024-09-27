using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class CicloGiornoNotte : MonoBehaviour
{
    Light sole;

    // Start is called before the first frame update
    void Start()
    {
        sole = transform.Find("Sole").GetComponent<Light>();
    }

    float DurataGiorno = 0f;

    void Update()
    {
        DurataGiorno %= 360;
        
        if (DurataGiorno > 0 && DurataGiorno < 180)
        {
            sole.intensity = 1;
            if (DurataGiorno > 20)
            {
                if (DurataGiorno < 160)
                {
                    sole.color = new Color(1f, 1f, 1f);
                }
                else
                {
                    sole.color = new Color(1f, 0.4f, 0,24f);
                }
            }
            else
            {
                sole.color = new Color(1f, 0.51f, 0,39f);
            }
        }
        else
        {
            sole.intensity = 0;
        }

        DurataGiorno += 1f;//0.01f
        transform.eulerAngles = new Vector3(0, 0, DurataGiorno);





        //if (DurataGiorno <= 360)
        //{
            

        //    //if(DurataGiorno >= 200)
        //    //{
        //    //    Sole.orientation = Quaternion.Euler(0, 90, 0);
        //    //}
        //    //else
        //    //{
        //    //    Sole.orientation = Quaternion.Euler(0, 270, 0);
        //    //}
        //}
        //else
        //{
        //    DurataGiorno = 0f;
        //}

    }
}
