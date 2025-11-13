using UnityEngine;
using TMPro;
using System.Collections; // if using TextMeshPro

public class GeminiUI : MonoBehaviour
{
    private GeminiClient gemini;

    void Start()
    {
        // Find the GeminiClient in the scene (or drag-assign in inspector)
        gemini = FindObjectOfType<GeminiClient>();

        if (gemini == null)
            print("GEMINI IS NULL");

        string prefix = "Can u generate a strictly 500 word or less story based on this setting: ";

        string layer1 = "Genre: High Fantasy";

        string layer2 = "Protagonist Name: Elaina";

        string layer3 = "Protagonist Ally: Poppy, Chino";

        string layer4 = "Antagonist Name: Nil";

        string layer5 = "Tone: Dark Brooding, tragic, grim themes";

        string additionalDescription = "Additional info: Do not include voicelines! Tell in a 3rd person narration, " +
            "treat it as though you are recapping an entire world's history. Treat each paragraph as an age/era that the " +
            "world goes through, IE: first paragraph is the stone age equivalent of this fantasy world, the second paragraph " +
            "is the bronze age, etc..., before each paragraph give a header that describes the age IE: dark age, golden age, etc...";

        string prompt = "High fantasy is a fictional genre set in an entirely invented universe with its own geography, " +
            "mythologies, cultures, and supernatural systems. It emphasizes epic storytelling where the outcomes of conflicts shape " +
            "the fate of kingdoms or the entire world. For this project, the tool will generate high-quality historical timelines that describe" +
            " major eras, the rise and fall of civilizations, magical developments, legendary figures, races, and world-defining events. " +
            "This approach ensures that the resulting content represents rich high fantasy worldbuilding, distinct from real-world medieval settings" +
            " and grounded in immersive, original lore.";

        gemini.SendPrompt(prefix + layer1 + layer2 + layer3 + layer4 + layer5 + additionalDescription);
    }
}
