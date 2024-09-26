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
        //Spostamento WASD
        Vector3 direz = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) direz.z = +2f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) direz.z = -2f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) direz.x = -2f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) direz.x = +2f;

        direz.Normalize();
        Vector3 movimento = transform.forward * direz.z + transform.right * direz.x;


        //Spostamento rotella del mouse
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
        if (scrollValue > 0f && videoCamera.transform.position.y > 50)
        {
            movimento.y = -3f;
        }
        else if (scrollValue < 0f && videoCamera.transform.position.y < 250)
        {
            movimento.y = 3f;
        }
        else { movimento.y = 0f; }


        //Spostamento
        float veloc = 70f;
        float sposX = videoCamera.transform.position.x + (movimento.x * veloc * Time.deltaTime);
        float sposZ = videoCamera.transform.position.z + (movimento.z * veloc * Time.deltaTime);

        if (sposX < 550 && sposX > -220 && sposZ < 275 && sposZ > -375)
        {
            if ((sposX < 325 || sposZ > -150) && (sposX < 230 || sposZ < 220))
            {
                videoCamera.transform.position += movimento * veloc * Time.deltaTime;
            }
        }



        //Spostamento visuale della telecamera
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(2))
        {
            ruota = true;
            sposMouseIniz = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(2))
        {
            ruota = false;
        }

        if (ruota)
        {
            sposMouseFin = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            rotazione = new Vector3(-(sposMouseFin.y - sposMouseIniz.y) * 0.2f, (sposMouseFin.x - sposMouseIniz.x) * 0.3f, 0);

            if (videoCamera.transform.eulerAngles.x - rotazione.x < 80 && videoCamera.transform.eulerAngles.x - rotazione.x > 0)
            {
                videoCamera.transform.eulerAngles = transform.eulerAngles - rotazione;
            }
            
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
