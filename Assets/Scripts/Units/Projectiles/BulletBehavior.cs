using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public float speed = 2;
    public float damage;
    public GameObject target;
    public Vector3 startPosition;
    public Vector3 targetPosition;

    private float distance;
    private float startTime;

    void Start()
    {
        startTime = Time.time;
        distance = Vector2.Distance(startPosition, targetPosition);
    }

    void Update()
    {
        if (target == null)
            Destroy(gameObject); // destroy the bullet
        else
        {
            float timeInterval = Time.time - startTime;
            gameObject.transform.position = Vector3.Lerp(startPosition, targetPosition, timeInterval * speed / distance);
            if (gameObject.transform.position.Equals(targetPosition))
            {
                if (target != null)
                    target.GetComponent<Enemy>().TakeDamage(-Mathf.Max(damage, 0));
                Destroy(gameObject); // destroy the bullet
            }
        }
    }
}
