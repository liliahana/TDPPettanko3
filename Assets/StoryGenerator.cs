using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class StoryGenerator : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI storyText;
    public Image currentImage;
    public Button nextButton;
    public Button previousButton;
    public TextMeshProUGUI progressText;
    public GameObject loadingPanel;
    public TextMeshProUGUI loadingText;

    [Header("Generation Settings")]
    public int imageWidth = 1024;
    public int imageHeight = 1024;
    public float delayBetweenGenerations = 1f;

    private List<string> paragraphs = new List<string>();
    private List<Sprite> generatedImages = new List<Sprite>();
    private int currentIndex = 0;
    private bool isGenerating = false;

    private string fullStory = @"The wind, once a playful whisper through the Eldrin woods, now carried the stench of ash and fear. Elaina moved with a predator's grace, each step a silent prayer against the crumbling earth. Behind her, Poppy's breath came in ragged gasps, her slight frame hunched beneath the weight of her enchanted bow. Chino, a bastion of quiet strength, walked point, his obsidian axe a dark promise against the encroaching shadows.

Nil's blight had consumed the land, twisting ancient oaks into skeletal claws, poisoning rivers into black veins. Hope, a fragile thing, had withered days ago, replaced by a grim resolve. They had sought the Heartstone, the last shard of light, believing it could turn the tide. Now, its faint thrumming felt like a mockery amidst the pervasive gloom.

A shriek tore through the oppressive silence. Not an enemy's cry, but Poppy's. Her feet had sunk into the corrupted soil, tendrils of black miasma coiling around her ankles. Chino whirled, his axe a blur, severing the first vine, but more erupted from the ground. Poppy's eyes, wide with terror, met Elaina's. A silent plea, quickly extinguished as the corruption surged, consuming her, leaving only a fading shriek and a ripple in the dying earth.

Elaina's hand instinctively went to her blade, but there was no target. Only the insidious, consuming presence of Nil, a formless hunger that had claimed her friend. Chino roared, a sound of pure agony and fury, charging blindly into the swirling darkness where Poppy had vanished. He was a beacon of defiance, but defiance was futile against such an ancient malevolence. The last Elaina saw was the flash of his axe, swallowed by the gathering gloom, then silence. Absolute, heavy, unforgiving silence.

