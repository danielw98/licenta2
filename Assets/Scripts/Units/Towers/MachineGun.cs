using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : MonoBehaviour
{
    private float lastShotTime;
    public GameObject turret;
    public AudioSource audioFire;

    public GameObject bulletPrefab;

   
    private void Start()
    {
        GetComponent<SphereCollider>().radius = GetComponent<Tower>().range * 0.565f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
            GetComponent<Tower>().enemiesInRange.Add(other.gameObject);
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
        //Vector3 targetPosition = new Vector3(collider.transform.position.x, collider.bounds.center.y , collider.transform.position.z);
        GameObject newBullet = Instantiate(bulletPrefab);
        audioFire.Play();
        newBullet.transform.position = startPosition;
        BulletBehavior bulletComp = newBullet.GetComponent<BulletBehavior>();
        bulletComp.target = collider.gameObject;
        bulletComp.startPosition = startPosition;
        bulletComp.targetPosition = collider.bounds.center;

        //Animator animator = monsterData.CurrentLevel.visualization.GetComponent<Animator>();
        //animator.SetTrigger("fireShot");
        //AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        //audioSource.PlayOneShot(audioSource.clip);
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
                        if (Time.time - lastShotTime > GetComponent<Tower>().fireRate)
                        {
                            Attack(targetCollider);
                            lastShotTime = Time.time;
                        }
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
                GetComponent<Tower>().enemiesInRange.Remove(target);
        }
    }
}
