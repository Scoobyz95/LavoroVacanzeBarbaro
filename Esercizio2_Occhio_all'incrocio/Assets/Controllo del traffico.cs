using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Controllodeltraffico : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject[] macchine;
    Macchina[] traffico;
    public GameObject videoCamera;
    bool macchinaCliccata = false;
    int macchinaSelezionata;

    public List<GameObject> semafori = new List<GameObject>();

    // i tipo1 sono verde inizialmente
    public List<GameObject> semaforitipo1 = new List<GameObject>();

    // i tipo2 sono rossi inizialmente
    public List<GameObject> semaforitipo2 = new List<GameObject>();
    //diventerà un input
    float limitedivelocita = 12f;
    float decelerazionesemaforo = 15f;
    float decelerazioneautodavanti = 50f;
    float accelerazione = 2f;
    System.Random random;

    public Image info;
    TMP_Text infoMacchina;

    //RAGIONARE SE USARE TRAFFICO OPUURE COLLEGARE LA CLASSE AL GAMEOBJECT (IO CONSIGLIO LA PRIMA)
    // bisogna sistemare la questione degli assi
    void Start()
    {
        macchine = GameObject.FindGameObjectsWithTag("Macchine");
        traffico = new Macchina[macchine.Length];       
        for (int i = 0; i < macchine.Length; i++)
        {
            macchine[i].name = i.ToString();
            traffico[i] = new Macchina(macchine[i], decelerazionesemaforo, decelerazioneautodavanti, accelerazione, limitedivelocita, traffico);
        }
        infoMacchina = info.transform.Find("infoMacchina").GetComponent<TMP_Text>();
    }

    int cont = 1;
    int tempo = 20;

    Material sferadailluminare;

    bool rossooverde = true;
    void Update()
    {
        Gestionesemafori();

        if (Time.time + 4 > tempo * cont)
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
            traffico[i].Azione(tempo, cont);
        }


        clickMacchina();
        if (macchinaCliccata)
        {
            Vector3 posMacchina = new Vector3(macchine[macchinaSelezionata].transform.position.x, 25, macchine[macchinaSelezionata].transform.position.z);
            videoCamera.transform.position = posMacchina;

            Vector3 rotazMacchina = new Vector3(89, macchine[macchinaSelezionata].transform.eulerAngles.y, 0);
            videoCamera.transform.eulerAngles = rotazMacchina;


            infoMacchina.text = "Metri percorsi: " + traffico[macchinaSelezionata].GetContaKm() + "\n\n" + "Velocità attuale: " + traffico[macchinaSelezionata].GetVelocità();
        }

        if (Input.GetKey(KeyCode.Escape) && macchinaCliccata)
        {
            escMacchina();
        }
    }


    public void clickMacchina()
    {
        
        if (Input.GetMouseButtonDown(0) && !macchinaCliccata)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.CompareTag("Macchine"))
                {
                    macchinaCliccata = true;
                    macchinaSelezionata = int.Parse(hit.collider.gameObject.name);

                    UnityEngine.Color coloreInvisibile = info.color;
                    coloreInvisibile.a = 0.35f;
                    info.color = coloreInvisibile;
                }
            }
        }
    }

    void escMacchina()
    {
        macchinaCliccata = false;
        Vector3 res = new Vector3(videoCamera.transform.position.x, 55, videoCamera.transform.position.z);
        videoCamera.transform.position = res;

        UnityEngine.Color coloreInvisibile = info.color;
        coloreInvisibile.a = 0;
        info.color = coloreInvisibile;

        infoMacchina.text = "";
    }


    void Gestionesemafori()
    {
        if (Time.time > tempo * cont)
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
                traffico[i].SetScelta();
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
