using UnityEngine;
 
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform cible;
    [SerializeField, Min(0.1f)] private float vitesseSuivi = 5f;
    [SerializeField] private float offsetY = 2f;
    [SerializeField] private Vector2 limiteMin = new(-8f, -2f);
    [SerializeField] private Vector2 limiteMax = new(8f, 6f);
 
    private void LateUpdate()
    {
        if (cible == null) return;
 
        Vector3 destination = new(
            Mathf.Clamp(cible.position.x, limiteMin.x, limiteMax.x),
            Mathf.Clamp(cible.position.y + offsetY, limiteMin.y, limiteMax.y),
            transform.position.z
        );
 
        transform.position = Vector3.Lerp(
            transform.position,
            destination,
            vitesseSuivi * Time.deltaTime
        );
    }
}
 