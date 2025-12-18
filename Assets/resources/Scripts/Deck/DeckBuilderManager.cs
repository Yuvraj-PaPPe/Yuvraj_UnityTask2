using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class DeckBuilderManager : MonoBehaviour
{
    [Header("Configuration")]
    public List<UnitData> allAvailableUnits; // Drag ALL your UnitData files here
    public int maxDeckSize = 3; // Keep it 3 to match your gameplay for now

    [Header("UI References")]
    public Transform libraryGrid; // The Content object of your Scroll View
    public Transform currentDeckGrid; // The horizontal layout group for your hand
    public GameObject cardPrefab; // The button prefab we will make next
    public Button startBattleButton;
    public TextMeshProUGUI statusText;

    private List<UnitData> currentDeck = new List<UnitData>();

    private void Start()
    {
        // Clear previous selection
        SelectedDeck.ClearDeck();
        UpdateUI();
        PopulateLibrary();
    }

    void PopulateLibrary()
    {
        // Create a button for every available unit
        foreach (var unit in allAvailableUnits)
        {
            GameObject newCard = Instantiate(cardPrefab, libraryGrid);
            CardUI cardScript = newCard.GetComponent<CardUI>();
            cardScript.Setup(unit, this);
        }
    }

    public void OnCardClicked(UnitData unit)
    {
        // If already in deck, remove it
        if (currentDeck.Contains(unit))
        {
            currentDeck.Remove(unit);
        }
        else
        {
            // If not in deck, add it (if space)
            if (currentDeck.Count < maxDeckSize)
            {
                currentDeck.Add(unit);
            }
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        // 1. Refresh the visual "Hand"
        // Destroy old icons
        foreach (Transform child in currentDeckGrid) Destroy(child.gameObject);

        // Create new icons for current selection
        foreach (var unit in currentDeck)
        {
            GameObject cardObj = Instantiate(cardPrefab, currentDeckGrid);
            // We reuse the card script just to show the icon
            cardObj.GetComponent<CardUI>().Setup(unit, this); 
        }

        // 2. Update Status Text
            statusText.text = $"Deck: {currentDeck.Count} / {maxDeckSize}";

        // 3. Enable Start Button only if full
        startBattleButton.interactable = (currentDeck.Count == maxDeckSize);
    }

    public void OnClickStartBattle()
    {
        // Save to the static class so the next scene can read it
        SelectedDeck.deck = new List<UnitData>(currentDeck);
        
        // Load the Battle Scene
        GameFlowManager.Instance.LoadScene("Level1");
    }
}