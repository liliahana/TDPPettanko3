using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ImageLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider slider;
    public void loadImage(int imageIndex)
    {
        StartCoroutine(LoadAsynchronously(imageIndex));
    }

    IEnumerator LoadAsynchronously(int imageIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(imageIndex);

        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            slider.value = progress;
            yield return null;
        }
    }
}