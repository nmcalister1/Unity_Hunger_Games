using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI countdownText; // Reference to the UI Text component
    private float countdownTime = 3f; // Countdown duration
    private bool countdownFinished = false;

    void Start()
    {
        countdownText.gameObject.SetActive(true); // Ensure the countdown text is active at the start
        StartCoroutine(CountdownCoroutine());
    }

    IEnumerator CountdownCoroutine()
    {
        while (countdownTime > 0)
        {
            countdownText.text = countdownTime.ToString("0");
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false); // Deactivate the countdown text
        countdownFinished = true;
    }

    public bool IsCountdownFinished()
    {
        return countdownFinished;
    }
}
