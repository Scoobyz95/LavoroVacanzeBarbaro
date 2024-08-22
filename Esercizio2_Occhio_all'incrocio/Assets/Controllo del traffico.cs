using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Controllodeltraffico : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject[] macchine;
    //diventerà un input
    float speed = 5f;
    float decelerazionesemaforo = 10f;
    float decelerazioneautodavanti = 5f;
    float[] velocita;

    // bisogna sistemare la questione degli assi
    void Start()
    {
        macchine = GameObject.FindGameObjectsWithTag("Macchine");
        //for (int i = 0; i < macchine.Length; i++)
        //{
        //    StartCoroutine(MuoviLaMacchina(macchine[i]));
        //}
        velocita = new float[macchine.Length];

        for (int i = 0; i < macchine.Length; i++)
        {
            velocita[i] = speed;
        }

    }

    // Update is called once per frame
    void Update()
    {
        int layerMasknonpassi = 1 << 7;
        int layerMaskveicolo = 1 << 8;

        
        for (int i = 0; i < macchine.Length; i++)
        {
            Vector3 avanti = -Vector3.forward;


            Debug.DrawRay(macchine[i].transform.position, avanti, Color.red, 1f);


            if (Physics.Raycast(macchine[i].transform.position, avanti, 10000f, layerMasknonpassi))
            {
                
                // Decelerazione graduale
                velocita[i] -= decelerazionesemaforo * Time.deltaTime;
                if ( velocita[i] < 3 ) velocita[i] = 0;
            }
            else if(Physics.Raycast(macchine[i].transform.position, avanti, 3f, layerMaskveicolo))
            {
                
                velocita[i] -= decelerazioneautodavanti * Time.deltaTime;
                if (velocita[i] < 0) velocita[i] = 0;
            }

            macchine[i].transform.Translate(Vector3.forward *velocita[i] * Time.deltaTime);
        }
    }

    //IEnumerator Decelera(float speedattuale, int i)
    //{
    //    while (speedattuale > 0)
    //    {
    //        speedattuale -= decelerazione * Time.deltaTime;
    //        macchine[i].transform.Translate(Vector3.forward * speedattuale * Time.deltaTime);
    //        yield return null;
    //    }
    //    speedattuale = 0;
    //}
    // IEnumerator MuoviLaMacchina(GameObject macchina)
    //{
    //    int layerMasknonpassi = 1 << 7;
    //    int layerMaskveicolo = 1 << 8;
    //    float speedattuale = speed;
      
        
    //    while (true)
    //    {
    //        // Controlla se c'è un ostacolo davanti
    //        Debug.DrawRay(macchina.transform.position, macchina.transform.forward, Color.red, 20f);

    //        if (Physics.Raycast(macchina.transform.position, macchina.transform.forward, 1.5f, layerMasknonpassi) ||
    //            Physics.Raycast(macchina.transform.position, macchina.transform.forward, 1.5f, layerMaskveicolo))
    //        {
    //            // Decelerazione graduale
    //            while (speedattuale > 0)
    //            {
    //                speedattuale -= decelerazione * Time.deltaTime;
    //                macchina.transform.Translate(Vector3.forward * speedattuale * Time.deltaTime);
    //                yield return null;  // Attende il prossimo frame
    //            }
    //            speedattuale = 0;  // Assicura che la velocità non vada sotto minSpeed
    //        }
    //        else
    //        {
    //            macchina.transform.Translate(Vector3.forward * speed * Time.deltaTime);
    //        }

    //        // Attende il prossimo frame
    //        yield return null;
    //    }
    //}
}
