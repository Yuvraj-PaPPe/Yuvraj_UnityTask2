using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic; // Required for Lists

public class TroopSpawnerVR : MonoBehaviour
{
    public EnergySystem energySystem;

    [Header("Spawn Locations")]
    // Drag your 3 Spawn Point Transforms here (Element 0, 1, 2)
    public Transform[] spawnPoints; 

    [Header("Deck Configuration")]
    // Drag your 3 UnitData cards (Knight, Archer, Tank) here for testing
    public List<UnitData> debugDeck; 

    // The actual deck the game will use (either from the Menu or the Debug list)
    private List<UnitData> activeDeck;

    void Start()
    {
        // 1. Check if we came from the Main Menu with a selected deck
        if (SelectedDeck.deck != null && SelectedDeck.deck.Count > 0)
        {
            activeDeck = SelectedDeck.deck;
        }
        else
        {
            // 2. If not (testing in Editor), use the Debug Deck we set in Inspector
            activeDeck = debugDeck;
        }
    }

    // Connect your Buttons to THIS function.
    // Top Button -> index 0
    // Middle Button -> index 1
    // Bottom Button -> index 2
    public void SpawnUnit(int deckIndex)
    {
        // Safety Checks
        if (activeDeck == null) return;
        if (deckIndex < 0 || deckIndex >= activeDeck.Count) return;

        // 1. Get the Card data
        UnitData unit = activeDeck[deckIndex];

        // 2. Check if we have enough Energy
        if (!energySystem.TrySpend(unit.cost)) return;

        // 3. Pick the Spawn Point
        // If we have 3 cards and 3 points, we map Card 0 to Point 0, etc.
        // The % operator ensures we don't crash if you have fewer points than cards.
        Transform spawnPoint = spawnPoints[deckIndex % spawnPoints.Length];

        // 4. Spawn the Unit
        Spawn(unit.prefab, spawnPoint);
    }

    void Spawn(GameObject prefab, Transform point)
    {
        GameObject troop = Instantiate(prefab, point.position, point.rotation);

        // Fix position using NavMesh so they don't spawn inside the floor/walls
        NavMeshHit hit;
        if (NavMesh.SamplePosition(point.position, out hit, 5f, NavMesh.AllAreas))
            troop.transform.position = hit.position;
            
        // Ensure the troop is on the Player Team
        TeamComponent tc = troop.GetComponent<TeamComponent>();
        if(tc != null) 
        {
            tc.team = Team.Player;
        }
    }
}