using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject malePrefab;
    public GameObject femalePrefab;
    public Transform spawnPoint;

    void Start()
    {
        GameObject prefabToSpawn = (CharacterData.SelectedAvatarIndex == 0) ? malePrefab : femalePrefab;

        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        }
    }
}