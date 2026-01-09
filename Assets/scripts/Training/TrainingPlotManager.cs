using UnityEngine;
using System.Collections.Generic;

public class TrainingPlotManager : MonoBehaviour
{
    [Header("References")]
    public WhisperManager whisper;

    public TrainingStep CurrentStep { get; private set; }

    private Dictionary<TrainingStep, string> textByStep;

    void Awake()
    {
        textByStep = new Dictionary<TrainingStep, string>
        {
            { TrainingStep.Movement, "You can move using A, D and W jeys to move and jump" },
            { TrainingStep.Attacks, "Use J, K and L to attack. Hold SHIFT to use shield." },
            { TrainingStep.Zombie, "This is regular Soul, slow, weak, but don't let them get too close. " },
            { TrainingStep.Dasher, "This is Dasher. The name says itself." },
            { TrainingStep.Giant, "The Giants move slow, hit Hard hit them while they are stuck." },
            { TrainingStep.Thief, "These little shits steal armor. be careful." },
            { TrainingStep.AllTogether, "a small situation awaiting you inside" },
            { TrainingStep.Mirror, "Now look at yourself. the mirror reflects sins" },
            { TrainingStep.Complete, "You may proceed." }
        };
    }

    void Start()
    {
        AdvanceTo(TrainingStep.Movement);
    }

    public void AdvanceTo(TrainingStep next)
    {
        if (CurrentStep == next)
            return;

        CurrentStep = next;

        if (textByStep.TryGetValue(next, out var line))
            whisper.Whisper(line);

    }
}
