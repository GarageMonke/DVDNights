using CorePatterns.ServiceLocator;
using DG.Tweening;
using DVDNights;
using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour, IScoreController
{
    [Header("References")]
    [SerializeField] private TMP_Text scoreText;
    
    private int _score;
    private int _visualScore;
    private int _borderScoreBonus = 10;
    private int _cornerScoreBonus = 50;

    private Tweener _scoreTween;
    private float _refreshTimer;
    private float _refreshRate = 10;

    private void Awake()
    {
        InstallService();
        scoreText.text = "POINTS: " + _visualScore.ToString("D10");
    }

    private void InstallService()
    {
        ServiceLocator.RegisterService<IScoreController>(this);   
    }


    private void HandleBorderHit(DiskDataSO diskData)
    {
        int amount = _borderScoreBonus * diskData.DiskMultiplier;
        _score += amount;
        TweenToScore();
    }

    private void HandleCornerHit(DiskDataSO diskData)
    {
        int amount = _cornerScoreBonus * diskData.DiskMultiplier;
        _score += amount;
        TweenToScore();
    }

    private void TweenToScore()
    {
        _scoreTween?.Kill();
    
        _scoreTween = DOTween.To(() => _visualScore, x => _visualScore = x, _score, 0.5f)
            .SetEase(Ease.OutExpo) // Sharp start, slow end
            .OnUpdate(() =>
            {
                _refreshTimer += Time.deltaTime;
            
                if (_refreshTimer >= _refreshRate)
                {
                    _refreshTimer = 0;
                    scoreText.text = "POINTS: " + _visualScore.ToString("D10");
                }
            })
            .OnComplete(() => {
                scoreText.text = "POINTS: " + _score.ToString("D10");
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

    private void UpdateScoreText()
    {
        scoreText.text = "POINTS: " + _score;
    }
}

public interface IScoreController
{
    public void RegisterBouncingDisk(IBouncerDisk bouncerDisk);
    public void UnregisterBouncingDisk(IBouncerDisk bouncerDisk);
    public void AddToBorderScoreBonus(int amountToAdd);
    public void AddToCornerScoreBonus(int amountToAdd);
}