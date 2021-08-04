using System;
using UnityEngine;
using UnityEngine.UI;

namespace TestingApp.Scripts.Classes.Graph
{
    [Serializable]
    public class DataPointObj
    {
        public Sprite sprite;
        public Vector2 size = new Vector2(15f, 15f);

        [HideInInspector]
        public GameObject gameObj;
        [HideInInspector]
        public RectTransform rectTransform;
        [HideInInspector]
        public Vector2 position;

        public GameObject CreateDataPoint(RectTransform container)
        {
            gameObj = new GameObject("Data", typeof(Image));
            gameObj.transform.SetParent(container, false);

            if (gameObj.TryGetComponent(out Image image))
            {
                image.sprite = sprite;
            }
            else
            {
                Debug.LogError("DataPointObj::CreateDataPoint()::Image is NULL.");
            }

            if (gameObj.TryGetComponent(out RectTransform rect))
            {
                rectTransform = rect;
                rectTransform.anchoredPosition = position;
                rectTransform.sizeDelta = size;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;

                return gameObj;
            }
            else
            {
                Debug.LogError("DataPointObj::CreateDataPoint()::RectTransform is NULL.");

                return null;
            }
        }
    }
}

