using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LMColorButton : LMBaseComponent
{
    [SerializeField] Image colorImage;

    private void Start()
    {
        Debug.Assert(colorImage != null, "LogoMaker :: Color image is not assigned.", this);

        GetComponent<Button>().onClick.AddListener(
            delegate ()
            {
                foreach (var canvas in affectedCanvas)
                {
                    canvas.Color = colorImage.color;
                }
            });
    }
}
