using UnityEngine;
using TMPro;

public class DeathAndRespawn : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;  // Reference to the player prefab
    [SerializeField] private Transform respawnArea;    // Transform of the respawn area
    [SerializeField] private RedGreen rg;
    [SerializeField] private TextMeshProUGUI playerCountText;  // Reference to the TextMeshProUGUI for player count

    public static float playerCount = 457f;

    [SerializeField] private SFXManager sfx;

    void Start()
    {
        UpdatePlayerCountText();
        RespawnPlayer();

    }

    public void RespawnPlayer()
    {
        if(playerCount <= 456){
            sfx.PlaySFX(0);
        }
        --playerCount;
        UpdatePlayerCountText();
        Debug.Log("Player Spawned.");
        rg.canMove = true;

        // Destroy all existing players with the name "skateboard(Clone)"
        GameObject[] existingPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in existingPlayers)
        {
            if (player.name == "skateboard(Clone)")
            {
                Destroy(player);
            }
        }

        // Instantiate a new player prefab at the respawn area's position and rotation
        Instantiate(playerPrefab, respawnArea.position, respawnArea.rotation);
    }

    private void UpdatePlayerCountText()
    {
        // Update the text of the TextMeshProUGUI with the current player count
        playerCountText.text = "" + playerCount;
    }
}
