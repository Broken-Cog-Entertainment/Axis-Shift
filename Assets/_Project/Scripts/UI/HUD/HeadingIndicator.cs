using AS.Utils.MathUtils;
using UnityEngine;

namespace AS.UI.HUD
{
    [RequireComponent(typeof(RectTransform))]
    public class HeadingIndicator : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float multiplier = 100f;

        [Header("Objective Marker")]
        [SerializeField] private Transform currentObjective;
        [SerializeField] private RectTransform objectiveMarker;
        
        private RectTransform _rect;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();

            if (currentObjective is null) objectiveMarker?.gameObject.SetActive(false);
        }

        private void Update()
        {
            var forward = target.forward.Flatten();
            var heading = Vector2.SignedAngle(Vector2.up, forward);

            _rect.anchoredPosition = new Vector2(heading * multiplier, 0);

            if (currentObjective is null || objectiveMarker is null) return;

            var dir = (currentObjective.position - target.position).Flatten();
            var angle = Vector2.SignedAngle(dir, forward);

            objectiveMarker.anchoredPosition = new Vector2((angle - heading) * multiplier, 0);
        }
    }
}
