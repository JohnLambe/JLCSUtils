using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util;

namespace JohnLambe.Tests.JLUtilsTest
{
    [TestClass]
    public class StrUtilsTest
    {
        [TestMethod]
        public void RemovePrefix()
        {
            Assert.AreEqual("Value-Prefix:asd", StrUtils.RemovePrefix("Prefix:Value-Prefix:asd", "Prefix:"));
            Assert.AreEqual("Prefix:Value-Prefix:asd", StrUtils.RemovePrefix("Prefix:Value-Prefix:asd", "PrefiX:"), "Wrongly case insensitive");
            Assert.AreEqual("Value-Prefix:asd", StrUtils.RemovePrefix("ASD:Value-Prefix:asd", "asD:", StringComparison.InvariantCultureIgnoreCase), "Non-case-sensitive");

            Assert.AreEqual(null, StrUtils.RemovePrefix(null,null, StringComparison.InvariantCultureIgnoreCase), "null");
        }

        [TestMethod]
        public void RemoveSuffix()
        {
            Assert.AreEqual("12345678", StrUtils.RemoveSuffix("1234567890", "90"));
            Assert.AreEqual("asdfghjkl", StrUtils.RemoveSuffix("asdfghjkl", "KL"), "Wrongly case insensitive");
            Assert.AreEqual("qwertyui", StrUtils.RemoveSuffix("qwertyuiop", "Op", StringComparison.InvariantCultureIgnoreCase), "Non-case-sensitive");
            Assert.AreEqual("test string", StrUtils.RemoveSuffix("test string", "", StringComparison.InvariantCultureIgnoreCase), "Should have returned unmodified");

            Assert.AreEqual(null, StrUtils.RemoveSuffix(null, "90"), "null");
        }

        [TestMethod]
        public void TotalLength()
        {
            Assert.AreEqual(13, StrUtils.TotalLength(2,new string[] {"asd","f",null," . . "}));
            Assert.AreEqual(0, StrUtils.TotalLength(2,null), "wrong length when no parts");
        }

        #region SplitToVars

        [TestMethod]
        public void SplitToVars_2Parts_Simple()
        {
            // Arrange:
            string a,b;

            // Act:
            "param1,param2".SplitToVars(',', out a, out b);

            // Assert:
            Assert.AreEqual("param1",a);
            Assert.AreEqual("param2",b);
        }

        [TestMethod]
        public void SplitToVars_3Parts_Simple()
        {
            // Arrange:
            string a, b, c;

            // Act:
            "param1 param2 param3".SplitToVars(' ', out a, out b, out c);

            // Assert:
            Assert.AreEqual("param1", a);
            Assert.AreEqual("param2", b);
            Assert.AreEqual("param3", c);
        }

        [TestMethod]
        public void SplitToVars_3Parts_1Present()
        {
            // Arrange:
            string a, b, c;

            // Act:
            "param1 param2 param3".SplitToVars('-', out a, out b, out c);

            // Assert:
            Assert.AreEqual("param1 param2 param3", a);
            Assert.AreEqual(null, b);
            Assert.AreEqual(null, c);
        }

        [TestMethod]
        public void SplitToVars_2Parts_Blank()
        {
            // Arrange:
            string a, b;

            // Act:
            "param1#".SplitToVars('#', out a, out b);

            // Assert:
            Assert.AreEqual("param1", a);
            Assert.AreEqual("", b);
        }

        #endregion

        [TestMethod]
        public void ConcatWithSeparator()
        {
            Assert.AreEqual("asd/f//gh/i",StrUtils.ConcatWithSeparator("/","asd","f/",null,"gh","","i"));
        }

        [TestMethod]
        public void NullToBlank()
        {
            Assert.AreEqual("", StrUtils.NullToBlank(null));
            Assert.AreEqual("asd ", StrUtils.NullToBlank("asd "));
        }

        [TestMethod]
        public void FirstNonBlank()
        {
            Assert.AreEqual("third", StrUtils.FirstNonBlank("",null,"third",null,"fifth"));
        }

        [TestMethod]
        public void DoublQuote()
        {
            Assert.AreEqual("\"value\"", "value".DoubleQuote(), "no quotes");
            Assert.AreEqual("\"value\"\"1234\"", "value\"1234".DoubleQuote());
        }

        [TestMethod]
        public void CharAt()
        {
            Assert.AreEqual('d', "Éb\0cdefghi".CharAt(4,'x'));
            Assert.AreEqual('#', StrUtils.CharAt(null, 4, '#'), "null");
            Assert.AreEqual('-', "qwerty".CharAt(100,'-'), "out of range");
            Assert.AreEqual('\0', "qwerty".CharAt(-1), "out of range, negative");
            Assert.AreEqual('\0', "abcd".CharAt(4), "out of range by one");
        }

        [TestMethod]
        public void CharFromEnd()
        {
            Assert.AreEqual('t',"qwerty".CharFromEnd(1));
        }

        [TestMethod]
        public void EndSubstring()
        {
            Assert.AreEqual("nm", "zxcvbnm".EndSubstring(2));
            Assert.AreEqual("", "ASDFghjkl".EndSubstring(0));
            Assert.AreEqual("é-qwerty-Á", "é-qwerty-Á".EndSubstring(500), "length longer than string");
            Assert.AreEqual(null, StrUtils.EndSubstring(null,0));
        }

