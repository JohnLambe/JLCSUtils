using JohnLambe.Util.Collections;
using JohnLambe.Util.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvpFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static JohnLambe.Tests.JLUtilsTest.TestUtil;

namespace MvpFrameworkTest
{
    [TestClass]
    public class AcceleratorCaptionUtilTest
    {
        [TestMethod]
        public virtual void GetAccelerator()
        {
            Multiple(
                () => Assert.AreEqual('P',util.GetAccelerator("&Print")),
                () => Assert.AreEqual('p', util.GetAccelerator("&print")),
                () => Assert.AreEqual('&', util.GetAccelerator("A&&B")),
                () => Assert.AreEqual(null, util.GetAccelerator(null)),
                () => Assert.AreEqual(null, util.GetAccelerator("No accelerator")),
                () => Assert.AreEqual('t', util.GetAccelerator("A but&ton")),
                () => Assert.AreEqual(null, util.GetAccelerator("A button&")),
                () => Assert.AreEqual('o', util.GetAccelerator("Tw&o accelerato&rs"))
            );
        }

        [TestMethod]
        public virtual void DisplayNameToCaption()
        {
            Multiple(
                () => Assert.AreEqual("Print and Store", util.DisplayNameToCaption("Print & Store")),
                () => Assert.AreEqual("Print / Store", util.DisplayNameToCaption("Print / Store")),
                () => Assert.AreEqual("Print and &Store", util.DisplayNameToCaption("Print & Store", 's')),
                () => Assert.AreEqual("A and B and C", util.DisplayNameToCaption("A & B & C"))
            //TODO: heuristic adding of space and capitalisation
            );
        }

        [TestMethod]
        public virtual void CaptionToDisplayName()
        {
            Multiple(
                () => Assert.AreEqual("Print and Store", util.CaptionToDisplayName("Print and Store")),
                () => Assert.AreEqual("Print", util.CaptionToDisplayName("&Print")),
                () => Assert.AreEqual("Abcde", util.CaptionToDisplayName("A&bc&de"))
            );
        }

        [TestMethod]
        public virtual void SetAccelerator()
        {
            util.AcceleratorsUsed = null;  // no duplicate tracking

            Multiple(
                () => Assert.AreEqual("Prin&t and Store", util.SetAccelerator("Print and Store",'t')),
                () => Assert.AreEqual("Prin&t and Store", util.SetAccelerator("Print and Store", 'T'), "Different letter case"),
                () => Assert.AreEqual("Print and Store", util.SetAccelerator("Print and Store", null)),
                () => Assert.AreEqual("Print and Store [&z]", util.SetAccelerator("Print and Store", 'z')),
                () => Assert.AreEqual("Print and Store [&Z]", util.SetAccelerator("Print & Store", 'Z')),
                () => Assert.AreEqual("Print and &Archive", util.SetAccelerator("Print and Archive", 'a')),
                () => Assert.AreEqual("Print &and Store", util.SetAccelerator("Print & Store", 'A')),
                () => Assert.AreEqual("&Print page", util.SetAccelerator("Print page", 'p'), "First character"),
                () => Assert.AreEqual("print &Page", util.SetAccelerator("print Page", 'p'), "Prefer capital"),
                () => Assert.AreEqual(" [&+]", util.SetAccelerator("", '+'), "Blank original"),
                () => Assert.AreEqual(null, util.SetAccelerator(null, '+'), "null original")
            );
        }

        [TestMethod]
        public virtual void SetAccelerator_Remove()
        {
            util.AcceleratorsUsed = null;  // no duplicate tracking

            Multiple(
                () => Assert.AreEqual("Undo", util.SetAccelerator("Undo [&Z]", null, AcceleratorCaptionUtil.ExistingAccelerartorAction.Remove)),
                () => Assert.AreEqual("&Undo", util.SetAccelerator("Undo [&Z]", 'u', AcceleratorCaptionUtil.ExistingAccelerartorAction.Remove)),
                () => Assert.AreEqual("Undo [&.]", util.SetAccelerator("Undo [&Z]", '.', AcceleratorCaptionUtil.ExistingAccelerartorAction.Remove))
            );
        }

        [TestMethod]
        [ExpectedException(typeof(KeyExistsException))]
        public virtual void SetAccelerator_Duplicate()
        {
            // Arrange:
            Console.Out.WriteLine(util.SetAccelerator("Print and Archive", 'a'));

            // Act:
            Console.Out.WriteLine(util.SetAccelerator("Print & Store", 'A'));
        }

        [TestMethod]
        [ExpectedException(typeof(KeyExistsException))]
        public virtual void SetAccelerator_Automatic()
        {
            // Arrange:
            util.SetAccelerator("Print and Archive", 'a');

            // Act:
            util.SetAccelerator("Print & Store", 'A');
        }

        [TestMethod]
        public virtual void GenerateAccelerator()
        {
            Multiple(
                () => Assert.AreEqual("&Print and Store", util.GenerateAccelerator("Print and Store")),
                () => Assert.AreEqual("Print and &Store", util.GenerateAccelerator("Print and Store")),
                () => Assert.AreEqual("PPS 4&b", util.GenerateAccelerator("PPS 4b"), "Chooses the lowercase letter over the digit"),
                () => Assert.AreEqual("PS-&2", util.GenerateAccelerator("PS-2"), "Chooses the digit because all others are used"),
                () => Assert.AreEqual("pPSB [&A]", util.GenerateAccelerator("pPSB"), ""),
                () => Assert.AreEqual("P2SB [&C]", util.GenerateAccelerator("P2SB"), "")
            );
        }

        /// <summary>
        /// Chooses a digit because all letters are used.
        /// </summary>
        [TestMethod]
        public virtual void GenerateAccelerator_AllLettersUsed()
        {
            // Arrange:
            foreach(var c in CharacterUtil.AsciiLowercaseLetters)   // flag all letters as used
                util.AddAcceleratorUsed(c);
            util.AddAcceleratorUsed('0');

            // Act / Assert:
            Assert.AreEqual("New Blog Entry [&1]", util.GenerateAccelerator("New Blog Entry"));
        }

        /// <summary>
        /// Can't assign any accelerator character because all valid ones are used.
        /// </summary>
        [TestMethod]
        public virtual void GenerateAccelerator_AllUsed()
        {
            // Arrange:
            foreach (var c in util.AllowedAddedAcceleratorCharacters)   // flag all valid as used
                util.AddAcceleratorUsed(c);

            // Act / Assert:
            Assert.AreEqual("New Blog Entry 5", util.GenerateAccelerator("New Blog Entry 5"));
        }

        //TODO: Test with other configuration settings


        protected MvpFramework.AcceleratorCaptionUtil util = new MvpFramework.AcceleratorCaptionUtil();
    }
}
