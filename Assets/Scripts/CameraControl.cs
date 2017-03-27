using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    Vector3 camSmoothDampV;
    float minScale = 5;
    float maxScale = 20;

    void Update()
    {
        Vector3 moveVector = Vector3.zero;
        if (Input.mousePosition.x < Screen.width / 20 && Camera.main.transform.position.x > 0)
        {
            moveVector = new Vector3(1f, moveVector.y, moveVector.z);
        }
        else if (Input.mousePosition.x > Screen.width - Screen.width / 20 && Camera.main.transform.position.x < GameManager.map.mapGrid.GetLength(0) - GameManager.map.mapGrid.GetLength(0)/4)
        {
            moveVector = new Vector3(-1f, moveVector.y, moveVector.z);
        }
        else
        {
            moveVector = new Vector3(0, moveVector.y, moveVector.z);
        }

        if (Input.mousePosition.y < Screen.height / 20 && Camera.main.transform.position.z > 0)
        {
            moveVector = new Vector3(moveVector.x, moveVector.y, 1);
        }
        else if (Input.mousePosition.y > Screen.height - Screen.height / 20 && Camera.main.transform.position.z < GameManager.map.mapGrid.GetLength(1) - GameManager.map.mapGrid.GetLength(1)/4)
        {
            moveVector = new Vector3(moveVector.x, moveVector.y, -1);
        }
        else
        {
            moveVector = new Vector3(moveVector.x, moveVector.y, 0);
        }
        // zoom in
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && Camera.main.transform.position.y > minScale)
        {
            moveVector = new Vector3(moveVector.x, 5, moveVector.z);
        }

        // zoom out
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && Camera.main.transform.position.y < maxScale)
        {
            moveVector = new Vector3(moveVector.x, -5, moveVector.z);
        }

        Camera.main.transform.position = Vector3.SmoothDamp(
            Camera.main.transform.position, Camera.main.transform.position - moveVector, ref camSmoothDampV, 0.1f);

    }
}