She stood alone, the Heartstone pulsing weakly in her grasp, its light barely illuminating the devastation. The air tasted of rot and unwept tears. Nil's victory was complete here, in this desolate pocket of the world. Elaina looked at the stone, then out into the endless, blighted landscape. There was no escape, only a path forward into a future she no longer recognized. Her grief was a lead weight in her chest, but beneath it, a sliver of cold, hard resolve began to form. A promise whispered on a silent, dead wind: this was not the end, only the beginning of an unholy reckoning.";

    void Start()
    {
        // Parse the story into paragraphs
        ParseStoryIntoParagraphs();

        // Setup UI
        nextButton.onClick.AddListener(NextSlide);
        previousButton.onClick.AddListener(PreviousSlide);

        // Start with first paragraph
        ShowCurrentParagraph();

        // Auto-start generation for first image
        GenerateCurrentImage();
    }

    void ParseStoryIntoParagraphs()
    {
        paragraphs.Clear();

        // Split by double newlines (standard paragraph separation)
        string[] parts = fullStory.Split(new[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string part in parts)
        {
            string trimmed = part.Trim();
            if (!string.IsNullOrEmpty(trimmed))
            {
                paragraphs.Add(trimmed);
            }
        }

        Debug.Log($"Parsed {paragraphs.Count} paragraphs from story");
    }

    public void GenerateCurrentImage()
    {
        if (isGenerating) return;

        if (currentIndex < paragraphs.Count)
        {
            StartCoroutine(GenerateImageForParagraph(paragraphs[currentIndex], currentIndex));
        }
    }

    private IEnumerator GenerateImageForParagraph(string paragraph, int paragraphIndex)
    {
        isGenerating = true;
        loadingPanel.SetActive(true);
        loadingText.text = $"Generating image for scene {paragraphIndex + 1}/{paragraphs.Count}...";

        // Clean and enhance the prompt
        string prompt = CleanParagraphForPrompt(paragraph);

        string apiUrl = $"https://image.pollinations.ai/prompt/{UnityWebRequest.EscapeURL(prompt)}?width={imageWidth}&height={imageHeight}";

        Debug.Log($"Generating for paragraph {paragraphIndex + 1}: {prompt}");

        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(apiUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );

                // Ensure we have enough slots in our list
                while (generatedImages.Count <= paragraphIndex)
                {
                    generatedImages.Add(null);
                }

                generatedImages[paragraphIndex] = sprite;
                Debug.Log($"Successfully generated image for paragraph {paragraphIndex + 1}");

                // Update display if this is the current image
                if (currentIndex == paragraphIndex)
                {
                    currentImage.sprite = sprite;
                    currentImage.color = Color.white;
                }

                // Auto-advance to next paragraph if available
                if (paragraphIndex == currentIndex && currentIndex < paragraphs.Count - 1)
                {
                    yield return new WaitForSeconds(0.5f); // Brief pause
                    NextSlide(); // Auto-move to next paragraph and start generating
                }
            }
            else
            {
                Debug.LogError($"Failed to generate image for paragraph {paragraphIndex + 1}: {webRequest.error}");
                // Add null placeholder to maintain list structure
                while (generatedImages.Count <= paragraphIndex)
                {
                    generatedImages.Add(null);
                }
            }
        }

        loadingPanel.SetActive(false);
        isGenerating = false;
        UpdateNavigation();
    }

    string CleanParagraphForPrompt(string paragraph)
    {
        // Remove excessive punctuation, trim, and add artistic enhancements
        string cleaned = paragraph.Trim();

        // Limit length to avoid API issues (optional)
        if (cleaned.Length > 200)
        {
            cleaned = cleaned.Substring(0, 200) + "...";
        }

        // Add style enhancements
        cleaned += ", fantasy art, cinematic, dramatic lighting, detailed";

        return cleaned;
    }

    void ShowCurrentParagraph()
    {
        if (currentIndex < paragraphs.Count)
        {
            storyText.text = paragraphs[currentIndex];

            // Show image if it's already generated
            if (currentIndex < generatedImages.Count && generatedImages[currentIndex] != null)
            {
                currentImage.sprite = generatedImages[currentIndex];
                currentImage.color = Color.white;
            }
            else
            {
                currentImage.color = Color.clear; // Hide image if not generated
            }
        }

        UpdateNavigation();
    }

    void UpdateNavigation()
    {
        previousButton.interactable = currentIndex > 0;
        nextButton.interactable = currentIndex < paragraphs.Count - 1;

        progressText.text = $"{currentIndex + 1}/{paragraphs.Count}";

        // Show generation status
        if (currentIndex < generatedImages.Count && generatedImages[currentIndex] != null)
        {
            progressText.text += " ✓";
        }
        else if (isGenerating && currentIndex < paragraphs.Count)
        {
            progressText.text += " ⟳";
        }
        else
        {
            progressText.text += " ◯";
        }
    }

    public void NextSlide()
    {
        if (currentIndex < paragraphs.Count - 1)
        {
            currentIndex++;
            ShowCurrentParagraph();

            // Auto-generate image for this paragraph if not already generated
            if (currentIndex >= generatedImages.Count || generatedImages[currentIndex] == null)
            {
                GenerateCurrentImage();
            }
        }
    }

    public void PreviousSlide()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowCurrentParagraph();
        }
    }

    // Public method to load a new story dynamically
    public void LoadNewStory(string newStory)
    {
        fullStory = newStory;
        paragraphs.Clear();
        generatedImages.Clear();
        currentIndex = 0;
        isGenerating = false;

        ParseStoryIntoParagraphs();
        ShowCurrentParagraph();
        GenerateCurrentImage();
    }

    // Method to manually trigger generation for all missing images
    public void GenerateAllMissingImages()
    {
        StartCoroutine(GenerateAllMissingImagesCoroutine());
    }

    private IEnumerator GenerateAllMissingImagesCoroutine()
    {
        for (int i = 0; i < paragraphs.Count; i++)
        {
            if (i >= generatedImages.Count || generatedImages[i] == null)
            {
                yield return StartCoroutine(GenerateImageForParagraph(paragraphs[i], i));
                yield return new WaitForSeconds(delayBetweenGenerations);
            }
        }
    }
}