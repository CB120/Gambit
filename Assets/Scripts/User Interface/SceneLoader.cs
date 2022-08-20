using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    static SceneLoader Singleton;
    [SerializeField] Animator loadingScreenAnimator;

    private void Awake() {
        if (Singleton) {
            Destroy(gameObject);
        } else {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    public static void LoadScene(string name) {
        if (Singleton) {
            Singleton.StartLoadSceneAsync(name);
        } else {
            SceneManager.LoadScene(name);
        }
    }

    public void StartLoadSceneAsync (string name) {
        StartCoroutine(LoadSceneAsyncOperation(name));
    }

    IEnumerator LoadSceneAsyncOperation (string scene) {
        loadingScreenAnimator.SetBool("Loading", true);

        while (loadingScreenAnimator.GetCurrentAnimatorStateInfo(0).tagHash != Animator.StringToHash("Closed"))
            yield return null;
        
        yield return SceneManager.LoadSceneAsync(scene);

        loadingScreenAnimator.SetBool("Loading", false);
    }
}
