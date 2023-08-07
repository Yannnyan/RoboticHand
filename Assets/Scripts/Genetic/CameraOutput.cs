using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOutput : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;
    private RenderTexture cameraOutput1;
    private RenderTexture cameraOutput2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public RenderTexture[] GetCamerasOutputs()
    {
        // Capture the camera output
        // Capture the secondary camera output
        cameraOutput1 = new RenderTexture(Screen.width, Screen.height, 24);
        camera1.targetTexture = cameraOutput1;
        camera1.Render();
        camera1.targetTexture = null;

        cameraOutput2 = new RenderTexture(Screen.width, Screen.height, 24);
        camera2.targetTexture = cameraOutput2;
        camera2.Render();
        camera2.targetTexture = null;
        return new RenderTexture[] { cameraOutput1, cameraOutput2};
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
