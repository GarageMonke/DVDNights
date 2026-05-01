using System;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using DVDNights;
using TMPro;
using UnityEngine;

public class PointsController : MonoBehaviour, IPointsController
{
    [Header("References")]
    [SerializeField] private TMP_Text scoreText;
    
    private int _points;
    private int _visualPoints;
    private int _borderScoreBonus = 1;
    private int _cornerScoreBonus = 5;

    private Tweener _pointsTween;
    private float _refreshTimer;
    private float _refreshRate = 10;
    
    public Action<int> OnScoreChanged { get; set; }
    
    private void Awake()
    {
        InstallService();
        scoreText.text = "POINTS: " + _visualPoints.ToString("D10");
    }

    private void InstallService()
    {
        ServiceLocator.RegisterService<IPointsController>(this);   
    }
    
    private void HandleBorderHit(DiskDataSO diskData)
    {
        int amount = _borderScoreBonus * diskData.DiskMultiplier;
        _points += amount;
        TweenToScore();
    }

    private void HandleCornerHit(DiskDataSO diskData)
    {
        int amount = _cornerScoreBonus * diskData.DiskMultiplier;
        _points += amount;
        TweenToScore();
    }

    private void TweenToScore()
    {
        _pointsTween?.Kill();
    
        _pointsTween = DOTween.To(() => _visualPoints, x => _visualPoints = x, _points, 0.5f)
            .SetEase(Ease.OutExpo)
            .OnUpdate(() =>
            {
                _refreshTimer += Time.deltaTime;
            
                if (_refreshTimer >= _refreshRate)
                {
                    _refreshTimer = 0;
                    scoreText.text = "POINTS: " + _visualPoints.ToString("D10");
                }
            })
            .OnComplete(() => {
                scoreText.text = "POINTS: " + _points.ToString("D10");
                OnScoreChanged?.Invoke(_points);
            });
    }

    public void RegisterBouncingDisk(IBouncerDisk bouncerDisk)
    {
        bouncerDisk.OnBorderHit += HandleBorderHit;
        bouncerDisk.OnCornerHit += HandleCornerHit;
    }

    public void UnregisterBouncingDisk(IBouncerDisk bouncerDisk)
    {
        bouncerDisk.OnBorderHit -= HandleBorderHit;
        bouncerDisk.OnCornerHit -= HandleCornerHit;
    }

    public void AddToBorderScoreBonus(int amountToAdd)
    {
        _borderScoreBonus += amountToAdd;
    }

    public void AddToCornerScoreBonus(int amountToAdd)
    {
        _cornerScoreBonus += amountToAdd;
    }

    public int GetTotalPoints()
    {
        return _points;
    }

    public int GetVisualPoints()
    {
        return _visualPoints;
    }

    private void UpdateScoreText()
    {
        scoreText.text = "POINTS: " + _points;
    }
}

public interface IPointsController
{
    public Action<int> OnScoreChanged { get; set; }
    public void RegisterBouncingDisk(IBouncerDisk bouncerDisk);
    public void UnregisterBouncingDisk(IBouncerDisk bouncerDisk);
    public void AddToBorderScoreBonus(int amountToAdd);
    public void AddToCornerScoreBonus(int amountToAdd);
    public int GetTotalPoints();
    public int GetVisualPoints();
}