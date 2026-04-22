using CorePatterns.ServiceLocator;
using DVDNights;
using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour, IScoreController
{
    [Header("References")]
    [SerializeField] private TMP_Text scoreText;
    
    private int _score;
    private int _borderScoreBonus = 1;
    private int _cornerScoreBonus = 5;

    private void Awake()
    {
        InstallService();
    }

    private void InstallService()
    {
        ServiceLocator.RegisterService<IScoreController>(this);   
    }


    private void HandleBorderHit(DiskDataSO diskData)
    {
        _score += _borderScoreBonus * diskData.DiskMultiplier;
        UpdateScoreText();
    }

    private void HandleCornerHit(DiskDataSO diskData)
    {
        _score += _cornerScoreBonus * diskData.DiskMultiplier;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "POINTS: " + _score;
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
}

public interface IScoreController
{
    public void RegisterBouncingDisk(IBouncerDisk bouncerDisk);
    public void UnregisterBouncingDisk(IBouncerDisk bouncerDisk);
    public void AddToBorderScoreBonus(int amountToAdd);
    public void AddToCornerScoreBonus(int amountToAdd);
}