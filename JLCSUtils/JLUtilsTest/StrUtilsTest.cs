using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using JohnLambe.Util;
using JohnLambe.Util.Text;
using System.Collections;
using System.Collections.Generic;
using JohnLambe.Util.Collections;

namespace JohnLambe.Tests.JLUtilsTest
{
    [TestClass]
    public class StrUtilsTest
    {
        [TestMethod]
        public void RemovePrefix()
        {
            Assert.AreEqual("Value-Prefix:asd", StrUtil.RemovePrefix("Prefix:Value-Prefix:asd", "Prefix:"));
            Assert.AreEqual("Prefix:Value-Prefix:asd", StrUtil.RemovePrefix("Prefix:Value-Prefix:asd", "PrefiX:"), "Wrongly case insensitive");
            Assert.AreEqual("Value-Prefix:asd", StrUtil.RemovePrefix("ASD:Value-Prefix:asd", "asD:", StringComparison.InvariantCultureIgnoreCase), "Non-case-sensitive");

            Assert.AreEqual(null, StrUtil.RemovePrefix(null,null, StringComparison.InvariantCultureIgnoreCase), "null");
        }

        [TestMethod]
        public void RemoveSuffix()
        {
            Assert.AreEqual("12345678", StrUtil.RemoveSuffix("1234567890", "90"));
            Assert.AreEqual("asdfghjkl", StrUtil.RemoveSuffix("asdfghjkl", "KL"), "Wrongly case insensitive");
            Assert.AreEqual("qwertyui", StrUtil.RemoveSuffix("qwertyuiop", "Op", StringComparison.InvariantCultureIgnoreCase), "Non-case-sensitive");
            Assert.AreEqual("test string", StrUtil.RemoveSuffix("test string", "", StringComparison.InvariantCultureIgnoreCase), "Should have returned unmodified");

            Assert.AreEqual(null, StrUtil.RemoveSuffix(null, "90"), "null");
        }

        [TestMethod]
        public void TotalLength()
        {
            Assert.AreEqual(13, StrUtil.TotalLength(2,new string[] {"asd","f",null," . . "}));
            Assert.AreEqual(0, StrUtil.TotalLength(2,null), "wrong length when no parts");
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
            parts[0] = StrUtil.SplitFirst('.', ref a);
            Assert.AreEqual("second..4th", a);

            parts[1] = StrUtil.SplitFirst('.', ref a);
            Assert.AreEqual(".4th", a);

            parts[2] = StrUtil.SplitFirst('.', ref a);
            Assert.AreEqual("4th", a);

            parts[3] = StrUtil.SplitFirst('.', ref a);
            Assert.AreEqual(null, a);

            // Nothing remaining at this point. Subsequent calls will return null.
            parts[4] = StrUtil.SplitFirst('.', ref a);

            // Assert:
            Assert.IsTrue(parts.SequenceEqual(
                new string[] { "first", "second", "", "4th", null, null }));
            Assert.AreEqual(null, a);
        }

        [TestMethod]
        public void SplitBefore()
        {
            Assert.AreEqual(null, StrUtil.SplitBefore(null, ":"));
            Assert.AreEqual("first", "first/second".SplitBefore("/"));
            Assert.AreEqual("a:b:c", "a:b:c::d:e:f::g".SplitBefore("::"));
        }

        [TestMethod]
        public void SplitAfter()
        {
            Assert.AreEqual(null, StrUtil.SplitAfter(null, "a"));
            Assert.AreEqual("second", "firstzsecond".SplitAfter("z"));
            Assert.AreEqual("d:e:f::g", "a:b:c::d:e:f::g".SplitAfter("::"));
        }

        #endregion

        [TestMethod]
        public void ConcatWithSeparator()
        {
            Assert.AreEqual("asd/f//gh/i",StrUtil.ConcatWithSeparator("/","asd","f/",null,"gh","","i"));
        }

        [TestMethod]
        public void NullToBlank()
        {
            Assert.AreEqual("", StrUtil.NullToBlank(null));
            Assert.AreEqual("asd ", StrUtil.NullToBlank("asd "));
        }

        [TestMethod]
        public void FirstNonBlank()
        {
            Assert.AreEqual("third", StrUtil.FirstNonBlank("",null,"third",null,"fifth"));
        }

        [TestMethod]
        public void DoubleQuote()
        {
            Assert.AreEqual("\"value\"", "value".DoubleQuote(), "no quotes");
            Assert.AreEqual("\"value\"\"1234\"", "value\"1234".DoubleQuote());
        }

