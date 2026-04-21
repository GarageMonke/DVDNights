using DVDNights;
using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DVDLogoBouncer dvdLogoBouncer;
    [SerializeField] private TMP_Text scoreText;
    private int _score;

    private void OnEnable()
    {
        dvdLogoBouncer.OnBorderHit += HandleBorderHit;
        dvdLogoBouncer.OnCornerHit += HandleCornerHit;
    }

    private void OnDisable()
    {
        dvdLogoBouncer.OnBorderHit -= HandleBorderHit;
        dvdLogoBouncer.OnCornerHit -= HandleCornerHit;
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