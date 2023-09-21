//Attach this script to your Camera
//This draws a line in the Scene view going through a point 200 pixels from the lower-left corner of the screen
//To see this, enter Play Mode and switch to the Scene tab. Zoom into your Camera's position.
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text; 


[RequireComponent(typeof(IRBGCamera))]
[RequireComponent(typeof(Camera))]
public class AutoLabel : MonoBehaviour
{
    [Serializable]
    public struct Anotation
    {
        public int category;
        public Vector2 min;
        public Vector2 max;
        public int id;
    }

    [Serializable]
    public struct ImageMetadata
    {
        public int id;
        public int width;
        public int height;
        public string fileName;
    }

    [Tooltip("Extraspace around the bounding boxes")]
    [SerializeField]int _padding = 10;
    [SerializeField]string _datasetDiscription;
    [SerializeField]string _version;
    [SerializeField]bool _showBoundingBoxes = true;
    [SerializeField]string _exportPath = "Assets/Exports/Labeled_Images/";
    [SerializeField] Color _gizmoColor = new Color(0,1,0,0.5f);
    [Tooltip("List of object tags that are supposed to be labeld.\n Be aware: all objects with these tags are required to have a collider!")]
    [SerializeField]List<String> _categories;
    private List<KeyValuePair<Collider,int>> _trackedObjects;
    private Camera _cam;
    private IRBGCamera _rgbCamera;
    private float _timeElapsed = 0f;
    private int _count = 0;
    private List<Anotation> _annotations = new List<Anotation>();
    private List<ImageMetadata> _images = new List<ImageMetadata>();

    void Awake()
    {   
        _rgbCamera = gameObject.GetComponent<IRBGCamera>();
        _trackedObjects = GetTrackedObjColliders();
        _cam = gameObject.GetComponent<Camera>();

        if(_exportPath.LastIndexOf('/')<_exportPath.Length)
        {
            _exportPath += '/';
        }
    }

        void Update()
    {   
        this._timeElapsed += Time.deltaTime;

        if(this._timeElapsed > (1f/this._rgbCamera.GetScannRate()))
        {
            _annotations.AddRange(GetBoundingBoxes());
            _images.Add( ExportImage(_annotations, _rgbCamera.GetCompressedImage()));
            this._timeElapsed = 0;
            _count++;
        }
    }

    void OnDisable()
    {
        Exportannotations();
    }

    ImageMetadata ExportImage(List<Anotation> detections, byte[] image)
    {
        var imageMetadata = new ImageMetadata();
        imageMetadata.fileName = _count.ToString()+".jpg";
        imageMetadata.id = _count;
        imageMetadata.width = ((int)_rgbCamera.GetResolution().x);
        imageMetadata.height = ((int)_rgbCamera.GetResolution().y);

        System.IO.Directory.CreateDirectory(_exportPath);
        File.WriteAllBytesAsync(_exportPath+imageMetadata.fileName, image);
        
        return imageMetadata;
    }

    void Exportannotations()
    {
        string json =$@"
        {{
            ""info"": {CreateInfoString()},
            ""images"":[
                {CreateImageString()}
            ],
            ""annotations"": [
                {CreateannotationsString()}
            ],
            ""categories"": {CreateCategoriesString()}
        }}
        ";

        File.WriteAllText(_exportPath+"/_annotations.coco.json",json);
    }

    List<KeyValuePair<Collider,int>> GetTrackedObjColliders()
    {   
        List<GameObject> trackedObjs = new List<GameObject>();
        List<KeyValuePair<Collider,int>> colliders = new List<KeyValuePair<Collider,int>>();
        int i = 1;
        foreach (var cat in _categories)
        {
            var objs = GameObject.FindGameObjectsWithTag(cat);
            if(objs == null) continue;
            foreach (var obj in objs)
            {
                var coll = obj.GetComponent<Collider>();
                if(coll == null) throw new Exception("Object " + obj.name + " doeset have a collider! the collider is needed to calculate the bounds");
                colliders.Add(new KeyValuePair<Collider, int>(coll,i));
            }
            i++;
        }

        return colliders;
    }

