using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    Vector3 camSmoothDampV;
    float minScale = 2;
    float maxScale = 5;

    void Update()
    {
        Vector3 moveVector = Vector3.zero;
        if (Input.mousePosition.x < Screen.width / 20 && Camera.main.transform.position.x > GameManager.map.mapGrid.GetLength(0)/2)
        {
            moveVector = new Vector3(0.5f, moveVector.y, 0.5f);
        }
        else if (Input.mousePosition.x > Screen.width - Screen.width / 20 && Camera.main.transform.position.x < GameManager.map.mapGrid.GetLength(0)+1)
        {
            moveVector = new Vector3(-0.5f, moveVector.y, -0.5f);
        }
        else
        {
            moveVector = new Vector3(0, moveVector.y, 0);
        }

        if (Input.mousePosition.y < Screen.height / 20 && Camera.main.transform.position.y > 0)
        {
            moveVector = new Vector3(moveVector.x, 1, moveVector.z);
        }
        else if (Input.mousePosition.y > Screen.height - Screen.height / 20 && Camera.main.transform.position.y < GameManager.map.mapGrid.GetLength(1)+1)
        {
            moveVector = new Vector3(moveVector.x, -1, moveVector.z);
        }
        else
        {
            moveVector = new Vector3(moveVector.x, 0, moveVector.z);
        }
        // zoom in
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && Camera.main.orthographicSize > minScale)
        {
            Camera.main.orthographicSize -= 0.3f;
            //moveVector = new Vector3(moveVector.x, 5, moveVector.z);
        }
        // zoom out
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && Camera.main.orthographicSize < maxScale)
        {
            Camera.main.orthographicSize += 0.3f;
            //moveVector = new Vector3(moveVector.x, -5, moveVector.z);
        }

        Camera.main.transform.position = Vector3.SmoothDamp(
            Camera.main.transform.position, Camera.main.transform.position - moveVector, ref camSmoothDampV, 0.1f);

    }
}
