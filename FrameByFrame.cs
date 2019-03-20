using UnityEngine;

public class FrameByFrame : MonoBehaviour
{
    [SerializeField] string _folder = "MovieOutput";
    [SerializeField] int _frameRate = 60;

    [SerializeField] int _sizeMultiplier = 1;

    string realFolder = "";

    void Start()
    {
        Time.captureFramerate = _frameRate;
        realFolder = _folder;

        int count = 1;
        while (System.IO.Directory.Exists(realFolder))
        {
            realFolder = _folder + count;
            count++;
        }

        System.IO.Directory.CreateDirectory(realFolder);
    }

    void Update()
    {
        var name = string.Format("{0}/shot {1:D04}.png", realFolder, Time.frameCount);

        ScreenCapture.CaptureScreenshot(name, _sizeMultiplier);
    }
}