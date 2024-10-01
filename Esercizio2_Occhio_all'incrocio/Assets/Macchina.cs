using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace Assets
{
    public class Macchina
    {
        GameObject macchina;

        float velocita;
        float accelerazione;
        float limitedivelocita;
        float decelerazionesemaforo;
        float decelerazioneautodavanti;

        int indici;
        int scelta;
        int decisionestop;
        double contachilometri;

        bool incrocio;
        bool superatoprimaprecedenza;
        bool fermato;
        bool stacurvando;
        bool gialloattivo;

        Vector3 direzione;
        Vector3 asseruota;
        
        
        Transform[] traiettorie;      
        Transform[] precedenzedarisp;
        Macchina[] traffico;
        
        

        System.Random random = new System.Random();

        public Macchina(GameObject macchina, float decelerazionesemaforo, float decelerazioneautodavanti, float accelerazione, float limitedivelocita, Macchina[] traffico)
        {
            this.macchina = macchina;
            velocita = 12;
            this.accelerazione = accelerazione;
            this.limitedivelocita = limitedivelocita;
            this.decelerazionesemaforo = decelerazionesemaforo;
            this.decelerazioneautodavanti = decelerazioneautodavanti;

            indici = 0;
            scelta = random.Next(1, 4);
            decisionestop = random.Next(1, 4);
            contachilometri = 0;

            incrocio = false;
            superatoprimaprecedenza = false;
            fermato = false;
            stacurvando = false;

            direzione = Vector3.forward;
            asseruota = Vector3.right;

            traiettorie = null;
            precedenzedarisp = null;
            this.traffico = traffico;
            
            
                  
        }

        public int GetScelta()
        {
            return scelta;
        }

        public float GetVelocità()
        {
            return (float)Math.Round(velocita, 2);
        }

        public double GetContaKm()
        {
            return Math.Round(contachilometri,2);
        }

        public void SetScelta()
        {
            scelta = random.Next(1, 4);
            decisionestop = random.Next(1, 4);
        }

        public void Azione(int tempo, int cont)
        {
            int layerMaskpassi = 1 << 6;
            int layerMasknonpassi = 1 << 7;
            int layerMaskveicolo = 1 << 8;
            int layermaskcurvare = 1 << 9;
            int layerMaskgiallo = 1 << 10;
            int layerMaskStop = 1 << 11;
            // lo rimetto a false così dopo controlla e se è ancora dentro lo rimette

            // se attraversa il cubo ed è giallo accelerera
            Vector3 avanti = macchina.transform.forward;

            if (stacurvando)
            {
                Curva( traiettorie, tempo, cont, precedenzedarisp);
            }
            else
            {
                Quaternion vector3;
                direzione = Vector3.forward;

                if (macchina.transform.eulerAngles.y == -90)
                {
                    vector3 = Quaternion.Euler(0, 270, 0);

                }else if(macchina.transform.eulerAngles.y == -180)
                {
                    vector3 = Quaternion.Euler(0, 0, 0);
                }
                else if(macchina.transform.eulerAngles.y == -270)
                {
                    vector3 = Quaternion.Euler(0, 90, 0);
                }

                if (macchina.transform.eulerAngles.y <= 145 && macchina.transform.eulerAngles.y >= 60)
                {
                    vector3 = Quaternion.Euler(0, 90, 0);
                }
                else if (macchina.transform.eulerAngles.y <= 230 && macchina.transform.eulerAngles.y >= 145)
                {
                    vector3 = Quaternion.Euler(0, 180, 0);
                }
                else if (macchina.transform.eulerAngles.y <= 330 && macchina.transform.eulerAngles.y >= 231)
                {
                    vector3 = Quaternion.Euler(0, 270, 0);
                }
                else
                {
                    vector3 = Quaternion.Euler(0, 0, 0);
                }


                //macchine[i].transform.Rotate(Vector3.up, differenza);
                macchina.transform.rotation = vector3;
                RaycastHit hit;
                //UnityEngine.Debug.DrawRay(macchine[i].transform.position, avanti * 14f, UnityEngine.Color.red);
                if (Time.time + 6 > tempo * cont)
                {
                    if (Physics.Raycast(macchina.transform.position, avanti, 4f, layerMaskgiallo))
                    {
                        Accelera(40, 40);
                        gialloattivo = true;
                    }
                    else if (Physics.Raycast(macchina.transform.position, avanti, 12f, layerMaskgiallo))
                    {
                        Rallenta(decelerazionesemaforo + 60);
                    }
                }              
                
                if (Physics.Raycast(macchina.transform.position, avanti, 10f, layerMasknonpassi))
                {
                    if (velocita > 0)
                        Rallenta(decelerazionesemaforo + 15);
                }
                else if (Physics.Raycast(macchina.transform.position, avanti, 8f, layerMaskveicolo))
                {
                    if (velocita > 0)
                        Rallenta(decelerazioneautodavanti);
                }
                else if (Physics.Raycast(macchina.transform.position, avanti, out hit, 2f, layerMaskpassi))
                {
                        switch (scelta)
                        {
                            case 1:
                                {
                                    //curva a destra
                                    GameObject curva = hit.collider.gameObject;
                                    Transform Waypoints = curva.transform.GetChild(0);
                                    Transform[] traiettoriaconpadre = Waypoints.GetComponentsInChildren<Transform>();
                                    Transform[] traiettoria = new Transform[traiettoriaconpadre.Length - 1];


                                    for (int j = 0; j < traiettoria.Length; j++)
                                    {
                                        traiettoria[j] = traiettoriaconpadre[j + 1];
                                    }
                                    if (Time.time + 4 >= tempo * cont)
                                    {
                                        Accelera(10,10);
                                    }
                                    else
                                    {
                                        stacurvando = true;
                                        incrocio= true;
                                        traiettorie = traiettoria;
                                        Curva( traiettoria,tempo,cont);
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
                                    Transform[] precedenze = new Transform[1];
                                    precedenze[0] = curva.transform.GetChild(2);
                                    precedenzedarisp = precedenze;


                                    for (int j = 0; j < traiettoria.Length; j++)
                                    {
                                        traiettoria[j] = traiettoriaconpadre[j + 1];
                                    }

                                    if (Time.time + 6 > tempo * cont)
                                    {
                                        Accelera(10,10);
                                    }
                                    else
                                    {
                                        incrocio = true;
                                        stacurvando = true;
                                        traiettorie = traiettoria;
                                        Curva(traiettoria, tempo, cont, precedenze);
                                    }

                                }
                                break;

                            case 3:
                                {
                                    Accelera();
                                }
                                break;
                        }
                    
                }
                else if (Physics.Raycast(macchina.transform.position, avanti, out hit, 8f, layermaskcurvare))
                {
                    GameObject curva = hit.collider.gameObject;
                    Transform[] traiettoriapadre = curva.transform.GetComponentsInChildren<Transform>();
                    Transform[] traiettoria = new Transform[traiettoriapadre.Length - 1];

                    for (int j = 0; j < traiettoria.Length; j++)
                    {
                        traiettoria[j] = traiettoriapadre[j + 1];
                    }

                    stacurvando = true;
                    traiettorie = traiettoria;
                    Curva(traiettoria, tempo, cont);
                }
                else if (Physics.Raycast(macchina.transform.position, avanti, out hit, 8f, layerMaskStop))
                {
                    if (!fermato)
                    {
                        if (velocita > 0)
                            Rallenta( decelerazionesemaforo + 30);
                        else
                        {
                            velocita = 0;
                            fermato = true;
                        }
                    }
                    else
                    {
                        switch (random.Next(1, 3))
                        {
                            case 1:
                                {
                                    //curva a destra
                                    GameObject curva = hit.collider.gameObject;
                                    Transform Waypoints = curva.transform.GetChild(0);
                                    Transform[] traiettoriaconpadre = Waypoints.GetComponentsInChildren<Transform>();
                                    Transform[] traiettoria = new Transform[traiettoriaconpadre.Length - 1];
                                    Transform[] precedenze = new Transform[1];
                                    precedenze[0] = curva.transform.GetChild(2);
                                    precedenzedarisp = precedenze;
                                    for (int j = 0; j < traiettoria.Length; j++)
                                    {
                                        traiettoria[j] = traiettoriaconpadre[j + 1];
                                    }

                                    stacurvando = true;
                                    traiettorie = traiettoria;
                                    Curva(traiettoria, tempo, cont, precedenze);
                                }
                                break;

                            case 2:
                                {
                                    //curva a sinistra
                                    GameObject curva = hit.collider.gameObject;
                                    Transform Waypoints = curva.transform.GetChild(1);
                                    Transform[] traiettoriaconpadre = Waypoints.GetComponentsInChildren<Transform>();
                                    Transform[] traiettoria = new Transform[traiettoriaconpadre.Length - 1];
                                    Transform[] precedenze = new Transform[2];
                                    precedenze[0] = curva.transform.GetChild(2);
                                    precedenze[1] = curva.transform.GetChild(3);
                                    precedenzedarisp = precedenze;

                                    for (int j = 0; j < traiettoria.Length; j++)
                                    {
                                        traiettoria[j] = traiettoriaconpadre[j + 1];
                                    }

                                    stacurvando = true;
                                    traiettorie = traiettoria;
                                    Curva(traiettoria, tempo, cont, precedenze);

                                }
                                break;
                        }
                    }
                }
                else if (Physics.Raycast(macchina.transform.position, avanti, out hit, 5f, 1 << 13))
                {
                    if ( decisionestop == 2)
                    {
                        GameObject curva = hit.collider.gameObject;
                        Transform Waypoints = curva.transform.GetChild(0);
                        Transform[] traiettoriaconpadre = Waypoints.GetComponentsInChildren<Transform>();
                        Transform[] traiettoria = new Transform[traiettoriaconpadre.Length - 1];
                        Transform[] precedenze = new Transform[1];
                        precedenze[0] = curva.transform.GetChild(1);
                        precedenzedarisp = precedenze;
                        for (int j = 0; j < traiettoria.Length; j++)
                        {
                            traiettoria[j] = traiettoriaconpadre[j + 1];
                        }

                        stacurvando = true;
                        traiettorie = traiettoria;
                        Curva(traiettoria, tempo, cont, precedenze);
                    }

                }
                else
                {
                    Accelera();
                }


                if (!stacurvando)
                {
                    if (velocita > 12)
                    {
                        if(!gialloattivo)
                        Rallenta( decelerazionesemaforo + 50);
                    }
                    MovimentoRuota(macchina.transform, velocita * 1000, asseruota);
                    macchina.transform.Translate(direzione * velocita * Time.deltaTime);
                }
                gialloattivo = false;

            }

            contachilometri += Time.deltaTime * velocita;

        }

        public void Rallenta(float decelerazione)
        {
            velocita -= decelerazione * Time.deltaTime;
            if (velocita < 0.2f)
            {
                velocita = 0;
                Rigidbody rb = macchina.GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;
            }
        }
         public void Accelera(float accaggiunta = 0, float maxaggiunta = 0)
        {
            if (velocita < limitedivelocita + maxaggiunta)
            {
                Rigidbody rb = macchina.GetComponent<Rigidbody>();

                if (velocita== 0)
                {

                    velocita += (accelerazione + accaggiunta) * Time.deltaTime;
                    rb.AddForce(direzione * velocita, ForceMode.VelocityChange);

                }
                else
                {
                    velocita += accelerazione + accaggiunta * Time.deltaTime;

                    if (velocita > limitedivelocita + maxaggiunta)
                    {
                        velocita = limitedivelocita + maxaggiunta;
                    }
                }

            }
        }

        float Distanzamin = 0.5f;
        public void Curva( Transform[] traiettoria, int tempo, int cont,Transform[] precedenze = null)
        {
            if (Time.time + 5 > tempo * cont && incrocio && velocita == 0)
            {
                stacurvando = false;
            }
            else
            {
                int layermask = 1 << 8;
                int layerBastaprecedenza = 1 << 14;
                bool dailaprecedenza = false;
                float distanza = 0;

                if (incrocio)
                {
                    distanza = 30f;
                }
                else
                {
                    distanza = 40f;
                }

                if (Physics.Raycast(macchina.transform.position, macchina.transform.forward, 7f, layerBastaprecedenza))
                {
                    precedenze = null;
                }


                if (precedenze != null)
                {
                    if (Physics.Raycast(macchina.transform.position, macchina.transform.forward, 7f, 1 << 12))
                    {
                        superatoprimaprecedenza = true;
                    }

                    for (int j = 0; j < precedenze.Length; j++)
                    {

                        if (superatoprimaprecedenza)
                        {
                            j = 1;
                        }

                        if (j < precedenze.Length)
                        {
                            UnityEngine.Debug.DrawRay(precedenze[j].position, precedenze[j].right * distanza, Color.green);


                            RaycastHit hit;
                            Collider myCollider = macchina.GetComponent<Collider>();
                            UnityEngine.Debug.DrawRay(macchina.transform.position, macchina.transform.forward * 7f, Color.red);


                            if (Physics.Raycast(precedenze[j].position, precedenze[j].right, out hit, distanza, layermask))
                            {
                                if (hit.collider != myCollider)
                                {
                                    int k = Int32.Parse(hit.collider.gameObject.name);

                                    if (incrocio)
                                    {
                                        if (traffico[k].GetScelta() != 2)
                                        {
                                            dailaprecedenza = true;
                                        }

                                    }
                                    else
                                    {
                                        dailaprecedenza = true;
                                    }
                                }

                            }
                        }
                    }
                }



                if (!Physics.Raycast(macchina.transform.position, macchina.transform.forward, 5f, 1 << 8))
                {
                    if (!dailaprecedenza)
                    {
                        if (indici < traiettoria.Length)
                        {
                            Accelera();
                            Transform posizionecorrente = traiettoria[indici];
                            Vector3 Direzione = (posizionecorrente.position - macchina.transform.position).normalized;
                            macchina.transform.position += Direzione * 10 * Time.deltaTime;
                            Quaternion rotazione = Quaternion.LookRotation(Direzione);
                            macchina.transform.rotation = Quaternion.Slerp(macchina.transform.rotation, rotazione, 8 * Time.deltaTime);

                            if (Vector3.Distance(macchina.transform.position, traiettoria[indici].position) < Distanzamin)
                            {
                                //Accelera(i, accelerazione);
                                indici++;
                            }


                        }
                        else
                        {
                            fermato = false;
                            precedenzedarisp = null;
                            stacurvando = false;
                            superatoprimaprecedenza = false;
                            incrocio = false;
                            indici = 0;
                        }
                    }
                    else
                    {
                        velocita = 0;

                    }
                }
                else
                {
                    velocita = 0;
                }

            }


        }

        public void MovimentoRuota(Transform macchina, float speed, Vector3 asseruota)
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
}
