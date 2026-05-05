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

    private Tweener _pointsTween;
    private float _refreshTimer;
    private float _refreshRate = 10;
    private IDiskLevelController _diskLevelController;


    public Action<int> OnScoreChanged { get; set; }
    
    private void Awake()
    {
        InstallService();
        UpdatePointsText();
    }

    private void InstallService()
    {
        ServiceLocator.RegisterService<IPointsController>(this);   
    }

    private void Start()
    {
        _diskLevelController = ServiceLocator.GetService<IDiskLevelController>();
        UpdatePoints(int.MaxValue - 10000);
    }

    private void HandleBorderHit(DiskDataSO diskData)
    {
        int amountToAdd = GetBorderPoints(diskData);
        _points += amountToAdd;
        TweenToScore();
    }

    private void HandleCornerHit(DiskDataSO diskData)
    {
        int amountToAdd = GetCornerPoints(diskData);
        _points += amountToAdd;
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

    public void UpdatePoints(int updatedPoints)
    {
        _points = updatedPoints;
        _visualPoints = updatedPoints;
        UpdatePointsText();
    }

    public int GetTotalPoints()
    {
        return _points;
    }

    public int GetVisualPoints()
    {
        return _visualPoints;
    }

    public int GetBorderPoints(DiskDataSO diskData)
    {
        int diskTier = (int) diskData.DiskType;
        double amountToAdd = GameProgression.DiscBaseBorderPoints * GameProgression.GetTierExtraMult(diskTier) * GameProgression.GetBorderBonusMult(_diskLevelController.DiskBorderBonusLevel) * diskData.DiskMultiplier + GameProgression.GetTierExtraPoints(diskTier);
        return (int)amountToAdd;
    }

    public int GetCornerPoints(DiskDataSO diskData)
    {
        int diskTier = (int) diskData.DiskType;
        double amountToAdd = GameProgression.DiscBaseCornerPoints * GameProgression.GetTierExtraMult(diskTier) * GameProgression.GetCornerBonusMult(_diskLevelController.DiskCornerBonusLevel + 1) * diskData.DiskMultiplier + GameProgression.GetTierExtraPoints(diskTier);
        return (int) amountToAdd;
    }

    private void UpdatePointsText()
    {
        scoreText.text = "POINTS: " + _visualPoints.ToString("D10");
    }
}

public interface IPointsController
{
    public Action<int> OnScoreChanged { get; set; }
    public void RegisterBouncingDisk(IBouncerDisk bouncerDisk);
    public void UnregisterBouncingDisk(IBouncerDisk bouncerDisk);
    public void UpdatePoints(int updatedPoints);
    public int GetTotalPoints();
    public int GetVisualPoints();
    public int GetBorderPoints(DiskDataSO diskData);
    public int GetCornerPoints(DiskDataSO diskData);
}