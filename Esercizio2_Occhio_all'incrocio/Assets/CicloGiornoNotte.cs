using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class CicloGiornoNotte : MonoBehaviour
{
    Light sole;
    float g;
    float b;

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
            if (DurataGiorno > 15)
            {
                if (DurataGiorno < 160)
                {
                    if (g < 1) { g += 0.005f; }
                    if (b < 1) { b += 0.005f; }

                    sole.color = new Color(1f, g, b);
                }
                else
                {
                    if (g > 0.4) { g -= 0.005f; }
                    if (b > 0.25) { b -= 0.005f; }
                    sole.color = new Color(1f, g, b);
                }
            }
            else
            {
                g = 0.5f;
                b = 0.4f;
                sole.color = new Color(1f, g, b);
            }
        }
        else
        {
            sole.intensity = 0;
        }

        DurataGiorno += 0.01f;//0.01f
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
