using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class MovimentoCamera : MonoBehaviour
{
    public GameObject videoCamera;

    private Vector2 sposMouseIniz;
    private Vector2 sposMouseFin;
    private Vector3 rotazione;

    private bool ruota;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direz = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W)) direz.z = +1f;
        if (Input.GetKey(KeyCode.S)) direz.z = -1f;
        if (Input.GetKey(KeyCode.A)) direz.x = -1f;
        if (Input.GetKey(KeyCode.D)) direz.x = +1f;

        direz.Normalize();
        Vector3 movimento = transform.forward * direz.z + transform.right * direz.x;


        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
        //Spostamento rotella del mouse
        if (scrollValue > 0f && videoCamera.transform.position.y > 50)
        {
            movimento.y = -1f;
        }
        else if (scrollValue < 0f && videoCamera.transform.position.y < 250)
        {
            movimento.y = 1f;
        }
        else { movimento.y = 0f; }


        float veloc = 50f;
        videoCamera.transform.position += movimento * veloc * Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            ruota = true;
            sposMouseIniz = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        if (Input.GetMouseButtonUp(0))
        {
            ruota = false;
        }

        if (ruota)
        {
            sposMouseFin = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            rotazione = new Vector3(-(sposMouseFin.y - sposMouseIniz.y) * 0.2f, (sposMouseFin.x - sposMouseIniz.x) * 0.3f, 0);

            videoCamera.transform.eulerAngles = transform.eulerAngles - rotazione;
            sposMouseIniz = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }




        ////transform.position = Vector3 videoCamera;

        //Vector3 pos = videoCamera.transform.position;

        //if (Input.GetKey(KeyCode.UpArrow) && pos.x + 7 < 470)
        //{
        //    if(pos.x + 7 < 330 || pos.z > -150)
        //        videoCamera.transform.position = new Vector3(pos.x + 7, pos.y, pos.z);
        //}

        //if (Input.GetKey(KeyCode.DownArrow) && pos.x - 7 > -270)
        //{
        //    videoCamera.transform.position = new Vector3(pos.x - 7, pos.y, pos.z);
        //}

        //if (Input.GetKey(KeyCode.RightArrow) && pos.z - 7 > -360)
        //{
        //    if (pos.x < 330 || pos.z - 7 > -150)
        //        videoCamera.transform.position = new Vector3(pos.x, pos.y, pos.z - 7);
        //}

        //if (Input.GetKey(KeyCode.LeftArrow) && pos.z + 7 < 225)
        //{
        //    videoCamera.transform.position = new Vector3(pos.x, pos.y, pos.z + 7);
        //}




    }
}
