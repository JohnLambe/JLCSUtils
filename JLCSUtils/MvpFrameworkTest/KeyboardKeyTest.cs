using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MvpFramework;
using JohnLambe.Tests.JLUtilsTest;
using System.Diagnostics;

namespace MvpFrameworkTest
{
    [TestClass]
    public class KeyboardKeyTest
    {
        [TestMethod]
        public void Masks()
        {
            // Masks include all bits with no overlap:
            Assert.AreEqual(KeyboardKey.None, KeyboardKey.KeyCode & KeyboardKey.ModifierKeys);
            Assert.AreEqual((KeyboardKey) (int)-1, KeyboardKey.KeyCode | KeyboardKey.ModifierKeys);

            Assert.AreEqual(KeyboardKey.None, KeyboardKey.BaseKey & KeyboardKey.Modifiers);
        }

        [TestMethod]
        public void AddModifier()
        {
            Assert.AreEqual(KeyboardKey.Control | KeyboardKey.Alt | KeyboardKey.Delete,
                KeyboardKey.Delete.AddModifier(KeyboardKey.Alt).AddModifier(KeyboardKey.Control));

            Assert.AreEqual(KeyboardKey.Control | KeyboardKey.Alt | KeyboardKey.Delete,
                (KeyboardKey.Control | KeyboardKey.Alt | KeyboardKey.Delete).AddModifier(KeyboardKey.Alt),
                "Already has the modifier - no change");

            Assert.AreEqual(KeyboardKey.Space,
                KeyboardKey.Space.AddModifier(KeyboardKey.None));

            Assert.AreEqual(KeyboardKey.F5 | KeyboardKey.Control | KeyboardKey.Shift,
                (KeyboardKey.F5 | KeyboardKey.Control).AddModifier(KeyboardKey.Shift), "Key that already has a modifier");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddModifier_InvalidBaseKey()
        {
            KeyboardKey.Control.AddModifier(KeyboardKey.A);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddModifier_InvalidModifier()
        {
            KeyboardKey.Shift.AddModifier(KeyboardKey.Control);
        }

        [TestMethod]
        public void GetKeyDisplayName()
        {
            TestUtil.Multiple(
                () => Assert.AreEqual("Space", KeyboardKey.Space.GetKeyDisplayName()),
                () => Assert.AreEqual("PgDn", KeyboardKey.PageDown.GetKeyDisplayName(true)),
                () => Assert.AreEqual(KeyboardKeyExtension.ShiftKeyName + KeyboardKeyExtension.AltKeyName + "Ins", (KeyboardKey.Insert | KeyboardKey.Shift | KeyboardKey.Alt).GetKeyDisplayName(true)),
                () => Assert.AreEqual("Shift-Alt-Insert", (KeyboardKey.Insert | KeyboardKey.Shift | KeyboardKey.Alt).GetKeyDisplayName())
                );
        }

        // Could compare to WinForms by reflection,
        // and test that each key code (with modifiers specifically excluded) has not bits outside the mask.

    }
}
