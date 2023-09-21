using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public sealed class AsyncRBGCamera : MonoBehaviour, IRBGCamera
{
    [SerializeField] private int _width  = 640;
    [SerializeField] private int _height = 480;
    [SerializeField] [Range(0,100)] int _quality = 50;
    [SerializeField] private float _scanRate = 30f;
    
    private UnityEngine.Camera _camera;
    private Texture2D _texture;
    private Rect _rect;

    [HideInInspector] public byte[] _data;

    private RenderTexture _rt;
    [SerializeField] private ComputeShader _computeShader;
    private int _kernelIndex;
    private int _threadGroupsX, _threadGroupsY, _bufferSize;
    private ComputeBuffer _colorBuffer;

    void Start()
    {
        this._camera  = GetComponent<UnityEngine.Camera>();
        this._texture = new Texture2D(this._width, this._height, TextureFormat.RGB24, false);
        this._rect = new Rect(0, 0, this._width, this._height);
        this._texture.Apply();
        _rt = new RenderTexture(this._width, this._height, 24);        
        this._camera.targetTexture = _rt;

        _kernelIndex = _computeShader.FindKernel("ReadAllPixels");
        _bufferSize = _rt.width * _rt.height;
        _colorBuffer = new ComputeBuffer(_bufferSize, sizeof(float) * 4);

        _computeShader.SetTexture(_kernelIndex, "sourceTexture", _rt);
        _computeShader.SetInt("sourceTextureWidth", _rt.width);
        _computeShader.SetBuffer(_kernelIndex, "colorBuffer", _colorBuffer);


        _threadGroupsX = Mathf.CeilToInt(_width / 8f);
        _threadGroupsY = Mathf.CeilToInt(_height / 8f);

    }

    private void ReadPixelsFast(){
        _computeShader.Dispatch(_kernelIndex, _threadGroupsX, _threadGroupsY, 1);
        float[] colors = new float[_bufferSize * 4];
        _colorBuffer.GetData(colors);
        for(int x = 0; x < _width; x++){
            for(int y=0; y< _height; y++){
                var index = x+y*_width;
                _texture.SetPixel(x,y,new Color(colors[index],colors[index+1],colors[index+2],colors[index+3]));
            }
        }
        _data = _texture.EncodeToJPG(_quality);
    }

    void OnDestroy()
    {
        // Release the output buffer
        _colorBuffer.Release();
    }

    public byte[] GetCompressedImage(){
        ReadPixelsFast();
        return _data;
    }

    public float GetScannRate(){
        return _scanRate;
    }

    public Vector2 GetResolution(){
      return new Vector2(_width,_height);
    }  
}