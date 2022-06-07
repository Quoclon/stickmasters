public class Platform
{
    //https://forum.unity.com/threads/how-to-detect-if-a-mobile-is-running-the-webgl-scene.440344/

    public static bool IsMobileBrowser()
    {
#if UNITY_EDITOR
        return false; // value to return in Play Mode (in the editor)
#elif UNITY_WEBGL
    return WebGLHandler.IsMobileBrowser(); // value based on the current browser
#else
    return false; // value for builds other than WebGL
#endif
    }
}