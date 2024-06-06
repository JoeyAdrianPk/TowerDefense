using UnityEngine;

public class MainCamera : MonoBehaviour
{
    // Public variables to adjust the position and size in the Unity Inspector
    //public float cameraX;
    //public float cameraY;
    //public float cameraSize; // For orthographic size or field of view

    private PathManager pathManager;

    public Camera myCamera;


    void Start()
    {
        pathManager = FindObjectOfType<PathManager>();
        int width = pathManager.gridWidth;
        int height = pathManager.gridHeight;
        //float cameraX = width / 2;
        float cameraY = height / 2;
        if(height % 2 != 0)
        {
            cameraY = (height /2) +1;
        }
        float cameraSize = width / 2;
        // Get the Camera component attached to this GameObject
        myCamera = GetComponent<Camera>();

        // Set camera position
        //transform.position = new Vector3(cameraX, cameraY, transform.position.z);

        // Set camera size
        if (myCamera.orthographic)
        {
            //Debug.Log(width);
            if(width <= 10)
            {
                myCamera.orthographicSize = cameraSize;
            }
            else if(width > 10 && width < 14)
            {
                myCamera.orthographicSize = 6;
            }
            else if(width == 14 || width == 15)
            {
                myCamera.orthographicSize = 7.5f;
            }
            else if(width == 16)
            {
                myCamera.orthographicSize = 8;
            }
            else if(width == 17)
            {
                myCamera.orthographicSize = 8.5f;
            }
            else if(width == 18)
            {
                myCamera.orthographicSize = 9;
            }
            else if(width == 25)
            {
                myCamera.orthographicSize = 13;
            }
            else if(width == 50)
            {
                myCamera.orthographicSize = 25;
            }
            
        }
        else
        {
            myCamera.fieldOfView = cameraSize;
        }

        // Calculate the camera's half-width in world units
        float cameraHalfWidth = myCamera.orthographicSize * myCamera.aspect;

        // Set the camera position so the left side is at 0 on the X axis
        float cameraX = cameraHalfWidth - 0.5f;
        transform.position = new Vector3(cameraX, cameraY, transform.position.z);
    }
}
