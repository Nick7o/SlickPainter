using UnityEngine;
using UnityEngine.UI;

namespace Naspey.SlickPainter.Demo
{
    [RequireComponent(typeof(Button))]
    public class SPColorButton : SPBaseComponent
    {
        [SerializeField] Image colorImage;

        private void Start()
        {
            Debug.Assert(colorImage != null, "SlickPainter :: Color image is not assigned.", this);

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
}