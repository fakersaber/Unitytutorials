using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class TestComponent : MonoBehaviour
{

    private Color originColor;
    private Color ChangeColor;
    private Image image;


    private void Awake()
    {
        image = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        originColor = image.color;
        ChangeColor = new Color(originColor.r, originColor.g, originColor.b,0f);
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void Disable()
    {
        image.color = ChangeColor;
    }

    public void Enable()
    {
        image.color = originColor;
    }
}
