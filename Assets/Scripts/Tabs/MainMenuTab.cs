using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TestingApp.Scripts.Panels;

public class MainMenuTab : MonoBehaviour
{
    [Header("Latest Test Panel")]
    public TextMeshProUGUI glucoseLvlDataTMP;
    public TextMeshProUGUI dateDataTMP;
    public TextMeshProUGUI timeDataTMP;

    private DataPoint _latestTest;
    private DateTime _latestDateTime;

    [Header("Data Panel")]
    public TextMeshProUGUI targetRangeDataTMP;
    public TextMeshProUGUI sevenDayAvgDataTMP;
    public TextMeshProUGUI thirtyDayAvgDataTMP;
    public TextMeshProUGUI estA1CDataTMP; // A1c = (46.7 + avg 3 MO blood glucose) / 28.7
    public int minTarget = 80;
    public int maxTarget = 120;
    public int minTestsForA1C = 50;

    private string _targetRange;

    private TimeSpan _sevenDays,
        _thirtyDays,
        _ninetyDays;

    private DateTime _now,
        _sevenDaysFromNow,
        _thirtyDaysFromNow,
        _ninetyDaysFromNow;

    private IEnumerable<DataPoint> _sevenDayData,
        _thirtyDayData,
        _ninetyDayData;

    private double _sevenDayAvg,
        _thirtyDayAvg,
        _ninetyDayAvg,
        _a1C;

    [Header("Insulin Panel")]
    public RectTransform insulinContentRecTransform;
    public GameObject insulinPanelPrefab;

    private List<DataPoint> _todaysTests = new List<DataPoint>();
    private List<DataPoint> _testList = new List<DataPoint>();
    private List<GameObject> _insulinPanelList = new List<GameObject>();

    private GameObject _newPanel,
        _defaultPanel;

    private InsulinPanel _insulinPanel;

    private DateTime _displayTime;

    private void Start()
    {
        CalculateLatestTestPanel();
        CalculateDataPanel();
        CalculateInsulinPanel();
    }

    public void CalculateLatestTestPanel()
    {
        _latestTest = DynamoDBManager.Instance.activeDataPoints.OrderByDescending(t => t.testTimestamp).FirstOrDefault();
        _latestDateTime = DateTime.Parse(_latestTest.testTimestamp);

        glucoseLvlDataTMP.text = _latestTest.glucoseLevel.ToString();
        dateDataTMP.text = _latestDateTime.ToString("M / d / yyyy");
        timeDataTMP.text = _latestDateTime.ToString("t");
    }

    public void CalculateDataPanel()
    {
        _targetRange = minTarget + " - " + maxTarget + " mg/dL";
        targetRangeDataTMP.text = _targetRange;

        _now = DateTime.Now;

        CalculateAvgData(_sevenDays, 7, _sevenDaysFromNow, _sevenDayData, _sevenDayAvg, false); // 7 days
        CalculateAvgData(_thirtyDays, 30, _thirtyDaysFromNow, _thirtyDayData, _thirtyDayAvg, false); // 30 days
        CalculateAvgData(_ninetyDays, 90, _ninetyDaysFromNow, _ninetyDayData, _ninetyDayAvg, true); // 90 days

        sevenDayAvgDataTMP.text = Mathf.Round((float)_sevenDayAvg).ToString();
        thirtyDayAvgDataTMP.text = Mathf.Round((float)_thirtyDayAvg).ToString();
    }

    private void CalculateAvgData(TimeSpan timeSpan, int days, DateTime fromNow, IEnumerable<DataPoint> data, double avg, bool checkA1C)
    {
        timeSpan = TimeSpan.FromDays(days);

        fromNow = _now - timeSpan;

        data = DynamoDBManager.Instance.activeDataPoints.OrderByDescending(t => DateTime.Parse(t.testTimestamp)).Where
            (d => DateTime.Parse(d.testTimestamp) <= _now && DateTime.Parse(d.testTimestamp) > fromNow);

        if (data.Any())
        {
            avg = data.Average(a => a.glucoseLevel);

            if (checkA1C)
            {
                if (data.Count() >= minTestsForA1C)
                {
                    _a1C = (46.7 + _ninetyDayAvg) / 28.7;

                    estA1CDataTMP.text = _a1C.ToString("F1");
                }
                else
                {
                    _a1C = 0;

                    estA1CDataTMP.text = "N/A";
                }
            }
        }
        else
        {
            avg = 0;
        }
    }

    public void CalculateInsulinPanel()
    {
        foreach (var obj in _insulinPanelList)
        {
            Destroy(obj);
        }

        _insulinPanelList.Clear();

        _todaysTests = DynamoDBManager.Instance.activeDataPoints.Where(d => DateTime.Parse(d.testTimestamp).Date == DateTime.Today && (d.basalInsulinDose > 0 || d.bolusInsulinDose > 0)).OrderByDescending
            (t => DateTime.Parse(t.testTimestamp)).Reverse().ToList();

        if (_todaysTests.Count != 0)
        {
            _testList.Clear();

            foreach (var item in _todaysTests)
            {
                _testList.Add(item);
            }

            for (int i = 0; i < _testList.Count; i++)
            {
                GeneratePanel(false, _newPanel, i);
            }
        }
        else
        {
            GeneratePanel(true, _defaultPanel, 0);
        }
    }

    private void GeneratePanel(bool isDefault, GameObject panel, int index)
    {
        panel = Instantiate(insulinPanelPrefab, transform.position, Quaternion.identity);
        panel.transform.SetParent(insulinContentRecTransform, false);

        if (panel.TryGetComponent(out _insulinPanel))
        {
            if (isDefault)
            {
                _insulinPanel.bolusUnitsDataTMP.text = "0";
                _insulinPanel.basalUnitsDataTMP.text = "0";
                _insulinPanel.timeDataTMP.text = "---";
            }
            else
            {
                _displayTime = DateTime.Parse(_testList[index].testTimestamp);

                _insulinPanel.bolusUnitsDataTMP.text = _testList[index].bolusInsulinDose.ToString();
                _insulinPanel.basalUnitsDataTMP.text = _testList[index].basalInsulinDose.ToString();
                _insulinPanel.timeDataTMP.text = _displayTime.ToString("t");
            }
        }
        else
        {
            Debug.LogError("MainMenuTab::CalculateInsulinPanel()::_insulinPanel is NULL.");
        }

        _insulinPanelList.Add(panel);
    }
}
