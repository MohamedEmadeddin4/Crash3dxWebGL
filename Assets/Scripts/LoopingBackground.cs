using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 1f;
    [SerializeField] private RectTransform background1;
    [SerializeField] private RectTransform background2;

    private float backgroundWidth;
    private bool isScrolling = false;

    private void Start()
    {
        // Initialize backgrounds by assuming both have the same width
        backgroundWidth = background1.sizeDelta.x; // Using sizeDelta.x for RectTransform width

        // Set initial positions for a seamless loop
        RepositionBackgrounds();
    }

    private void Update()
    {
        if (isScrolling)
        {
            ScrollBackgrounds();
            CheckAndRepositionBackgrounds();
        }
    }

    private void ScrollBackgrounds()
    {
        // Calculate offset for this frame
        float offset = scrollSpeed * Time.deltaTime;

        // Apply movement
        MoveBackground(background1, offset);
        MoveBackground(background2, offset);
    }

    private void MoveBackground(RectTransform background, float offset)
    {
        // Update position with wrap-around logic
        background.anchoredPosition -= new Vector2(offset, 0f);
        if (background.anchoredPosition.x <= -backgroundWidth)
        {
            // Wrap background to start position
            background.anchoredPosition += new Vector2(backgroundWidth * 2, 0);
        }
    }

    private void CheckAndRepositionBackgrounds()
    {
        // If a background has moved completely off-screen, reposition it to simulate endless scrolling
        RepositionIfNecessary(background1);
        RepositionIfNecessary(background2);
    }

    private void RepositionIfNecessary(RectTransform background)
    {
        if (background.anchoredPosition.x <= -backgroundWidth)
        {
            // Move background to the right side, effectively reusing it
            background.anchoredPosition += new Vector2(backgroundWidth * 2, 0);
        }
    }

    private void RepositionBackgrounds()
    {
        // Initial repositioning for a seamless loop
        background1.anchoredPosition = new Vector2(0, background1.anchoredPosition.y);
        background2.anchoredPosition = new Vector2(backgroundWidth, background2.anchoredPosition.y);
    }

    public void StartScrolling()
    {
        isScrolling = true;
    }

    public void StopScrolling()
    {
        isScrolling = false;
    }
}
