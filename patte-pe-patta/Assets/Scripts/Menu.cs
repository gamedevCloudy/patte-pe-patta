using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
public class Menu : MonoBehaviour
{
   
    [SerializeField] private GameObject _howTo; 
    [SerializeField] private AudioSource _tapSound; 

    public void LoadGame()
    {
        PlayTapAudio(); 
        StartCoroutine("LoadSceneAfterDelay"); 
    }

    public void QuitGame()
    {
        PlayTapAudio(); 
        Application.Quit(); 
    }

    public void ActiveHowTo()
    {
        PlayTapAudio(); 
        _howTo.SetActive(true);
    }

    public void DeactivateHowTo()
    {
        PlayTapAudio(); 
        _howTo.SetActive(false);
    }

    IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(1f); 
        SceneManager.LoadScene("Game Scene"); 
    }

    public void PlayTapAudio()
    {
        _tapSound.Play(); 
    }
}
