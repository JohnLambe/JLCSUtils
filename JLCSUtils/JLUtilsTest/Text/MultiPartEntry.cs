using JohnLambe.Util.Math;
using JohnLambe.Util.Misc;
using JohnLambe.Util.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace JohnLambe.Util
{
    public class MultiPartEntry
    {

        public void Clear()
        {
            Parts = new EntryPart[PartFormats.Length];
            for (int p = 0; p < PartFormats.Length; p++)
            {
                Parts[p].Format = PartFormats[p];
            }
        }

        public void Append(char c)
        {
            if (c == Ascii.BS && CurrentPart.IsBlank)
            {
                CurrentPartIndex--;
                //                Parts[CurrentPartIndex - 1].IsFinished = false;
            }
            else
            {
                CurrentPart.Append(c);
                if (CurrentPart.IsFinished)
                    CurrentPartIndex++;
            }
        }

        public virtual int CurrentPartIndex
        {
            get { return _currentPartIndex; }
            protected set { _currentPartIndex = RangeUtil.ConstrainToRange(value, 0, Parts.Length - 1); }
        }
        protected int _currentPartIndex = 0;
        /*
        {
            get
            {
                for (int p = 0; p < Parts.Length; p++)
                {
                    if (!Parts[p].IsFinished)
                        return p - 1;
                }
                return Parts.Length - 1;
            }
        }
        */

        public virtual EntryPart CurrentPart
        {
            get
            {
                var i = CurrentPartIndex;
                if (i < 0)
                    return Parts[0];
                else
                    return Parts[i];
            }
        }

        public EntryPartFormat[] PartFormats = new EntryPartFormat[] { new EntryPartFormat(), new EntryPartFormat(), new EntryPartFormat(4, "") };
        protected EntryPart[] Parts;


        /// <summary>
        /// The value being input (possibly incomplete) for display.
        /// </summary>
        public virtual string DisplayValue
        {
            get
            {
                _displayValue.Clear();
                foreach (var part in Parts)
                {
                    _displayValue.Append(part.Value);
                }
                return _displayValue.ToString();
            }
        }

        /// <summary>
        /// Reused between calls, for efficiency.
        /// </summary>
        protected StringBuilder _displayValue = new StringBuilder(20);
    }

    public class DateTimePartEntry : MultiPartEntry
    {
        /// <summary>
        /// The datetime value.
        /// null if not enough is entered to provide a value, i.e. if the year is not entered.
        /// </summary>
        public virtual DateTime? Value { get; }

    }


    public class EntryPartFormat
    {
        public EntryPartFormat(int length = 2, string separator = "-")
        {
            this.Length = length;
            this.Separator = separator;
        }

        /// <summary>
        /// The maximum length (or fixed length) of this part.
        /// </summary>
        public virtual int Length { get; set; }

        /// <summary>
        /// The separator after this part, as displayed.
        /// </summary>
        public virtual string Separator { get; set; }

        /// <summary>
        /// Characters that can be typed to end input of this part (move to next part).
        /// </summary>
        public virtual string EndCharacters { get; set; } = "-./:; ";

        /// <summary>
        /// Pattern that the input must match even if incomplete.
        /// </summary>
        public virtual string Pattern { get; set; } = "^[0-9]*$";

        /// <summary>
        /// Padding character to use when the input value is incomplete.
        /// </summary>
        public virtual char? Padding { get; set; } = '_';

        //TODO:
        /// <summary>
        /// Padding character to use when the value is fully input.
        /// </summary>
        public virtual char? PaddingComplete { get; set; } = '_';

        public virtual TextAlignment Alignment { get; set; } = TextAlignment.Left;

        public virtual string Pad(string value)
        {
            if (Padding.HasValue)
                return value?.Pad(Length, Alignment, Padding.Value);
            else
                return value;
        }
    }

    public class EntryPart
    {
        public EntryPart(EntryPartFormat format)
        {
            this.Format = format;
        }

        public void Append(char c)
        {
            if (c == Ascii.BS)
            {
                _inputValue.RemoveLast();
                IsFinished = false;
            }
            else if (Format.EndCharacters.Contains(c))    // if a separator character
            {
                IsFinished = true;
            }
            else
            {
                _inputValue.Append(c);
                if (!Regex.IsMatch(_inputValue.ToString(), Format.Pattern))
                {
                    _inputValue.RemoveLast();
                    if (_inputValue.Length == Format.Length)
                        IsFinished = true;
                }
            }
        }

        public virtual EntryPartFormat Format
        {
            get { return _format; }
            set
            {
                _format = value;
                _inputValue = new StringBuilder(Format?.Length ?? 4);
            }
        }
        private EntryPartFormat _format;

        /// <summary>
        /// The (possibly incomplete) value for display.
        /// </summary>
        public virtual string Value => Format?.Pad(_inputValue.ToString()) + Format?.Separator;

        /// <summary>
        /// true if input of this part is complete.
        /// </summary>
        public virtual bool IsFinished { get; protected set; }

        /// <summary>
        /// true iff this part is blank (nothing entered).
        /// </summary>
        public virtual bool IsBlank => _inputValue?.ToString()?.Equals("") ?? true;

        protected StringBuilder _inputValue;
    }

}
