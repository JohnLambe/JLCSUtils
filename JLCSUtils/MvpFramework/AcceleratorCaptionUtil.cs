using JohnLambe.Util.Types;
using JohnLambe.Util;
using JohnLambe.Util.Collections;
using JohnLambe.Util.Diagnostic;
using JohnLambe.Util.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework
{
    /// <summary>
    /// Utility for working with WinForms accelerator characters in captions (letters prefixed with '&' to indicate the shortcut key, using <see cref="KeyboardKey.Alt"/>).
    /// <para>
    /// This adds/removes accelerator characters, and tracks what characters are used in its context (for checking for duplicates).
    /// When there is no suitable character in the caption to make the acceleratotr character, a character can be added for use as an accelerator character (refered to here as an "Added Accelerator Character").
    /// </para>
    /// </summary>
    public class AcceleratorCaptionUtil
    {
        /// <summary>
        /// The character that indicates that the character immediately following it is the accelerator character.
        /// </summary>
        public const char AcceleratorIndicator = '&';

        /// <summary>
        /// See <see cref="SetAccelerator(string, char?, ExistingAccelerartorAction?)"/>.
        /// </summary>
        public const char None = '\0';
        /// <summary>
        /// See <see cref="SetAccelerator(string, char?, ExistingAccelerartorAction?)"/>.
        /// </summary>
        public const char Auto = '\x01';

        /// <summary>
        /// See <see cref="AddedAcceleratorFormat"/>.
        /// </summary>
        public string Token_Original = "{0}";
        /// <summary>
        /// See <see cref="AddedAcceleratorFormat"/>.
        /// </summary>
        public string Token_Accelerator = "{1}";

        /// <summary>
        /// Convert a general string to a string that can be used a WinForms caption 
        /// (handling the <see cref="AcceleratorIndicator"/>, which would otherwise be displayed wrongly in WinForms).
        /// </summary>
        /// <param name="displayName">Human-readable string.</param>
        /// <param name="acceleratorCharacter">If not null, this will be the accelerator character in the returned caption string.</param>
        /// <returns>WinForms caption string.</returns>
        [return: Nullable]
        public virtual string DisplayNameToCaption([Nullable]string displayName, char? acceleratorCharacter = null)
        {
            return SetAccelerator(displayName, acceleratorCharacter, ExistingAccelerartorAction.ConvertToWord);
        }

        /// <summary>
        /// Converts a WinForms caption (which may have an accelerator character) to a standard string for display
        /// (without any special processing).
        /// </summary>
        /// <param name="caption"></param>
        /// <returns>String for display (human-readable).</returns>
        public virtual string CaptionToDisplayName([Nullable]string caption)
        {
            return RemoveAccelerator(caption);
        }

        /// <summary>
        /// Returns the accelerator character in a given caption.
        /// </summary>
        /// <param name="caption">A caption that may contain an accelerator character. If this is null, null is returned.</param>
        /// <returns>The accelerator character. null if there is none.</returns>
        [return: Nullable]
        public virtual char? GetAccelerator([Nullable] string caption)
        {
            if (caption == null)
                return null;
            int acceleratorPosition = caption.IndexOf(AcceleratorIndicator);
            if(acceleratorPosition > -1 && acceleratorPosition < caption.Length -1)   // if '&' found and not the last character
            {
                return caption[acceleratorPosition + 1];                     // return the character after it
            }
            return null;
        }

        /// <summary>
        /// Change the accelerator character of the given caption (it may or may not have one already).
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="accelerator">
        /// The character to be the accelerator character in the returned string.<br/>
        /// null or <see cref="None"/> to have no accelerator character.<br/>
        /// <see cref="Auto"/> to have an accelerator character automatically chosen. (If there is no valid one available, this silently assigns none).
        /// </param>
        /// <param name="existingAction">Overrides <see cref="ExistingAction"/>, if not null.</param>
        /// <returns>WinForms caption string.</returns>
        [return: Nullable]
        public virtual string SetAccelerator([Nullable]string caption, char? accelerator, ExistingAccelerartorAction? existingAction = null)
        {
            if (caption == null)
                return null;

            int position = -1000;   // index of the chosen accelerator character in the caption

            if (accelerator == None)
                accelerator = null;
            else if (accelerator == Auto)
                accelerator = ChooseAccelerator(caption, out position);

            char? existing = GetAccelerator(caption);       // the existing accelerator character, if any
            if(existing.HasValue)
            {
                switch(existingAction ?? ExistingAction)
                {
                    case ExistingAccelerartorAction.Remove:
//                        caption = caption.RemoveCharacter(AcceleratorIndicator);
                        caption = RemoveAccelerator(caption);
                        break;
                    case ExistingAccelerartorAction.ConvertToWord:
                        caption = ConvertAcceleratorIndicator(caption);
                        break;
                    case ExistingAccelerartorAction.Keep:
                        //TODO
                        break;
                    default:
                        Diagnostics.UnhandledEnum(ExistingAction);
                        break;
                }
            }

            if (IsUsed(accelerator))   // if the given character is already used
            {
                switch(DuplicateAction)
                {
                    case DuplicateAcceleratorAction.Exception:
                        throw new KeyExistsException("Duplicate accelerator key: " + accelerator + " (Trying to assign to '" + caption + "')");
                    case DuplicateAcceleratorAction.Allow:
                        break;
                    case DuplicateAcceleratorAction.NoAccelerator:
                        accelerator = null;
                        break;
                    case DuplicateAcceleratorAction.NoChange:
                        AddAcceleratorUsed(existing);   // add the existing character to the list
                        /*  Should we remove it if not unique? It could be in the list because if this control itself.
                            if (AddAcceleratorUsed(existing))   // add the existing character to the list
                        {
                            accelerator = null;
                        }
                        else
                        {
                            return caption;
                        }
                        */
                        break;
                    case DuplicateAcceleratorAction.Change:
                        accelerator = ChooseAccelerator(caption,out position);
                        break;
                    default:
                        Diagnostics.UnhandledEnum(DuplicateAction);
                        break;
                }
            }

            caption = RemoveAccelerator(caption);

            if (accelerator != null)
            {
                if(position < 0)                  // if the position is not already determined
                    position = ChooseAcceleratorPosition(caption, accelerator.Value);   // choose the best instance of this character in the caption
                caption = AddAccelerator(caption, accelerator.Value, position);         // modify the caption - add the accelerator character

                AcceleratorsUsed?.Add(Char.ToUpper(accelerator.Value));              // add to the list of used characters
            }

            return caption;
        }

        public virtual string GenerateAccelerator([Nullable]string caption, ExistingAccelerartorAction? existingAction = null)
        {
            return SetAccelerator(caption, Auto, existingAction);
        }

        /// <summary>
        /// Choose the character to use as an accelerator character for the given caption.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="chosenPosition">The 0-based index of the chosen character in the string.
        /// -1 if it has to be added to it.
        /// -2 if no character was chosen (null is returned).
        /// </param>
        /// <returns>The chosen character. null if no valid character was found (within the constraints set by properties of this class).</returns>
        [return: Nullable]
        protected virtual char? ChooseAccelerator([NotNull]string caption, out int chosenPosition)
        {
            foreach(var func in AllowedAcceleratorCharacters)
            {
                if (func == null)               // if the delegate is null
                {                               // choose a character to add
                    foreach (var candidateChar in AllowedAddedAcceleratorCharacters)
                    {
                        if (!IsUsed(candidateChar))      // if not already used
                        {
                            chosenPosition = -1;
                            return candidateChar;
                        }
                    }
                }
                else   
                {   // evaluate the characters in the caption according the delegate:
                    int bestScore = 0;
                    int bestPosition = -1;
                    char? best = null;
                    int position = 0;
                    foreach (var c in caption)
                    {
                        if (func(c))      // if this character is allowed
                        {
                            if (!IsUsed(c))      // if not already used
                            {
                                // we have a usable character. Evaluate it and compare to the best so far.
                                int score = ScoreAcceleratorCharacter(caption, position);
                                if(score > bestScore)      // if best so far
                                {
                                    best = c;
                                    bestScore = score;
                                    bestPosition = position;
                                }
                            }
                        }
                        position++;
                    }
                    
                    if(best != null)
                    {
                        chosenPosition = bestPosition;
                        return best;
                    }
                }
            }

            chosenPosition = -2;
            return null;
        }

        /// <summary>
        /// Decides which character in the caption to make the accelerator character, when the accelerator character is known.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="accelerator"></param>
        /// <returns>The index of the best character to use as the accelerator. -1 if it must be added.</returns>
        protected virtual int ChooseAcceleratorPosition([NotNull]string caption, char accelerator)
        {
            int position = -1;
            int best = -1;
            int bestScore = 0;
            do
            {
                position = caption.IndexOf("" + accelerator, position + 1, StringComparison.InvariantCultureIgnoreCase);  // find next occurrence
                if (position > -1)
                {
                    int score = ScoreAcceleratorCharacter(caption, position);
                    if (score > bestScore)   // if better than the best match so far
                    {
                        best = position;
                        bestScore = score;
                    }
                }
            } while (position > -1);    // until not found
            return best;
        }

        /// <summary>
        /// Replaces <see cref="AcceleratorIndicator"/> with the word "and",
        /// possibly heuristically adding adding space and capitalisation.
        /// </summary>
        /// <param name="caption"></param>
        /// <returns></returns>
        protected virtual string ConvertAcceleratorIndicator([NotNull]string caption)
        {
            return caption.Replace("" + AcceleratorIndicator, "and");
            //TODO: heuristic adding of space and capitalisation
        }

        /// <summary>
        /// Evaluate how desirable a character is to use an accelerator character.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="position">0-based index of the candidate accelerator character. Must be in range.</param>
        /// <returns>
        /// A value indiciating how desirable this character is to use an accelerator character.
        /// Higher values are more desirable.
        /// Any valid character must yield a positive result.
        /// </returns>
        protected virtual int ScoreAcceleratorCharacter([NotNull]string caption, int position)
        {
            int score = 10;
            if (PreferCapital && Char.IsUpper(caption[position]))
                score += 10;
            if (PreferStartOfWord && StrUtil.IsStartOfWord(caption, position))
            {
                score += 20;
                //TODO: Reduce score if at the start of a common conjunction and not the start of the string.
            }
            return score;
        }

        /// <summary>
        /// Modify a caption to make a character at a given position (or an added character) the accelerator character.
        /// </summary>
        /// <param name="caption">The caption to be modified.</param>
        /// <param name="accelerator">The character to be the accelerator character.</param>
        /// <param name="position">The index of the character to make the accelerator character. -1 to add it.</param>
        /// <returns></returns>
        [return: Nullable]
        protected virtual string AddAccelerator([Nullable]string caption, char accelerator, int position)
        {
            if (caption == null)
                return null;
            if(position >= 0)
                return caption.Insert(position, "" + AcceleratorIndicator);   // insert the '&'
            else
                return string.Format(AddedAcceleratorFormat, caption.Trim(), accelerator);   // add the character to the string as an accelerator character
        }

        /// <summary>
        /// Remove any accelerator character from the given caption (along with any prefix/suffix characters for an added accelerator character).
        /// </summary>
        /// <param name="caption"></param>
        /// <returns></returns>
        [return: Nullable]
        protected virtual string RemoveAccelerator([Nullable]string caption)
        {
            if (caption == null)
                return null;

            int acceleratorPosition = 0;
            do    // find all accelerator character and remove them, along with any prefix/suffix characters:
            {
                acceleratorPosition = caption.IndexOf(AcceleratorIndicator, acceleratorPosition);      // find next '&'
                                                                                                       // Using a starting offset of acceleratorPosition is not necessary (because previous matches have been removed) but probably improves performance.
                if (acceleratorPosition > -1)   // if '&' found
                {
                    if (acceleratorPosition > 0 && acceleratorPosition < caption.Length - 1
                        && (AddedAcceleratorPrefix.HasValue && caption[acceleratorPosition-1] == AddedAcceleratorPrefix)          // if a prefix is defined and mathces (a suffix without a prefix is not supported)
                        && (!AddedAcceleratorSuffix.HasValue || caption.CharAt(acceleratorPosition + 2) == AddedAcceleratorSuffix)       // if the suffix is NOT defined OR matches (because it is optional)
                        )
                    {
                        int start = acceleratorPosition - 1;
                        int len = AddedAcceleratorSuffix.HasValue ? 4 : 3;
                        if (caption.CharAt(acceleratorPosition - 2) == ' ')   // if preceded by SPACE
                        {
                            start--;
                            len++;
                        }
                        caption = caption.Remove(start, len);  // remove the character itself, the accelerator indicator ('&') and the enclosing characters
                    }
                    else
                    {
                        caption = caption.Remove(acceleratorPosition, 1);   // just remove the accelerator indicator ('&')
                    }
                }
            } while (acceleratorPosition > -1 && acceleratorPosition < caption.Length);   // repeat until there are no AcceleratorIndicator ('&') characters left
            return caption;
        }

        /// <summary>
        /// Clear the list of used accelerator characters.
        /// </summary>
        public virtual void Clear()
        {
            AcceleratorsUsed = new HashSet<char>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="acceleratorChar"></param>
        /// <returns>true iff <paramref name="acceleratorChar"/> is not null and is used (in <see cref="AcceleratorsUsed"/>.</returns>
        public virtual bool IsUsed(char? acceleratorChar)
        {
            return acceleratorChar==null ? false : AcceleratorsUsed?.Contains(Char.ToUpper(acceleratorChar.Value)) ?? false;
        }

        /// <summary>
        /// Add a character to the list of used acclerator characters.
        /// Does nothing if it already exists, or is null.
        /// </summary>
        /// <param name="accelerator"></param>
        /// <returns>true iff the accelerator character was already used.</returns>
        public virtual bool AddAcceleratorUsed([Nullable]char? accelerator)
        {
            if (accelerator != null)
            {
                accelerator = Char.ToUpper(accelerator.Value);
                if (AcceleratorsUsed?.Contains(accelerator.Value) ?? false)   // if we have a collection of used characters and this is in it
                    return true;
                AcceleratorsUsed?.Add(accelerator.Value);
            }
            return false;
        }

        /// <summary>
        /// Accelerator characters being used in the current context.
        /// null if this is not being tracked.
        /// This can be assigned if the consumer has a list of used or reserved characters,
        /// or set to null to disable tracking and checking for duplicates.
        /// </summary>
        public virtual ISet<char> AcceleratorsUsed { get; set; } = new HashSet<char>();


        #region Settings

        /// <summary>
        /// Letters at the start of a word (preceded by whitespace or punctuation) are preferred for accelerator keys.
        /// When combined with <see cref="PreferCapital"/>, this is preferred over other capital letters,
        /// but a letter that is both capital and at the start of a word is preferred over one which is either but not both of these.
        /// </summary>
        public virtual bool PreferStartOfWord { get; set; } = true;

        /// <summary>
        /// Letters which are capital in the caption are preferred for accelerator keys.
        /// </summary>
        public virtual bool PreferCapital { get; set; } = true;

        /// <summary>
        /// When a character is added to a caption for use as an accelerator key, this is the format of
        /// the whole caption after adding it.
        /// If null, a default is based on <see cref="AddedAcceleratorPrefix"/> and <see cref="AddedAcceleratorSuffix"/> is used.
        /// <para>
        /// <see cref="Token_Original"/> is the original caption with no accelerator character, with leading and trailing space removed;<br/>
        /// <see cref="Token_Accelerator"/> is the accelerator character.
        /// </para>
        /// <para>
        /// The case (capital or lowercase) of the added accelerator character is as it is passed to the method of this class,
        /// or in <see cref="AllowedAcceleratorCharacters"/> (if it was chosen automatically).
        /// </para>
        /// </summary>
        public virtual string AddedAcceleratorFormat
        {
            get
            {
                return _addedAcceleratorFormat ??
                    Token_Original + " " + AddedAcceleratorPrefix + AcceleratorIndicator + Token_Accelerator + (AddedAcceleratorSuffix?.ToString() ?? "");
            }
            set { _addedAcceleratorFormat = value; }
            // Default is "{0} [&{1}]"
        }
        protected string _addedAcceleratorFormat;

        /// <summary>
        /// An optional prefix, in the caption, of an accelerator character that is added to the caption.
        /// <para>
        /// When removing an accelerator character, this and <see cref="AddedAcceleratorSuffix"/> are also removed (when both are present, if they are defined),
        /// along with the added character itself
        /// (so don't use values that could occur by coincidence).
        /// If this is not used (null), the added accelerator character is not removed on reassigning or removing the accelerator character (though it would no longer be an accelerator character).
        /// </para>
        /// <para>
        /// The default is square brackets. If using this, it is recommended that square brackets should be used only for this purpose.
        /// </para>
        /// </summary>
        /// <seealso cref="AddedAcceleratorSuffix"/>
        public virtual char? AddedAcceleratorPrefix { get; set; } = '[';

        /// <summary>
        /// An optional suffix, in the caption, of an accelerator character that is added to the caption.
        /// <para>
        /// Currently, this is ignored if <see cref="AddedAcceleratorPrefix"/> is null (but it can be null when it is not, to have only a prefix).
        /// This could change in future versions, so don't set a suffix without a prefix.
        /// </para>
        /// </summary>
        /// <seealso cref="AddedAcceleratorPrefix"/>
        public virtual char? AddedAcceleratorSuffix { get; set; } = ']';

        /// <summary>
        /// Characters that may be chosen as accelerator characters, in ascending order of preference.
        /// A null means that a character will be added if a valid one can be found in <see cref="AllowedAddedAcceleratorCharacters"/>.
        /// </summary>
        public virtual IEnumerable<Func<char,bool>> AllowedAcceleratorCharacters { get; set; }
            = new List<Func<char,bool>>(
                new Func<char, bool>[]
                {
                    c => Char.IsLetter(c),
                    c => Char.IsDigit(c),
                    null,
    //                c => "+-*/=\".Contains(c)    // some punctuation?
                }
                );
        //| We could, alternatively, assign scores to characters or categories of characters and apply them in ScoreAcceleratorCharacter(...).

        /// <summary>
        /// Characters that may be added to a caption to use an an accelerator character,
        /// in order of preference.
        /// </summary>
        public virtual IEnumerable<char> AllowedAddedAcceleratorCharacters { get; set; }
            = CharacterUtil.AsciiCapitalLetters
            + "1234567890";   // start numbering at '1' (so that a series of items that all have numbers assigned starts at 1)

        /// <summary>
        /// Don't choose a character as an acclerator character unless it matches one of the preferred patterns,
        /// even if this means not assigning one at all.
        /// </summary>
        public virtual bool AssignPreferredOnly { get; set; }

        /// <summary>
        /// True to not add accelerator characters to strings, i.e. assign one only if a character existing in the caption can be used.
        /// </summary>
        public virtual bool AssignExistingOnly { get; set; }

        /// <summary>
        /// What to do with an existing accelerator character in a caption that is being assigned an accelerator character.
        /// </summary>
        public virtual ExistingAccelerartorAction ExistingAction { get; set; } = ExistingAccelerartorAction.ConvertToWord;

        /// <summary>
        /// What to do when trying to assign an accelerator character that is already used (in <see cref="AcceleratorsUsed"/>);
        /// </summary>
        public virtual DuplicateAcceleratorAction DuplicateAction { get; set; } = DuplicateAcceleratorAction.Exception;

        /*
        /// <summary>
        /// Iff true, accelerator characters set are added to <see cref="AcceleratorsUsed"/>,
        /// and trying to set one that is already in this fails.
        /// </summary>
        public virtual bool CheckForDuplicates { get; set; }
        */
        
        #endregion


        /// <summary>
        /// Actions when a caption already has an accelerator character.
        /// </summary>
        public enum ExistingAccelerartorAction
        {
            /// <summary>
            /// Remove the <see cref="AcceleratorIndicator"/> character.
            /// </summary>
            Remove = 1,
            //            Escape,   // it couldn't be escaped in all cases
            /// <summary>
            /// Replace the <see cref="AcceleratorIndicator"/> with "and" (spaces and/or capitalisation may be applied heuristically).
            /// </summary>
            ConvertToWord,
            /// <summary>
            /// Keep the existing one.
            /// </summary>
            Keep
        }

        /// <summary>
        /// Actions for what to do when an accelerator character beign assigned is already used.
        /// </summary>
        public enum DuplicateAcceleratorAction
        {
            /// <summary>
            /// Ignore the error and assign it.
            /// </summary>
            Allow = 1,
            /// <summary>
            /// Silently fail and leave the accelerator character unchanged
            /// (same a <see cref="NoAccelerator"/> if there isn't one already).
            /// </summary>
            NoChange,
            /// <summary>
            /// Silently fail and set no accelerator character.
            /// </summary>
            NoAccelerator,
            /// <summary>
            /// Silently choose a different accelerator character.
            /// </summary>
            Change,
            /// <summary>
            /// Throw an exception.
            /// </summary>
            Exception
        }
    }
}
