using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DateTimeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _timerSpeedText;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private TextMeshProUGUI _pauseButtonText;
    [SerializeField] private Button _addSpeedButton;
    [SerializeField] private Button _minusSpeedButton;

    private GameTimer _timer;

    private void Start()
    {
        _timer = FindObjectOfType<GameTimer>();
        GameTimer.HourEnd += TimerAnimate;
        _addSpeedButton.onClick.AddListener(delegate
        {
            if(_timer.Speed < 6)
            {
                _timer.SetSpeed(_timer.Speed + 1);
            }
        });
        _minusSpeedButton.onClick.AddListener(delegate
        {
            if (_timer.Speed > 1)
            {
                _timer.SetSpeed(_timer.Speed - 1);
            }
        });
        _pauseButton.onClick.AddListener(delegate 
        {
            _timer.Pause = !_timer.Pause;
            if(_timer.Pause == true)
            {
                _pauseButtonText.text = "Снять с паузы";
            }
            if (_timer.Pause == false) 
            {
                _pauseButtonText.text = "Пауза";
            }
        });
    }

    private void Update()
    {
        _timerSpeedText.text = "Скорость: " + _timer.Speed;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _timer.Pause = !_timer.Pause;
        }

    }
    private void TimerAnimate()
    {
        _timerText.text = _timer.DTime.ToLongDateString() + " " + _timer.DTime.ToShortTimeString();
    }
}
