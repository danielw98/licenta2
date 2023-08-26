using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Validator for tower ghosts while hovering over valid or invalid tiles
/// </summary>
public class TowerBuilderManager : MonoBehaviour
{
    public Tilemap tilemap;
    private LineRenderer lineRenderer;
    private bool isPlacementAllowed;
    private Vector3 defaultPosition;
    public GameObject towerPrefab;

    private const int InitialBufferSize = 10; // Initial size for the buffer
    private RaycastHit[] hitBuffer = new RaycastHit[InitialBufferSize]; // Preallocated buffer
    private bool isGrabbed;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        defaultPosition = transform.position;
    }

    public void HandleGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
    }

    public void HandleDrop(SelectExitEventArgs args)
    {
        isGrabbed = false;
        Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
        if (cellPosition.x >= 0 && cellPosition.x <= Map.Tiles.GetUpperBound(0) && cellPosition.y >= 0 && cellPosition.y <= Map.Tiles.GetUpperBound(1))
        {
            CellEntity hoveredCell = Map.Tiles[cellPosition.x, cellPosition.y];
            // when the tower ghost is dropped over a valid tile, return it to its default position and create the real tower on the valid tile 
            if (!hoveredCell.HasBuilding) // TODO: || enemyIsBlocked)
            {
                if (Game.Instance.credit >= towerPrefab.GetComponent<Tower>().cost)
                {
                    Map.Tiles[cellPosition.x, cellPosition.y].HasBuilding = true;
                    Game.Instance.towers.Add(Instantiate(towerPrefab, new Vector3(hoveredCell.X + 0.5f, 0, hoveredCell.Y + 0.5f), Quaternion.identity));
                    Game.Instance.towers[^1].GetComponent<AudioSource>().clip = Resources.Load("Audio/UsedSounds/tower_place2") as AudioClip;
                    Game.Instance.towers[^1].GetComponent<AudioSource>().Play();

                    Game.Instance.groundTileMap.SetTile(new Vector3Int(cellPosition.x, cellPosition.y, 0), Game.Instance.groundTiles[1]);
                    Game.Instance.credit -= (int)towerPrefab.GetComponent<Tower>().cost;
                    // make all enemie re-evaluate their pathing when the walkable tiles change upon a tower drop
                    foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                        enemy.GetComponent<Enemy>().isNewPathNeeded = true;
                }

            }
        }
        // return the ghost tower to the initial position
        transform.position = defaultPosition;
    }

    private void Update()
    {
        if (!Game.Instance.isGameOver)
        {
            // set the start point of the line to the object's position
            lineRenderer.SetPosition(0, transform.position);
            // set the end point of the line to the ground (y = 0)
            lineRenderer.SetPosition(1, new Vector3(transform.position.x, 0, transform.position.z));
            // after the object is grabbed, perform a raycast from the object downwards
            if (isGrabbed)
            {
                int hitCount = Physics.RaycastNonAlloc(transform.position, Vector3.down, hitBuffer, 100f);
                // If the buffer is filled, resize and raycast again
                while (hitCount >= hitBuffer.Length)
                {
                    hitBuffer = new RaycastHit[hitBuffer.Length * 2]; // Double the buffer size
                    hitCount = Physics.RaycastNonAlloc(transform.position, Vector3.down, hitBuffer, 100f);
                }
                RaycastHit? hit = hitBuffer.Take(hitCount).FirstOrDefault(h => h.collider.gameObject.CompareTag("BuildableArea"));
                if (hit?.collider != null)
                {
                    // Check if the "m" key is pressed
                    if (Keyboard.current.mKey.wasPressedThisFrame)
                        HandleDrop(null);
                    Vector3Int cellPosition = tilemap.WorldToCell(hit.Value.point);
                    isPlacementAllowed = !Map.Tiles[cellPosition.x, cellPosition.y].HasBuilding;
                }
                else
                    isPlacementAllowed = false;
            }
            else
                isPlacementAllowed = false;
            // change the color of the line based on whether placement is allowed
            if (isPlacementAllowed)
                lineRenderer.material.color = Color.green;
            else
                lineRenderer.material.color = Color.red;
        }
    }
}