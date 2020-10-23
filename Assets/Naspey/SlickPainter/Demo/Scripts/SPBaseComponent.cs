using System.Collections.Generic;
using UnityEngine;

namespace Naspey.SlickPainter.Demo
{
    public class SPBaseComponent : MonoBehaviour
    {
        [SerializeField] protected List<SlickPainter> affectedCanvas = new List<SlickPainter>();

        public List<SlickPainter> AffectedCanvas => affectedCanvas;
    }
}