        [TestMethod]
        public void CharAt()
        {
            Assert.AreEqual('d', "Éb\0cdefghi".CharAt(4,'x'));
            Assert.AreEqual('#', StrUtil.CharAt(null, 4, '#'), "null");
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
            Assert.AreEqual(null, StrUtil.EndSubstring(null,0));
        }

        [TestMethod]
        public void NullPropagate()
        {
            Assert.AreEqual(null, StrUtil.NullPropagate("first",null,"third"));
            Assert.AreEqual("firstthird", StrUtil.NullPropagate("first", "", "third"));
            Assert.AreEqual(null, StrUtil.NullPropagate());
            Assert.AreEqual("", StrUtil.NullPropagate("",""));
        }

        [TestMethod]
        public void ConcatForEach()
        {
            Assert.AreEqual("<2> <4> <6>",StrUtil.ConcatForEach(new int[] { 2, 4, 6 }, x => "<" + x + ">", " "));
            Assert.AreEqual("<2><4><6>", StrUtil.ConcatForEach(new int[] { 2, 4, 6 }, x => "<" + x + ">"));
            Assert.AreEqual("#a -> #b -> #c", StrUtil.ConcatForEach(new string[] { "a", null, "b", "c" }, x => x == null ? null : "#" + x, " -> "));
            Assert.AreEqual("12 -> 22 -> 32", StrUtil.ConcatForEach(new float?[] { 10, 20, null, 30 }, x => x + 2, " -> "));
        }

        [TestMethod]
        public void RemoveCharacters()
        {
            Assert.AreEqual("12345", "asd1fgh23jkl45í".RemoveCharacters(CharacterUtil.Digits,true));
            Assert.AreEqual("word1word2é", "word1 word-2 é".RemoveCharacters(" -"));

            Assert.AreEqual(null, ((string)null).RemoveCharacters("12345"));
            Assert.AreEqual(null, ((string)null).RemoveCharacters(null));
            Assert.AreEqual("the string", "the string".RemoveCharacters(null));
        }

        [TestMethod]
        public void Repeat()
        {
            Assert.AreEqual("éfgéfgéfgéfgéfg", StrUtil.Repeat("éfg", 5));
            Assert.AreEqual("", StrUtil.Repeat("qwerty", 0));

            Assert.AreEqual(null, StrUtil.Repeat(null, 10));
            TestUtil.AssertThrows(typeof(ArgumentException), () => StrUtil.Repeat("xyz", -1));
        }

        [TestMethod]
        public void IsEnclosedIn()
        {
            Assert.AreEqual(true, "(test)".IsEnclosedIn("(",")"));
            Assert.AreEqual(true, "'asdfg'".IsEnclosedIn("'"));

            Assert.AreEqual(true, "##".IsEnclosedIn("#", "#"));
            Assert.AreEqual(false, "'".IsEnclosedIn("'"));

            Assert.AreEqual(true, "<--JKL->".IsEnclosedIn("<--", "->"), "different length prefix and suffix");

            Assert.AreEqual(false, StrUtil.IsEnclosedIn(null, "%"), "null");
        }

        [TestMethod]
        public void IsEnclosedIn_Char()
        {
            Assert.AreEqual(true, "<AAA>".IsEnclosedIn('<', '>'));
            Assert.AreEqual(true, "%qwertá%".IsEnclosedIn('%'));

            Assert.AreEqual(false, "<AAA> ".IsEnclosedIn('<', '>'));
            Assert.AreEqual(false, "".IsEnclosedIn('%'));
            Assert.AreEqual(false, "%".IsEnclosedIn('%'));  // the same character can't count as both `prefix` and `suffix`
            Assert.AreEqual(false, StrUtil.IsEnclosedIn(null,'%'), "null");
        }

        [TestMethod]
        public void ExtractEnclosed()
        {
            Assert.AreEqual("Ábc", StrUtil.ExtractEnclosed("/Ábc/","/"));
            Assert.AreEqual(null, StrUtil.ExtractEnclosed("zxcvbnm", "/"), "null");
            Assert.AreEqual("ExtractEnclosed", StrUtil.ExtractEnclosed("(*ExtractEnclosed*)", "(*","*)"));
        }

        [TestMethod]
        public void ExtractEnclosed_Char()
        {
            Assert.AreEqual("sdf", StrUtil.ExtractEnclosed("asdfg", 'a', 'g'));
            Assert.AreEqual("", StrUtil.ExtractEnclosed("@@", '@', '@'),"empty enclosed string");
            Assert.AreEqual(null, StrUtil.ExtractEnclosed("]a[", '[', ']'));
            Assert.AreEqual(null, StrUtil.ExtractEnclosed(null, '[', ']'),"null");
        }

