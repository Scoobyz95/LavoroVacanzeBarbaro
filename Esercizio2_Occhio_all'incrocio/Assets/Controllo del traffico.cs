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
    float limitedivelocita = 8f;
    float decelerazionesemaforo = 15f;
    float decelerazioneautodavanti = 30f;
    float accelerazione = 15f;
    float aggiuntacc;
    float aggiuntavel;

    float[] velocita;
    int[] prossimamossa;
    bool[] stacurvando;
    Transform[][] traiettorie;
    int[] indici;

    // bisogna sistemare la questione degli assi
    void Start()
    {
        macchine = GameObject.FindGameObjectsWithTag("Macchine");
        velocita = new float[macchine.Length];
        prossimamossa = new int[macchine.Length];
        stacurvando = new bool[macchine.Length];
        traiettorie = new Transform[macchine.Length][];
        indici = new int[macchine.Length];
        for (int i = 0; i < macchine.Length; i++)
        {
            velocita[i] = limitedivelocita;
            prossimamossa[i] = Random.Range(1,4);
            stacurvando[i] = false;
         
        }

        timersemafori = new Stopwatch();
        timersemafori.Start();

    }

    int cont = 1;
    float tempo = 10;

    Material sferadailluminare;


    bool rossooverde = true;
    Vector3 direzione;
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
            // lo rimetto a false così dopo controlla e se è ancora dentro lo rimette
            aggiuntacc = 0;
            aggiuntavel = 0;
            //OnTriggerStay(macchine[i].GetComponent<Collider>());
            Vector3 avanti = macchine[i].transform.forward;
            Vector3 asseruota = Vector3.zero;
            if (stacurvando[i])
            {
                Curva(i, velocita[i], traiettorie[i]);

            }
            else
            {

                if(macchine[i].transform.rotation[2] <= 90)
                {
                     direzione = Vector3.forward;
                     asseruota = Vector3.right;

                }
                else if (macchine[i].transform.rotation[2] <= 180)
                {

                    direzione = Vector3.right;
                    asseruota = Vector3.forward;
                }
                else if(macchine[i].transform.rotation[2] <= 270)
                {

                    direzione = -Vector3.forward;
                    asseruota = Vector3.right;
                }
                else
                {
                    direzione = -Vector3.right;
                    asseruota = Vector3.forward;
                }

                UnityEngine.Debug.DrawRay(macchine[i].transform.position, avanti * 14f, UnityEngine.Color.red);

                RaycastHit hit;
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
                else if (Physics.Raycast(macchine[i].transform.position, avanti, out hit, 14f, layerMaskpassi))
                {
                    if (timersemafori.Elapsed.TotalSeconds + 1.5 > tempo * cont)
                    {
                        //if (aggiuntacc != 0)
                        //{
                        //    Accelera(i, aggiuntacc, aggiuntavel);
                        //}
                        if (velocita[i] > 0) Rallenta(i, decelerazionesemaforo + 10f);
                    }
                    else
                    {
                        
                            switch (prossimamossa[i])
                            {
                                case 1:
                                    {
                                        //curva a destra
                                        //Controlla se ha il tempo per farla

                                        GameObject curva = hit.collider.gameObject;

                                        Transform Waypoints = curva.transform.GetChild(0);
                                        Transform[] traiettoriaconpadre = Waypoints.GetComponentsInChildren<Transform>();
                                        Transform[] traiettoria = new Transform[traiettoriaconpadre.Length - 1];

                                        for (int j = 0, k = 0; j < traiettoriaconpadre.Length; j++)
                                        {
                                        if (traiettoriaconpadre[j].tag != "Destra")
                                            {
                                                traiettoria[k] = traiettoriaconpadre[j];
                                                k++;
                                            }
                                        }



                                    if (timersemafori.Elapsed.TotalSeconds + 3 >= tempo * cont)
                                    {
                                        Accelera(i);
                                    }
                                    else
                                    {
                                        stacurvando[i] = true;
                                        traiettorie[i] = traiettoria;
                                        Curva(i, velocita[i], traiettoria);
                                    }


                                }
                                break;

                                case 2:
                                    {
                                    //curva a sinistra
                                    GameObject curva = hit.collider.gameObject;

                                    Transform Waypoints = curva.transform.GetChild(1);
                                    Transform[] traiettoriaconpadre = Waypoints.GetComponentsInChildren<Transform>();
                                    Transform[] traiettoria = new Transform[traiettoriaconpadre.Length - 1];

                                    for (int j = 0, k = 0; j < traiettoriaconpadre.Length; j++)
                                    {
                                        if (traiettoriaconpadre[j].tag != "Sinistra")
                                        {
                                            traiettoria[k] = traiettoriaconpadre[j];
                                            k++;
                                        }
                                    }

                                    if (timersemafori.Elapsed.TotalSeconds + 5 >= tempo * cont)
                                    {
                                        Accelera(i);
                                    }
                                    else
                                    {
                                        stacurvando[i] = true;
                                        traiettorie[i] = traiettoria;
                                        Curva(i, velocita[i], traiettoria);
                                    }

                                }
                                break;

                                case 3:
                                    {
                                        Accelera(i);
                                    }
                                    break;
                            
                            }



                    }
                }
                else
                {

                    Accelera(i);

                }

                MovimentoRuota(macchine[i].transform, velocita[i] * 1000, asseruota);
                macchine[i].transform.Translate(direzione * velocita[i] * Time.deltaTime);
            }
        }
    }

    void Rallenta(int i, float decelerazione)
    {
        velocita[i] -= decelerazione * Time.deltaTime;
        if (velocita[i] < 0.2f)
        {
            velocita[i] = 0;
            Rigidbody rb = macchine[i].GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
        }
    }

    void Accelera(int i, float accaggiunta = 0, float maxaggiunta = 0)
    {
        if (velocita[i] < limitedivelocita + maxaggiunta)
        {
            Rigidbody rb = macchine[i].GetComponent<Rigidbody>();

            if (velocita[i] == 0)
            {

                velocita[i] += (accelerazione + accaggiunta) * Time.deltaTime;
                rb.AddForce(direzione * velocita[i], ForceMode.VelocityChange);

            }
            else
            {
                velocita[i] += accelerazione * Time.deltaTime;

                if (velocita[i] > limitedivelocita + maxaggiunta)
                {
                    velocita[i] = limitedivelocita + maxaggiunta;
                }
            }

        }
    }
    
    float Distanzamin = 0.5f;
    void Curva(int i, float velocita, Transform[] traiettoria)
    {
        
        if (indici[i] < traiettoria.Length)
        {
            Transform posizionecorrente = traiettoria[indici[i]];
            Vector3 Direzione = (posizionecorrente.position - macchine[i].transform.position ).normalized;
            macchine[i].transform.position += Direzione * velocita * Time.deltaTime;
            Quaternion rotazione = Quaternion.LookRotation(Direzione);
            macchine[i].transform.rotation = Quaternion.Slerp(macchine[i].transform.rotation, rotazione, velocita * Time.deltaTime);

            if (Vector3.Distance(macchine[i].transform.position, traiettoria[indici[i]].position) < Distanzamin)
            {
                //Accelera(i, accelerazione);
                indici[i]++;
            }
        }
        else
        {
            stacurvando[i] = false;
            indici[i] = 0;
           
        }
        //Rigidbody rb = macchine[i].GetComponent<Rigidbody>();
        //float angoloDiCurvatura = 90 * Time.deltaTime;
        //Quaternion rotazioneCurva = Quaternion.Euler(0, velocita, 0);
        ////rb.MoveRotation(rb.rotation * rotazioneCurva);
        //macchine[i].transform.parent = pivot;
        //pivot.transform.Rotate(Vector3.up, 90);


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

            for (int i = 0; i < macchine.Length; i++)
            {
                prossimamossa[i] = Random.Range(1, 4);
            }
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

    void OnTriggerStay(Collider other)
    {
        if (other != null)
        {
            aggiuntacc = 5f;
            aggiuntavel = 7f;
        }

    }

    void MovimentoRuota(Transform macchina, float speed, Vector3 asseruota)
    {
        // Uso una lista perchè ci sono anche camion che hanno 6 ruote
        List<Transform> ruote = new List<Transform>();
        foreach (Transform ruota in macchina)
        {
            // Controlla se il figlio ha il tag specificato
            if (ruota.CompareTag("Ruota"))
            {
                ruote.Add(ruota);
            }         
        }

        for (int i = 0; i < ruote.Count; i++)
        {
            ruote[i].Rotate(asseruota, speed * Time.deltaTime);
        }
    }

}
