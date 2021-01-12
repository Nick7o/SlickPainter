using UnityEngine;
using UnityEngine.UI;
using Naspey.SlickPainter;

namespace Naspey.SlickPainter.Demo
{
    [RequireComponent(typeof(Button))]
    public class SPStamp : SPBaseComponent
    {
        [SerializeField]
        private Image _colorImage;

        private void Start()
        {
            Debug.Assert(_colorImage != null, "SlickPainter :: Stamp image is not assigned.", this);

            GetComponent<Button>().onClick.AddListener(
                delegate ()
                {
                    foreach (var canvas in affectedCanvas)
                    {
                        StampBrush brush;
                        if (canvas.Brush is StampBrush == false)
                            brush = new StampBrush(canvas.Brush.Size, canvas.Brush.Hardness, _colorImage.sprite);
                        else
                        {
                            brush = (StampBrush)canvas.Brush;
                            brush.Stamp = _colorImage.sprite;
                        }

                        canvas.Brush = brush;
                    }
                });
        }
    }
}