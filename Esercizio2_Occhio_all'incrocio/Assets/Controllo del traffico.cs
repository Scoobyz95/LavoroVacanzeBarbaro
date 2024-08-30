using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class Controllodeltraffico : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject[] macchine;
    Stopwatch timersemafori;
    public List<GameObject> semafori = new List<GameObject>();

    // i tipo1 sono verde inizialmente
    public List<GameObject> semaforitipo1 = new List<GameObject>();

    // i tipo2 sono rossi inizialmente
    public List<GameObject> semaforitipo2 = new List<GameObject>();
    //diventerà un input
    float speed = 8f;
    float decelerazionesemaforo = 15f;
    float decelerazioneautodavanti = 30f;
    float accelerazione = 15f;

    float[] velocita;
    int[] prossimamossa;

    // bisogna sistemare la questione degli assi
    void Start()
    {
        macchine = GameObject.FindGameObjectsWithTag("Macchine");
        velocita = new float[macchine.Length];
        prossimamossa = new int[macchine.Length];

        //for (int i = 0; i < macchine.Length; i++)
        //{
        //    velocita[i] = speed;
        //    prossimamossa[i] = 1;
        //}

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

    Material sferadailluminare;


    bool rossooverde = true;
    void Update()
    {

        Gestionesemafori();

        if (timersemafori.Elapsed.TotalSeconds + 1.5 > tempo * cont)
        {
            //il semaforo passa da Verde a Giallo
            if (rossooverde)
            {
                foreach (GameObject semaforo in semaforitipo1)
                {
                    AccendioSpegni("Verde", semaforo, 0f);
                    AccendioSpegni("Giallo", semaforo, 2f);

                }
            }
            else
            {
                foreach (GameObject semaforo in semaforitipo2)
                {
                    AccendioSpegni("Verde", semaforo, 0f);
                    AccendioSpegni("Giallo", semaforo, 2f);

                }
            }
        }
        else if (rossooverde)
        {
            foreach (GameObject semaforo in semaforitipo1)
            {
                Lucisemafori("Verde", semaforo);
            }

            foreach (GameObject semaforo in semaforitipo2)
            {
                Lucisemafori("Rosso", semaforo);
            }
        }
        else
        {
            foreach (GameObject semaforo in semaforitipo1)
            {
                Lucisemafori("Rosso", semaforo);
            }

            foreach (GameObject semaforo in semaforitipo2)
            {
                Lucisemafori("Verde", semaforo);
            }
        }
         int layerMaskpassi = 1 << 6;
        int layerMasknonpassi = 1 << 7;
        int layerMaskveicolo = 1 << 8;


        for (int i = 0; i < macchine.Length; i++)
        {
            Vector3 avanti = macchine[i].transform.forward;

            //if (macchine[i].transform.rotation.eulerAngles[1] == 90)
            //{
            //    avanti = macchine[i].transform.right;
            //}
            Vector3 asseruota = macchine[i].transform.forward; ;

            UnityEngine.Debug.DrawRay(macchine[i].transform.position, avanti * 14f, UnityEngine.Color.red);
            

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
                if (timersemafori.Elapsed.TotalSeconds + 1 > tempo * cont)
                {
                    if (velocita[i] > 0)
                        Rallenta(i, decelerazionesemaforo + 25f);
                }
                else
                {
                    //switch (prossimamossa[i])
                    //{
                    //    case 1:
                    //        {
                    //            //curva a destra
                                
                    //            Curva(macchine[i].transform.position, macchine[i].transform.right, i, velocita[i]);
                    //        }
                    //        break;
                    //    case 2:
                    //        {
                    //            //curva a sinistra
                    //            Curva(macchine[i].transform.position, -macchine[i].transform.right, i, velocita[i]);
                    //        }
                    //        break;
                    //    case 3:
                    //        {
                    //            Accelera(i, accelerazione);
                    //        }
                    //        break;
                    //}

                    
                }               
            }
            else
            {

                Accelera(i, accelerazione);

            }

            MovimentoRuota(macchine[i].transform, velocita[i] * 1000,asseruota);
            macchine[i].transform.Translate(Vector3.forward * velocita[i] * Time.deltaTime);
        }
    }

    void Rallenta(int i, float decelerazione)
    {
        velocita[i] -= decelerazione * Time.deltaTime;
        if (velocita[i] < 0.1f)
        {
            velocita[i] = 0;
            Rigidbody rb = macchine[i].GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
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



    void Curva(Vector3 macchina,Vector3 direzione,int i, float velocita)
    {
        RaycastHit hit;
        int layerMask = 1 << 9;
        UnityEngine.Debug.DrawRay(macchina, direzione * 14f, UnityEngine.Color.green);
        if (Physics.Raycast(macchina, direzione, out hit, 14f, layerMask))
        {
            Transform Originale = macchine[i].transform.parent;
            Transform pivot = hit.collider.transform;


            //Rigidbody rb = macchine[i].GetComponent<Rigidbody>();
            //float angoloDiCurvatura = 90 * Time.deltaTime;
            //Quaternion rotazioneCurva = Quaternion.Euler(0, velocita, 0);
            //rb.MoveRotation(rb.rotation * rotazioneCurva);
            macchine[i].transform.parent = pivot;
            pivot.transform.Rotate(Vector3.up, 90);

            macchine[i].transform.parent = Originale;
        }
        else
        {
            Accelera(i, accelerazione);
        }
    }



    void Gestionesemafori()
    {
        if (timersemafori.Elapsed.TotalSeconds > tempo * cont)
        {
            cont++;
            foreach (GameObject semaforo in semafori)
            {
                semaforo.transform.Rotate(Vector3.up, 90);
            }
            rossooverde = !rossooverde;

            //for (int i = 0; i < macchine.Length; i++)
            //{
            //    prossimamossa[i] = Random.RandomRange(0, 3);
            //}
        }
    }

    void Lucisemafori(string colore, GameObject semaforo)
    {
        if(colore == "Rosso") 
        {
            AccendioSpegni("Giallo", semaforo, 0f);
        }
        else if(colore == "Verde")
        {
            AccendioSpegni("Rosso", semaforo, 0f);
        }

        AccendioSpegni(colore, semaforo, 2f);
    }

    
    void AccendioSpegni(string colore, GameObject semaforo, float intensita)
    {
        Transform childTransform = semaforo.transform.Find(colore);

        if (childTransform != null)
        {
            GameObject childGameObject = childTransform.gameObject;
            Renderer childRenderer = childGameObject.GetComponent<Renderer>();
            sferadailluminare = childRenderer.material;
            UnityEngine.Color emissionColor;
            if (colore == "Verde")
            {
                emissionColor = UnityEngine.Color.green;

            }else if(colore == "Rosso")
            {
                emissionColor = UnityEngine.Color.red;
            }
            else
            {
                emissionColor = UnityEngine.Color.yellow;
            }
            sferadailluminare.SetColor("_EmissionColor", emissionColor * intensita);
        }
    }

    void MovimentoRuota(Transform macchina, float speed, Vector3 asseruota)
    {
        Transform[] ruote = new Transform[4];
        int cont = 0;
        foreach (Transform ruota in macchina)
        {
            // Controlla se il figlio ha il tag specificato
            if (ruota.CompareTag("Ruota"))
            {
                ruote[cont] = ruota;
                cont++;
            }         
        }

        for (int i = 0; i < ruote.Length; i++)
        {
            ruote[i].Rotate(asseruota, speed * Time.deltaTime);
        }
    }

}
