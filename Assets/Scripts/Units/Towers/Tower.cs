using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.XR.Interaction.Toolkit;

public class Tower : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> enemiesInRange;
    public float range = 10.0f;
    public float turnSpeed = 1f;
    public float fireRate = 0.5f;
    public float damage = 1.0f;
    public float sellCost;
    public GameObject turret;

    public float cost = 10f;
    [HideInInspector]
    public int upgradeLevel = 1;

    private void Start()
    {
        GetComponent<SphereCollider>().radius = range * 0.565f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
            enemiesInRange.Add(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
            enemiesInRange.Remove(other.gameObject);
    }

    public void OnActivated(ActivateEventArgs args)
    {
        Game.Instance.upgradePanelTransform.gameObject.SetActive(true);
        Game.Instance.towerLevelLabel.text = upgradeLevel.ToString();
        Game.Instance.towerDamageLabel.text = damage.ToString();
        Game.Instance.towerUpgradeCostLabel.text = cost.ToString();
        Game.Instance.towerNameLabel.text = name.Replace("Tower(Clone)", "").Replace("MachineGun", "Machine Gun");
        Game.Instance.selectedTower = gameObject;
        Game.Instance.towerUpgradeButton.enabled = Game.Instance.credit >= cost;
    }

    public static Transform FindChildWithNameRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;
            // recursively check the children of the current child.
            Transform found = FindChildWithNameRecursive(child, name);
            if (found != null)
                return found;
        }
        return null;
    }
}