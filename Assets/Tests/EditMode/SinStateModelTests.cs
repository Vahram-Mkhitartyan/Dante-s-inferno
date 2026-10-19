using NUnit.Framework;

namespace DantesInferno.SinSystem.Tests
{
    public class SinStateModelTests
    {
        [Test]
        public void NewModelUsesOriginHeadStartAndNormalizedComposition()
        {
            var model = new SinStateModel(SinType.Wrath);

            Assert.That(model.OriginSin, Is.EqualTo(SinType.Wrath));
            Assert.That(model.CurrentDominantSin, Is.EqualTo(SinType.Wrath));
            Assert.That(model.GetInfluenceWeight(SinType.Wrath), Is.EqualTo(34f));
            Assert.That(SumComposition(model), Is.EqualTo(1f).Within(0.0001f));
        }

        [Test]
        public void InfluenceCannotFallBelowConfiguredFloor()
        {
            var rules = new SinStateRules(minimumInfluenceWeight: 2f);
            var model = new SinStateModel(SinType.Pride, rules);

            model.AddInfluence(SinType.Greed, -100f);

            Assert.That(model.GetInfluenceWeight(SinType.Greed), Is.EqualTo(2f));
            Assert.That(SumComposition(model), Is.EqualTo(1f).Within(0.0001f));
        }

        [Test]
        public void IncreasingOneInfluenceRedistributesNormalizedComposition()
        {
            var model = new SinStateModel(SinType.Wrath);
            float greedBefore = model.GetComposition(SinType.Greed);

            model.AddInfluence(SinType.Wrath, 20f);

            Assert.That(model.GetComposition(SinType.Wrath), Is.GreaterThan(0.34f));
            Assert.That(model.GetComposition(SinType.Greed), Is.LessThan(greedBefore));
            Assert.That(SumComposition(model), Is.EqualTo(1f).Within(0.0001f));
        }

        [Test]
        public void DominanceRequiresMarginAndSustainedLead()
        {
            var rules = new SinStateRules(
                pressureDominanceWeight: 1f,
                dominanceMargin: 0.1f,
                dominanceSustainSeconds: 2f);
            var model = new SinStateModel(SinType.Wrath, rules);

            model.SetPressure(SinType.Wrath, 0.2f);
            model.SetPressure(SinType.Greed, 0.8f);

            model.AdvanceDominance(1f);
            Assert.That(model.CurrentDominantSin, Is.EqualTo(SinType.Wrath));

            model.AdvanceDominance(1f);
            Assert.That(model.CurrentDominantSin, Is.EqualTo(SinType.Greed));
            Assert.That(model.OriginSin, Is.EqualTo(SinType.Wrath));
        }

        [Test]
        public void DominanceCandidateResetsWhenLeadDisappears()
        {
            var rules = new SinStateRules(
                pressureDominanceWeight: 1f,
                dominanceMargin: 0.1f,
                dominanceSustainSeconds: 2f);
            var model = new SinStateModel(SinType.Wrath, rules);

            model.SetPressure(SinType.Greed, 0.8f);
            model.AdvanceDominance(1.5f);

            model.SetPressure(SinType.Greed, 0f);
            model.AdvanceDominance(1f);

            model.SetPressure(SinType.Greed, 0.8f);
            model.AdvanceDominance(1f);

            Assert.That(model.CurrentDominantSin, Is.EqualTo(SinType.Wrath));
        }

        [Test]
        public void CombinedRiskAccumulatesLocalPressures()
        {
            var rules = new SinStateRules(pressureAggregation: OverallPressureAggregation.CombinedRisk);
            var model = new SinStateModel(SinType.Wrath, rules);

            model.SetPressure(SinType.Wrath, 0.5f);
            model.SetPressure(SinType.Greed, 0.5f);

            Assert.That(model.OverallPressure, Is.EqualTo(0.75f).Within(0.0001f));
            Assert.That(model.OverallState, Is.EqualTo(SinPressureState.Unstable));
        }

        [Test]
        public void PressureAndMasteryAreClamped()
        {
            var model = new SinStateModel(SinType.Sloth);

            model.SetPressure(SinType.Sloth, 5f);
            model.SetMastery(SinType.Sloth, -2f);

            Assert.That(model.GetPressure(SinType.Sloth), Is.EqualTo(1f));
            Assert.That(model.GetMastery(SinType.Sloth), Is.EqualTo(0f));

            model.AddMastery(SinType.Sloth, 2f);
            Assert.That(model.GetMastery(SinType.Sloth), Is.EqualTo(1f));
        }

        [Test]
        public void SnapshotContainsAllSevenIndependentAxes()
        {
            var model = new SinStateModel(SinType.Lust);
            model.SetPressure(SinType.Envy, 0.4f);
            model.SetMastery(SinType.Pride, 0.6f);

            SinStateSnapshot snapshot = model.CreateSnapshot();

            Assert.That(snapshot.SchemaVersion, Is.EqualTo(SinStateSnapshot.CurrentSchemaVersion));
            Assert.That(snapshot.AxisCount, Is.EqualTo(7));
            Assert.That(snapshot.OriginSin, Is.EqualTo(SinType.Lust));
            Assert.That(snapshot.GetAxis(SinType.Envy).Pressure, Is.EqualTo(0.4f));
            Assert.That(snapshot.GetAxis(SinType.Pride).Mastery, Is.EqualTo(0.6f));
        }

        private static float SumComposition(SinStateModel model)
        {
            float total = 0f;
            for (int index = 0; index < 7; index++)
                total += model.GetComposition((SinType)index);

            return total;
        }
    }
}
