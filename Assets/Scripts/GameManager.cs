using System;
using System.Text;
using UnityEngine;
using TMPro;


public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI randomNumText;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TextMeshProUGUI nameLabel;

    private float timer;
    private bool isGameActive = false;
    private const float UpdateInterval = 5f;


    private async void Start()
    {

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
            timer = 0f; 
        }
    }

    public void DisplayEnteredName()
    {
        nameLabel.text = nameInputField.text;
    }
    public async void GameActivate()  {
        isGameActive = true; 
        nameLabel.text = nameInputField.text;
        
    }
    private void UpdateTimeDisplay()
    {
        timeText.text = DateTime.Now.ToString("HH:mm");
    }

    private void UpdateRandomNumberDisplay()
    {
        randomNumText.text = UnityEngine.Random.Range(79, 92).ToString();
    }

  
}
