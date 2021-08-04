using System;
using System.Collections.Generic;
using UnityEngine;

namespace TestingApp.Scripts.Classes.Graph
{
    [Serializable]
    public class YAxisLineGraph
    {
        public int labelCount = 10;
        [HideInInspector]
        public int tempLabelCount;
        public float gridlineWidth = 2f;
        public float edgeBuffer = 0.2f;

        [HideInInspector]
        public int currentValue;

        public float defaultMinValue = 75;
        public float defaultMaxValue = 150;

        [HideInInspector]
        public float minValue,
            maxValue,
            valueRange;

        [HideInInspector]
        public float labelPosNormal,
            labelPos;

        [HideInInspector]
        public RectTransform labelRect,
            gridlineRect;

        [HideInInspector]
        public float graphPos;

        public void SetYAxisScale(List<DataPoint> dataSeries)
        {
            if (dataSeries.Count > 0)
            {
                minValue = dataSeries[0].glucoseLevel;
                maxValue = dataSeries[0].glucoseLevel;

                for (int i = 0; i < dataSeries.Count; i++)
                {
                    currentValue = dataSeries[i].glucoseLevel;

                    if (currentValue < minValue)
                    {
                        minValue = currentValue;
                    }

                    if (currentValue > maxValue)
                    {
                        maxValue = currentValue;
                    }
                }

                valueRange = maxValue - minValue;

                if (valueRange <= 0)
                {
                    valueRange = 1f;
                }

                minValue -= (valueRange * edgeBuffer);
                maxValue += (valueRange * edgeBuffer);
            }
            else
            {
                minValue = defaultMinValue;
                maxValue = defaultMaxValue;
            }
        }
    }
}

