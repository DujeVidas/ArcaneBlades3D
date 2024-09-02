using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform mainCamera;
    public float maxDistance = 100f;
    public GameObject decalPrefab;
    public int maxDecals = 10;
    public float bulletDamage = 1;
    public int magSize = 10;
    public ParticleSystem muzzleFlash;
    private int bulletsLeft;
    public AudioSource bulletSound;
    public AudioSource reloadSound;

    public Animator animator;
    public UI uiManager; // Reference to the UIManager
    private bool canShoot = true;
    private bool canReload = true;

    private List<GameObject> instantiatedDecals = new List<GameObject>();

    void Start()
    {
        bulletsLeft = magSize;
        uiManager.SetReloadingUIActive(false); // Hide UI elements initially
        uiManager.UpdateReloadProgress(0);
        uiManager.SetBulletsText(bulletsLeft, magSize);
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && bulletsLeft > 0 && canShoot)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && canReload)
        {
            canShoot = false;
            canReload = false;
            StartCoroutine(WaitForReload());
        }
    }

    IEnumerator WaitForReload()
    {
        uiManager.SetReloadingUIActive(true);
        animator.SetTrigger("Reload");
        reloadSound.Play();
        
        float reloadTime = 1.5f; // Duration of the reload animation
        float elapsed = 0f;

        while (elapsed < reloadTime)
        {
            elapsed += Time.deltaTime;
            uiManager.UpdateReloadProgress(elapsed / reloadTime);
            yield return null;
        }

        bulletsLeft = magSize;
        uiManager.SetBulletsText(bulletsLeft, magSize);
        uiManager.SetReloadingUIActive(false);
        canShoot = true;
        canReload = true;
    }

    void Shoot()
    {
        bulletSound.PlayOneShot(bulletSound.clip);
        muzzleFlash.Play();
        Ray shotRay = new Ray(mainCamera.position, mainCamera.forward);
        if (Physics.Raycast(shotRay, out RaycastHit rayHit, maxDistance))
        {
            InstantiateDecal(rayHit.point, rayHit.normal, rayHit.transform);
            if (rayHit.collider.gameObject.TryGetComponent(out ShootableObject shotObject))
            {
                shotObject.TakeShot(bulletDamage);
            }
            else if (rayHit.collider.gameObject.TryGetComponent(out BossHealth bossHealth))
            {
                bossHealth.TakeDamage(10);
            }
        }
        bulletsLeft--;
        uiManager.SetBulletsText(bulletsLeft, magSize);
    }

    void InstantiateDecal(Vector3 position, Vector3 normal, Transform hitTransform)
    {
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);
        GameObject decal = Instantiate(decalPrefab, position, rotation);
        decal.transform.position += normal * 0.01f;
        decal.transform.parent = hitTransform;
        instantiatedDecals.Add(decal);
        RemoveOldDecals();
    }

    void RemoveOldDecals()
    {
        if (instantiatedDecals.Count > maxDecals)
        {
            int excessDecals = instantiatedDecals.Count - maxDecals;
            for (int i = 0; i < excessDecals; i++)
            {
                GameObject decal = instantiatedDecals[0];
                instantiatedDecals.RemoveAt(0);
                Destroy(decal);
            }
        }
    }

    public int GetBulletsLeft()
    {
        return bulletsLeft;
    }
}
