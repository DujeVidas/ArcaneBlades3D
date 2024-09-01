using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Shooting : MonoBehaviour
{
    public Transform mainCamera;
    public float maxDistance = 100f;
    public GameObject decalPrefab;
    public int maxDecals = 10;
    public float bulletDamage = 1;

    public GameObject uiManager; // Reference to the UI script
    private UI ui; // Reference to the UI component

    private List<GameObject> instantiatedDecals = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        if (uiManager != null)
        {
            ui = uiManager.GetComponent<UI>();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !ui.IsGamePaused())
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray shotRay = new Ray(mainCamera.position, mainCamera.forward);
        if(Physics.Raycast(shotRay, out RaycastHit rayHit, maxDistance))
        {
            InstantiateDecal(rayHit.point, rayHit.normal, rayHit.transform);
            if (rayHit.collider.gameObject.TryGetComponent(out ShootableObject shotObject))
            {
                shotObject.TakeShot(bulletDamage);
            }
        }
    }

    void InstantiateDecal(Vector3 position, Vector3 normal, Transform hitTransform)
    {
        // Determine the rotation to align with the surface
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);

        // Instantiate the decal prefab
        GameObject decal = Instantiate(decalPrefab, position, rotation);

        // Adjust the decal's position slightly to prevent z-fighting
        decal.transform.position += normal * 0.01f;

        // Parent the decal to the hit object to keep it in the correct position
        decal.transform.parent = hitTransform;

        // Add the decal to the list of instantiated decals
        instantiatedDecals.Add(decal);

        // Ensure that only the newest decals are kept
        RemoveOldDecals();
    }

    void RemoveOldDecals()
    {
        // If the number of decals exceeds the maximum limit
        if (instantiatedDecals.Count > maxDecals)
        {
            // Calculate the number of excess decals
            int excessDecals = instantiatedDecals.Count - maxDecals;

            // Remove the oldest excess decals from the list and destroy them
            for (int i = 0; i < excessDecals; i++)
            {
                GameObject decal = instantiatedDecals[0]; // Get the oldest decal
                instantiatedDecals.RemoveAt(0); // Remove it from the list
                Destroy(decal); // Destroy the GameObject
            }
        }
    }
}
