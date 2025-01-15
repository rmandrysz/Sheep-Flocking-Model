using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotTaker : MonoBehaviour {
    public bool record = false;
    public int resWidth = 2550; 
    public int resHeight = 3300;
    private int screenshotCount = 0;
    private int skipFrame = 0;

    private string ScreenShotName() {
        return string.Format("{0}/Screenshots/screen_{1}.png", 
                             Application.dataPath, 
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-" + screenshotCount));
    }

    void LateUpdate() {
        if (!record)
        {
            return;
        }
        Camera camera = Camera.main;
        if (Input.GetKeyDown(KeyCode.K)) 
        {
            screenshotCount = 6;
        }
        if (screenshotCount > 0 && skipFrame != 0)
        {
            RenderTexture rt = new(resWidth, resHeight, 32);
            camera.targetTexture = rt;
            Texture2D screenShot = new(resWidth, resHeight, TextureFormat.RGBA32, false);
            camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName();
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            screenshotCount -= 1;
        }
        skipFrame = 1 - skipFrame;
    }
}