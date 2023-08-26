using System.Collections;
using System.Collections.Generic;
using TMPro;
using Units.Enemies;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public int mapHeight;
    public int mapWidth;
    public GameObject prefabEmpTowerGhost;
    public GameObject prefabLaserTowerGhost;
    public GameObject prefabMachineGunGhost;
    public GameObject prefabRockerGhost;
    public GameObject prefabSuperGhost;
    public GameObject prefabTeslaGhost;

    public GameObject prefabEmpTower;
    public GameObject prefabLaserTower;
    public GameObject prefabMachineGun;
    public GameObject prefabRocker;
    public GameObject prefabSuper;
    public GameObject prefabTesla;


    public GameObject prefabHoverTank;
    public GameObject prefabHoverCopter;
    public GameObject prefabHoverBoss;
    public GameObject prefabSuperHoverTank;
    public GameObject prefabSuperHoverCopter;
    public GameObject prefabSuperHoverBoss;

    public GameObject prefabEntryGate;
    public GameObject prefabExitGate;
    public GameObject raycastReceiver;
    public Tilemap groundTileMap; // the "visual" ground
    public Tile[] groundTiles; // array containing the availble tilemap sprites
    public Transform playerPlaneTransform;
    public Transform playerPlaneTeleportTransform;

    public TextMeshProUGUI livesLabel;
    public TextMeshProUGUI creditsLabel;
    public TextMeshProUGUI currentWaveLabel;
    public TextMeshProUGUI gamePauseToggleLabel;

    public Transform upgradePanelTransform;
    public TextMeshProUGUI towerLevelLabel;
    public TextMeshProUGUI towerDamageLabel;
    public TextMeshProUGUI towerUpgradeCostLabel;
    public TextMeshProUGUI towerNameLabel;
    public Button towerUpgradeButton;
    [HideInInspector]
    public GameObject selectedTower;

    public Map map;

    //private Transform soldierTransform;
    [HideInInspector]
    public List<GameObject> enemies = new();
    [HideInInspector]
    public List<GameObject> towers = new();
    [HideInInspector]
    private bool isStarted;
    [HideInInspector]
    public bool isPaused = true;
    [HideInInspector]
    public bool isGameOver;
    [HideInInspector]
    public bool isMapGenerated;
    public int lives = 10;
    public int currentWave = 0;
    public int credit = 0;
    public int enemyHealthMultiplier = 0;
    public int enemyBonusMultiplier = 0;

    /// <summary>
    /// Singleton
    /// </summary>
    private static Game game;
    public static Game Instance
    {
        get
        {
            if (game == null)
            {
                game = FindObjectOfType<Game>();
                if (game == null)
                {
                    GameObject container = new("Game");
                    game = container.AddComponent<Game>();
                }
            }
            return game;
        }
    }

    public void SellTower()
    {
        if (selectedTower != null)
        {
            credit += (int)selectedTower.GetComponent<Tower>().sellCost;
            Vector3Int cellPosition = groundTileMap.WorldToCell(transform.position);
            CellEntity hoveredCell = Map.Tiles[cellPosition.x, cellPosition.y];
            hoveredCell.HasBuilding = false;
            groundTileMap.SetTile(new Vector3Int(cellPosition.x, cellPosition.y, 0), groundTiles[0]);
            towers.Remove(selectedTower);
            Destroy(selectedTower);
        }
    }

    public void UpgradeTower()
    {
        if (selectedTower != null)
        {
            Tower tower = selectedTower.GetComponent<Tower>();
            tower.upgradeLevel++;
            tower.damage += 1;
            tower.sellCost += (tower.cost / 100f) * 75f; ;
            credit -= (int)tower.cost;
            towerLevelLabel.text = tower.upgradeLevel.ToString();
            towerDamageLabel.text = tower.damage.ToString();
            towerUpgradeCostLabel.text = tower.cost.ToString();
        }
    }

    public void ToggleGamePauseGame()
    {
        if (!isStarted)
            AudioManager.Instance.PlayMusic(AudioManager.Instance.gameMusic);
        isStarted = true;
        isPaused = !isPaused;
        gamePauseToggleLabel.text = isPaused ? "Start" : "Pause";
    }

    private void Start()
    {
        upgradePanelTransform.gameObject.SetActive(false);
        AudioManager.Instance.PlayMusic(AudioManager.Instance.menuMusic);
        StopAllCoroutines();
        map = new Map();
        Map.Tiles = new CellEntity[mapWidth, mapHeight];
        // set the size of the ground
        groundTileMap.size = new Vector3Int(mapWidth, mapHeight, 0);
        // assign each map cell an entity, and each ground cell a tile
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                groundTileMap.SetTile(new Vector3Int(x, (Map.Tiles.GetLength(1) - 1) - y, 0), groundTiles[0]);
                Map.Tiles[x, y] = new CellEntity(x, y) { Id = (y * mapWidth) + x };
            }
        }
        Map.Tiles[0, mapHeight / 2].IsEntry = true;
        Map.Tiles[mapWidth - 1, mapHeight / 2].IsExit = true;
        isMapGenerated = true;
        // position and scale the plane where the player can walk, it needs to be a little bigger than the ground tilemap itself, and slightly offset it on each edge (walkable edges)
        playerPlaneTransform.transform.localScale = new Vector3(groundTileMap.size.x / 10.0f + 0.2f, 1, groundTileMap.size.y / 10.0f + 0.2f);
        playerPlaneTransform.position = new Vector3(mapWidth / 2 + (mapWidth % 2 == 0 ? 0 : 0.5f), -0.01f, mapHeight / 2 + (mapHeight % 2 == 0 ? 0 : 0.5f));
        playerPlaneTeleportTransform.transform.localScale = new Vector3(groundTileMap.size.x / 10.0f + 0.2f, 1, groundTileMap.size.y / 10.0f + 0.2f);
        playerPlaneTeleportTransform.position = new Vector3(mapWidth / 2 + (mapWidth % 2 == 0 ? 0 : 0.5f), 0.01f, mapHeight / 2 + (mapHeight % 2 == 0 ? 0 : 0.5f));

        // position the raycast receiver plane to be exactly the size and at the same position as the ground tile map, and slightly above the ground
        // (otherwise, the collider doesn't get hit)
        raycastReceiver.transform.localScale = new Vector3(groundTileMap.size.x / 10.0f, 1, groundTileMap.size.y / 10.0f);
        raycastReceiver.transform.position = new Vector3(mapWidth / 2 + (mapWidth % 2 == 0 ? 0 : 0.5f), 0.02f, mapHeight / 2 + (mapHeight % 2 == 0 ? 0 : 0.5f));
        // instantiate the gates for entry and exit
        Instantiate(prefabEntryGate, new Vector3(0.5f, 0.01f, mapHeight / 2 + 0.5f), Quaternion.identity);
        Instantiate(prefabExitGate, new Vector3(mapWidth - 0.5f, 0.0f, mapHeight / 2 + 0.5f), Quaternion.identity);
    }

    // Update is called once per frame
    private void Update()
    {
        //soldierTransform.position = new Vector3(soldierTransform.position.x - 1 * Time.deltaTime, soldierTransform.position.y,
        //    soldierTransform.position.z);
        livesLabel.text = lives.ToString();
        creditsLabel.text = credit.ToString();
        currentWaveLabel.text = currentWave.ToString();
        if (!isPaused)
        {
            if (enemies.Count == 0)
                StartCoroutine(SpawnEnemy());
        }
    }

    public IEnumerator SpawnEnemy()
    {
        currentWave++;
        enemyHealthMultiplier += 55;
        enemyBonusMultiplier += 1;
        GameObject enemy = null;        
        int enemyType = Random.Range(0, 6);
        if (enemyType == 0)
            enemy = prefabHoverTank;
        else if (enemyType == 1)
            enemy = prefabHoverCopter;
        else if (enemyType == 2)
            enemy = prefabHoverBoss;
        else if (enemyType == 3)
            enemy = prefabSuperHoverTank;
        else if (enemyType == 4)
            enemy = prefabSuperHoverCopter;
        else if (enemyType == 5)
            enemy = prefabSuperHoverBoss;
        for (int _count = 0; _count < 1; _count++)
        {
            enemies.Add(Instantiate(enemy, new Vector3(0.5f, 0.5f, mapHeight / 2 + 0.5f), Quaternion.identity));
            yield return new WaitForSeconds(2); // wait between enemy spawns
        }

    }
}
