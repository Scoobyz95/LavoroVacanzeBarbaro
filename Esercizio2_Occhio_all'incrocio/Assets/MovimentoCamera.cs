using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentoCamera : MonoBehaviour
{
    public GameObject videoCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Vector3 videoCamera;

        Vector3 pos = videoCamera.transform.position;

        if (Input.GetKey(KeyCode.UpArrow) && pos.x + 7 < 470)
        {
            if(pos.x + 7 < 330 || pos.z > -150)
                videoCamera.transform.position = new Vector3(pos.x + 7, pos.y, pos.z);
        }

        if (Input.GetKey(KeyCode.DownArrow) && pos.x - 7 > -270)
        {
            videoCamera.transform.position = new Vector3(pos.x - 7, pos.y, pos.z);
        }

        if (Input.GetKey(KeyCode.RightArrow) && pos.z - 7 > -360)
        {
            if (pos.x < 330 || pos.z - 7 > -150)
                videoCamera.transform.position = new Vector3(pos.x, pos.y, pos.z - 7);
        }

        if (Input.GetKey(KeyCode.LeftArrow) && pos.z + 7 < 225)
        {
            videoCamera.transform.position = new Vector3(pos.x, pos.y, pos.z + 7);
        }


        float scrollValue = Input.GetAxis("Mouse ScrollWheel");

        // Verifica se la rotella è stata spostata
        if (scrollValue > 0f && pos.y - 7 > 50)
        {
            videoCamera.transform.position = new Vector3(pos.x, pos.y - 7, pos.z);
        }
        else if (scrollValue < 0f && pos.y + 7 < 250)
        {
            videoCamera.transform.position = new Vector3(pos.x, pos.y + 7, pos.z);
        }

    }
}
