using UnityEngine;
using System; 
using System.Collections;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;




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
    private string gameStatus = "init";
    private ClientWebSocket ws = new ClientWebSocket();
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private Task webSocketTask;

    private void Start()
    {
        BackgroundScroller.Instance.StopScrolling();
        anim.Play("Idle");
        VFX.SetActive(true);
        // GenerateRandomNumber();
        LaunchGameAndConnectWebSocket();
    }

    private void Update()
    {

        if(gameStatus == "wait")
        {
            HandleCountdown();
        }
        else  if(gameStatus == "start")
        {
            HandleCounter();
        }
    }

    private void HandleCountdown()
    {
        if(gameStatus == "wait")
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
        // GenerateRandomNumber();
        gameStatus = "start";
    }

    private void HandleCounter()
    {
        if (gameStatus == "start" && !isCounterComplete )
        {
            UpdateCounter();
        }
      
    }

    private void UpdateCounter()
    {
        counterValue += Time.deltaTime;
        counterText.text = $"{counterValue:F2}x";
       
    }

private void EndGenerationPhase()
{
    BackgroundScroller.Instance.StopScrolling();
    anim.Play("Crash"); 
    VFX.SetActive(true);
    vfx2.SetActive(false);
    counterText.color = Color.red;
    t2.gameObject.SetActive(false);
    t1.gameObject.SetActive(true);
    isCounterComplete = true;
    gameStatus = "crash"; 
}


private void ResetForNextGeneration()
{
    countdownValue = 7f;
    counterValue = 0f; 
    isCounterComplete = false; 
    isGenerating = false; 
    bets.alpha = 1f; 
    BackgroundScroller.Instance.StopScrolling(); 
    anim.Play("Idle"); 
    VFX.SetActive(false); 
    vfx2.SetActive(false); 
    counterText.color = Color.white; 
    counterText.text = "0.00x";
    gameStatus = "init"; 
 
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
        randomNumber = UnityEngine.Random.Range(0.2f, 1.00f) * 5.5f;
        counterText.text = "0.00x";
        counterText.color = Color.white;
        countdownValue = 7f; 
    }

    private async Task LaunchGameAndConnectWebSocket()
    {
        string requestUri = "https://alienapi.imoon.com/api/stage/v2/fun-launch";
        string requestData = "{\"balance\":1000,\"gameId\":\"1001\",\"currency\":\"USD\",\"nickname\":\"amanda\",\"lang\":\"en\",\"extraData\":{}}";

        using (var httpClient = new HttpClient())
        {
            var requestContent = new StringContent(requestData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(requestUri, requestContent);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                JObject jsonResponse = JObject.Parse(responseContent);
                string gameUrl = jsonResponse["url"].ToString();

                Uri gameUri = new Uri(gameUrl);
                string fullToken = System.Web.HttpUtility.ParseQueryString(gameUri.Query).Get("token");

                JObject tokenObject = JObject.Parse(fullToken);
                string connectionToken = tokenObject["connectionToken"].ToString();
                Debug.Log(connectionToken);

                await ConnectToWebSocket(connectionToken);
            }
            else
            {
                Debug.LogError("Failed to launch game and obtain WebSocket URL.");
            }
        }
    }


    private async Task ConnectToWebSocket(string connectionToken)
    {
        try
        {
            await ws.ConnectAsync(new Uri(connectionToken), cancellationTokenSource.Token);
            Debug.Log("WebSocket Connected!");
            webSocketTask = ReceiveMessages();
        }
        catch (Exception ex)
        {
            Debug.LogError($"WebSocket connection failed: {ex.Message}");
        }
    }

    private async Task ReceiveMessages()
    {
        var buffer = new byte[1024 * 4];

       
            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationTokenSource.Token);
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Debug.Log("Received: " + message);

                ProcessReceivedMessage(message);
            }
        
    }

    private void ProcessReceivedMessage(string message)
{
    JObject messageObject = null;
    try
    {
        messageObject = JObject.Parse(message);
    }
    catch (JsonReaderException ex)
    {
        Debug.LogError($"Invalid JSON received: {message}. Error: {ex.Message}");
        return; 
    }

    string messageType = messageObject["type"]?.ToString();
   Debug.LogWarning("Received  message type: " + messageType);
    switch (messageType)
    {
        case "crash":
             EndGenerationPhase();
            break;
        case "start":
            StartGenerationPhase(); 
            break;
        case "wait":
        if(gameStatus != "wait")
            gameStatus = "wait";
            break;
        case "new-round":
            ResetForNextGeneration(); 
            break;    
        default:
            break;
    }
}



    private void OnDestroy()
    {
        if (ws != null)
        {
            cancellationTokenSource.Cancel(); 
            if (ws.State == WebSocketState.Open)
            {
                ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
            ws.Dispose();
        }
    }
}
