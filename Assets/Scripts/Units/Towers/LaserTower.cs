using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LaserTower : MonoBehaviour
{
    // Start is called before the first frame update
    public LineRenderer lineRenderer; // Reference to the Line Renderer component
    public GameObject turret;
    public AudioSource audioFire;

    private float minWidth = 0.01f; // Minimum width of the laser beam
    private float maxWidth = 0.03f; // Maximum width of the laser beam
    private float scintillationSpeed = 10f; // Speed of the scintillation effect

    private void Start()
    {
        GetComponent<SphereCollider>().radius = GetComponent<Tower>().range * 0.565f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            GetComponent<Tower>().enemiesInRange.Add(other.gameObject);
            audioFire.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
            GetComponent<Tower>().enemiesInRange.Remove(other.gameObject);
    }

    public void Attack(Collider collider)
    {
        Vector3 startPosition = gameObject.transform.position;
        Transform barrelChild = Tower.FindChildWithNameRecursive(transform, "GunBarrelPoint");
        if (barrelChild != null)
            startPosition = barrelChild.transform.position;
        // Set the start and end positions of the laser beam
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, collider.bounds.center);
        collider.gameObject.GetComponent<Enemy>().TakeDamage(-GetComponent<Tower>().damage * Time.deltaTime);
        float scintillation = Mathf.PingPong(Time.time * scintillationSpeed, 1f);
        // Lerp between the minimum and maximum width based on the scintillation value
        float width = Mathf.Lerp(minWidth, maxWidth, scintillation);
        // Set the start and end width of the laser beam
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        // Enable the Line Renderer to show the laser beam
        lineRenderer.enabled = true;
    }

    void Update()
    {
        if (GetComponent<Tower>().enemiesInRange?.Count > 0)
        {
            var target = GetComponent<Tower>().enemiesInRange[0];
            if (target != null)
            {
                if (target.TryGetComponent<Collider>(out var targetCollider))
                {
                    if (!Game.Instance.isPaused & !Game.Instance.isGameOver)
                    {
                        GetComponent<Tower>().sellCost = (GetComponent<Tower>().cost / 100f) * 75f;
                        Attack(targetCollider);
                    }
                    // only rotate if enemies in range
                    if (!Game.Instance.isPaused && turret != null)
                    {
                        Quaternion rotation = Quaternion.LookRotation(targetCollider.bounds.center - turret.transform.position);
                        rotation.x = 0;
                        turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, rotation, GetComponent<Tower>().turnSpeed * Time.deltaTime);
                    }
                }
            }
            else
            {
                lineRenderer.enabled = false;
                GetComponent<Tower>().enemiesInRange.Remove(target);
            }
        }
        else
            lineRenderer.enabled = false;
    }
}
