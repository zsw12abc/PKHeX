using Microsoft.VisualStudio.TestTools.UnitTesting;
using PKHeX.Reflection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace PKHeX.Tests.Reflection
{
    [TestClass]
    public class ReflectUtilTests
    {
        private class TestClass
        {
            public int EXP { get; set; }
            public int EV_HP { get; set; }
            public int Stat_Level { get; set; }
            public int Stat_HPMax { get; set; }
            public int Species { get; set; }
        }
        const string BatchEditorCategory = "Batch Editor";
        CultureInfo PreviousCulture { get; set; }

        [TestInitialize] public void TestInit()
        {
            PreviousCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
        }

        [TestCleanup] public void TestCleanup()
        {
            Thread.CurrentThread.CurrentCulture = PreviousCulture;
        }

        [TestMethod]
        [TestCategory(BatchEditorCategory)]
        public void TestGetFilters()
        {
            var testScript = "=EXP=10\n\n\n.Stat_Level=16\n\n.Stat_HPMax=25\n\n\n\n\n!EV_HP=42";
            var filters = ReflectUtil.getFilters(testScript).ToList();

            Assert.AreEqual(2, filters.Count);

            // Check first filter
            Assert.AreEqual(true, filters[0].Evaluator);
            Assert.AreEqual("EXP", filters[0].PropertyName);
            Assert.AreEqual("10", filters[0].PropertyValue);

            // Check second filter
            Assert.AreEqual(false, filters[1].Evaluator);
            Assert.AreEqual("EV_HP", filters[1].PropertyName);
            Assert.AreEqual("42", filters[1].PropertyValue);
        }

        [TestMethod]
        [TestCategory(BatchEditorCategory)]
        public void TestGetInstructionsIgnoringBlankLines()
        {
            var testScript = "=EXP=10\n\n\n.Stat_Level=16\n\n.Stat_HPMax=25\n\n\n\n\n!EV_HP=42";
            var filters = ReflectUtil.getInstructions(testScript).ToList();

            Assert.AreEqual(2, filters.Count);

            // Check first filter
            Assert.AreEqual("Stat_Level", filters[0].PropertyName);
            Assert.AreEqual("16", filters[0].PropertyValue);

            // Check second filter
            Assert.AreEqual("Stat_HPMax", filters[1].PropertyName);
            Assert.AreEqual("25", filters[1].PropertyValue);
        }

        [TestMethod]
        [TestCategory(BatchEditorCategory)]
        public void TestFilterScreening()
        {
            var testScript = @"
=Species=Riolu
=HeldItem=Oran Berry
=Move1=Aura Sphere
=Move2=Shadow Claw
=Move3=Bullet Punch
=Move4=Drain Punch
=RelearnMove1=Foresight
=RelearnMove2=Endure
=RelearnMove3=Quick Attack
=RelearnMove4=Bite
=Ability=Steadfast
=Nature=Serious
=Ball=Poké Ball";
            var filters = ReflectUtil.getFilters(testScript).ToList();

            // Check filter Species
            Assert.AreEqual("447", filters[0].PropertyValue);

            // Check filter Held Item
            Assert.AreEqual("155", filters[1].PropertyValue);

            // Check filter Move1
            Assert.AreEqual("396", filters[2].PropertyValue);

            // Check filter Move2
            Assert.AreEqual("421", filters[3].PropertyValue);

            // Check filter Move3
            Assert.AreEqual("418", filters[4].PropertyValue);

            // Check filter Move4
            Assert.AreEqual("409", filters[5].PropertyValue);

            // Check filter RelearnMove1
            Assert.AreEqual("193", filters[6].PropertyValue);

            // Check filter RelearnMove2
            Assert.AreEqual("203", filters[7].PropertyValue);

            // Check filter RelearnMove3
            Assert.AreEqual("98", filters[8].PropertyValue);

            // Check filter RelearnMove4
            Assert.AreEqual("44", filters[9].PropertyValue);

            // Check filter Ability
            Assert.AreEqual("80", filters[10].PropertyValue);

            // Check filter Serious
            Assert.AreEqual("12", filters[11].PropertyValue);

            // Check filter Item
            Assert.AreEqual("4", filters[12].PropertyValue);
        }
    }
}
