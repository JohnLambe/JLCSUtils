﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using JohnLambe.Util;
using System.Collections;
using System.Collections.Generic;

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

        #region Other splitting

        [TestMethod]
        public void SplitFirst()
        {
            // Arrange:
            string a = "first.second..4th";
            string[] parts = new string[6];

            // Act:
            parts[0] = StrUtils.SplitFirst('.', ref a);
            Assert.AreEqual("second..4th", a);

            parts[1] = StrUtils.SplitFirst('.', ref a);
            Assert.AreEqual(".4th", a);

            parts[2] = StrUtils.SplitFirst('.', ref a);
            Assert.AreEqual("4th", a);

            parts[3] = StrUtils.SplitFirst('.', ref a);
            Assert.AreEqual(null, a);

            // Nothing remaining at this point. Subsequent calls will return null.
            parts[4] = StrUtils.SplitFirst('.', ref a);

            // Assert:
            Assert.IsTrue(parts.SequenceEqual(
                new string[] { "first", "second", "", "4th", null, null }));
            Assert.AreEqual(null, a);
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
        public void OverwriteSubstring()
        {
            Assert.AreEqual("qweABCDiop", StrUtils.OverwriteSubstring("qwertyuiop",3,"ABCD"));
            Assert.AreEqual("qweABCDEFGHIJK", StrUtils.OverwriteSubstring("qwertyuiop", 3, "ABCDEFGHIJK"));

            //TODO: null
        }

        [TestMethod]
        public void OverwriteSubstringNoExtend()
        {
            Assert.AreEqual("qweABCDiop", StrUtils.OverwriteSubstring("qwertyuiop", 3, "ABCD", false));
            Assert.AreEqual("qweABCDEFG", StrUtils.OverwriteSubstring("qwertyuiop", 3, "ABCDEFGHIJK", false));

            //TODO: null
        }

        [TestMethod]
        public void ReplaceSubstring()
        {
            Assert.AreEqual("qweABCDyuiop", StrUtils.ReplaceSubstring("qwertyuiop", 3, 2, "ABCD"));

            //TODO: null
        }

        [TestMethod]
        public void StartsWith()
        {
            Assert.AreEqual(true, "asdfg".StartsWith('a'));
            Assert.AreEqual(true, "Éasdfg".StartsWith('É'), "Unicode");
            Assert.AreEqual(true, "\0asdfg".StartsWith('\0'), "Invisible");
            Assert.AreEqual(false, " asdfg".StartsWith('a'), "Trimmed");
            Assert.AreEqual(false, "aAdfg".StartsWith('A'), "Should be case-sensitive");
            Assert.AreEqual(false, "".StartsWith('A'), "Empty string - should always return false");

            TestUtil.AssertThrows(typeof(NullReferenceException), () => ((string)null).StartsWith('x'));
            TestUtil.AssertThrows(typeof(NullReferenceException), () => ((string)null).StartsWith("x")); // testing the behaviour of System.String, to check that our method is equivalent
        }

        [TestMethod]
        public void EndsWith()
        {
            Assert.AreEqual(true, "asdfg".EndsWith('g'));
            Assert.AreEqual(true, "asdfgÉ".EndsWith('É'), "Unicode");
            Assert.AreEqual(true, "asdfg\t".EndsWith('\t'), "Invisible");
            Assert.AreEqual(false, "aAdfg".EndsWith('G'), "Should be case-sensitive");
            Assert.AreEqual(false, "asdfgH ".StartsWith('H'), "Trimmed");
            Assert.AreEqual(false, "".EndsWith('A'), "Empty string - should always return false");

            TestUtil.AssertThrows(typeof(NullReferenceException), () => ((string)null).EndsWith('x'));
            TestUtil.AssertThrows(typeof(NullReferenceException), () => ((string)null).EndsWith("x")); // testing the behaviour of System.String, to check that our method is equivalent
        }

        [TestMethod]
        public void ConcatWithSeparatorsTrim()
        {
            int? x = null;

            Assert.AreEqual("", StrUtils.ConcatWithSeparatorsTrim());
            Assert.AreEqual("one part", StrUtils.ConcatWithSeparatorsTrim("one part"));
            Assert.AreEqual("", StrUtils.ConcatWithSeparatorsTrim(x), "null");
            Assert.AreEqual("[1", StrUtils.ConcatWithSeparatorsTrim("[", 1));
            Assert.AreEqual("[1]", StrUtils.ConcatWithSeparatorsTrim("[", 1, ']'));
            Assert.AreEqual("1 b]", StrUtils.ConcatWithSeparatorsTrim( 1, " ", "b", ']'));
            Assert.AreEqual("1", StrUtils.ConcatWithSeparatorsTrim(1, " ", ""), "Separator excluded before blank");
            Assert.AreEqual("1", StrUtils.ConcatWithSeparatorsTrim(1, " ", null), "Separator excluded before null");  // string trailer
            Assert.AreEqual("second", StrUtils.ConcatWithSeparatorsTrim(null, " ", "second"), "Separator excluded after null");
            Assert.AreEqual("firstsecond - third €", StrUtils.ConcatWithSeparatorsTrim("first", null, "second", " - ", "third €"), "no trailer");
            Assert.AreEqual("firstsecond - third €", StrUtils.ConcatWithSeparatorsTrim("first", null, "second", " - ", "third €", null));
        }

        [TestMethod]
        public void ConcatWithSeparatorsTrimEnclosed()
        {
            Assert.AreEqual("", StrUtils.ConcatWithSeparatorsTrimEnclosed("["));
            Assert.AreEqual("[1", StrUtils.ConcatWithSeparatorsTrimEnclosed("[",1));
            Assert.AreEqual("[1]", StrUtils.ConcatWithSeparatorsTrimEnclosed("[", 1, ']'));
            Assert.AreEqual("[1 b]", StrUtils.ConcatWithSeparatorsTrimEnclosed("[", 1, " ", "  b ", ']'));
            Assert.AreEqual("[1]", StrUtils.ConcatWithSeparatorsTrimEnclosed("[", 1, " ", "", ']'), "Separator excluded before blank");
            Assert.AreEqual("[1]", StrUtils.ConcatWithSeparatorsTrimEnclosed("[", 1, " ", null, "]"), "Separator excluded before null");  // string trailer
            Assert.AreEqual("[second]", StrUtils.ConcatWithSeparatorsTrimEnclosed("[", null, " ", " second  ", ']'), "Separator excluded after null; trim");
            Assert.AreEqual("[firstsecond - third €", StrUtils.ConcatWithSeparatorsTrimEnclosed("[", "first", null, "second", " - ", "third €"), "no trailer");
            Assert.AreEqual("firstsecond - third €", StrUtils.ConcatWithSeparatorsTrimEnclosed(null, "first", null, "second", " - ", "third €", null), "no leader");
            Assert.AreEqual("", StrUtils.ConcatWithSeparatorsTrimEnclosed("<", null, ",", "", " ", "", ">"), "all null/blank except separators, leader and trailer");
            Assert.AreEqual(" < a > ", StrUtils.ConcatWithSeparatorsTrimEnclosed(" < ", "a", " > "), "shouldn't trim leader/trailer");
        }

        [TestMethod]
        public void SplitAtInclusive()
        {
            string first, second;

            // Act:
            StrUtils.SplitAtInclusive("abcdefgh", 4, out first, out second);

            // Assert:
            Assert.AreEqual(first, "abcd");
            Assert.AreEqual(second, "efgh");
        }

        [TestMethod]
        public void SplitAtInclusive_0()
        {
            string first, second;

            // Act:
            StrUtils.SplitAtInclusive("abcdefgh", 0, out first, out second);

            // Assert:
            Assert.AreEqual(first, "");
            Assert.AreEqual(second, "abcdefgh");
        }

        [TestMethod]
        public void SplitOn()
        {
            string first, second;

            // Act:
            StrUtils.SplitOn("abcd-efghi", 4, out first, out second);

            // Assert:
            Assert.AreEqual(first, "abcd");
            Assert.AreEqual(second, "efghi");
        }

        [TestMethod]
        public void SplitOn_0()
        {
            string first, second;

            // Act:
            StrUtils.SplitOn("abcdefgh", 0, out first, out second);

            // Assert:
            Assert.AreEqual(first, "");
            Assert.AreEqual(second, "bcdefgh");
        }

        [TestMethod]
        public void ReplaceSubstringBetween()
        {
            Assert.AreEqual("dfasdfdsf sdfsadfd df [NEW VALUE] end of string", 
                "dfasdfdsf sdfsadfd df [to be replaced] end of string".ReplaceSubstringBetween("[","]","NEW VALUE"));

            Assert.AreEqual("before /*(new value /*test*/)*/ after\t ...",
                "before /*/ to be replaced */ after\t ...".ReplaceSubstringBetween("/*", "*/", "(new value /*test*/)"), "overlapping delimiters");

            Assert.AreEqual("before /*/ to be replaced */ after\t ...",
                "before /*/ to be replaced */ after\t ...".ReplaceSubstringBetween("/*", "}", "(new value /*test*/)"), "end delimiter not found");
            Assert.AreEqual("before /*/ to be replaced */ after\t ...",
                "before /*/ to be replaced */ after\t ...".ReplaceSubstringBetween("{", "*/", "(new value /*test*/)"), "start delimiter not found");
            Assert.AreEqual("before /*/ to be replaced */ after\t ...",
                "before /*/ to be replaced */ after\t ...".ReplaceSubstringBetween("{", "}", "(new value /*test*/)"), "delimiters not found");

            Assert.AreEqual("(new value)| after",
                "to be replaced | after".ReplaceSubstringBetween(null, "|", "(new value)"), "no start delimiter");
            Assert.AreEqual("before |(new value)",
                "before | to be replaced".ReplaceSubstringBetween("|", null, "(new value)"), "no end delimiter");
        }
    }
}
