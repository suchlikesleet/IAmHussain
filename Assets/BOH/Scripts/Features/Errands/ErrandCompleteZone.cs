using UnityEngine;

namespace BOH
{
    public class ErrandCompleteZone : MonoBehaviour
    {
        [Header("Errand")]
        [SerializeField] private string errandIdToComplete;
        [SerializeField] private Color gizmoColor = Color.green;
        
        private ErrandSystem errandSystem;

        private void Start()
        {
            errandSystem = GameServices.Errands ?? FindFirstObjectByType<ErrandSystem>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && errandSystem != null)
            {
                if (errandSystem.TryCompleteErrand(errandIdToComplete))
                {
                    Debug.Log($"Completed errand at zone: {errandIdToComplete}");
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            if (TryGetComponent<Collider2D>(out var col))
            {
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            }
        }
    }

    // ScriptRole: Trigger zone for completing errands
    // RelatedScripts: ErrandSystem
}
