using DVDNights;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public DVDLogoBouncer bouncer;
    private int _score;

    private void OnEnable()
    {
        bouncer.OnBorderHit += HandleBorderHit;
        bouncer.OnCornerHit += HandleCornerHit;
    }

    private void OnDisable()
    {
        bouncer.OnBorderHit -= HandleBorderHit;
        bouncer.OnCornerHit -= HandleCornerHit;
    }

    private void HandleBorderHit()
    {
        _score += 1;
        Debug.Log($"Border hit — Score: {_score}");
    }

    private void HandleCornerHit(CornerTarget corner)
    {
        _score += 5;
        Debug.Log($"Corner hit: {corner} — Score: {_score}");
    }
}