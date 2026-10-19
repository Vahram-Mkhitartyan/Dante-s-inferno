using NUnit.Framework;

namespace DantesInferno.SinSystem.Tests
{
    public class SinSaveTests
    {
        [Test]
        public void VersionedSaveRoundTripsAllAuthoritativeFields()
        {
            var model = new SinStateModel(SinType.Wrath);
            model.AddInfluence(SinType.Greed, 8f);
            model.SetPressure(SinType.Wrath, 0.65f);
            model.SetMastery(SinType.Greed, 0.25f);

            var original = new SinRunSaveData(
                "run-17",
                42,
                SinType.Wrath,
                SinType.Wrath,
                model.CreateSaveAxes(),
                3,
                0.4f,
                DeathCompanionChoice.ContinueWithoutDeath,
                12345,
                19,
                new[] { new SinEventId("run-17", 41) },
                new[] { "temptation-1" });

            string json = SinSaveJson.Serialize(original);
            SinRunSaveData restored = SinSaveJson.Deserialize(json);

            Assert.That(restored.SchemaVersion, Is.EqualTo(SinRunSaveData.CurrentSchemaVersion));
            Assert.That(restored.RulesVersion, Is.EqualTo(SinRunSaveData.CurrentRulesVersion));
            Assert.That(restored.RunId, Is.EqualTo("run-17"));
            Assert.That(restored.NextEventSequence, Is.EqualTo(42));
            Assert.That(restored.CollapseCount, Is.EqualTo(3));
            Assert.That(restored.DeathCorruption, Is.EqualTo(0.4f).Within(0.0001f));
            Assert.That(restored.CompanionChoice, Is.EqualTo(DeathCompanionChoice.ContinueWithoutDeath));
            Assert.That(restored.Seed, Is.EqualTo(12345));
            Assert.That(restored.RandomDrawCount, Is.EqualTo(19));
            Assert.That(restored.ProcessedEventIds[0], Is.EqualTo(new SinEventId("run-17", 41)));
            Assert.That(restored.ResolvedTemptationIds[0], Is.EqualTo("temptation-1"));
        }

        [Test]
        public void ModelRestoresSevenIndependentAxes()
        {
            var source = new SinStateModel(SinType.Wrath);
            source.SetPressure(SinType.Greed, 0.7f);
            source.SetMastery(SinType.Greed, 0.3f);
            source.AddInfluence(SinType.Greed, 12f);

            var restored = new SinStateModel(SinType.Wrath);
            restored.Restore(SinType.Greed, source.CreateSaveAxes());

            Assert.That(restored.CurrentDominantSin, Is.EqualTo(SinType.Greed));
            Assert.That(restored.GetPressure(SinType.Greed), Is.EqualTo(0.7f).Within(0.0001f));
            Assert.That(restored.GetMastery(SinType.Greed), Is.EqualTo(0.3f).Within(0.0001f));
            Assert.That(restored.GetInfluenceWeight(SinType.Greed), Is.EqualTo(source.GetInfluenceWeight(SinType.Greed)));
        }

        [Test]
        public void SaveRejectsRulesFromAnotherGeneration()
        {
            var model = new SinStateModel(SinType.Pride);
            var data = new SinRunSaveData(
                "run",
                1,
                SinType.Pride,
                SinType.Pride,
                model.CreateSaveAxes(),
                0,
                0f,
                DeathCompanionChoice.Undecided,
                1,
                0,
                null,
                null,
                "future-rules");

            Assert.Throws<System.InvalidOperationException>(() => data.Validate());
        }
    }
}
