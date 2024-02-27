using UnityEngine;
using System.Collections;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI t1;
    [SerializeField] private TextMeshProUGUI t2;
    [SerializeField] private GameObject obj1;
    [SerializeField] private GameObject obj2;
    [SerializeField] private CanvasGroup bets;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject VFX;
    [SerializeField] private GameObject vfx2;

    private float randomNumber;
    private float counterValue = 0f;
    private float countdownValue = 7f;
    private bool isCounterComplete = false;
    private bool isGenerating = false;

    private void Start()
    {
        BackgroundScroller.Instance.StopScrolling();
        anim.Play("Idle");
        VFX.SetActive(true);
        GenerateRandomNumber();
    }

    private void Update()
    {
        if (!isGenerating)
        {
            HandleCountdown();
        }
        else
        {
            HandleCounter();
        }
    }

    private void HandleCountdown()
    {
        countdownValue -= Time.deltaTime;
        if (countdownValue <= 1f)
        {
            t2.gameObject.SetActive(true);
            t1.gameObject.SetActive(false);
        }
        if (countdownValue <= 0f)
        {
            StartGenerationPhase();
        }
        countdownText.text = countdownValue.ToString("F2");
    }

    private void StartGenerationPhase()
    {
        BackgroundScroller.Instance.StartScrolling();
        anim.Play("RocketMove");
        VFX.SetActive(false);
        vfx2.SetActive(true);
        bets.alpha = 0f;
        countdownValue = 0f;
        isGenerating = true;
        GenerateRandomNumber();
    }

    private void HandleCounter()
    {
        if (!isCounterComplete && counterValue <= randomNumber)
        {
            UpdateCounter();
        }
        else if (isCounterComplete)
        {
            ResetForNextGeneration();
        }
    }

    private void UpdateCounter()
    {
        counterValue += Time.deltaTime;
        counterText.text = $"{counterValue:F2}x";
        if (counterValue >= randomNumber)
        {
            EndGenerationPhase();
        }
    }

    private void EndGenerationPhase()
    {
        BackgroundScroller.Instance.StopScrolling();
        anim.Play("Idle");
        VFX.SetActive(true);
        vfx2.SetActive(false);
        counterText.color = Color.red;
        t2.gameObject.SetActive(false);
        t1.gameObject.SetActive(true);
        isCounterComplete = true;
    }

    private void ResetForNextGeneration()
    {
        counterValue = 0f;
        isCounterComplete = false;
        isGenerating = false;
        bets.alpha = 1f;
        StartCoroutine(ShowAndHideCrashed());
    }

    IEnumerator ShowAndHideCrashed()
    {
        obj1.SetActive(true);
        obj2.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        obj1.SetActive(false);
        obj2.SetActive(true);
        yield return new WaitForSeconds(1f);
        counterText.color = Color.white;
        counterText.text = "0.00x";
    }

    private void GenerateRandomNumber()
    {
        randomNumber = Random.Range(0.2f, 1.00f) * 5.5f; // Adjusted for clarity
        counterText.text = "0.00x";
        counterText.color = Color.white;
        countdownValue = 7f; // Reset countdown for next generation
    }
}