        [TestMethod]
        public void OverwriteSubstring()
        {
            Assert.AreEqual("qweABCDiop", StrUtil.OverwriteSubstring("qwertyuiop",3,"ABCD"));
            Assert.AreEqual("qweABCDEFGHIJK", StrUtil.OverwriteSubstring("qwertyuiop", 3, "ABCDEFGHIJK"));

            //TODO: null
        }

        [TestMethod]
        public void OverwriteSubstringNoExtend()
        {
            Assert.AreEqual("qweABCDiop", StrUtil.OverwriteSubstring("qwertyuiop", 3, "ABCD", false));
            Assert.AreEqual("qweABCDEFG", StrUtil.OverwriteSubstring("qwertyuiop", 3, "ABCDEFGHIJK", false));

            //TODO: null
        }

        [TestMethod]
        public void ReplaceSubstring()
        {
            Assert.AreEqual("qweABCDyuiop", StrUtil.ReplaceSubstring("qwertyuiop", 3, 2, "ABCD"));

            Assert.AreEqual("qweABCDrtyuiop", StrUtil.ReplaceSubstring("qwertyuiop", 3, 0, "ABCD"), "0-length section being replaced");

            TestUtil.AssertThrows<ArgumentException>(
                () => StrUtil.ReplaceSubstring("qwertyuiop", 3, -1, "ABCD"));
            TestUtil.AssertThrows<ArgumentException>(
                () => StrUtil.ReplaceSubstring("qwertyuiop", -0x1000000, 5, "ABCD"));

            //TODO: null
        }

        [TestMethod]
        public void ReplaceSubstring_WithNull()
        {
            Assert.AreEqual("qwéyuiop", StrUtil.ReplaceSubstring("qwértyuiop", 3, 2, null));
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

            Assert.AreEqual("", StrUtil.ConcatWithSeparatorsTrim());
            Assert.AreEqual("one part", StrUtil.ConcatWithSeparatorsTrim("one part"));
            Assert.AreEqual("", StrUtil.ConcatWithSeparatorsTrim(x), "null");
            Assert.AreEqual("[1", StrUtil.ConcatWithSeparatorsTrim("[", 1));
            Assert.AreEqual("[1]", StrUtil.ConcatWithSeparatorsTrim("[", 1, ']'));
            Assert.AreEqual("1 b]", StrUtil.ConcatWithSeparatorsTrim( 1, " ", "b", ']'));
            Assert.AreEqual("1", StrUtil.ConcatWithSeparatorsTrim(1, " ", ""), "Separator excluded before blank");
            Assert.AreEqual("1", StrUtil.ConcatWithSeparatorsTrim(1, " ", null), "Separator excluded before null");  // string trailer
            Assert.AreEqual("second", StrUtil.ConcatWithSeparatorsTrim(null, " ", "second"), "Separator excluded after null");
            Assert.AreEqual("firstsecond - third €", StrUtil.ConcatWithSeparatorsTrim("first", null, "second", " - ", "third €"), "no trailer");
            Assert.AreEqual("firstsecond - third €", StrUtil.ConcatWithSeparatorsTrim("first", null, "second", " - ", "third €", null));
        }

        [TestMethod]
        public void ConcatWithSeparatorsTrimEnclosed()
        {
            Assert.AreEqual("", StrUtil.ConcatWithSeparatorsTrimEnclosed("["));
            Assert.AreEqual("[1", StrUtil.ConcatWithSeparatorsTrimEnclosed("[",1));
            Assert.AreEqual("[1]", StrUtil.ConcatWithSeparatorsTrimEnclosed("[", 1, ']'));
            Assert.AreEqual("[1 b]", StrUtil.ConcatWithSeparatorsTrimEnclosed("[", 1, " ", "  b ", ']'));
            Assert.AreEqual("[1]", StrUtil.ConcatWithSeparatorsTrimEnclosed("[", 1, " ", "", ']'), "Separator excluded before blank");
            Assert.AreEqual("[1]", StrUtil.ConcatWithSeparatorsTrimEnclosed("[", 1, " ", null, "]"), "Separator excluded before null");  // string trailer
            Assert.AreEqual("[second]", StrUtil.ConcatWithSeparatorsTrimEnclosed("[", null, " ", " second  ", ']'), "Separator excluded after null; trim");
            Assert.AreEqual("[firstsecond - third €", StrUtil.ConcatWithSeparatorsTrimEnclosed("[", "first", null, "second", " - ", "third €"), "no trailer");
            Assert.AreEqual("firstsecond - third €", StrUtil.ConcatWithSeparatorsTrimEnclosed(null, "first", null, "second", " - ", "third €", null), "no leader");
            Assert.AreEqual("", StrUtil.ConcatWithSeparatorsTrimEnclosed("<", null, ",", "", " ", "", ">"), "all null/blank except separators, leader and trailer");
            Assert.AreEqual(" < a > ", StrUtil.ConcatWithSeparatorsTrimEnclosed(" < ", "a", " > "), "shouldn't trim leader/trailer");
        }

