using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Text;

namespace JohnLambe.Util.Types
{
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
                FirstNames = value + StrUtils.BlankPropagate(" ", MiddleNames);
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
                FirstNames = StrUtils.ConcatWithSeparator(" ", FirstName, value);
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
                    StrUtils.ConcatWithSeparatorsTrimEnclosed("",
                        Title, " ",
                        FirstNames, " ",
                        Surname, " ",
                        Suffix, ", ",
                        Qualifications);
            }
            /*
            set
            {

            }
            */
        }

        //[NotMapped]
        public virtual string FormalFullName
        {
            get
            {
                return StrUtils.ConcatWithSeparatorsTrimEnclosed("",
                        Surname, ", ",
                        Title, " ",
                        FirstNames, " ",
                        Suffix, ", ",
                        Qualifications
                    );
            }
            /*
            set
            {

            }
            */
        }

        /// <summary>
        /// Name in the format: '{Firstnames} {Surname}'.
        /// Assigning this overwrites <see cref="FirstNames"/> and <see cref="Surname"/>.
        /// </summary>
        //[NotMapped]
        public virtual string SimpleName
        {
            get
            {
                return StrUtils.ConcatWithSeparatorTrim(" ", FirstNames, Surname);
            }
            set
            {
                string firstNames, surname;
                value.SplitOn(value.LastIndexOf(" "), out firstNames, out surname);
                FirstNames = firstNames;
                Surname = surname;
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
