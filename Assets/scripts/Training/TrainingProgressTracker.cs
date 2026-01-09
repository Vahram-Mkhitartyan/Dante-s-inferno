using UnityEngine;

public class TrainingProgressTracker : MonoBehaviour
{
    [Header("References")]
    public TrainingPlotManager plot;

    // =====================
    // MOVEMENT TRACKING
    // =====================
    private bool movedLeft;
    private bool movedRight;
    private bool jumped;

    // =====================
    // ATTACK TRACKING
    // =====================
    private bool attackJ;
    private bool attackK;
    private bool attackL;

    // =====================
    // MONSTER TRACKING
    // =====================
    private bool zombieDone;
    private bool dasherDone;
    private bool giantDone;
    private bool thiefDone;

    void OnEnable()
    {
        EnemyEvents.OnEnemyKilled += OnEnemyKilled;
    }

    void OnDisable()
    {
        EnemyEvents.OnEnemyKilled -= OnEnemyKilled;
    }

    void Update()
    {
        if (!plot) return;

        switch (plot.CurrentStep)
        {
            case TrainingStep.Movement:
                TrackMovement();
                break;

            case TrainingStep.Attacks:
                TrackAttacks();
                break;
        }
    }

    // =====================
    // MOVEMENT
    // =====================
    void TrackMovement()
    {
        if (Input.GetKey(KeyCode.A)) movedLeft = true;
        if (Input.GetKey(KeyCode.D)) movedRight = true;
        if (Input.GetKeyDown(KeyCode.W)) jumped = true;

        if (movedLeft && movedRight && jumped)
        {
            plot.AdvanceTo(TrainingStep.Attacks);
        }
    }

    // =====================
    // ATTACKS
    // =====================
    void TrackAttacks()
    {
        if (Input.GetKeyDown(KeyCode.J)) attackJ = true;
        if (Input.GetKeyDown(KeyCode.K)) attackK = true;
        if (Input.GetKeyDown(KeyCode.L)) attackL = true;

        if (attackJ && attackK && attackL)
        {
            plot.AdvanceTo(TrainingStep.Zombie);
        }
    }

    // =====================
    // MONSTER REPORTS
    // =====================
    void OnEnemyKilled(EnemyType type)
    {
        if (!plot) return;

        if (plot.CurrentStep == TrainingStep.AllTogether)
        {
            MarkKilled(type);
            if (zombieDone && dasherDone && giantDone && thiefDone)
                ReportAllTogetherCompleted();
            return;
        }

        switch (type)
        {
            case EnemyType.Zombie:
                ReportZombieCompleted();
                break;
            case EnemyType.Dasher:
                ReportDasherCompleted();
                break;
            case EnemyType.Giant:
                ReportGiantCompleted();
                break;
            case EnemyType.Thief:
                ReportThiefCompleted();
                break;
        }
    }

    void MarkKilled(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Zombie:
                zombieDone = true;
                break;
            case EnemyType.Dasher:
                dasherDone = true;
                break;
            case EnemyType.Giant:
                giantDone = true;
                break;
            case EnemyType.Thief:
                thiefDone = true;
                break;
        }
    }


    public void ReportZombieCompleted()
    {
        if (plot.CurrentStep != TrainingStep.Zombie) return;

        zombieDone = true;
        plot.AdvanceTo(TrainingStep.Dasher);
    }

    public void ReportDasherCompleted()
    {
        if (plot.CurrentStep != TrainingStep.Dasher) return;

        dasherDone = true;
        plot.AdvanceTo(TrainingStep.Giant);
    }

    public void ReportGiantCompleted()
    {
        if (plot.CurrentStep != TrainingStep.Giant) return;

        giantDone = true;
        plot.AdvanceTo(TrainingStep.Thief);
    }

    public void ReportThiefCompleted()
    {
        if (plot.CurrentStep != TrainingStep.Thief) return;

        thiefDone = true;
        plot.AdvanceTo(TrainingStep.AllTogether);
    }

    public void ReportAllTogetherCompleted()
    {
        if (plot.CurrentStep != TrainingStep.AllTogether) return;

        plot.AdvanceTo(TrainingStep.Mirror);
    }

    // =====================
    // RESET (OPTIONAL)
    // =====================
    public void ResetProgress()
    {
        movedLeft = movedRight = jumped = false;
        attackJ = attackK = attackL = false;
        zombieDone = dasherDone = giantDone = thiefDone = false;
    }
}
