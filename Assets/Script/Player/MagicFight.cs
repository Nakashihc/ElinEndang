using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicFight : MonoBehaviour
{
    public Movement playerMovement;
    [SerializeField] private GameObject ProjectilePrefab;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private float projectileSpeed;

    [SerializeField] private int projectileCount = 3;
    [SerializeField] private float delayBetweenSpawns = 1f;
    [SerializeField] private float delayToFollowTarget = 2f;

    public bool canMagic;

    private void Update()
    {
        if (canMagic)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                StartCoroutine(SpawnProjectiles());
            }
        }
    }

    private IEnumerator SpawnProjectiles()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length > 0)
        {
            for (int i = 0; i < projectileCount; i++)
            {
                Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

                GameObject randomEnemy = enemies[Random.Range(0, enemies.Length)];
                Transform targetTransform = randomEnemy.transform;

                Projectile projectile = Instantiate(ProjectilePrefab, randomSpawnPoint.position, Quaternion.identity).GetComponent<Projectile>();

                StartCoroutine(DelayFollowTarget(projectile, targetTransform));
                playerMovement.canmove = false;
                canMagic = false;

                yield return new WaitForSeconds(delayBetweenSpawns);
            }
        }
    }

    private IEnumerator DelayFollowTarget(Projectile projectile, Transform target)
    {
        yield return new WaitForSeconds(delayToFollowTarget);

        // Jika target tidak null
        if (target != null)
        {
            playerMovement.canmove = true;
            projectile.InitializeProjectile(target, projectileSpeed);
        }
        canMagic = true;
    }
}
