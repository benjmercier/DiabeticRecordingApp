using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DeadMosquito.AndroidGoodies;
using TestingApp.Scripts.Classes.Graph;

public class GraphingTab : MonoBehaviour
{
    [Header("Graph Objects")]
    [SerializeField]
    private RectTransform _graphContainer;

    [SerializeField]
    private RectTransform _tempLabelX,
        _tempLabelY;

    [SerializeField]
    private RectTransform _tempGridlineX,
        _tempGridlineY;

    public Text dateButtonText;

    private float _graphWidth,
        _graphHeight;

    private GameObject _lastDataPointObj,
        _newDataPointObj,
        _connectorObj;

    private List<DataPoint> _dataToGraphList = new List<DataPoint>();
    private List<DataPoint> _orderedDataSeries = new List<DataPoint>();
    private List<GameObject> _graphedObjList = new List<GameObject>();

    private DateTime _graphDate;
    private DateTime _dataPointDate;

    private DateTime[] _dailyTimePeriods = new DateTime[]
    {
        new DateTime(2001, 01, 01, 0, 0, 0), // Midnight
        new DateTime(2001, 01, 01, 4, 0, 0), // 4 AM
        new DateTime(2001, 01, 01, 8, 0, 0), // 8 AM
        new DateTime(2001, 01, 01, 12, 0, 0), // 12 PM
        new DateTime(2001, 01, 01, 16, 0, 0), // 4 PM
        new DateTime(2001, 01, 01, 20, 0, 0), // 8 PM
        new DateTime(2001, 01, 01, 0, 0, 0) // Midnight
    };

    private DateTime _dataPointTimeStamp;
    private float _dataPointMinutes;
    private float _totalDateMinutes;

    [Header("Data Point Settings")]
    [SerializeField]
    private DataPointObj _dataPointObj;

    [SerializeField]
    private DataPointConnector _dataPointConnector;

    private Vector3 _defaultScale = Vector3.one;

    [Header("Axis Values")]
    [SerializeField]
    private XAxisLineGraph _xAxis;
    [SerializeField]
    private YAxisLineGraph _yAxis;

    private void OnEnable()
    {
        _graphWidth = _graphContainer.sizeDelta.x;
        _graphHeight = _graphContainer.sizeDelta.y;

        _xAxis.SetXAxisScale(_dailyTimePeriods);

        //DateSelection(2021, 04, 26);
        DateSelection(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
    }

    #region Date Picker
    public void OnDatePickerClick()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return;
        }

