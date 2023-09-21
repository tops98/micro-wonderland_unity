using UnityEngine;

public class PrintGpuName : MonoBehaviour
{
    void Start()
    {
        // Prints using the following format - "ATI Radeon X1600 OpenGL Engine" on MacBook Pro running Mac OS X 10.4.8
        print(SystemInfo.graphicsDeviceName);
    }
}