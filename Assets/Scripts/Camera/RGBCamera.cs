using UnityEngine;
using UnityEngine.Rendering;

namespace FRJ.Sensor
{
  [RequireComponent(typeof(UnityEngine.Camera))]
  public class RGBCamera : MonoBehaviour, IRBGCamera
  {
    [SerializeField] private int _width  = 640;
    [SerializeField] private int _height = 480;
    [SerializeField] [Range(0,100)] int _quality = 50;
    [SerializeField] private float _scanRate = 30f;

    public uint width  { get => (uint)this._width; }
    public uint height { get => (uint)this._height; }
    public float scanRate { get => this._scanRate; }
    
    private UnityEngine.Camera _camera;
    private Texture2D _texture;
    private Rect _rect;
    private byte[] _data;

    public void Awake()
    {
      this._camera  = GetComponent<UnityEngine.Camera>();
      this._texture = new Texture2D(this._width, this._height, TextureFormat.RGB24, false);
      this._rect = new Rect(0, 0, this._width, this._height);
      this._texture.Apply();
      this._camera.targetTexture = new RenderTexture(this._width, this._height, 24);
      RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
    }

     private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext context, Camera camera)
     {
        if (this._texture != null && camera == this._camera) {
          this._texture.ReadPixels(this._rect, 0, 0);
          this._data = this._texture.EncodeToJPG(this._quality);
        }
     }

    public Vector2 GetResolution(){
      return new Vector2(width,height);
    }  

    public byte[] GetCompressedImage()
    {
      return _data;
    }

    public float GetScannRate(){
      return _scanRate;
    }
  }
}
