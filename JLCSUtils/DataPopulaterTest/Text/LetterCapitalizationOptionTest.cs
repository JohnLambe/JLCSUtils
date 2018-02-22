using JohnLambe.Util.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JohnLambe.Tests.JLUtilsTest.TestUtil;

namespace JohnLambe.Tests.JLUtilsTest.Text
{
    [TestClass]
    public class LetterCapitalizationOptionTest
    {
        [TestMethod]
        public void MixedCase()
        {
            Unchanged(LetterCapitalizationOption.Unspecified);
            Unchanged(LetterCapitalizationOption.MixedCase);
        }

        protected void Unchanged(LetterCapitalizationOption option)
        {
            Multiple(
                () => Assert.AreEqual("óne two? ThreÉ", option.ChangeCapitalization("óne two? ThreÉ")),
                () => Assert.AreEqual("", option.ChangeCapitalization("")),
                () => Assert.AreEqual(null, option.ChangeCapitalization(null))
            );
        }

        [TestMethod]
        public void FirstLetterCapitalOnly()
        {
            Multiple(
                () => Assert.AreEqual("Óne two threé!",LetterCapitalizationOption.FirstLetterCapitalOnly.ChangeCapitalization("óne two ThreÉ!")),
                () => Assert.AreEqual(" óne two threé", LetterCapitalizationOption.FirstLetterCapitalOnly.ChangeCapitalization(" óne two ThreÉ")),
                () => Assert.AreEqual("", LetterCapitalizationOption.FirstLetterCapitalOnly.ChangeCapitalization("")),
                () => Assert.AreEqual(null, LetterCapitalizationOption.FirstLetterCapitalOnly.ChangeCapitalization(null))
            );
        }

        [TestMethod]
        public void FirstLetterCapital()
        {
            Multiple(
                () => Assert.AreEqual("Óne two ThreÉ", LetterCapitalizationOption.FirstLetterCapital.ChangeCapitalization("óne two ThreÉ")),
                () => Assert.AreEqual("", LetterCapitalizationOption.FirstLetterCapital.ChangeCapitalization("")),
                () => Assert.AreEqual(null, LetterCapitalizationOption.FirstLetterCapital.ChangeCapitalization(null))
            );
        }

        [TestMethod]
        public void AllCapital()
        {
            Multiple(
                () => Assert.AreEqual("ÓNE TWO  THREÉ €", LetterCapitalizationOption.AllCapital.ChangeCapitalization("óne two  ThreÉ €")),
                () => Assert.AreEqual("", LetterCapitalizationOption.AllCapital.ChangeCapitalization("")),
                () => Assert.AreEqual(null, LetterCapitalizationOption.AllCapital.ChangeCapitalization(null))
            );
        }

        [TestMethod]
        public void AllLowercase()
        {
            Multiple(
                () => Assert.AreEqual("óne two threé.", LetterCapitalizationOption.AllLowercase.ChangeCapitalization("óne two ThreÉ.")),
                () => Assert.AreEqual("", LetterCapitalizationOption.AllLowercase.ChangeCapitalization("")),
                () => Assert.AreEqual(null, LetterCapitalizationOption.AllLowercase.ChangeCapitalization(null))
            );
        }

        [TestMethod]
        public void TitleCase()
        {
            Multiple(
                () => Assert.AreEqual("Óne Two ThreÉ", LetterCapitalizationOption.TitleCase.ChangeCapitalization("óne two ThreÉ")),
                () => Assert.AreEqual(" Óne  Two ThreÉ", LetterCapitalizationOption.TitleCase.ChangeCapitalization(" óne  two ThreÉ")),
                () => Assert.AreEqual(" Ó+n-e  Two ThreÉ", LetterCapitalizationOption.TitleCase.ChangeCapitalization(" ó+n-e  two ThreÉ")),
                () => Assert.AreEqual(" #óne  Two ThreÉ", LetterCapitalizationOption.TitleCase.ChangeCapitalization(" #óne  two ThreÉ")),
                () => Assert.AreEqual("", LetterCapitalizationOption.TitleCase.ChangeCapitalization("")),
                () => Assert.AreEqual(null, LetterCapitalizationOption.TitleCase.ChangeCapitalization(null))
            );
        }

        [TestMethod]
        public void TitleCaseOnly()
        {
            Multiple(
                () => Assert.AreEqual("Óne Two Threé", LetterCapitalizationOption.TitleCaseOnly.ChangeCapitalization("óne two ThreÉ")),
                () => Assert.AreEqual("", LetterCapitalizationOption.TitleCaseOnly.ChangeCapitalization("")),
                () => Assert.AreEqual(null, LetterCapitalizationOption.TitleCaseOnly.ChangeCapitalization(null))
            );
        }
    }
}
