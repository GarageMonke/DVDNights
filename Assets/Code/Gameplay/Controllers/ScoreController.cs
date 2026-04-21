using DVDNights;
using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DVDDiskBouncer dvdDiskBouncer;
    [SerializeField] private TMP_Text scoreText;
    private int _score;

    private void OnEnable()
    {
        dvdDiskBouncer.OnBorderHit += HandleBorderHit;
        dvdDiskBouncer.OnCornerHit += HandleCornerHit;
    }

    private void OnDisable()
    {
        dvdDiskBouncer.OnBorderHit -= HandleBorderHit;
        dvdDiskBouncer.OnCornerHit -= HandleCornerHit;
    }

    private void HandleBorderHit()
    {
        _score += 1;
        UpdateScoreText();
    }

    private void HandleCornerHit(CornerTarget corner)
    {
        _score += 5;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "POINTS: " + _score;
    }
}