using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField] GameObject menu;
    private void OnPause()
    {
        if (menu.activeSelf)
        {
            Time.timeScale = 1f;
            print(Time.timeScale);
            menu.SetActive(false);
        }
        else
        {
            Time.timeScale = 0f;
            print(Time.timeScale);
            menu.SetActive(true); ;
        }
    }
}
