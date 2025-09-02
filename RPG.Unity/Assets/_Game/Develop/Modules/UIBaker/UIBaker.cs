using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.IdleRPG.Runtime.Core.UI.UIBaker
{
    public class UIBaker : MonoBehaviour
    {
        [SerializeField] private CanvasScaler _canvasScaler;
        [SerializeField] private GameObject _nameReference;
        [SerializeField] private string _outputFileName = "ProceduralImage";
        [SerializeField] private string _savePath = "";
        [SerializeField] private bool _resizeCanvas = true;
        [SerializeField] private Vector2Int _textureSize = new(512, 512);
        [SerializeField] private UnityEngine.Camera _cam;
    
        private void ResizeCanvas()
        {
            _canvasScaler.referenceResolution = _textureSize;
        }
    
        [Button]
        private void SaveProceduralImage()
        {
            if(_resizeCanvas)
                ResizeCanvas();
            var renderTexture = new RenderTexture(_textureSize.x, _textureSize.y, 24);
        
            _cam.backgroundColor = Color.clear;
            _cam.clearFlags = CameraClearFlags.SolidColor;
            _cam.orthographic = true;
            _cam.targetTexture = renderTexture;
        
            _cam.Render();
            RenderTexture.active = renderTexture;
            var texture = new Texture2D(_textureSize.x, _textureSize.y, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, _textureSize.x, _textureSize.y), 0, 0);
            texture.Apply();

            var imageName = _nameReference ? _nameReference.name : _outputFileName;
            SaveTextureAsPNG(texture, imageName, _savePath);
        
            RenderTexture.active = null;
            _cam.targetTexture = null;
            DestroyImmediate(renderTexture);
        
            Debug.Log("Procedural image saved as " + imageName + ".png at " + _savePath);
        }
    
        private void SaveTextureAsPNG(Texture2D texture, string fileName, string path)
        {
            byte[] bytes = texture.EncodeToPNG();
        
            if (string.IsNullOrEmpty(path))
                path = Application.dataPath;
            else
                path = Application.dataPath + "/" + path;
        
            if (!Directory.Exists(path)) 
                Directory.CreateDirectory(path);
        
            File.WriteAllBytes(Path.Combine(path, fileName + ".png"), bytes);
            Debug.Log("Saved procedural texture to " + Path.Combine(path, fileName + ".png"));
        }
    }
}