        AndroidDateTimePicker.ShowDatePicker(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
            DateSelection, CancelDateSelection);
    }

    private void DateSelection(int year, int month, int day)
    {
        _graphDate = new DateTime(year, month, day);

        dateButtonText.text = _graphDate.ToString("M / d / yyyy");

        CompileGraphData(_graphDate);
    }

    private void CancelDateSelection()
    {
        dateButtonText.text = "Date Needed";
    }
    #endregion

    private void CompileGraphData(DateTime graphDate)
    {
        _dataToGraphList.Clear();

        _graphedObjList.ForEach(obj => Destroy(obj));
        _graphedObjList.Clear();

        foreach (var dataPoint in DynamoDBManager.Instance.activeDataPoints)
        {
            _dataPointDate = DateTime.Parse(dataPoint.testTimestamp);

            if (_dataPointDate.Date == graphDate)
            {
                _dataToGraphList.Add(dataPoint);
            }
        }

        OrderDataByAscending(_dataToGraphList);
    }

    private void OrderDataByAscending(List<DataPoint> dataToGraph)
    {
        _orderedDataSeries = dataToGraph.OrderByDescending(data => DateTime.Parse(data.testTimestamp)).Reverse().ToList();

        GraphDataSeries(_orderedDataSeries);
    }

    private void GraphDataSeries(List<DataPoint> dataSeries)
    {
        _yAxis.SetYAxisScale(dataSeries);

        PlotXAxisGrid();
        PlotYAxisGrid();
        PlotDataSeries(dataSeries);
    }

    #region X Axis Labels & Gridlines
    private void PlotXAxisGrid()
    {
        _xAxis.minLabelPos = 0f;
        _xAxis.maxLabelPos = 0f;
        _xAxis.labelCount = _dailyTimePeriods.Length;
        _xAxis.labelIndex = 0;

        for (int i = 0; i < _xAxis.labelCount; i++)
        {
            InstantiateXAxisLabel(i);
            InstantiateXAxisGridline();

            _xAxis.labelIndex++;
        }
    }

    private void InstantiateXAxisLabel(int index)
    {
        _xAxis.currentLabelSpread = _graphWidth / (_xAxis.labelCount + _xAxis.edgeBuffer);
        _xAxis.currentLabelPos = _xAxis.currentLabelSpread + _xAxis.labelIndex * _xAxis.currentLabelSpread;

        if (index == 0)
        {
            _xAxis.minLabelPos = _xAxis.currentLabelPos;
        }
        else if (index == _xAxis.labelCount - 1)
        {
            _xAxis.maxLabelPos = _xAxis.currentLabelPos;
        }

        _xAxis.labelRect = Instantiate(_tempLabelX);
        _xAxis.labelRect.SetParent(_graphContainer);
        _xAxis.labelRect.gameObject.SetActive(true);
        _xAxis.labelRect.anchoredPosition = new Vector2(_xAxis.currentLabelPos, _xAxis.labelRect.position.y);

        if (_xAxis.labelRect.TryGetComponent(out Text text))
        {
            text.text = _dailyTimePeriods[index].ToString("h tt");
        }
        else
        {
            Debug.LogError("GraphingTab::InstantiateXAxisLabel()::Text is NULL.");
        }

        _xAxis.labelRect.localScale = _defaultScale;

        _graphedObjList.Add(_xAxis.labelRect.gameObject);
    }

    private void InstantiateXAxisGridline()
    {
        _xAxis.gridlineRect = Instantiate(_tempGridlineX);
        _xAxis.gridlineRect.SetParent(_graphContainer);
        _xAxis.gridlineRect.gameObject.SetActive(true);
        _xAxis.gridlineRect.anchoredPosition = new Vector2(_xAxis.currentLabelPos, _xAxis.gridlineRect.position.y);
        _xAxis.gridlineRect.localScale = _defaultScale;

        _graphedObjList.Add(_xAxis.gridlineRect.gameObject);
    }
    #endregion

    #region Y Axis Labels & Gridlines
    private void PlotYAxisGrid()
    {
        _yAxis.tempLabelCount = _yAxis.labelCount;

        if (_yAxis.tempLabelCount > _yAxis.valueRange)
        {
            if (_yAxis.valueRange % 2 != 0)
            {
                _yAxis.tempLabelCount = ReturnYAxisLabelCheck((int)_yAxis.valueRange);
            }
            else
            {
                _yAxis.tempLabelCount = (int)_yAxis.valueRange;
            }

            if (_yAxis.valueRange == 1)
            {
                _yAxis.tempLabelCount = Mathf.RoundToInt(_yAxis.valueRange) + 3;
                _yAxis.minValue -= 2;
                _yAxis.maxValue += 2;
            }
        }

        for (int i = 0; i <= _yAxis.tempLabelCount; i++)
        {
            _yAxis.labelPosNormal = (i * 1f) / _yAxis.tempLabelCount;
            _yAxis.labelPos = _yAxis.minValue + (_yAxis.labelPosNormal * (_yAxis.maxValue - _yAxis.minValue));

            InstantiateYAxisLabel();

            if (i != 0 && i != _yAxis.tempLabelCount)
            {
                InstantiateYAxisGridline();
            }
        }
    }

    private int ReturnYAxisLabelCheck(int to)
    {
        return (to % 2 == 0) ? to : (to + 2);
    }

    private void InstantiateYAxisLabel()
    {
        _yAxis.labelRect = Instantiate(_tempLabelY);
        _yAxis.labelRect.SetParent(_graphContainer);
        _yAxis.labelRect.gameObject.SetActive(true);
        _yAxis.labelRect.anchoredPosition = new Vector2(_yAxis.labelRect.position.x, _yAxis.labelPosNormal * _graphHeight);

        if (_yAxis.labelRect.TryGetComponent(out Text text))
        {
            text.text = Mathf.RoundToInt(_yAxis.labelPos).ToString();
        }
        else
        {
            Debug.LogError("GraphingTab::InstantiateYAxisLabel()::Text is NULL.");
        }

        _yAxis.labelRect.localScale = _defaultScale;

        _graphedObjList.Add(_yAxis.labelRect.gameObject);
    }

    private void InstantiateYAxisGridline()
    {
        _yAxis.gridlineRect = Instantiate(_tempGridlineY);
        _yAxis.gridlineRect.SetParent(_graphContainer);
        _yAxis.gridlineRect.gameObject.SetActive(true);
        _yAxis.gridlineRect.anchoredPosition = new Vector2(_yAxis.gridlineRect.position.x, _yAxis.labelPosNormal * _graphHeight);
        _yAxis.gridlineRect.localScale = _defaultScale;

        _graphedObjList.Add(_yAxis.gridlineRect.gameObject);
    }
    #endregion

    private void PlotDataSeries(List<DataPoint> dataSeries)
    {
        _lastDataPointObj = null;

        _xAxis.totalTime = TimeSpan.FromTicks(_xAxis.maxDateTime.Ticks - _xAxis.minDateTime.Ticks);

        for (int i = 0; i < dataSeries.Count; i++)
        {
            _dataPointTimeStamp = DateTime.Parse(dataSeries[i].testTimestamp);
            _dataPointMinutes = (float)(_dataPointTimeStamp.TimeOfDay.TotalMinutes - _xAxis.minDateTime.TimeOfDay.TotalMinutes);
            _totalDateMinutes = (float)_xAxis.totalTime.TotalMinutes;

            _xAxis.minMaxLabelVariance = _xAxis.maxLabelPos - _xAxis.minLabelPos;

            _xAxis.graphPos = (_dataPointMinutes / _totalDateMinutes) * _xAxis.minMaxLabelVariance + _xAxis.minLabelPos;
            _yAxis.graphPos = ((dataSeries[i].glucoseLevel - _yAxis.minValue) / (_yAxis.maxValue - _yAxis.minValue)) * _graphHeight;
            
            _dataPointObj.position = new Vector2(_xAxis.graphPos, _yAxis.graphPos);

            _newDataPointObj = _dataPointObj.CreateDataPoint(_graphContainer);

            _graphedObjList.Add(_newDataPointObj);

            if (_lastDataPointObj != null)
            {
                if (_lastDataPointObj.TryGetComponent(out RectTransform rectTransformA))
                {
                    _dataPointConnector.pointA = rectTransformA.anchoredPosition;
                }
                else
                {
                    Debug.LogError("GraphingTab::PlotDataSeries()::LastDataPoint RectTransform is NULL.");
                }

                if (_newDataPointObj.TryGetComponent(out RectTransform rectTransformB))
                {
                    _dataPointConnector.pointB = rectTransformB.anchoredPosition;
                }
                else
                {
                    Debug.LogError("GraphingTab::PlotDataSeries()::NewDataPoint RectTransform is NULL.");
                }

                _connectorObj = _dataPointConnector.CreateDataConnector(_graphContainer, _yAxis.gridlineWidth);

                _graphedObjList.Add(_connectorObj);
            }

            _lastDataPointObj = _newDataPointObj;
        }
    }
}