using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DebugPanel.Core;
using DebugPanel.DebugActions.Containers;
using DebugPanel.Examples;
using DebugPanel.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugPanelGameExample : MonoBehaviour
{
    [Header("References")]
    public Camera Camera;
    public DebugPanelExampleField Field;
    public DebugPanelExamplePlayer PlayerLeft;
    public DebugPanelExamplePlayer PlayerRight;
    public DebugPanelExampleBall Ball;
    public TextMeshProUGUI ScoreLabel;
    public Button OpenPanelButton;

    [Header("Values")] 
    [Min(0)] public Vector2 CameraBounds = Vector2.one;
    public float CameraZoomMultiplier = 1f;

    int _scoreLeft;
    int _scoreRight;
    bool _paused;
    float _timeScale = 1f;
    readonly List<DebugPanelExampleBall> _balls = new();
    
    void Start()
    {
        OpenPanelButton.onClick.AddListener(UDebugPanel.Toggle);
        
        UDebugPanel.SetupToggleInput();
        UDebugPanel.Show();
        
        SetupDebugOptions();

        SetupPlayers();
        SetupBall(Ball);
        RefreshScoreUi();
    }
    
    void Update()
    {
        UpdateCameraOrthSize();
    }

    void SetupDebugOptions()
    {
        IDebugActionsSection gameplaySection = UDebugPanel.AddSection("Gameplay");
        gameplaySection.AddButton("Reset", ResetField, true);
        
        gameplaySection.AddToggle("Paused", v =>
            {
                _paused = v;
                Time.timeScale = _paused ? 0 : _timeScale;
            },
            () => _paused
        );
        
        gameplaySection.AddFloatSelector(
            "Time Scale",
            0.1f,
            v =>
            {
                _timeScale = Mathf.Max(0, v);
                
                if (!_paused)
                {
                    Time.timeScale = _timeScale;
                }
            },
            () => _timeScale
        );

        gameplaySection.AddFloatSelector(
            "Camera Zoom",
            0.1f,
            v => CameraZoomMultiplier = Mathf.Max(0.1f, v), () => CameraZoomMultiplier
        );

        gameplaySection.AddToggle("Left Player Ia", v => PlayerLeft.IsIa = v, () => PlayerLeft.IsIa);
        gameplaySection.AddToggle("Right Player Ia", v => PlayerRight.IsIa = v, () => PlayerRight.IsIa);
        gameplaySection.AddFloatSelector("Ball Speed", v => Ball.Speed = v, () => Ball.Speed);
        gameplaySection.AddButton("Add Ball", () =>
        {
            DebugPanelExampleBall ball = Instantiate(Ball);
            ball.name += $" {_balls.Count}";
            ball.Reset();
            _balls.Add(ball);
            SetupBall(ball);
        });
        gameplaySection.AddButtonListInput(
            "Destroy Ball",
            () => _balls.Select(b => b.name).ToList(),
            i =>
            {
                DebugPanelExampleBall ball = _balls[i];
                Destroy(ball.gameObject);
                _balls.RemoveAt(i);
            }
        );
        
        IDebugActionsSection colorsSection = UDebugPanel.AddSection("Colors");
        colorsSection.AddButtonEnumSelector("Field Color", v => Field.SetColor(v), () => Field.BackgroundColor);
        colorsSection.AddButtonEnumSelector("Left Player Color", v => PlayerLeft.SetColor(v), () => PlayerLeft.Color);
        colorsSection.AddButtonEnumSelector("Right Player Color", v => PlayerRight.SetColor(v), () => PlayerRight.Color);
        
        IDebugActionsSection specsSection = UDebugPanel.AddSection("Specs");
        specsSection.AddInfo($"Device Name: {SystemInfo.deviceName}");
        specsSection.AddInfo($"Device Model: {SystemInfo.deviceModel}");
        specsSection.AddInfo($"Operating System: {SystemInfo.operatingSystem}");
        specsSection.AddInfo($"Processor: {SystemInfo.processorType}");
        specsSection.AddInfo($"Processor Cores: {SystemInfo.processorCount}");
        specsSection.AddInfo($"System Memory: {SystemInfo.systemMemorySize} MB");
        specsSection.AddInfo($"Graphics Device: {SystemInfo.graphicsDeviceName}");
        specsSection.AddInfo($"Graphics Vendor: {SystemInfo.graphicsDeviceVendor}");
        specsSection.AddInfo($"Graphics Memory: {SystemInfo.graphicsMemorySize} MB");
        specsSection.AddInfo($"Graphics Shader Level: {SystemInfo.graphicsShaderLevel}");
        specsSection.AddInfo($"Supports Compute Shaders: {SystemInfo.supportsComputeShaders}");
        specsSection.AddButtonLargeInfo("Show Screen Info", () =>
        {
            StringBuilder builder = new();
            builder.AppendLine($"- Screen Resolution: <b>{Screen.currentResolution}</b>");
            builder.AppendLine($"- DPI: <b>{Screen.dpi}</b>");
            builder.AppendLine($"- Full Screen Mode: <b>{Screen.fullScreenMode}</b>");
            return builder.ToString();
        });
    }

    IReadOnlyList<DebugPanelExampleBall> GetBalls()
    {
        List<DebugPanelExampleBall> balls = new(_balls) { Ball };
        return balls;
    }

    void SetupPlayers()
    {
        PlayerLeft.Setup(GetBalls);
        PlayerRight.Setup(GetBalls);
    }
    
    void SetupBall(DebugPanelExampleBall ball)
    {
        ball.OnGoal += () => WhenGoal(ball);
    }

    void UpdateCameraOrthSize()
    {
        float screenAspect = (float)Screen.width / Screen.height;
        
        float targetHeight = CameraBounds.y * 0.5f;
        float targetWidth = CameraBounds.x / (2 * screenAspect);
        
        Camera.orthographicSize = Mathf.Max(targetHeight, targetWidth) * CameraZoomMultiplier;
    }
    
    void ResetField()
    {
        _scoreRight = 0;
        _scoreLeft = 0;
        RefreshScoreUi();
        
        PlayerLeft.transform.localPosition = new Vector2(PlayerLeft.transform.localPosition.x, 0);
        PlayerRight.transform.localPosition = new Vector2(PlayerRight.transform.localPosition.x, 0);
        
        Ball.Reset();

        for (int i = _balls.Count - 1; i >= 0; i--)
        {
            Destroy(_balls[i].gameObject);
        }
        
        _balls.Clear();
    }

    void WhenGoal(DebugPanelExampleBall ball)
    {
        bool isLeft = ball.transform.position.x < 0;

        if (isLeft)
        {
            _scoreRight += 1;
        }
        else
        {
            _scoreLeft += 1;
        }

        RefreshScoreUi();
        
        if (ball == Ball)
        {
            Ball.Reset();
        }
        else
        {
            _balls.Remove(ball);
            Destroy(ball.gameObject);
        }
    }

    void RefreshScoreUi()
    {
        ScoreLabel.text = $"{_scoreLeft} | {_scoreRight}";
    }
}
