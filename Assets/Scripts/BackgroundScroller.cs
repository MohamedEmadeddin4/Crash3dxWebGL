using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Scrolls the background texture at a specified speed.
/// </summary>
public class BackgroundScroller : MonoBehaviour
{
    [SerializeField, Range(-1f, 1f)]
    private float scrollSpeed = 0.5f;
    private float offset;
    private Material material;

    // Singleton instance
    public static BackgroundScroller Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Initialize variables
    private void Start()
    {
        material = GetComponent<RawImage>().material;
    }

    // Update is called once per frame
    private void Update()
    {
        ScrollTexture();
    }

    /// <summary>
    /// Scrolls the texture based on time and speed.
    /// </summary>
    private void ScrollTexture()
    {
        offset += (Time.deltaTime * scrollSpeed) / 10f;
        material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }

    /// <summary>
    /// Starts the background scrolling.
    /// </summary>
    public void StartScrolling()
    {
        scrollSpeed = 0.5f;
    }

    /// <summary>
    /// Stops the background from scrolling.
    /// </summary>
    public void StopScrolling()
    {
        scrollSpeed = 0f;
    }
}