    List<Anotation> GetBoundingBoxes()
    {
        var boundingBoxes = new List <Anotation>();
        foreach (var obj in _trackedObjects)
        {
            var coll = obj.Key;
            Anotation anotation = new Anotation();
            var p1 = _cam.WorldToScreenPoint(coll.bounds.min);
            var p2   = _cam.WorldToScreenPoint(coll.bounds.max);
            var res = _rgbCamera.GetResolution();



            // move origin to top left
            p1.y = res.y -1 - p1.y;
            p2.y = res.y -1 - p2.y;


            anotation.min = new Vector2(Math.Min(p1.x,p2.x)-_padding, Math.Min(p1.y,p2.y)-_padding);
            anotation.max = new Vector2(Math.Max(p1.x,p2.x)+_padding, Math.Max(p1.y,p2.y)+_padding);
            
            if(anotation.min.x <0 && anotation.max.x < 0){
                continue;
            }
            if(anotation.min.y <0 && anotation.max.y < 0){
                continue;
            }

            anotation.max = ClampAtZero(anotation.max);
            anotation.min = ClampAtZero(anotation.min);
            

            anotation.id = _count;
            anotation.category = obj.Value;
            boundingBoxes.Add(anotation);
        }
        return boundingBoxes;
    }

    private Vector3 ClampAtZero(Vector3 vec)
    {
        vec.x = (vec.x<0)? 0: vec.x;
        vec.y = (vec.y<0)? 0: vec.y;
        vec.z = (vec.z<0)? 0: vec.z;
        return vec;
    }

    void OnDrawGizmos()
    {
        if(!_showBoundingBoxes) return;
        
        if(_trackedObjects != null && _trackedObjects.Count >0)
        {
            DrawBoundingBoxes();
        }
    }

    void DrawBoundingBoxes()
    {
        foreach (var obj in _trackedObjects)
        {
            Bounds bounds = obj.Key.bounds;
            Gizmos.color = _gizmoColor;
            Gizmos.DrawCube(bounds.center,new Vector3( bounds.size.x, bounds.size.y, bounds.size.z));
        }
    }

    string CreateInfoString()
    {
        var timeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK";
        return$@"{{
            ""description"": ""{_datasetDiscription}"",
            ""version"": ""{_version}"",
            ""year"": {DateTime.Now.Year},
            ""date_created"": ""{DateTime.Now.ToString(timeFormat)}""
        }}";
    }

    string CreateImageString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var image in _images)
        {
            sb.Append($@"
                {{
                    ""id"": {image.id},
                    ""width"": {image.width},
                    ""height"": {image.height},
                    ""file_name"": ""{image.fileName}""
                }},");
        }

        sb.Remove(sb.Length-1,1);
        return sb.ToString();
    }

    string CreateannotationsString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var anotation in _annotations)
        {
            var width = Math.Abs( anotation.max.x - anotation.min.x);
            var height = Math.Abs( anotation.max.y - anotation.min.y);
            sb.Append($@"
                {{
                    ""id"": {anotation.id},
                    ""image_id"": {anotation.id},
                    ""category_id"": {anotation.category},
                    ""bbox"": [{anotation.min.x}, {anotation.min.y}, {width}, {height}],
                    ""area"": {width*height},
                    ""iscrowd"": 0
                }},");
        }
        sb.Remove(sb.Length-1,1);
        return sb.ToString();
    }

    string CreateCategoriesString()
    {
        StringBuilder sb = new StringBuilder(@"[{""id"": 0, ""name"": ""none""},");
        int i = 1;
        foreach (var cat in _categories)
        {
            sb.AppendFormat(@"{{""id"": {0}, ""name"": ""{1}""}},",i,cat);
            i++;
        }
        sb.Append("]");
        sb.Remove(sb.Length-2,1);
        
        return sb.ToString();
    }
}

