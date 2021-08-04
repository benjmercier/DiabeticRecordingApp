using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TestingApp.Scripts.Classes.Graph
{
    public class HistoryScrollRect : MonoBehaviour
    {
        public ScrollRect historyScrollRect;
        public RectTransform historyContentRecTransform;
        public GameObject historyPanelPrefab;

        private List<DataPoint> _activeTests = new List<DataPoint>();
        private int _testCount;
        private DateTime _testTimestamp = new DateTime();

        private HistoryPanel _historyPanel;
        private HistoryPanel _tempHistoryPanel;

        private GameObject _newPanel,
            _defaultPanel;

        private List<GameObject> _historyPanelList = new List<GameObject>();

        public void CalculateHistoryToShow()
        {
            foreach (var obj in _historyPanelList)
            {
                Destroy(obj);
            }

            _historyPanelList.Clear();

            _activeTests = DynamoDBManager.Instance.activeDataPoints.OrderByDescending(t => DateTime.Parse(t.testTimestamp)).Reverse().ToList();

            _testCount = 0;

            if (_activeTests.Count > 0)
            {
                for (int i = _activeTests.Count - 1; i >= 0; i--)
                {
                    _testCount = i + 1;
                    _testTimestamp = DateTime.Parse(_activeTests[i].testTimestamp);

                    _newPanel = Instantiate(historyPanelPrefab, transform.position, Quaternion.identity);
                    _newPanel.transform.SetParent(historyContentRecTransform, false);

                    _tempHistoryPanel = GetPanelInfo(_newPanel, i, _testCount, _testTimestamp, false);

                    _historyPanelList.Add(_newPanel);
                }
            }
            else
            {
                _defaultPanel = Instantiate(historyPanelPrefab, transform.position, Quaternion.identity);
                _defaultPanel.transform.SetParent(historyContentRecTransform, false);

                _tempHistoryPanel = GetPanelInfo(_defaultPanel, 0, _testCount, _testTimestamp, true);

                _historyPanelList.Add(_defaultPanel);
            }
        }

        HistoryPanel GetPanelInfo(GameObject panel, int index, int count, DateTime dateTime, bool isDefault)
        {
            if (panel.TryGetComponent(out _historyPanel))
            {
                if (!isDefault)
                {
                    _historyPanel.countTMP.text = "#" + count.ToString();
                    _historyPanel.dateTMP.text = dateTime.ToString("M / d\nyyyy");
                    _historyPanel.timeTMP.text = dateTime.ToString("h:mm\ntt");
                    _historyPanel.glucoseLvlTMP.text = _activeTests[index].glucoseLevel.ToString();
                    _historyPanel.bolusUnitsTMP.text = _activeTests[index].bolusInsulinDose.ToString();
                    _historyPanel.basalUnitsTMP.text = _activeTests[index].basalInsulinDose.ToString();

                    if (!string.IsNullOrEmpty(_activeTests[index].testNotes))
                    {
                        _historyPanel.notesImg.gameObject.SetActive(true);
                    }
                }
                else
                {
                    _historyPanel.countTMP.text = "#";
                    _historyPanel.dateTMP.text = "---";
                    _historyPanel.timeTMP.text = "---";
                    _historyPanel.glucoseLvlTMP.text = "0";
                    _historyPanel.bolusUnitsTMP.text = "0";
                    _historyPanel.basalUnitsTMP.text = "0";
                }

                return _historyPanel;
            }
            else
            {
                Debug.LogError("HistoryScrollRect::GetPanelInfo()::_historyPanel is nULL");

                return null;
            }
        }
    }
}

