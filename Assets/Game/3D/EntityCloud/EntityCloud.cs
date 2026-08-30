using UnityEngine;

public class EntityCloud : MonoBehaviour
{
    [Header("Position")]
    [SerializeField] private float positionAmount = 0.05f;

    [Header("Rotation")]
    [SerializeField] private float rotationAmount = 6f;

    [Header("Movement Speed")]
    [SerializeField] private float movementSpeed = 12f;

    [Header("Terror")]
    [SerializeField] private float twitchFrequency = 3f;
    [SerializeField] private float twitchIntensity = 1f;

    [Header("Sudden Movements")]
    [SerializeField] private float suddenMovementChance = 0.08f;
    [SerializeField] private float suddenMovementAmount = 2f;

    [Header("Nervous Vibration")]
    [SerializeField] private float vibrationPosition = 0.008f;
    [SerializeField] private float vibrationRotation = 1.5f;
    [SerializeField] private float vibrationSpeed = 35f;
    [SerializeField] private float vibrationRandomness = 0.7f;

    [Header("Glitch")]
    [SerializeField] private float glitchChance = 0.08f;
    [SerializeField] private float glitchPosition = 0.04f;
    [SerializeField] private float glitchRotation = 8f;
    [SerializeField] private float glitchDuration = 0.06f;

    [Header("Violent Side Rotation")]
    [SerializeField] private float violentRotationChance = 0.04f;
    [SerializeField] private float violentRotationAmount = 25f;
    [SerializeField] private float violentRotationDuration = 1f;
    [SerializeField] private float violentRotationSpeed = 35f;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private float twitchTimer;
    private float vibrationSeed;

    private float glitchTimer;
    private Vector3 glitchPositionOffset;
    private Vector3 glitchRotationOffset;

    private float violentRotationTimer;
    private bool violentRotationActive;
    private float violentRotationStartTime;
    private float violentRotationDirection;


    private void Start()
    {
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;

        targetPosition = startPosition;
        targetRotation = startRotation;

        twitchTimer = 0f;

        vibrationSeed = Random.Range(0f, 1000f);
    }


    private void Update()
    {
        // =========================================================
        // MOVIMIENTO PRINCIPAL
        // =========================================================

        twitchTimer -= Time.deltaTime;

        if (twitchTimer <= 0f)
        {
            CreateTwitch();

            twitchTimer = Random.Range(
                1f / twitchFrequency,
                1f / (twitchFrequency * 0.35f)
            );
        }

        Vector3 basePosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            movementSpeed * Time.deltaTime
        );

        Quaternion baseRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            movementSpeed * Time.deltaTime
        );


        // =========================================================
        // VIBRACIÓN NERVIOSA
        // =========================================================

        float time = Time.time * vibrationSpeed;

        Vector3 nervousPosition = new Vector3(
            Mathf.PerlinNoise(time, vibrationSeed) - 0.5f,
            Mathf.PerlinNoise(time + 100f, vibrationSeed) - 0.5f,
            Mathf.PerlinNoise(time + 200f, vibrationSeed) - 0.5f
        );

        Vector3 nervousRotation = new Vector3(
            Mathf.PerlinNoise(time + 300f, vibrationSeed) - 0.5f,
            Mathf.PerlinNoise(time + 400f, vibrationSeed) - 0.5f,
            Mathf.PerlinNoise(time + 500f, vibrationSeed) - 0.5f
        );

        float randomIntensity = Mathf.Lerp(
            1f - vibrationRandomness,
            1f,
            Mathf.PerlinNoise(Time.time * 2f, vibrationSeed)
        );

        nervousPosition *= vibrationPosition * randomIntensity;
        nervousRotation *= vibrationRotation * randomIntensity;


        // =========================================================
        // GLITCH
        // =========================================================

        glitchTimer -= Time.deltaTime;

        if (glitchTimer <= 0f)
        {
            if (Random.value < glitchChance * Time.deltaTime * 60f)
            {
                CreateGlitch();
            }
        }

        if (glitchTimer > 0f)
        {
            glitchTimer -= Time.deltaTime;
        }
        else
        {
            glitchPositionOffset = Vector3.Lerp(
                glitchPositionOffset,
                Vector3.zero,
                25f * Time.deltaTime
            );

            glitchRotationOffset = Vector3.Lerp(
                glitchRotationOffset,
                Vector3.zero,
                25f * Time.deltaTime
            );
        }


        // =========================================================
        // ROTACIÓN VIOLENTA DE LADO A LADO
        // =========================================================

        violentRotationTimer -= Time.deltaTime;

        // Comprobar si comienza una nueva sacudida
        if (!violentRotationActive && Random.value <
            violentRotationChance * Time.deltaTime * 60f)
        {
            StartViolentRotation();
        }

        float violentRotation = 0f;

        if (violentRotationActive)
        {
            float elapsed =
                Time.time - violentRotationStartTime;

            if (elapsed < violentRotationDuration)
            {
                // Oscilación extremadamente rápida
                violentRotation =
                    Mathf.Sin(elapsed * violentRotationSpeed) *
                    violentRotationAmount *
                    violentRotationDirection;
            }
            else
            {
                violentRotationActive = false;
            }
        }


        // =========================================================
        // APLICAR TODO
        // =========================================================

        transform.localPosition =
            basePosition +
            nervousPosition +
            glitchPositionOffset;

        transform.localRotation =
            baseRotation *
            Quaternion.Euler(
                nervousRotation +
                glitchRotationOffset +
                new Vector3(0f, 0f, violentRotation)
            );
    }


    // =============================================================
    // MOVIMIENTO PRINCIPAL
    // =============================================================

    private void CreateTwitch()
    {
        float amount = positionAmount * twitchIntensity;

        Vector3 randomPosition = new Vector3(
            Random.Range(-amount, amount),
            Random.Range(-amount, amount),
            Random.Range(-amount, amount)
        );

        float rotation = rotationAmount * twitchIntensity;

        Vector3 randomRotation = new Vector3(
            Random.Range(-rotation, rotation),
            Random.Range(-rotation, rotation),
            Random.Range(-rotation, rotation)
        );

        if (Random.value < suddenMovementChance)
        {
            randomPosition *= suddenMovementAmount;
            randomRotation *= suddenMovementAmount;
        }

        targetPosition = startPosition + randomPosition;

        targetRotation =
            startRotation *
            Quaternion.Euler(randomRotation);
    }


    // =============================================================
    // GLITCH
    // =============================================================

    private void CreateGlitch()
    {
        glitchTimer = glitchDuration;

        glitchPositionOffset = new Vector3(
            Random.Range(-glitchPosition, glitchPosition),
            Random.Range(-glitchPosition, glitchPosition),
            Random.Range(-glitchPosition, glitchPosition)
        );

        glitchRotationOffset = new Vector3(
            Random.Range(-glitchRotation, glitchRotation),
            Random.Range(-glitchRotation, glitchRotation),
            Random.Range(-glitchRotation, glitchRotation)
        );

        if (Random.value < 0.15f)
        {
            glitchPositionOffset *= 2f;
            glitchRotationOffset *= 2f;
        }
    }


    // =============================================================
    // ROTACIÓN VIOLENTA
    // =============================================================

    private void StartViolentRotation()
    {
        violentRotationActive = true;

        violentRotationStartTime = Time.time;

        violentRotationDirection =
            Random.value > 0.5f ? 1f : -1f;
    }
}