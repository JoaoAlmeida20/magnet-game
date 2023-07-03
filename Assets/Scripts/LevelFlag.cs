using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFlag : MonoBehaviour
{
    public AudioSource source;
    public AudioClip clip;
    public Canvas fadeToBlack;

    bool isLoad;
    float isLoadtime = 0.0f;

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Player") {
            source.PlayOneShot(clip);
            isLoad = true;
        }
    }

    void Update() {
        if (isLoad)
            isLoadtime += Time.deltaTime;
        if (isLoadtime > 0.3f) {
            fadeToBlack.gameObject.SetActive(true);
        }
        if (isLoadtime > 2.15f) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
