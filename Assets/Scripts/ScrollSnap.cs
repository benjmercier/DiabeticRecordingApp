using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollSnap : MonoBehaviour
{
    public RectTransform panel;

    public Button[] bttn;

    public RectTransform center;

    private float[] _distance;
    public float[] _distanceRep;
    private bool _dragging = false;
    private int _bttnDistance;
    private int _minBttnNum;
    private int _buttonLength;

    private void Start()
    {
        _buttonLength = bttn.Length;
        _distance = new float[_buttonLength];
        _distanceRep = new float[_buttonLength];

        _bttnDistance = (int)Mathf.Abs(bttn[1].GetComponent<RectTransform>().anchoredPosition.y - bttn[0].GetComponent<RectTransform>().anchoredPosition.y);
    }

    private void Update()
    {
        CalculateButtonDistance();
    }

    private void CalculateButtonDistance()
    {
        for (int i = 0; i < bttn.Length; i++)
        {
            _distanceRep[i] = center.GetComponent<RectTransform>().position.y - bttn[i].GetComponent<RectTransform>().position.y;
            _distance[i] = Mathf.Abs(_distanceRep[i]);

            if (_distanceRep[i] < -250)
            {
                float curX = bttn[i].GetComponent<RectTransform>().anchoredPosition.x;
                float curY = bttn[i].GetComponent<RectTransform>().anchoredPosition.y;

                Vector2 anchorPos = new Vector2(curX, curY - (_buttonLength * _bttnDistance));

                bttn[i].GetComponent<RectTransform>().anchoredPosition = anchorPos;
            }
            else if (_distanceRep[i] > 250)
            {
                float curX = bttn[i].GetComponent<RectTransform>().anchoredPosition.x;
                float curY = bttn[i].GetComponent<RectTransform>().anchoredPosition.y;

                Vector2 anchorPos = new Vector2(curX, curY + (_buttonLength * _bttnDistance));

                bttn[i].GetComponent<RectTransform>().anchoredPosition = anchorPos;
            }
        }

        float minDistance = Mathf.Min(_distance);

        for (int a = 0; a < bttn.Length; a++)
        {
            if (minDistance == _distance[a])
            {
                _minBttnNum = a;  // center pos = bttn[minButtNum].name
            }
        }

        if (!_dragging)
        {
            LerpToButton(-bttn[_minBttnNum].GetComponent<RectTransform>().anchoredPosition.y);
        }
    }

    private void LerpToButton(float pos)
    {
        float newY = Mathf.Lerp(panel.anchoredPosition.y, pos, Time.deltaTime * 10f);
        Vector2 newPos = new Vector2(panel.anchoredPosition.x, newY);

        panel.anchoredPosition = newPos;
    }

    public void StartScroll()
    {
        _dragging = true;
    }

    public void EndScroll()
    {
        _dragging = false;
    }
}
