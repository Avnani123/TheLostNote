using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPuzzleManager : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public List<string> allPossibleKeys = new List<string> 
    { 
        "White Key_0", "White Key_0 (1)", "White Key_0 (2)", "White Key_0 (3)", "White Key_0 (4)", "White Key_0 (5)", "White Key_0 (6)", "White Key_0 (7)", "White Key_0 (8)", "White Key_0 (9)", "White Key_0 (10)", "White Key_0 (11)", 
    "White Key_0 (12)", "White Key_0 (13)", "White Key_0 (14)", "White Key_0 (15)" 
    }; // All 16 possible keys

    public int puzzleLength = 5; // Number of keys in the puzzle sequence
    private List<string> correctSequence = new List<string>(); // Generated random sequence
    private List<string> playerInput = new List<string>(); // Track player input

    [Header("Horror effect")]
    public HorrorEffect2D horrorEffect; // Reference to the HorrorEffect2D script
    private bool horrorTriggered = false; // To avoid repeated horror effect triggers

    [Header("Success Settings")]
    public AudioSource successSound; // Sound to play on puzzle success
    public GameObject reward; // Optional reward to show on success (e.g., key, clue)

    void Start()
    {
        GenerateRandomSequence(); // Generate random puzzle on start

        if (reward != null)
            reward.SetActive(false); // Hide the reward initially

        // Null reference checks
        if (successSound == null)
            Debug.LogWarning("Success Sound is not assigned!");

        if (horrorEffect == null)
            Debug.LogWarning("Horror Effect is not assigned!");
    }

    // Generate a random sequence of keys for the puzzle
    private void GenerateRandomSequence()
    {
        correctSequence.Clear();
        List<string> availableKeys = new List<string>(allPossibleKeys);

        for (int i = 0; i < puzzleLength && availableKeys.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableKeys.Count);
            string randomKey = availableKeys[randomIndex];
            correctSequence.Add(randomKey);
            availableKeys.RemoveAt(randomIndex);
        }

        Debug.Log("Generated Random Sequence: " + string.Join(", ", correctSequence));
    }

    // Call this method when a piano key is pressed
    public void RegisterKeyPress(string keyName, bool isCorrectKey)
    {
        // Prevent input overflow
        if (playerInput.Count >= correctSequence.Count)
        {
            Debug.LogWarning("Player input exceeds the expected sequence length. Resetting puzzle.");
            ResetPuzzle();
            return;
        }

        playerInput.Add(keyName);
        Debug.Log($"Key Pressed: {keyName}");

        // Check if the player's input matches the puzzle sequence
        if (!IsInputCorrect())
        {
            TriggerHorrorEffect();
            ResetPuzzle();
        }
        else if (playerInput.Count == correctSequence.Count)
        {
            SolvePuzzle();
        }
    }

    // Check if the current input matches the expected sequence so far
    private bool IsInputCorrect()
    {
        for (int i = 0; i < playerInput.Count; i++)
        {
            if (playerInput[i] != correctSequence[i])
                return false;
        }
        return true;
    }

    // Trigger horror effect and reset the puzzle
    private void TriggerHorrorEffect()
    {
        if (horrorEffect != null && !horrorTriggered)
        {
            horrorEffect.TriggerScare();
            horrorTriggered = true;
            Debug.Log("Wrong Sequence! Horror Effect Triggered!");
        }
    }

    // Solve the puzzle (success scenario)
    private void SolvePuzzle()
    {
        Debug.Log("Puzzle Solved! Playing Success Sound...");
        successSound?.Play();

        if (reward != null)
            reward.SetActive(true); // Show reward (e.g., key, clue)

        GenerateRandomSequence(); // Generate a new random puzzle after success
        ResetPuzzle();
    }

    // Reset the puzzle input and horror state
    private void ResetPuzzle()
    {
        playerInput.Clear();
        horrorTriggered = false; // Reset the horror state
        Debug.Log("Puzzle Reset. Ready for new input.");
    }
}

