using UnityEngine;
using System.Runtime.InteropServices;
public class WebGLHandler : MonoBehaviour
{
    //https://forum.unity.com/threads/how-to-detect-if-a-mobile-is-running-the-webgl-scene.440344/

    [DllImport("__Internal")]
    public static extern bool IsMobileBrowser();
}