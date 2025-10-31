using System;
using System.Collections; // if using TextMeshPro
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GeminiUI : MonoBehaviour
{
    private string prefix;
    private string layer1;
    private string layer2;
    private string layer3;
    private string layer4;
    private string layer5;
    private string additionalDescription;
    private string prompt;

    private string kingdomName;
    private string mood;


    public TMP_InputField inputField;
    public TMP_Dropdown dropdown;
    public List<string> moodList = new List<string>() { 
        "Epic/Grand", 
        "Tragic/Somber", 
        "Mysterious/Cryptic", 
        "Triumphant/Heroic", 
        "Pragmatic/Objective", 
        "Fateful/Prophetic",
        "Cautionary/Didactic",
        "Dark/Ominous",
    };

    public List<string> moodDescription = new List<string>()
    {
        "Focuses on monumental events, heroism, and the vast sweep of time. Elevated, formal language",
        "Emphasizes loss, decline, fate, and the inevitable suffering of the people. Mournful and reflective",
        "Hints at secrets, unknown forces, and incomplete knowledge. Uses evocative, suggestive language",
        "Celebrates victories, strong leadership, and overcoming adversity. Inspiring and declarative",
        "Focuses on facts, consequences, and socio-economic factors. Neutral, academic, and dry.",
        "Suggests events were predetermined or guided by divine/magical forces. Often uses 'must,' 'shall,' or hints at future events.",
        "Aims to teach a moral lesson or warn future generations about past mistakes (e.g., hubris, greed).",
        "Focuses on terror, corruption, and the slow, insidious growth of evil. Creates tension and dread."
    };

    public Dictionary<string, string> moodDict;

    //public TMP_Text outputText;

    //private GeminiClient gemini;

    //void Start()
    //{
    //    gemini = FindObjectOfType<GeminiClient>();
    //}

    //public void OnSendPressed()
    //{
    //    string userPrompt = inputField.text;
    //    StartCoroutine(SendToGemini(userPrompt));
    //}

    //private IEnumerator SendToGemini(string prompt)
    //{
    //    yield return null;
    //    gemini.SendPrompt(prompt);
    //    outputText.text = "Waiting for response...";
    //    // You can modify GeminiClient to store the latest response in a variable
    //    // and then update outputText when it arrives
    //}

    private GeminiClient gemini;

    void Start()
    {
        // Find the GeminiClient in the scene (or drag-assign in inspector)
        gemini = FindObjectOfType<GeminiClient>();

        if (gemini == null)
            print("GEMINI IS NULL");


        prefix = "Can u generate a strictly 500-1000 word story based on this setting: ";

        layer1 = "Genre: High Fantasy";

        //layer2 = "Protagonist Name: Elaina";

        //layer3 = "Protagonist Ally: Poppy, Chino";

        //layer4 = "Antagonist Name: Nil";

        //layer5 = "Tone: Dark Brooding, tragic, grim themes";

        additionalDescription = "Additional info: Do not include voicelines! Tell in a 3rd person narration, " +
            "treat it as though you are recapping an entire world's history. Treat each paragraph as an age/era that the " +
            "world goes through, IE: first paragraph is the stone age equivalent of this fantasy world, the second paragraph " +
            "is the bronze age, etc..., before each paragraph give a header that describes the age IE: dark age, golden age, etc...";

        prompt = "High fantasy is a fictional genre set in an entirely invented universe with its own geography, " +
            "mythologies, cultures, and supernatural systems. It emphasizes epic storytelling where the outcomes of conflicts shape " +
            "the fate of kingdoms or the entire world. For this project, the tool will generate high-quality historical timelines that describe" +
            " major eras, the rise and fall of civilizations, magical developments, legendary figures, races, and world-defining events. " +
            "This approach ensures that the resulting content represents rich high fantasy worldbuilding, distinct from real-world medieval settings" +
            " and grounded in immersive, original lore.";

        dropdown.options.Clear();
        foreach (string t in moodList)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = t });
        }

        moodDict = new Dictionary<string, string>();


        for (int i = 0; i < moodDescription.Count; i++) 
        {
            moodDict.Add(moodList[i], moodDescription[i]);
        }
    }

    public void GenerateContent()
    {
        kingdomName = "kingdom name: " + inputField.text;
        Debug.Log(kingdomName);
        Debug.Log(dropdown.options[dropdown.value].text);

        mood = "mood : " + dropdown.options[dropdown.value].text;

        var x = dropdown.options[dropdown.value].text;

        Debug.Log("Selected mood desc : " + moodDict[x]);

        gemini.SendPrompt(kingdomName + mood + prefix + layer1 + layer2 + layer3 + layer4 + layer5 + additionalDescription);
    }
}
