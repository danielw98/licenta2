using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Enemy : Pathfinding
{
    private int currentPathCellIndex = 0;
    private Vector3 pathTarget;
    private float nodeDistance;
    private Vector3 newDirection;
    public float speed = 0.5f;
    [SerializeField]
    private int creditsBonus;

    public void HandlePathChanged()
    {
        isNewPathNeeded = true;
    }

    private void OnEnable()
    {
        creditsBonus += Game.Instance.enemyBonusMultiplier;
    }

    private void Update()
    {
        if (!Game.Instance.isGameOver && Game.Instance.isMapGenerated && !Game.Instance.isPaused)
        {
            //startRotation = Quaternion.Euler(m_enemyLocation);
            // if a new path is needed, get it
            if (isNewPathNeeded)
            {
                currentPathCellIndex = 0;
                StartCoroutine(PathTimer());
            }
            else
            {
                if (Path.Count > 0)
                {
                    // set the target as the location of the tile next in path
                    pathTarget = new Vector3(Path[currentPathCellIndex].X + 0.5f, .15f, Path[currentPathCellIndex].Y + 0.5f);
                    Debug.Log(Path[currentPathCellIndex]);
                    //pathTarget = new Vector3(Path[currentPathCellIndex].X + 0.5f, .15f, Game.Instance.mapHeight - 1 - Path[currentPathCellIndex].Y - 0.5f);

                    // get the direction from our current position to our target
                    Vector3 _direction = pathTarget - transform.position;

                    // Move and rotate unit
                    transform.Translate(_direction.normalized * speed * Time.deltaTime, Space.World); // Move to the target

                    // Calculate distance enemy object is from next node
                    nodeDistance = Vector3.Distance(transform.position, pathTarget);

                    // Calculate rotation of enemy unit
                    Rotate(transform.position, pathTarget, nodeDistance - 0.2f);
                    //m_directionChange = 360f + m_directionAngle - Quaternion.Angle(startRotation, finalRotation);

                    // Phase in rotation gradually
                    //StartCoroutine(RotateOverTime(transform.position, m_pathTarget, m_speed));           

                    /* This caused a 10 degree angle bug */
                    //if (Map.GetCellFromWorldPosition(transform.position).m_isPathNode)
                    //{
                    //    //StartCoroutine(RotateIntoMoveDirection(Map.GetDirectionToCell(Map.GetCellFromWorldPosition(transform.position), Path[m_currentPathCellIndex])));
                    //    Rotate(transform.position, m_pathTarget);
                    //}



                    // if we are within .2 of target, assign next target (in order to avoid microstutter)
                    if (nodeDistance <= 0.2f)
                        currentPathCellIndex++;

                    if (currentPathCellIndex == Path.Count) // if we reached end of path
                    {
                        Game.Instance.enemies.Remove(gameObject);
                        // if the first node is base, or if we reached base, it's time to die
                        Destroy(gameObject); // base reached
                        Game.Instance.lives--;
                        return;
                    }
                }
            }
            if (GetComponentInChildren<Health>().currentHealth <= 0)
                Destroy();
        }
    }

    private void Rotate(Vector3 _current_position, Vector3 _next_tile_position, float _distance)
    {
        // Calculate new rotation
        Vector3 newDirection2 = _next_tile_position - _current_position;
        Quaternion rotation = Quaternion.LookRotation(newDirection2, Vector3.up);

        if (_distance > 0)
        {
            // Quanternion(from rotation, to rotation, increment percent 0f to 1f)
            transform.rotation = Quaternion.Slerp(Quaternion.Euler(0f, 0f, 0f), rotation, 1f) * Quaternion.Euler(0f, 0f, 0f);
            rotation = rotation * Quaternion.Euler(0f, 270f, 0f);
            // Copy so we can inspect it without screwing it. Seems just viewing it in 
            // inspector causes enemy units to get stuck at path nodes.
            newDirection = rotation.eulerAngles;
        }
    }

    public void TakeDamage(float damage)
    {
        GetComponentInChildren<Health>().ModifyHealth(damage);
    }

    public void Destroy()
    {
        // todo: play destroy animation
        Destroy(gameObject);
        Game.Instance.enemies.Remove(gameObject);
        // enemy has been destroyed, add the bonus to the global credits
        Game.Instance.credit += creditsBonus;
    }
}