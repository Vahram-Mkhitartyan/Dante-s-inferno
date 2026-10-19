using NUnit.Framework;

namespace DantesInferno.SinSystem.Tests
{
    public class SinEvaluationTests
    {
        [TestCase(0f, WrathFeedbackLevel.Quiet)]
        [TestCase(0.2f, WrathFeedbackLevel.Stirring)]
        [TestCase(0.5f, WrathFeedbackLevel.Surging)]
        [TestCase(0.8f, WrathFeedbackLevel.Overwhelming)]
        public void WrathFeedbackPolicy_UsesStableThresholds(
            float pressure,
            WrathFeedbackLevel expected)
        {
            Assert.That(WrathFeedbackPolicy.GetLevel(pressure), Is.EqualTo(expected));
        }

        [Test]
        public void WrathFeedbackPolicy_SubduesNonDominantWrath()
        {
            float dominant = WrathFeedbackPolicy.GetVisualIntensity(0.8f, true);
            float secondary = WrathFeedbackPolicy.GetVisualIntensity(0.8f, false);

            Assert.That(secondary, Is.LessThan(dominant));
        }

        [Test]
        public void RepeatedAttacksCreateMoreWrathPressureThanDeliberateAttacks()
        {
            var evaluator = new WrathActionEvaluator();
            var model = new SinStateModel(SinType.Wrath);
            SinStateSnapshot snapshot = model.CreateSnapshot();

            evaluator.TryEvaluate(
                CreateAction(1, SinActionKind.AttackStarted, SinBehaviorPattern.Balanced),
                snapshot,
                out SinEvaluationDelta deliberate);
            evaluator.TryEvaluate(
                CreateAction(2, SinActionKind.AttackStarted, SinBehaviorPattern.Repeated),
                snapshot,
                out SinEvaluationDelta repeated);

            Assert.That(repeated.Pressure, Is.GreaterThan(deliberate.Pressure));
            Assert.That(repeated.Influence, Is.GreaterThan(deliberate.Influence));
            Assert.That(repeated.ReasonCode, Is.EqualTo("wrath.repeated-attack"));
        }

        [Test]
        public void BlockingProvidesWrathRegulationEvidence()
        {
            var evaluator = new WrathActionEvaluator();
            var model = new SinStateModel(SinType.Wrath);

            bool handled = evaluator.TryEvaluate(
                CreateAction(1, SinActionKind.DamageBlocked),
                model.CreateSnapshot(),
                out SinEvaluationDelta delta);

            Assert.That(handled, Is.True);
            Assert.That(delta.Pressure, Is.LessThan(0f));
            Assert.That(delta.Mastery, Is.GreaterThan(0f));
        }

        [Test]
        public void GreedCollectionAndRejectionUseTheSameEvaluationContract()
        {
            var evaluator = new GreedActionEvaluator();
            var model = new SinStateModel(SinType.Greed);
            SinStateSnapshot snapshot = model.CreateSnapshot();

            var collected = new SinActionRecord(
                new SinEventId("run", 1),
                SinActionKind.RewardCollected,
                1d,
                "temptation",
                rewardValue: 4f,
                availableAlternatives: 2f);
            var rejected = new SinActionRecord(
                new SinEventId("run", 2),
                SinActionKind.RewardRejected,
                2d,
                "temptation",
                rewardValue: 4f);

            Assert.That(evaluator.TryEvaluate(collected, snapshot, out SinEvaluationDelta collection), Is.True);
            Assert.That(evaluator.TryEvaluate(rejected, snapshot, out SinEvaluationDelta rejection), Is.True);
            Assert.That(collection.Influence, Is.GreaterThan(0f));
            Assert.That(collection.Pressure, Is.GreaterThan(0f));
            Assert.That(rejection.Pressure, Is.LessThan(0f));
            Assert.That(rejection.Mastery, Is.GreaterThan(0f));
        }

        [Test]
        public void EventIdentityUsesRunAndSequence()
        {
            var first = new SinEventId("run-a", 1);
            var same = new SinEventId("run-a", 1);
            var otherRun = new SinEventId("run-b", 1);

            Assert.That(first, Is.EqualTo(same));
            Assert.That(first, Is.Not.EqualTo(otherRun));
        }

        private static SinActionRecord CreateAction(
            long sequence,
            SinActionKind kind,
            SinBehaviorPattern pattern = SinBehaviorPattern.Balanced)
        {
            return new SinActionRecord(new SinEventId("run", sequence), kind, sequence, behaviorPattern: pattern);
        }
    }
}
