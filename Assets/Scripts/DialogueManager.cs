using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class Dialogue
    {
        public string speakerName;
        [TextArea(3, 10)]
        public string dialogueText;
    }

    [Header("Dialogue Settings")]
    public List<Dialogue> dialogues = new List<Dialogue>();
    public float shakeIntensity = 0.5f;
    public float shakeSpeed = 10f;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public Image speechBubble;

    private int currentDialogueIndex = 0;
    private Vector3 originalTextPosition;
    private Coroutine shakeCoroutine;

    void Start()
    {
        // UI elementlerini başlangıçta gizle
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Oyunu başlangıçta durdur
        GameManager.gameRunning = false;

        // İlk diyalogu başlat
        StartDialogue();
    }

    void Update()
    {
        // Enter tuşu ile diyaloğu ilerlet
        if (Input.GetKeyDown(KeyCode.Return))
        {
            NextDialogue();
        }
    }

    public void StartDialogue()
    {
        if (dialogues.Count == 0) return;

        currentDialogueIndex = 0;
        ShowDialogue(currentDialogueIndex);
    }

    private void ShowDialogue(int index)
    {
        if (index >= dialogues.Count)
        {
            EndDialogue();
            return;
        }

        // UI'ı göster
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        // Metinleri güncelle
        speakerText.text = dialogues[index].speakerName;
        dialogueText.text = dialogues[index].dialogueText;

        // Orijinal pozisyonu kaydet
        originalTextPosition = dialogueText.rectTransform.localPosition;

        // Shake efektini başlat
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeText());
    }

    private IEnumerator ShakeText()
    {
        while (true)
        {
            float x = originalTextPosition.x + Random.Range(-shakeIntensity, shakeIntensity);
            float y = originalTextPosition.y + Random.Range(-shakeIntensity, shakeIntensity);
            dialogueText.rectTransform.localPosition = new Vector3(x, y, originalTextPosition.z);
            yield return new WaitForSeconds(1f / shakeSpeed);
        }
    }

    public void NextDialogue()
    {
        currentDialogueIndex++;
        ShowDialogue(currentDialogueIndex);
    }

    private void EndDialogue()
    {
        // UI'ı gizle
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Shake efektini durdur
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        // Oyunu başlat
        GameManager.gameRunning = true;
    }

    // Yeni diyalog eklemek için
    public void AddDialogue(string speaker, string text)
    {
        Dialogue newDialogue = new Dialogue
        {
            speakerName = speaker,
            dialogueText = text
        };
        dialogues.Add(newDialogue);
    }
} 