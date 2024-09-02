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
    public UI uiManager;
    public PointerController pointerController; // Reference to the PointerController

    private bool canShoot = true;
    private bool canReload = true;

    private List<GameObject> instantiatedDecals = new List<GameObject>();

    private float reloadTime = 1.5f;

    private bool reloadInProgress = false; // To track if reload is happening
    private bool reloadKeyPressed = false; // To track if 'R' key was pressed


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

        if (Input.GetKeyDown(KeyCode.R) && canReload && !reloadInProgress)
        {
            StartReload();
        }
        else if (Input.GetKeyDown(KeyCode.R) && reloadInProgress)
        {
            reloadKeyPressed = true; // Mark reload key as pressed for modification during reload
        }
    }

    void StartReload()
    {
        reloadInProgress = true;
        canShoot = false;
        canReload = false;
        StartCoroutine(WaitForReload());
    }

    IEnumerator WaitForReload()
    {
        uiManager.SetReloadingUIActive(true);
        animator.SetTrigger("Reload");
        reloadSound.Play();
        pointerController.StartMoving(); // Start moving the pointer

        reloadTime = 1.5f; // Duration of the reload animation
        magSize = 10;
        float elapsed = 0f;

        while (elapsed < reloadTime)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / reloadTime);
            uiManager.UpdateReloadProgress(progress);
            pointerController.SetPointerPosition(progress); // Update pointer position

            if (reloadKeyPressed) // Check if the reload key was pressed again
            {
                if (CheckSkillCheck(progress))
                {
                    magSize = 12; // Give extra bullets
                    reloadTime = 0f; // Instant reload
                }
                else
                {
                    reloadTime = 2f; // Increase reload time by 30%
                    magSize = Mathf.Min(bulletsLeft + 5, 8);
                }
                reloadKeyPressed = false; // Reset the flag
                break;
            }

            yield return null;
        }

        while (elapsed < reloadTime)
        {
            elapsed += 0.1f; // Increment elapsed time
            yield return new WaitForSeconds(0.1f);
        }

        // End the reload process
        bulletsLeft = magSize;
        uiManager.SetBulletsText(bulletsLeft, magSize);
        uiManager.SetReloadingUIActive(false);
        pointerController.StopMoving(); // Stop the pointer
        canShoot = true;
        canReload = true;
        reloadInProgress = false; // Reset the reload state
    }

    bool CheckSkillCheck(float progress)
    {
        // Check if the pointer is within the safe zone
        if (RectTransformUtility.RectangleContainsScreenPoint(
            pointerController.safeZone,
            pointerController.GetComponent<RectTransform>().position, null))
        {
            Debug.Log("Success!");
            return true;
        }
        else
        {
            Debug.Log("Fail!");
            return false;
        }
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
