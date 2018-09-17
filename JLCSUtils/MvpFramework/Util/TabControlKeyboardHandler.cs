using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Util
{
    public class TabControlKeyboardHandler
    {
        public TabControlKeyboardHandler(Func<int> getSelectedTab, Action<int> setSelectedTab)
        {
            GetSelectedTab = getSelectedTab;
            SetSelectedTab = setSelectedTab;
        }

        /// <summary>
        /// Returns the 0-based index of the selected tab.
        /// </summary>
        public virtual Func<int> GetSelectedTab { get; }

        /// <summary>
        /// Selects the tab with the given 0-based index. Does nothing if the argument is too high.
        /// </summary>
        public virtual Action<int> SetSelectedTab { get; }

        /// <summary>
        /// Modifier keys that must be used with all other keys.
        /// This can be <see cref="KeyboardKey.None"/>, but that may then prevent the normal function of the relevant keys from working.
        /// </summary>
        public virtual KeyboardKey ModifierKeys { get; set; } = KeyboardKey.Control;

        /// <summary>
        /// The key to go to the next tab.
        /// If at the last tab already, it does nothing. (It doesn't wrap around.)
        /// <para><see cref="KeyboardKey.None"/> to disable.</para>
        /// </summary>
        public virtual KeyboardKey NextKey { get; set; } = KeyboardKey.PageDown;

        /// <summary>
        /// The key to go to the previous tab.
        /// If at the first tab already, it does nothing. (It doesn't wrap around.)
        /// <para><see cref="KeyboardKey.None"/> to disable.</para>
        /// </summary>
        public virtual KeyboardKey PreviousKey { get; set; } = KeyboardKey.PageDown;

        /// <summary>
        /// Which sets of kys can be used to directly choose a tab.
        /// </summary>
        public virtual NumberKeysType NumbersKeys { get; set; } = NumberKeysType.NumPad;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns>true if the keystroke was processed.</returns>
        public virtual bool NotifyKey(KeyboardKey keyData)
        {
            if (keyData.GetModifiers() == ModifierKeys)    // if the actual modifier keys exactly match the required ones
            {
                var baseKey = keyData.GetBaseKey();
                if (baseKey != KeyboardKey.None)
                {
                    if (baseKey == NextKey)
                    {
                        SetSelectedTab.Invoke(GetSelectedTab() + 1);
                        return true;
                    }
                    else if (baseKey == PreviousKey)
                    {
                        int tabIndex = GetSelectedTab.Invoke() - 1;
                        if (tabIndex >= 0)
                        {
                            SetSelectedTab.Invoke(tabIndex);
                            return true;
                        }
                    }
                    else
                    {
                        int? tabIndex = null;
                        int? digit = keyData.GetDigit();
                        if (digit != null)
                        {
                            if (keyData.IsNumPadKey())
                            {
                                if (NumbersKeys.HasFlag(NumberKeysType.NumPadZeroHigh))
                                {
                                    tabIndex = ZeroAdjust(digit.Value);
                                }
                                else if (NumbersKeys.HasFlag(NumberKeysType.NumPad))
                                {
                                    tabIndex = digit;
                                }
                            }
                            else
                            {
                                if (NumbersKeys.HasFlag(NumberKeysType.MainZeroHigh))
                                {
                                    tabIndex = ZeroAdjust(digit.Value);
                                }
                                else if (NumbersKeys.HasFlag(NumberKeysType.Main))
                                {
                                    tabIndex = digit;
                                }
                            }
                        }
                        if (tabIndex == null && NumbersKeys.HasFlag(NumberKeysType.FKeys))
                        {
                            tabIndex = keyData.GetFKeyNumber() - 1;  // F1..F24 mapped to 0..23.
                        }
                        if (tabIndex.HasValue)
                        {
                            SetSelectedTab.Invoke(tabIndex.Value);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Adjust <paramref name="value"/> so that (1..9,0) is mapped to (0..9).
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Adjusted value.</returns>
        protected virtual int ZeroAdjust(int value)
            => value == 0 ? 10 : value - 1;
    }


    [Flags]
    public enum NumberKeysType
    {
        /// <summary>
        /// No keys. Disables a feature that a set of keys was to be configured for.
        /// </summary>
        None = 0,
        /// <summary>
        /// The number keys in the main section of the keyboard, starting at 0.
        /// <para>Must not be combined with <see cref="MainZeroHigh"/>.</para>
        /// </summary>
        Main = 1,
        /// <summary>
        /// The number keys in the main section of the keyboard, starting at 1, with 0 following 9.
        /// <para>Must not be combined with <see cref="Main"/>.</para>
        /// </summary>
        MainZeroHigh = 2,
        /// <summary>
        /// The number keys if the numeric keypad, starting at 0.
        /// <para>Must not be combined with <see cref="NumPadZeroHigh"/>.</para>
        /// </summary>
        NumPad = 4,
        /// <summary>
        /// The number keys if the numeric keypad, starting at 1, with 0 following 9.
        /// <para>Must not be combined with <see cref="NumPad"/>.</para>
        /// </summary>
        NumPadZeroHigh = 8,
        /// <summary>
        /// The F keys.
        /// </summary>
        FKeys = 0x10
    }
}