        [TestMethod]
        public void NullPropagate()
        {
            Assert.AreEqual(null, StrUtils.NullPropagate("first",null,"third"));
            Assert.AreEqual("firstthird", StrUtils.NullPropagate("first", "", "third"));
            Assert.AreEqual(null, StrUtils.NullPropagate());
            Assert.AreEqual("", StrUtils.NullPropagate("",""));
        }

        [TestMethod]
        public void ConcatForEach()
        {
            Assert.AreEqual("<2> <4> <6>",StrUtils.ConcatForEach(new int[] { 2, 4, 6 }, x => "<" + x + ">", " "));
            Assert.AreEqual("<2><4><6>", StrUtils.ConcatForEach(new int[] { 2, 4, 6 }, x => "<" + x + ">"));
            Assert.AreEqual("#a -> #b -> #c", StrUtils.ConcatForEach(new string[] { "a", null, "b", "c" }, x => x == null ? null : "#" + x, " -> "));
            Assert.AreEqual("12 -> 22 -> 32", StrUtils.ConcatForEach(new float?[] { 10, 20, null, 30 }, x => x + 2, " -> "));
        }

        [TestMethod]
        public void RemoveCharacters()
        {
            Assert.AreEqual("12345", "asd1fgh23jkl45í".RemoveCharacters(CharacterUtils.Digits,true));
            Assert.AreEqual("word1word2é", "word1 word-2 é".RemoveCharacters(" -"));

            Assert.AreEqual(null, ((string)null).RemoveCharacters("12345"));
            Assert.AreEqual(null, ((string)null).RemoveCharacters(null));
            Assert.AreEqual("the string", "the string".RemoveCharacters(null));
        }

        [TestMethod]
        public void Repeat()
        {
            Assert.AreEqual("éfgéfgéfgéfgéfg", StrUtils.Repeat("éfg", 5));
            Assert.AreEqual("", StrUtils.Repeat("qwerty", 0));

            Assert.AreEqual(null, StrUtils.Repeat(null, 10));
            TestUtil.AssertThrows(typeof(ArgumentException), () => StrUtils.Repeat("xyz", -1));
        }

        [TestMethod]
        public void IsEnclosedIn()
        {
            Assert.AreEqual(true, "(test)".IsEnclosedIn("(",")"));
            Assert.AreEqual(true, "'asdfg'".IsEnclosedIn("'"));

            Assert.AreEqual(true, "##".IsEnclosedIn("#", "#"));
            Assert.AreEqual(false, "'".IsEnclosedIn("'"));

            Assert.AreEqual(true, "<--JKL->".IsEnclosedIn("<--", "->"), "different length prefix and suffix");

            Assert.AreEqual(false, StrUtils.IsEnclosedIn(null, "%"), "null");
        }

        [TestMethod]
        public void IsEnclosedIn_Char()
        {
            Assert.AreEqual(true, "<AAA>".IsEnclosedIn('<', '>'));
            Assert.AreEqual(true, "%qwertá%".IsEnclosedIn('%'));

            Assert.AreEqual(false, "<AAA> ".IsEnclosedIn('<', '>'));
            Assert.AreEqual(false, "".IsEnclosedIn('%'));
            Assert.AreEqual(false, "%".IsEnclosedIn('%'));  // the same character can't count as both `prefix` and `suffix`
            Assert.AreEqual(false, StrUtils.IsEnclosedIn(null,'%'), "null");
        }

        [TestMethod]
        public void ExtractEnclosed()
        {
            Assert.AreEqual("Ábc", StrUtils.ExtractEnclosed("/Ábc/","/"));
            Assert.AreEqual(null, StrUtils.ExtractEnclosed("zxcvbnm", "/"), "null");
            Assert.AreEqual("ExtractEnclosed", StrUtils.ExtractEnclosed("(*ExtractEnclosed*)", "(*","*)"));
        }

        [TestMethod]
        public void ExtractEnclosed_Char()
        {
            Assert.AreEqual("sdf", StrUtils.ExtractEnclosed("asdfg", 'a', 'g'));
            Assert.AreEqual("", StrUtils.ExtractEnclosed("@@", '@', '@'),"empty enclosed string");
            Assert.AreEqual(null, StrUtils.ExtractEnclosed("]a[", '[', ']'));
            Assert.AreEqual(null, StrUtils.ExtractEnclosed(null, '[', ']'),"null");
        }

        [TestMethod]
        public void SplitBefore()
        {
            Assert.AreEqual(null, StrUtils.SplitBefore(null, ":"));
            Assert.AreEqual("first", "first/second".SplitBefore("/"));
            Assert.AreEqual("a:b:c", "a:b:c::d:e:f::g".SplitBefore("::"));
        }

        [TestMethod]
        public void SplitAfter()
        {
            Assert.AreEqual(null, StrUtils.SplitAfter(null, "a"));
            Assert.AreEqual("second", "firstzsecond".SplitAfter("z"));
            Assert.AreEqual("d:e:f::g", "a:b:c::d:e:f::g".SplitAfter("::"));
        }
    }
}
