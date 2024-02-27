using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI randomNumText;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TextMeshProUGUI nameLabel;

    private float timer;
    private const float UpdateInterval = 5f; // Use a constant for fixed values

    private void Start()
    {
        // Initialize the UI elements immediately when the game starts
        UpdateTimeDisplay();
        UpdateRandomNumberDisplay();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= UpdateInterval)
        {
            UpdateTimeDisplay();
            UpdateRandomNumberDisplay();
            timer = 0f; // Reset the timer
        }
    }

    public void DisplayEnteredName()
    {
        nameLabel.text = nameInputField.text; // Directly use the text from input
    }

    private void UpdateTimeDisplay()
    {
        // Use DateTime.Now for the current time and format it in "HH:mm" for hours and minutes
        timeText.text = System.DateTime.Now.ToString("HH:mm");
    }

    private void UpdateRandomNumberDisplay()
    {
        // Random.Range is inclusive for int, so 79 to 92 will never actually produce 92, making it 79-91
        randomNumText.text = Random.Range(79, 92).ToString();
    }
}
