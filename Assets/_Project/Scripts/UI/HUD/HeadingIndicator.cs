using AS.Utils.MathUtils;
using UnityEngine;

namespace AS.UI.HUD
{
    [RequireComponent(typeof(RectTransform))]
    public class HeadingIndicator : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float multiplier = 100f;
        
        private RectTransform _rect;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        private void Update()
        {
            var forward = target.forward;
            var heading = Vector2.SignedAngle(Vector2.up, forward.Flatten());
            _rect.anchoredPosition = new Vector2(heading * multiplier, 0);
        }
    }
}
