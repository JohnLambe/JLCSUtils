using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Text;

namespace JohnLambe.Util.Types
{
    /// <summary>
    /// Represents a person's name, and combines and parses parts of it.
    /// </summary>
    //[ComplexType]
    public class PersonName
    {
        #region The parts of the name, as stored

        /// <summary>
        /// First name, and possibly middle name(s).
        /// </summary>
        public virtual string FirstNames { get; set; }
        public virtual string Surname { get; set; }
        /// <summary>
        /// Title/prefix.
        /// </summary>
        public virtual string Title { get; set; }

        //TODO: Consider removing.
        public virtual string Suffix { get; set; }
        /// <summary>
        /// Qualifications that appear after the name.
        /// </summary>
        public virtual string Qualifications { get; set; }

        #endregion

        /// <summary>
        /// Everything before the first SPACE in <see cref="FirstNames"/>, or all of it if there are no SPACEs.
        /// </summary>
        //[NotMapped]
        public virtual string FirstName
        {
            get
            {
                return FirstNames.SplitBefore(" ");
            }
            set
            {
                FirstNames = value + StrUtil.BlankPropagate(" ", MiddleNames);
            }
        }

        /// <summary>
        /// Everything before the first SPACE in <see cref="FirstNames"/>,
        /// or "" of there are no SPACEs.
        /// <para>Setting this replaces all middle names. null is treated the same as "".</para>
        /// </summary>
        //[NotMapped]
        public virtual string MiddleNames
        {
            get
            {
                return FirstNames.SplitAfter(" ");
            }
            set
            {
                FirstNames = StrUtil.ConcatWithSeparator(" ", FirstName, value);
            }
        }

        /// <summary>
        /// The first letter of the middle name (the second name if there are multiple middle names).
        /// Assigning this overwrites the middle name(s) with an initial.
        /// </summary>
        //[NotMapped]
        public virtual char? MiddleInitial
        {
            get
            {
                return MiddleNames?.CharAtNullable(0);
            }
            set
            {   // replace the whole middle name(s) with the initial.
                MiddleNames = value == null ? "" : "" + value + ".";
            }
        }

        //[NotMapped]
        public virtual string FullName
        {
            get
            {
                return
                    StrUtil.ConcatWithSeparatorsTrim(
                        StrUtil.ConcatWithSeparatorsTrimEnclosed(null,
                            Title, " ",
                            FirstNames, " ",
                            Surname, " ",
                            Suffix
                            ), ", ",
                            Qualifications
                    );
            }
            set
            {
                var parts = value.NullToBlank().Split(',');
                parts[0] = parts[0].NullToBlank().Trim();

                int index = parts[0].LastIndexOf(' ');
                Surname = parts[0].Substring(index + 1);

                //TODO: Identfy title at start of this part.
                //TODO: Identify suffix at end of this part.
                FirstNames = parts[0].Substring(0,index);

                Qualifications = parts.ElementAtOrDefault(1)?.Trim();
            }
        }

        /// <summary>
        /// Full name, with the surname first, followed by a comma.
        /// </summary>
        //[NotMapped]
        public virtual string FormalFullName
        {
            get
            {
                return 
                    StrUtil.ConcatWithSeparatorsTrim(
                        StrUtil.ConcatWithSeparatorsTrimEnclosed(null,
                            Surname, ", ",
                            Title, " ",
                            FirstNames, " ",
                            Suffix
                            ), ", ",
                            Qualifications
                    );
            }
            set
            {
                var parts = value.Split(',');
                Surname = parts.ElementAtOrDefault(0)?.Trim();

                //TODO: Identfy title at start of this part.
                //TODO: Identify suffix at end of this part.
                FirstNames = parts.ElementAtOrDefault(1)?.Trim();

                Qualifications = parts.ElementAtOrDefault(2)?.Trim();
            }
        }

        /// <summary>
        /// Name in the format: '{Firstnames} {Surname}'.
        /// <para>
        /// Assigning this overwrites <see cref="FirstNames"/> and <see cref="Surname"/>.
        /// The surname is assumed to be everything after the last space, unless there is no space,
        /// in which the whole string is assumed to be the surname.
        /// If there is a comma, the part before the comma is the surname.
        /// </para>
        /// </summary>
        //[NotMapped]
        public virtual string SimpleName
        {
            get
            {
                return StrUtil.ConcatWithSeparatorTrim(" ", FirstNames, Surname);
            }
            set
            {
                if (value == null)
                {
                    FirstNames = null;
                    Surname = null;
                }
                else
                {
                    if (value.Contains(','))
                    {
                        string firstNames, surname;
                        value.SplitOn(value.LastIndexOf(","), out surname, out firstNames);
                        FirstNames = firstNames?.Trim();
                        Surname = surname?.Trim();
                    }
                    else if (value.Contains(' '))
                    {
                        string firstNames, surname;
                        value.SplitOn(value.LastIndexOf(" "), out firstNames, out surname);
                        FirstNames = firstNames?.Trim();
                        Surname = surname?.Trim();
                    }
                    else
                    {
                        Surname = value;
                        FirstNames = null;
                    }
                }
            }
        }

        /// <summary>
        /// A string value that encodes the full state of this instance.
        /// </summary>
        //[NotMapped]
        public virtual string EncodedString
        {
            get
            {
                return Surname + Separator + FirstNames + Title + Separator + Suffix + Separator + Qualifications;
                // the order is chosen to be useful for sorting.
            }
            set
            {
                var parts = value.Split(Separator);
                Surname = parts[0];
                FirstNames = parts[1];
                Title = parts[2];
                Suffix = parts[3];
                Qualifications = parts[4];
            }
        }

        public const char Separator = '\t';

        public override string ToString()
        {
            return FullName;
        }
    }
}
