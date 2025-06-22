using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Animator playerAnimator;
    public GameObject playerCam;
    public float range = 100f;
    public float damage = 25f;
    public ParticleSystem muzzleFlash;

    public GameObject hitParticles;

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        muzzleFlash.Play();
        RaycastHit hit;
        playerAnimator.SetTrigger("isShooting");

        if (Physics.Raycast(playerCam.transform.position, transform.forward, out hit, range))
        {
            EnemyManager enemyManager = hit.transform.GetComponent<EnemyManager>();

            if (enemyManager != null)
            {
                enemyManager.Hit(damage);
                GameObject instParticles = Instantiate(hitParticles, hit.point, Quaternion.LookRotation(hit.normal));
                instParticles.transform.parent = hit.transform;

                Destroy(instParticles, 2f);
            }
        }
    }
}