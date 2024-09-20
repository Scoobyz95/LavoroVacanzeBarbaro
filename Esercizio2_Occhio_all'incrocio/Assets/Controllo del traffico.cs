using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Controllodeltraffico : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject[] macchine;
    public Macchina[] traffico;
    Stopwatch timersemafori;
    public List<GameObject> semafori = new List<GameObject>();

    // i tipo1 sono verde inizialmente
    public List<GameObject> semaforitipo1 = new List<GameObject>();

    // i tipo2 sono rossi inizialmente
    public List<GameObject> semaforitipo2 = new List<GameObject>();
    //diventerà un input
    float limitedivelocita = 8f;
    float decelerazionesemaforo = 20f;
    float decelerazioneautodavanti = 30f;
    float accelerazione = 2f;
    float aggiuntacc;
    float aggiuntavel;

    float[] velocita;
    bool[] stacurvando;
    Transform[][] traiettorie;
    bool[] fermato;
    int[] indici;
    Transform[][] precedenzedarisp;
    int[] scelta;
    bool[] incrocio;
    bool[] superatoprimaprecedenza;
    int[] decisionestop;
    System.Random random;


    //RAGIONARE SE USARE TRAFFICO OPUURE COLLEGARE LA CLASSE AL GAMEOBJECT (IO CONSIGLIO LA PRIMA)
    // bisogna sistemare la questione degli assi
    void Start()
    {
        timersemafori = new Stopwatch();
        timersemafori.Start();
        random = new System.Random();
        macchine = GameObject.FindGameObjectsWithTag("Macchine");
        velocita = new float[macchine.Length];
        stacurvando = new bool[macchine.Length];
        traiettorie = new Transform[macchine.Length][];
        indici = new int[macchine.Length];
        precedenzedarisp = new Transform[macchine.Length][];
        incrocio = new bool[macchine.Length];
        scelta = new int[macchine.Length];
        superatoprimaprecedenza = new bool[macchine.Length];
        fermato = new bool[macchine.Length];
        decisionestop = new int[macchine.Length];

        for (int i = 0; i < macchine.Length; i++)
        {
            macchine[i].AddComponent<Macchina()>();

            macchine[i].name = i.ToString();
            velocita[i] = limitedivelocita;
            stacurvando[i] = false;
            scelta[i] = random.Next(1,4);
            decisionestop[i] = random.Next(1, 4);
            incrocio[i] = false;
            superatoprimaprecedenza[i] = false;
            fermato[i] = false; 
        }
       
    }

    int cont = 1;
    float tempo = 20;

    Material sferadailluminare;

    bool rossooverde = true;
    void Update()
    {
        Gestionesemafori();

        if (timersemafori.Elapsed.TotalSeconds + 4 > tempo * cont)
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

        

        for (int i = 0; i < macchine.Length; i++)
        {

        }
        
    }
    


    
    

    void Gestionesemafori()
    {
        if (timersemafori.Elapsed.TotalSeconds > tempo * cont)
        {
            cont++;
            foreach (GameObject semaforo in semafori)
            {
                if(semaforo.transform.rotation.y == 0)
                    semaforo.transform.Rotate(Vector3.up, 90);
                else
                    semaforo.transform.Rotate(Vector3.up, -90);

            }
            rossooverde = !rossooverde;

            for (int i = 0; i < macchine.Length; i++)
            {
                scelta[i] = random.Next(1, 4);
                decisionestop[i] = random.Next(1, 4);
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

    

}
