using UnityEngine;
using UnityEngine.UI;

public abstract class WebCamera : MonoBehaviour
{
    public RawImage display;

    private WebCamTexture webcamTexture;
    private Texture2D outputTexture;

    protected virtual void Start()
    {
        webcamTexture = new WebCamTexture();
        webcamTexture.Play();
    }

    protected virtual void Update()
    {
        if (webcamTexture == null || !webcamTexture.didUpdateThisFrame) return;

        if (outputTexture == null || outputTexture.width != webcamTexture.width || outputTexture.height != webcamTexture.height)
        {
            outputTexture = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGBA32, false);
        }

        if (ProcessTexture(webcamTexture, ref outputTexture))
        {
            display.texture = outputTexture;
        }
    }

    protected abstract bool ProcessTexture(WebCamTexture inputTexture, ref Texture2D outputTexture);
}
