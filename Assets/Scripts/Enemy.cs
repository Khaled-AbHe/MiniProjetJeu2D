using System;
using UnityEngine;
 
public class Enemy: MonoBehaviour
{
    [SerializeField] private Transform cible;
    [SerializeField, Min(0.1f)] private float vitesseSuivi = 5f;
    [SerializeField, Min(1f)] private float distance = 5f;
    [SerializeField] private Vector2 limiteMin = new(-8f, -4f);
    [SerializeField] private Vector2 limiteMax = new(8f, 4f);
    
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        if (cible == null) return;

        float currDistance = Math.Abs(Vector2.Distance(transform.position, cible.position));

        bool active = currDistance > distance;

        animator.SetBool(
            "EnReact",
            !active
        );  

        if (active) return;

        Vector3 destination = new(
            Mathf.Clamp(cible.position.x, limiteMin.x, limiteMax.x),
            Mathf.Clamp(cible.position.y, limiteMin.y, limiteMax.y),
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            destination,
            vitesseSuivi * Time.deltaTime
        );
    }
}