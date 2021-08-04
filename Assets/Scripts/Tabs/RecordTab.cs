using System;
using UnityEngine;
using UnityEngine.UI;
using DeadMosquito.AndroidGoodies;

public class RecordTab : MonoBehaviour, IPanel
{
    public InputField bloodSugarInput,
        bolusInsulinInput,
        basalInsulinInput,
        notesInput;

    public Text dateButtonText;
    private string _dateToUpload;

    public Text timeButtonText;
    private string _timeToUpload;

    public Text bolusInsulin; // fast (assign through user input?)
    public Text basalInsulin; // long (assign through user input?)

    private int _period;
    
    private string[] _dailyPeriods = new string[]
    {
        "Night",
        "Early Morning",
        "Morning",
        "Afternoon",
        "Evening",
        "Night"
    };

    void OnEnable()
    {
        bloodSugarInput.text = null;
        bolusInsulinInput.text = null;
        basalInsulinInput.text = null;
        notesInput.text = null;

        var today = DateTime.Now;

        dateButtonText.text = today.ToString("M / d / yyyy");
        _dateToUpload = today.ToString("yyyy-MM-dd");

        timeButtonText.text = today.ToString("t");
        _timeToUpload = today.ToString("HH:mm");
    }

    public void ProcessInfo()
    {
        if (string.IsNullOrEmpty(bloodSugarInput.text) || string.IsNullOrEmpty(dateButtonText.text) || string.IsNullOrEmpty(timeButtonText.text))
        {
            // make required input fields red?
            Debug.Log("Required Input is Null.");

            return;
        }
        else
        {
            CalculatePeriodOfDay();

            int bolusDose = 0;
            int basalDose = 0;

            if (!string.IsNullOrEmpty(bolusInsulinInput.text))
            {
                bolusDose = int.Parse(bolusInsulinInput.text);
            }

            if (!string.IsNullOrEmpty(basalInsulinInput.text))
            {
                basalDose = int.Parse(basalInsulinInput.text);
            }

            DynamoDBManager.Instance.currentDataPoint.testTimestamp = _dateToUpload + " " + _timeToUpload;
            DynamoDBManager.Instance.currentDataPoint.dailyPeriod = _dailyPeriods[_period];
            DynamoDBManager.Instance.currentDataPoint.glucoseLevel = int.Parse(bloodSugarInput.text);
            DynamoDBManager.Instance.currentDataPoint.bolusInsulinDose = bolusDose;
            DynamoDBManager.Instance.currentDataPoint.basalInsulinDose = basalDose;
            DynamoDBManager.Instance.currentDataPoint.testNotes = notesInput.text;

            DynamoDBManager.Instance.activeDataPoints.Add(DynamoDBManager.Instance.currentDataPoint);

            DynamoDBManager.Instance.UploadToDynamoDB();
        }
    }
    
    // Date Picker
    public void OnPickDateClick()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return;
        }

        var now = DateTime.Now;

        AndroidDateTimePicker.ShowDatePicker(now.Year, now.Month, now.Day, OnDatePicked, OnDatePickCancel);
    }

    private void OnDatePicked(int year, int month, int day)
    {
        var picked = new DateTime(year, month, day);

        dateButtonText.text = picked.ToString("M / d / yyyy");
        _dateToUpload = picked.ToString("yyyy-MM-dd");
    }

    private void OnDatePickCancel()
    {
        dateButtonText.text = "Date Needed.";
    }

    // Time Picker
    public void OnTimePickerClick()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return;
        }

        var now = DateTime.Now;

        AndroidDateTimePicker.ShowTimePicker(now.Hour, now.Minute, OnTimePicked, OnTimePickedCancel);
    }

    private void OnTimePicked(int hour, int minute)
    {
        var picked = new DateTime(2016, 11, 11, hour, minute, 00);

        timeButtonText.text = picked.ToString("t");
        _timeToUpload = picked.ToString("HH:mm");
    }

    private void OnTimePickedCancel()
    {
        timeButtonText.text = "Time Needed.";
    }

    private void CalculatePeriodOfDay()
    {
        DateTime time = DateTime.Parse(_timeToUpload);

        if (time.Hour >= 0 && time.Hour < 4)
        {
            _period = 0;
        }
        else if (time.Hour >= 4 && time.Hour < 8)
        {
            _period = 1;
        }
        else if (time.Hour >= 8 && time.Hour < 12)
        {
            _period = 2;
        }
        else if (time.Hour >= 12 && time.Hour < 16)
        {
            _period = 3;
        }
        else if (time.Hour >= 16 && time.Hour < 20)
        {
            _period = 4;
        }
        else if (time.Hour >= 20 && time.Hour < 24)
        {
            _period = 5;
        }
    }
}
