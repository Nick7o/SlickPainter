using UnityEngine;
using UnityEngine.UI;

namespace Naspey.SlickPainter.Demo
{ 
    [RequireComponent(typeof(Button))]
    public class SPCircularBrush : SPBaseComponent
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
}