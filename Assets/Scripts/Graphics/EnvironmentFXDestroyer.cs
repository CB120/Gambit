using UnityEngine;

public class EnvironmentFXDestroyer : MonoBehaviour
{
    private void Awake() {
        if (PlayerPrefs.GetInt("GRAPHICS_EnvironmentFX", 1) == 1) {
            Destroy(this);
        } else {
            Destroy(gameObject);
        }
    }
}
