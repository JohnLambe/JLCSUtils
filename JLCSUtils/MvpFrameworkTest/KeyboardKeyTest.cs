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
                () => Assert.AreEqual("Shift+Alt+Insert", (KeyboardKey.Insert | KeyboardKey.Shift | KeyboardKey.Alt).GetKeyDisplayName())
                );
        }

        // Could compare to WinForms by reflection,
        // and test that each key code (with modifiers specifically excluded) has not bits outside the mask.

        [TestMethod]
        public void GetBaseKey()
        {
            TestUtil.Multiple(
                () => Assert.AreEqual(KeyboardKey.Right, (KeyboardKey.Alt | KeyboardKey.Control | KeyboardKey.Right).GetBaseKey()),
                () => Assert.AreEqual(KeyboardKey.D, KeyboardKey.D.GetBaseKey()),
                () => Assert.AreEqual(KeyboardKey.None, KeyboardKey.None.GetBaseKey()),
                () => Assert.AreEqual(KeyboardKey.RControlKey, (KeyboardKey.RControlKey | KeyboardKey.Shift).GetBaseKey())
                );
        }

        [TestMethod]
        public void GetModifiers()
        {
            TestUtil.Multiple(
                () => Assert.AreEqual(KeyboardKey.Alt | KeyboardKey.Control, (KeyboardKey.Alt | KeyboardKey.Control | KeyboardKey.PageDown).GetModifiers()),
                () => Assert.AreEqual(KeyboardKey.Shift, (KeyboardKey.Shift | KeyboardKey.D2).GetModifiers()),
                () => Assert.AreEqual(KeyboardKey.None, KeyboardKey.ShiftKey.GetModifiers()),    // not a modifier key
                () => Assert.AreEqual(KeyboardKey.None, KeyboardKey.D.GetModifiers()),
                () => Assert.AreEqual(KeyboardKey.None, KeyboardKey.None.GetModifiers())
                );
        }

        [TestMethod]
        public void ConsoleModifiersTest()
        {
            TestUtil.Multiple(
                () => Assert.AreEqual(ConsoleModifiers.Alt | ConsoleModifiers.Control, (KeyboardKey.Alt | KeyboardKey.Control | KeyboardKey.F11).ConsoleModifiers()),
                () => Assert.AreEqual(ConsoleModifiers.Shift, (KeyboardKey.Shift | KeyboardKey.D2).ConsoleModifiers()),
                () => Assert.AreEqual((ConsoleModifiers)0, KeyboardKey.ShiftKey.ConsoleModifiers()),    // not a modifier key
                () => Assert.AreEqual(KeyboardKeyExtension.NoConsoleModifiers, KeyboardKey.D.ConsoleModifiers())    // same value as previous test
                );
        }

        [TestMethod]
        public void ToConsoleKey()
        {
            TestUtil.Multiple(
                () => Assert.AreEqual(ConsoleKey.F14, (KeyboardKey.Alt | KeyboardKey.Control | KeyboardKey.F14).ToConsoleKey()),   // converts only the base key
                () => Assert.AreEqual(ConsoleKey.D2, (KeyboardKey.Shift | KeyboardKey.D2).ToConsoleKey()),
                () => Assert.AreEqual(KeyboardKey.D, KeyboardKey.D.ToConsoleKey())
                );
        }

        [TestMethod]
        public void FromConsoleModifiers()
        {
            TestUtil.Multiple(
                () => Assert.AreEqual(KeyboardKey.Control | KeyboardKey.Shift, KeyboardKeyExtension.FromConsoleModifiers(ConsoleModifiers.Control | ConsoleModifiers.Shift))
                );
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void ConsoleKey_NonConvertible()
        {
            KeyboardKey.ShiftKey.ToConsoleKey();
        }

        [TestMethod]
        public void ToKeyboardKey()
        {
            TestUtil.Multiple(
                () => Assert.AreEqual(KeyboardKey.F5, KeyboardKeyExtension.ToKeyboardKey(ConsoleKey.F5)),
                () => Assert.AreEqual(KeyboardKey.G | KeyboardKey.Alt | KeyboardKey.Shift, KeyboardKeyExtension.ToKeyboardKey(ConsoleKey.G, ConsoleModifiers.Alt | ConsoleModifiers.Shift))
                );
        }
    }
}
