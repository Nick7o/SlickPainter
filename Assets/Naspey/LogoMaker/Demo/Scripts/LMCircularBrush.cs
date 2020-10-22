using UnityEngine;
using UnityEngine.UI;
using Naspey.LogoMaker;

[RequireComponent(typeof(Button))]
public class LMCircularBrush : LMBaseComponent
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(
            delegate ()
            {
                foreach (var canvas in affectedCanvas)
                {
                    if (canvas.Brush is CircleBrush == false)
                        canvas.Brush = new CircleBrush(canvas.Brush.Size, canvas.Brush.Hardness);
                }
            });
    }
}
