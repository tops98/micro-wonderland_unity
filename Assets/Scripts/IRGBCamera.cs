using UnityEngine;

public interface IRBGCamera{
    public byte[] GetCompressedImage();
    public float GetScannRate(); 
    public Vector2 GetResolution();
}