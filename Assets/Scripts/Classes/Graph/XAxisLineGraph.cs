using System;
using UnityEngine;

namespace TestingApp.Scripts.Classes.Graph
{
    [Serializable]
    public class XAxisLineGraph
    {
        public int labelCount = 6;
        public float edgeBuffer = 1f;

        public DateTime minDateTime,
            maxDateTime;
        public TimeSpan totalTime;

        [HideInInspector]
        public int labelIndex;

        [HideInInspector]
        public float minLabelPos,
            maxLabelPos,
            minMaxLabelVariance,
            currentLabelSpread,
            currentLabelPos;

        [HideInInspector]
        public RectTransform labelRect,
            gridlineRect;

        [HideInInspector]
        public float graphPos;

        public void SetXAxisScale(DateTime[] timePeriods)
        {
            minDateTime = timePeriods[0];
            maxDateTime = timePeriods[timePeriods.Length - 2].AddMinutes(240);
        }
    }
}