        [TestMethod]
        public void SplitAtInclusive()
        {
            string first, second;

            // Act:
            StrUtil.SplitAtInclusive("abcdefgh", 4, out first, out second);

            // Assert:
            Assert.AreEqual(first, "abcd");
            Assert.AreEqual(second, "efgh");
        }

        [TestMethod]
        public void SplitAtInclusive_0()
        {
            string first, second;

            // Act:
            StrUtil.SplitAtInclusive("abcdefgh", 0, out first, out second);

            // Assert:
            Assert.AreEqual(first, "");
            Assert.AreEqual(second, "abcdefgh");
        }

        [TestMethod]
        public void SplitOn()
        {
            string first, second;

            // Act:
            StrUtil.SplitOn("abcd-efghi", 4, out first, out second);

            // Assert:
            Assert.AreEqual(first, "abcd");
            Assert.AreEqual(second, "efghi");
        }

        [TestMethod]
        public void SplitOn_0()
        {
            string first, second;

            // Act:
            StrUtil.SplitOn("abcdefgh", 0, out first, out second);

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

        [TestMethod]
        public void ContainsAnyCharacters()
        {
            Assert.AreEqual(false, StrUtil.ContainsAnyCharacters("asdfghjkl", new HashSet<char> { 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p' }));
            Assert.AreEqual(true, StrUtil.ContainsAnyCharacters("ZXCVBNM", new HashSet<char> { 'é', 'q', 'w', 'e', 'r', 't', 'y', 'B', 'u', 'i', 'o', 'p' }));

            Assert.AreEqual(true, StrUtil.ContainsOnlyCharacters("ABCDDEFABD", new HashSet<char> { 'A', 'B', 'C', 'X', 'D', 'E', 'F' }));
            Assert.AreEqual(true, StrUtil.ContainsOnlyCharacters("ABCDDEFABD", new HashSet<char> { 'A', 'B', 'C', 'D', 'F', 'E' }), "Set exactly matches the characters in the string");

            Assert.AreEqual(false, StrUtil.ContainsAnyCharacters(null, new HashSet<char> { 'q', 'w', 'e', 'r', 't', 'y', 'B', 'u', 'i', 'o', 'p' }));
            Assert.AreEqual(false, StrUtil.ContainsAnyCharacters(null, null));
            Assert.AreEqual(false, StrUtil.ContainsAnyCharacters("ZXCVBNM", null));
        }

        [TestMethod]
        public void ContainsOnlyCharacters()
        {
            Assert.AreEqual(false, StrUtil.ContainsOnlyCharacters("asdfghjkl", new HashSet<char> { 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p' }), "Contains none of them");
            Assert.AreEqual(false, StrUtil.ContainsOnlyCharacters("ZXCVBNM", new HashSet<char> { 'é', 'q', 'w', 'e', 'r', 't', 'y', 'B', 'u', 'i', 'o', 'p' }), "Contains one of them");

            Assert.AreEqual(true, StrUtil.ContainsOnlyCharacters("ABCDDEFABD", new HashSet<char> { 'A', 'B', 'C', 'X', 'D', 'E', 'F' }));
            Assert.AreEqual(true, StrUtil.ContainsOnlyCharacters("ABCDDEFABD", new HashSet<char> { 'A', 'B', 'C', 'D', 'F', 'E' }), "Set exactly matches the characters in the string");

            Assert.AreEqual(true, StrUtil.ContainsOnlyCharacters(null, new HashSet<char> { 'q', 'w', 'e', 'r', 't', 'y', 'B', 'u', 'i', 'o', 'p' }));
            Assert.AreEqual(true, StrUtil.ContainsOnlyCharacters(null, null));
            Assert.AreEqual(false, StrUtil.ContainsOnlyCharacters("ZXCVBNM", null));
        }
    }
}
