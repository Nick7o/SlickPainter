using UnityEngine;
using UnityEngine.UI;
using Naspey.LogoMaker;

[RequireComponent(typeof(Button))]
public class LMStamp : LMBaseComponent
{
    [SerializeField] Image colorImage;

    private void Start()
    {
        Debug.Assert(colorImage != null, "LogoMaker :: Stamp image is not assigned.", this);

        GetComponent<Button>().onClick.AddListener(
            delegate ()
            {
                foreach (var canvas in affectedCanvas)
                {
                    StampBrush brush;
                    if (canvas.Brush is StampBrush == false)
                        brush = new StampBrush(canvas.Brush.Size, canvas.Brush.Hardness, colorImage.sprite);
                    else
                    {
                        brush = (StampBrush)canvas.Brush;
                        brush.Stamp = colorImage.sprite;
                    }

                    canvas.Brush = brush;
                }
            });
    }
}
