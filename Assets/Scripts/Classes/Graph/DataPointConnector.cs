using System;
using UnityEngine;
using UnityEngine.UI;

namespace TestingApp.Scripts.Classes.Graph
{
    [Serializable]
    public class DataPointConnector
    {
        public Color color = new Color(0f, 0f, 0f, 0.25f);

        [HideInInspector]
        public GameObject gameObj;
        [HideInInspector]
        public RectTransform rectTransform;
        [HideInInspector]
        public Vector2 pointA,
            pointB,
            direction;
        [HideInInspector]
        public float distance;
        [HideInInspector]
        public float angle;

        public GameObject CreateDataConnector(RectTransform container, float width)
        {
            gameObj = new GameObject("Connection", typeof(Image));
            gameObj.transform.SetParent(container, false);

            if (gameObj.TryGetComponent(out Image image))
            {
                image.color = color;
            }
            else
            {
                Debug.LogError("GraphingTab::CreateDataConnector()::Image is NULL.");
            }

            direction = (pointB - pointA).normalized;
            distance = Vector2.Distance(pointA, pointB);
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (gameObj.TryGetComponent(out RectTransform rect))
            {
                rectTransform = rect;
                rectTransform.anchoredPosition = pointA + direction * distance * 0.5f;
                rectTransform.sizeDelta = new Vector2(distance, width);
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;
                rectTransform.localEulerAngles = new Vector3(0f, 0f, angle);

                return gameObj;
            }
            else
            {
                Debug.LogError("GraphingTab::CreateDataConnector()::RectTransform is NULL.");

                return null;
            }
        }
    }
}

