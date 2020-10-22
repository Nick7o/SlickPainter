using System.Collections.Generic;
using UnityEngine;
using Naspey.LogoMaker;

public class LMBaseComponent : MonoBehaviour
{
    [SerializeField] protected List<LogoMaker> affectedCanvas = new List<LogoMaker>();

    public List<LogoMaker> AffectedCanvas => affectedCanvas;
}
