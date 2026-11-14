#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace DebugPanel.Datas
{
    public sealed class PopupsData
    {
        public readonly List<GameObject> PopupsStack = new();
        public GameObject? PreviouslySelected;
    }
}