using UnityEngine;

namespace AS.Misc
{
    [RequireComponent(typeof(BoxCollider))]
    public class AimTargetTest : MonoBehaviour
    {
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private Camera _camera;
        private MeshRenderer _renderer;

        private MaterialPropertyBlock _myColor;

        private void Start()
        {
            _camera = Camera.main;
            _renderer = GetComponent<MeshRenderer>();
            _myColor = new MaterialPropertyBlock();
            _myColor.SetColor(BaseColor, Color.green);
            _renderer.SetPropertyBlock(_myColor, 0);
        }

        private void Update()
        {
            if (Physics.Raycast(_camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f)), out var hitInfo,1000f) && hitInfo.transform == transform)
            {
                _myColor.SetColor(BaseColor, Color.green);
            }
            else
            {
                _myColor.SetColor(BaseColor, Color.red);
            }
            _renderer.SetPropertyBlock(_myColor, 0);
        }
    }
}
