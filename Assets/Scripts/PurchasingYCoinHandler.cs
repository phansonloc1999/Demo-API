using UnityEngine;
using UnityEngine.UI;

public class PurchasingYCoinHandler : MonoBehaviour
{
    public Sprite selectedSprite;
    public Sprite deselectedSprite;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void onSelect()
    {
        GetComponent<Image>().sprite = selectedSprite;
    }

    public void onDeselect()
    {
        GetComponent<Image>().sprite = deselectedSprite;
    }
}
