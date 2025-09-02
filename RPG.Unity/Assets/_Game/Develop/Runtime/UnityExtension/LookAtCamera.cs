using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.UnityExtension
{
    [HideMonoScript, DisallowMultipleComponent]
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField] private bool _invokeInUpdate;
        [SerializeField] private bool _X;
        [SerializeField] private bool _Y = true;
        [SerializeField] private bool _Z;
        private Transform _cameraTransform;

        private void OnEnable()
        {
            if(Camera.main == null) return;
            
            _cameraTransform = Camera.main.transform;
            Rotate();
        }

        private void Start() =>
            Rotate();

        private void Update()
        {
            if(!_invokeInUpdate) return;
            Rotate();
        }

        private void Rotate()
        {
            var vectorRotation = new Vector3()
            {
                x = _X ? _cameraTransform.rotation.eulerAngles.x : transform.rotation.eulerAngles.x,
                y = _Y ? _cameraTransform.rotation.eulerAngles.y : transform.rotation.eulerAngles.y,
                z = _Z ? _cameraTransform.rotation.eulerAngles.z : transform.rotation.eulerAngles.z
            };
            
            transform.rotation = Quaternion.Euler(vectorRotation);
        }
    }
}
