using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class Controllodeltraffico : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject[] macchine;
    Stopwatch timersemafori;
    public List<GameObject> semafori = new List<GameObject>();
    public List<GameObject> semafori_segnali = new List<GameObject>();
    //diventerà un input
    float speed = 8f;
    float decelerazionesemaforo = 15f;
    float decelerazioneautodavanti = 30f;
    float accelerazione = 15f;
    
    float[] velocita;
    

    // bisogna sistemare la questione degli assi
    void Start()
    {
        macchine = GameObject.FindGameObjectsWithTag("Macchine");
        velocita = new float[macchine.Length];

        for (int i = 0; i < macchine.Length; i++)
        {
            velocita[i] = speed;
        }

        timersemafori = new Stopwatch();
        timersemafori.Start();

    }

    // Update is called once per frame
    // 0 avanti = forward

    // 180 avanti = -forward
    //Questo è il giallo
    bool giallo = false;
    int cont = 1;
    float tempo = 10;


    public float emissionIntensity = 5.0f; // Intensità dell'emissione
    Material sferadailluminare;

    bool rossooverde = true;
    void Update()
    {
        int layerMaskpassi = 1 << 6;
        int layerMasknonpassi = 1 << 7;
        int layerMaskveicolo = 1 << 8;

        
        for (int i = 0; i < macchine.Length; i++)
        {
            Vector3 avanti = macchine[i].transform.forward ;


            //Debug.DrawRay(macchine[i].transform.position, avanti * 14f, Color.red);


            if (Physics.Raycast(macchine[i].transform.position, avanti, 14f, layerMasknonpassi))
            {
              
                
                    if (velocita[i] > 0)
                        Rallenta(i, decelerazionesemaforo);
                
            }
            else if (Physics.Raycast(macchine[i].transform.position, avanti, 8f, layerMaskveicolo))
            {
                if (velocita[i] > 0)
                Rallenta(i, decelerazioneautodavanti);
                
            }
            else if (Physics.Raycast(macchine[i].transform.position, avanti, 14f, layerMaskpassi))
            {
                if (timersemafori.Elapsed.TotalSeconds + 3 > tempo * cont)
                {
                    if (velocita[i] > 0)
                        Rallenta(i, decelerazionesemaforo + 25f);
                }
                Accelera(i, accelerazione);


            }
            else
            {

                Accelera(i, accelerazione);

            }

            macchine[i].transform.Translate(Vector3.forward * velocita[i] * Time.deltaTime);
        }

        if (rossooverde)
        {
            // da continuare
            //Verde
            //sferadailluminare = semafori_segnali[0]
        }


        if (timersemafori.Elapsed.TotalSeconds > tempo * cont)
        {
            cont++;
            foreach (GameObject semaforo in semafori)
            {
                semaforo.transform.Rotate(Vector3.up, 90);

                
            }

            
           
        }
        //else if (timersemafori.Elapsed.TotalSeconds > tempo * cont + 2)
        //{
        //    cont++;
        //    foreach (GameObject semaforo in semafori)
        //    {
        //        semaforo.transform.Rotate(Vector3.up, -45);
        //        giallo = false;
        //    }

        //}
    }

    void Rallenta(int i, float decelerazione)
    {
        velocita[i] -= decelerazione * Time.deltaTime;
        if (velocita[i] < 0.1f)
        {
            velocita[i] = 0;
            Rigidbody rb = macchine[i].GetComponent<Rigidbody>();
            rb.velocity= Vector3.zero;
        }
    }

    void Accelera(int i, float accelerazione)
    {
        if (velocita[i] < 8)
        {
            Rigidbody rb = macchine[i].GetComponent<Rigidbody>();

            if (velocita[i] == 0)
            {
                
                velocita[i] += accelerazione * Time.deltaTime;
                rb.AddForce(macchine[i].transform.forward * velocita[i], ForceMode.VelocityChange);
                
            }
            else
            {
                velocita[i] += accelerazione * Time.deltaTime;

                if (velocita[i] > 8)
                {
                    velocita[i] = 8;
                }
            }

        }
    }


}
