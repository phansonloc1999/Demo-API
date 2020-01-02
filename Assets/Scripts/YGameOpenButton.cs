using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YGameOpenButton : MonoBehaviour
{
    public RectTransform rectTransform;
    public Canvas canvas;
    private static float BUTTON_WIDTH;
    private static float BUTTON_HEIGHT;

    private static readonly Vector2 TOP_ANCHOR_MIN = new Vector2(0.5f, 1);
    private static readonly Vector2 TOP_ANCHOR_MAX = new Vector2(0.5f, 1);

    // Use this for initialization
    void Start()
    {
        BUTTON_WIDTH = rectTransform.rect.width;
        BUTTON_HEIGHT = rectTransform.rect.height;

        // rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, BUTTON_WIDTH);
        // rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, BUTTON_WIDTH);
        // rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, BUTTON_HEIGHT);
        // rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, BUTTON_HEIGHT);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDrag()
    {
        rectTransform.position = Camera.main.ScreenToWorldPoint(new Vector3(
            Mathf.Clamp(Input.mousePosition.x, BUTTON_WIDTH / 2 * canvas.scaleFactor, Screen.width - BUTTON_WIDTH / 2 * canvas.scaleFactor),
            Mathf.Clamp(Input.mousePosition.y, BUTTON_HEIGHT / 2 * canvas.scaleFactor, Screen.height - BUTTON_HEIGHT / 2 * canvas.scaleFactor),
            rectTransform.position.z)
        );
    }

    private void OnMouseUp()
    {
        if (Input.mousePosition.x < Screen.width - Input.mousePosition.x)
        {
            if (Input.mousePosition.x < Input.mousePosition.y && Input.mousePosition.x < Screen.height - Input.mousePosition.y)
            {
                rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, BUTTON_WIDTH);
                return;
            }
        }
        else
        {
            if (Screen.width - Input.mousePosition.x < Input.mousePosition.x && Screen.width - Input.mousePosition.x < Screen.height - Input.mousePosition.y)
            {
                rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, BUTTON_WIDTH);
                return;
            }
        }

        if (Input.mousePosition.y < Screen.height - Input.mousePosition.y)
        {
            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, BUTTON_HEIGHT);
        }
        else
            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, BUTTON_HEIGHT);
    }
}
