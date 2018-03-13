using JohnLambe.Util.Misc;
using JohnLambe.Util.Reflection;
using MvpFramework.Binding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework
{
    /// <summary>
    /// Equivalent to WinForms Sytem.Windows.Forms.Keys (avoids referencing WinForms).
    /// Can be considered an extension of <see cref="System.ConsoleKey"/>.
    /// This has all members defined on either of these (and where constant names differ, this has both).
    /// <para><seealso cref="System.ConsoleKey"/> - doesn't have modifier keys, mouse buttons and some other keys.</para>
    /// <para><seealso cref="KeyboardKeyExtension"/> - for conversion to/from <see cref="System.ConsoleKey"/>.</para>
    /// </summary>
    [HybridEnum]
    public enum KeyboardKey
    {
        // This type only:
        /// <summary>
        /// Mask for the main key (without modifiers).
        /// </summary>
        [EnumMask]
        BaseKey = 0xFF,

        // WinForms only:
        //
        // Summary:
        //     The bitmask to extract modifiers from a key value.
        [EnumMask]
        Modifiers = -65536,
        //
        // Summary:
        //     No key pressed.
        [EnumNullValue]
        None = 0,
        //
        // Summary:
        //     The left mouse button.
        [DisplayNameAny("Left Button", ShortName = "LeftBtn")]
        [Description("The left mouse button")]
        LButton = 1,
        //
        // Summary:
        //     The right mouse button.
        [DisplayNameAny("Right Button", ShortName = "RightBtn")]
        [Description("The right mouse button")]
        RButton = 2,
        //
        // Summary:
        //     The CANCEL key.
        Cancel = 3,
        //
        // Summary:
        //     The middle mouse button (three-button mouse).
        [DisplayNameAny("Middle Button", ShortName = "MidBtn")]
        [Description("The middle mouse button")]
        MButton = 4,
        //
        // Summary:
        //     The first x mouse button (five-button mouse).
        [Description("The first x mouse button (five-button mouse)")]
        XButton1 = 5,
        //
        // Summary:
        //     The second x mouse button (five-button mouse).
        [Description("The second x mouse button (five-button mouse)")]
        XButton2 = 6,

        //
        // Summary:
        //     The BACKSPACE key.
        [DisplayNameAny("Backspace", ShortName = "\u232B")]
        Back = 8,
        //
        // Summary:
        //     The TAB key.
        [DisplayNameAny(ShortName = "\u21E5")]  // RIGHTWARDS ARROW TO BAR'
        Tab = 9,
        //
        // Summary:
        //     The LINEFEED key.
        LineFeed = 10,
        //
        // Summary:
        //     The CLEAR key.
        Clear = 12,
        //
        // Summary:
        //     The RETURN key.
        [DisplayNameAny(ShortName = "\u23CE")]  // https://www.fileformat.info/info/unicode/char/23ce/index.htm
        Return = 13,
        //
        // Summary:
        //     The ENTER key.
        [EnumDuplicate]
        Enter = 13,

        // WinForms only:
        //
        // Summary:
        //     The SHIFT key.
        [DisplayNameAny("Shift", ShortName = "\u21E7")] // ⇧ https://www.fileformat.info/info/unicode/char/21e7/index.htm
        ShiftKey = 16,
        //
        // Summary:
        //     The CTRL key.
        [DisplayNameAny("Ctrl")]
        [Description("The Control key")]
        ControlKey = 17,
        //
        // Summary:
        //     The ALT key.
        [DisplayNameAny("Alt")]
        Menu = 18,

        //
        // Summary:
        //     The PAUSE key.
        Pause = 19,

        // WinForms only:
        //
        // Summary:
        //     The CAPS LOCK key.
        [DisplayNameAny("Caps Lock", ShortName = "CapsLock")]
        [EnumDuplicate]
        Capital = 20,
        //
        // Summary:
        //     The CAPS LOCK key.
        CapsLock = 20,
        //
        // Summary:
        //     The IME Kana mode key.
        KanaMode = 21,
        //
        // Summary:
        //     The IME Hanguel mode key. (maintained for compatibility; use HangulMode)
        //| This is included for consistency with the WinForms type.
//        [EnumDuplicate]
//        [Obsolete,EnumObsolete]
//        HanguelMode = 21,
        //
        // Summary:
        //     The IME Hangul mode key.
        HangulMode = 21,
        //
        // Summary:
        //     The IME Junja mode key.
        JunjaMode = 23,
        //
        // Summary:
        //     The IME final mode key.
        FinalMode = 24,
        //
        // Summary:
        //     The IME Hanja mode key.
        HanjaMode = 25,
        //
        // Summary:
        //     The IME Kanji mode key.
        KanjiMode = 25,

        //
        // Summary:
        //     The ESC key.
        [DisplayNameAny(ShortName = "Esc")]
        Escape = 27,

        // WinForms only:
        //
        // Summary:
        //     The IME convert key.
        IMEConvert = 28,
        //
        // Summary:
        //     The IME nonconvert key.
        [EnumDuplicate]
        IMENonconvert = 29,  // Matches WinForms capitalisation
        IMENonConvert = 29,
        //
        // Summary:
        //     The IME accept key, replaces System.Windows.Forms.Keys.IMEAceept.
        IMEAccept = 30,
        //
        // Summary:
        //     The IME accept key. Obsolete, use System.Windows.Forms.Keys.IMEAccept instead.
//        [EnumDuplicate]
//        [Obsolete, EnumObsolete]
//        IMEAceept = 30,
        //
        // Summary:
        //     The IME mode change key.
        IMEModeChange = 31,

        //
        // Summary:
        //     The SPACEBAR key.
        Space = 32,
        //
        // Summary:
        //     The PAGE UP key.
        [EnumDuplicate]
        [DisplayNameAny("Page Up", ShortName = "PgUp")]
        Prior = 33,
        //
        // Summary:
        //     The PAGE UP key.
        [DisplayNameAny("Page Up", ShortName = "PgUp")]
        PageUp = 33,
        //
        // Summary:
        //     The PAGE DOWN key.
        [EnumDuplicate]
        [DisplayNameAny("Page Down", ShortName = "PgDn")]
        Next = 34,
        //
        // Summary:
        //     The PAGE DOWN key.
        [DisplayNameAny("Page Down", ShortName = "PgDn")]
        PageDown = 34,
        //
        // Summary:
        //     The END key.
        End = 35,
        //
        // Summary:
        //     The HOME key.
        Home = 36,
        //
        // Summary:
        //     The LEFT ARROW key.
        //[DisplayNameAny(ShortName = )]
        [Description("The Left Arrow key")]
        Left = 37,
        //
        // Summary:
        //     The UP ARROW key.
        [Description("The Up Arrow key")]
        Up = 38,
        //
        // Summary:
        //     The RIGHT ARROW key.
        [Description("The Right Arrow key")]
        Right = 39,
        //
        // Summary:
        //     The DOWN ARROW key.
        [Description("The Down Arrow key")]
        Down = 40,
        //
        // Summary:
        //     The SELECT key.
        Select = 41,
        //
        // Summary:
        //     The PRINT key.
        Print = 42,
        //
        // Summary:
        //     The EXECUTE key.
        Execute = 43,
        //
        // Summary:
        //     The PRINT SCREEN key.
        [EnumDuplicate]
        [DisplayNameAny("Print Screen", ShortName = "PrtSc")]
        Snapshot = 44,
        //
        // Summary:
        //     The PRINT SCREEN key.
        [DisplayNameAny("Print Screen", ShortName = "PrtSc")]
        PrintScreen = 44,
        //
        // Summary:
        //     The INS key.
        [DisplayNameAny(ShortName = "Ins")]
        Insert = 45,
        //
        // Summary:
        //     The DEL key.
        [DisplayNameAny(ShortName = "Del")]
        Delete = 46,
        //
        // Summary:
        //     The HELP key.
        Help = 47,
        //
        // Summary:
        //     The 0 key.
        [DisplayNameAny("0")]
        D0 = 48,
        //
        // Summary:
        //     The 1 key.
        [DisplayNameAny("1")]
        D1 = 49,
        //
        // Summary:
        //     The 2 key.
        [DisplayNameAny("2")]
        D2 = 50,
        //
        // Summary:
        //     The 3 key.
        [DisplayNameAny("3")]
        D3 = 51,
        //
        // Summary:
        //     The 4 key.
        [DisplayNameAny("4")]
        D4 = 52,
        //
        // Summary:
        //     The 5 key.
        [DisplayNameAny("5")]
        D5 = 53,
        //
        // Summary:
        //     The 6 key.
        [DisplayNameAny("6")]
        D6 = 54,
        //
        // Summary:
        //     The 7 key.
        [DisplayNameAny("7")]
        D7 = 55,
        //
        // Summary:
        //     The 8 key.
        [DisplayNameAny("8")]
        D8 = 56,
        //
        // Summary:
        //     The 9 key.
        [DisplayNameAny("9")]
        D9 = 57,
        //
        // Summary:
        //     The A key.
        A = 65,
        //
        // Summary:
        //     The B key.
        B = 66,
        //
        // Summary:
        //     The C key.
        C = 67,
        //
        // Summary:
        //     The D key.
        D = 68,
        //
        // Summary:
        //     The E key.
        E = 69,
        //
        // Summary:
        //     The F key.
        F = 70,
        //
        // Summary:
        //     The G key.
        G = 71,
        //
        // Summary:
        //     The H key.
        H = 72,
        //
        // Summary:
        //     The I key.
        I = 73,
        //
        // Summary:
        //     The J key.
        J = 74,
        //
        // Summary:
        //     The K key.
        K = 75,
        //
        // Summary:
        //     The L key.
        L = 76,
        //
        // Summary:
        //     The M key.
        M = 77,
        //
        // Summary:
        //     The N key.
        N = 78,
        //
        // Summary:
        //     The O key.
        O = 79,
        //
        // Summary:
        //     The P key.
        P = 80,
        //
        // Summary:
        //     The Q key.
        Q = 81,
        //
        // Summary:
        //     The R key.
        R = 82,
        //
        // Summary:
        //     The S key.
        S = 83,
        //
        // Summary:
        //     The T key.
        T = 84,
        //
        // Summary:
        //     The U key.
        U = 85,
        //
        // Summary:
        //     The V key.
        V = 86,
        //
        // Summary:
        //     The W key.
        W = 87,
        //
        // Summary:
        //     The X key.
        X = 88,
        //
        // Summary:
        //     The Y key.
        Y = 89,
        //
        // Summary:
        //     The Z key.
        Z = 90,
        //
        // Summary:
        //     The left Windows logo key (Microsoft Natural Keyboard).
        [EnumDuplicate]
        [DisplayNameAny("Left Windows", ShortName = "LWin")]
        LWin = 91,
        /// <summary>
        /// The left Windows logo key (Microsoft Natural Keyboard).
        /// <para>This name matches <see cref="System.ConsoleKey"/> (not Sytem.Windows.Forms.Keys).</para>
        /// </summary>
        [DisplayNameAny("Left Windows", ShortName = "LWin")]
        LeftWindows = 91,
        //
        // Summary:
        //     The right Windows logo key (Microsoft Natural Keyboard).
        [EnumDuplicate]
        [DisplayNameAny("Right Windows", ShortName = "RWin")]
        RWin = 92,
        /// <summary>
        /// The left Windows logo key (Microsoft Natural Keyboard).
        /// <para>This name matches <see cref="System.ConsoleKey"/> (not Sytem.Windows.Forms.Keys).</para>
        /// </summary>
        [DisplayNameAny("Right Windows", ShortName = "RWin")]
        RightWindows = 92,
        //
        // Summary:
        //     The application key (Microsoft Natural Keyboard).
        [EnumDuplicate]
        Apps = 93,
        /// <summary>
        /// The application key (Microsoft Natural Keyboard).
        /// <para>This name matches <see cref="System.ConsoleKey"/> (not Sytem.Windows.Forms.Keys).</para>
        /// </summary>
        Applications = 93,
        //
        // Summary:
        //     The computer sleep key.
        Sleep = 95,
        //
        // Summary:
        //     The 0 key on the numeric keypad.
        NumPad0 = 96,
        //
        // Summary:
        //     The 1 key on the numeric keypad.
        NumPad1 = 97,
        //
        // Summary:
        //     The 2 key on the numeric keypad.
        NumPad2 = 98,
        //
        // Summary:
        //     The 3 key on the numeric keypad.
        NumPad3 = 99,
        //
        // Summary:
        //     The 4 key on the numeric keypad.
        NumPad4 = 100,
        //
        // Summary:
        //     The 5 key on the numeric keypad.
        NumPad5 = 101,
        //
        // Summary:
        //     The 6 key on the numeric keypad.
        NumPad6 = 102,
        //
        // Summary:
        //     The 7 key on the numeric keypad.
        NumPad7 = 103,
        //
        // Summary:
        //     The 8 key on the numeric keypad.
        NumPad8 = 104,
        //
        // Summary:
        //     The 9 key on the numeric keypad.
        NumPad9 = 105,
        //
        // Summary:
        //     The multiply key.
        Multiply = 106,
        //
        // Summary:
        //     The add key.
        /// <summary>
        /// The '+' key on the numeric keypad.
        /// </summary>
        [DisplayNameAny("+")]
        Add = 107,
        //
        // Summary:
        //     The separator key.
        Separator = 108,
        //
        // Summary:
        //     The subtract key.
        /// <summary>
        /// The '-' key on the numeric keypad.
        /// </summary>
        [DisplayNameAny("-")]
        Subtract = 109,
        //
        // Summary:
        //     The decimal key.
        /// <summary>
        /// The '+' key on the numeric keypad.
        /// </summary>
        [DisplayNameAny(".")]
        Decimal = 110,
        //
        // Summary:
        //     The divide key.
        /// <summary>
        /// The '/' key on the numeric keypad.
        /// </summary>
        [DisplayNameAny("/")]
        Divide = 111,
        //
        // Summary:
        //     The F1 key.
        F1 = 112,
        //
        // Summary:
        //     The F2 key.
        F2 = 113,
        //
        // Summary:
        //     The F3 key.
        F3 = 114,
        //
        // Summary:
        //     The F4 key.
        F4 = 115,
        //
        // Summary:
        //     The F5 key.
        F5 = 116,
        //
        // Summary:
        //     The F6 key.
        F6 = 117,
        //
        // Summary:
        //     The F7 key.
        F7 = 118,
        //
        // Summary:
        //     The F8 key.
        F8 = 119,
        //
        // Summary:
        //     The F9 key.
        F9 = 120,
        //
        // Summary:
        //     The F10 key.
        F10 = 121,
        //
        // Summary:
        //     The F11 key.
        F11 = 122,
        //
        // Summary:
        //     The F12 key.
        F12 = 123,
        //
        // Summary:
        //     The F13 key.
        F13 = 124,
        //
        // Summary:
        //     The F14 key.
        F14 = 125,
        //
        // Summary:
        //     The F15 key.
        F15 = 126,
        //
        // Summary:
        //     The F16 key.
        F16 = 127,
        //
        // Summary:
        //     The F17 key.
        F17 = 128,
        //
        // Summary:
        //     The F18 key.
        F18 = 129,
        //
        // Summary:
        //     The F19 key.
        F19 = 130,
        //
        // Summary:
        //     The F20 key.
        F20 = 131,
        //
        // Summary:
        //     The F21 key.
        F21 = 132,
        //
        // Summary:
        //     The F22 key.
        F22 = 133,
        //
        // Summary:
        //     The F23 key.
        F23 = 134,
        //
        // Summary:
        //     The F24 key.
        F24 = 135,

        // Not in System.ConsoleKey:
        //
        // Summary:
        //     The NUM LOCK key.
        [DisplayNameAny(ShortName = "NumLock")]
        NumLock = 144,
        //
        // Summary:
        //     The SCROLL LOCK key.
        [DisplayNameAny("Scroll Lock", ShortName = "ScrLock")]
        Scroll = 145,
        //
        // Summary:
        //     The left SHIFT key.
        [DisplayNameAny("Left Shift", ShortName = "LShift")]
        LShiftKey = 160,
        //
        // Summary:
        //     The right SHIFT key.
        [DisplayNameAny("Right Shift", ShortName = "RShift")]
        RShiftKey = 161,
        //
        // Summary:
        //     The left CTRL key.
        [DisplayNameAny("Left Ctrl", ShortName = "LCtrl")]
        LControlKey = 162,
        //
        // Summary:
        //     The right CTRL key.
        [DisplayNameAny("Right Ctrl", ShortName = "RCtrl")]
        RControlKey = 163,
        //
        // Summary:
        //     The left ALT key.
        [DisplayNameAny("Left Alt", ShortName = "LAlt")]
        LMenu = 164,
        //
        // Summary:
        //     The right ALT key.
        [DisplayNameAny("Right Alt", ShortName = "RAlt")]
        RMenu = 165,

        //
        // Summary:
        //     The browser back key (Windows 2000 or later).
        BrowserBack = 166,
        //
        // Summary:
        //     The browser forward key (Windows 2000 or later).
        BrowserForward = 167,
        //
        // Summary:
        //     The browser refresh key (Windows 2000 or later).
        BrowserRefresh = 168,
        //
        // Summary:
        //     The browser stop key (Windows 2000 or later).
        BrowserStop = 169,
        //
        // Summary:
        //     The browser search key (Windows 2000 or later).
        BrowserSearch = 170,
        //
        // Summary:
        //     The browser favorites key (Windows 2000 or later).
        BrowserFavorites = 171,
        //
        // Summary:
        //     The browser home key (Windows 2000 or later).
        BrowserHome = 172,
        //
        // Summary:
        //     The volume mute key (Windows 2000 or later).
        VolumeMute = 173,
        //
        // Summary:
        //     The volume down key (Windows 2000 or later).
        VolumeDown = 174,
        //
        // Summary:
        //     The volume up key (Windows 2000 or later).
        VolumeUp = 175,
        //
        // Summary:
        //     The media next track key (Windows 2000 or later).
        [DisplayNameAny("Next Track", ShortName = "NextTrack")]
        MediaNextTrack = 176,
        /// <summary>
        /// The media next track key (Windows 2000 or later).
        /// <para>This name matches <see cref="System.ConsoleKey"/> (not Sytem.Windows.Forms.Keys).</para>
        /// </summary>
        [EnumDuplicate]
        MediaNext = 176,
        //
        // Summary:
        //     The media previous track key (Windows 2000 or later).
        [DisplayNameAny("Previous Track", ShortName = "PrevTrack")]
        MediaPreviousTrack = 177,
        /// <summary>
        /// The media previous track key (Windows 2000 or later).
        /// <para>This name matches <see cref="System.ConsoleKey"/> (not Sytem.Windows.Forms.Keys).</para>
        /// </summary>
        [EnumDuplicate]
        MediaPrevious = 177,
        //
        // Summary:
        //     The media Stop key (Windows 2000 or later).
        MediaStop = 178,
        //
        // Summary:
        //     The media play pause key (Windows 2000 or later).
        MediaPlayPause = 179,
        //
        // Summary:
        //     The launch mail key (Windows 2000 or later).
        LaunchMail = 180,
        //
        // Summary:
        //     The select media key (Windows 2000 or later).
        SelectMedia = 181,
        /// <summary>
        /// The select media key (Windows 2000 or later).
        /// <para>This name matches <see cref="System.ConsoleKey"/> (not Sytem.Windows.Forms.Keys).</para>
        /// </summary>
        [EnumDuplicate]
        LaunchMediaSelect = 181,
        //
        // Summary:
        //     The start application one key (Windows 2000 or later).
        [DisplayNameAny(ShortName = "LaunchApp1")]
        LaunchApplication1 = 182,
        /// <summary>
        /// The start application one key (Windows 2000 or later).
        /// <para>This name matches <see cref="System.ConsoleKey"/> (not Sytem.Windows.Forms.Keys).</para>
        /// </summary>
        [DisplayNameAny(ShortName = "LaunchApp1")]
        [EnumDuplicate]
        LaunchApp1 = 182,
        //
        // Summary:
        //     The start application two key (Windows 2000 or later).
        [DisplayNameAny(ShortName = "LaunchApp2")]
        LaunchApplication2 = 183,
        /// <summary>
        /// The start application two key (Windows 2000 or later).
        /// <para>This name matches <see cref="System.ConsoleKey"/> (not Sytem.Windows.Forms.Keys).</para>
        /// </summary>
        [DisplayNameAny(ShortName = "LaunchApp2")]
        [EnumDuplicate]
        LaunchApp2 = 183,
        //
        // Summary:
        //     The OEM Semicolon key on a US standard keyboard (Windows 2000 or later).
        OemSemicolon = 186,
        //
        // Summary:
        //     The OEM 1 key.
        [EnumDuplicate]
        Oem1 = 186,
        //
        // Summary:
        //     The OEM plus key on any country/region keyboard (Windows 2000 or later).
        [EnumDuplicate]
        Oemplus = 187,
        //| This corrects the capitalisation of the WinForms name.
        OemPlus = 187,
        //
        // Summary:
        //     The OEM comma key on any country/region keyboard (Windows 2000 or later).
        [EnumDuplicate]
        Oemcomma = 188,
        //| This corrects the capitalisation of the WinForms name.
        OemComma = 188,   
        //
        // Summary:
        //     The OEM minus key on any country/region keyboard (Windows 2000 or later).
        OemMinus = 189,
        //
        // Summary:
        //     The OEM period key on any country/region keyboard (Windows 2000 or later).
        OemPeriod = 190,
        //
        // Summary:
        //     The OEM question mark key on a US standard keyboard (Windows 2000 or later).
        OemQuestion = 191,
        //
        // Summary:
        //     The OEM 2 key.
        [EnumDuplicate]
        Oem2 = 191,
        //
        // Summary:
        //     The OEM tilde key on a US standard keyboard (Windows 2000 or later).
        [EnumDuplicate]
        Oemtilde = 192,
        //| This corrects the capitalisation of the WinForms name.
        OemTilde = 192,
        //
        // Summary:
        //     The OEM 3 key.
        [EnumDuplicate]
        Oem3 = 192,
        //
        // Summary:
        //     The OEM open bracket key on a US standard keyboard (Windows 2000 or later).
        OemOpenBrackets = 219,
        //
        // Summary:
        //     The OEM 4 key.
        [EnumDuplicate]
        Oem4 = 219,
        //
        // Summary:
        //     The OEM pipe key on a US standard keyboard (Windows 2000 or later).
        OemPipe = 220,
        //
        // Summary:
        //     The OEM 5 key.
        [EnumDuplicate]
        Oem5 = 220,
        //
        // Summary:
        //     The OEM close bracket key on a US standard keyboard (Windows 2000 or later).
        OemCloseBrackets = 221,
        //
        // Summary:
        //     The OEM 6 key.
        [EnumDuplicate]
        Oem6 = 221,
        //
        // Summary:
        //     The OEM singled/double quote key on a US standard keyboard (Windows 2000 or later).
        OemQuotes = 222,
        //
        // Summary:
        //     The OEM 7 key.
        [EnumDuplicate]
        Oem7 = 222,
        //
        // Summary:
        //     The OEM 8 key.
        Oem8 = 223,
        //
        // Summary:
        //     The OEM angle bracket or backslash key on the RT 102 key keyboard (Windows 2000
        //     or later).
        OemBackslash = 226,
        //
        // Summary:
        //     The OEM 102 key.
        [EnumDuplicate]
        Oem102 = 226,
        //
        // Summary:
        //     The PROCESS KEY key.
        [EnumDuplicate]
        ProcessKey = 229,
        /// <summary>
        /// The PROCESS KEY key.
        /// <para>This name matches <see cref="System.ConsoleKey"/> (not Sytem.Windows.Forms.Keys).</para>
        /// </summary>
        Process = 229,
        //
        // Summary:
        //     Used to pass Unicode characters as if they were keystrokes. The Packet key value
        //     is the low word of a 32-bit virtual-key value used for non-keyboard input methods.
        Packet = 231,
        //
        // Summary:
        //     The ATTN key.
        [EnumDuplicate]
        Attn = 246,
        /// <summary>
        ///     The ATTN key.
        /// <para>This name matches <see cref="System.ConsoleKey"/> (not Sytem.Windows.Forms.Keys).</para>
        /// </summary>
        Attention = 246,
        //
        // Summary:
        //     The CRSEL key.
        Crsel = 247,
        //
        // Summary:
        //     The EXSEL key.
        Exsel = 248,
        //
        // Summary:
        //     The ERASE EOF key.
        EraseEof = 249,
        /// <summary>
        ///     The ERASE EOF key.
        /// </summary>
        /// <remarks>This name matches <see cref="System.ConsoleKey"/> (not Sytem.Windows.Forms.Keys).</remarks>
        [EnumDuplicate]
        EraseEndOfFile = 249,
        //
        // Summary:
        //     The PLAY key.
        Play = 250,
        //
        // Summary:
        //     The ZOOM key.
        Zoom = 251,
        //
        // Summary:
        //     A constant reserved for future use.
        NoName = 252,
        //
        // Summary:
        //     The PA1 key.
        Pa1 = 253,
        //
        // Summary:
        //     The CLEAR key.
        OemClear = 254,


        ModifierKeys = unchecked( (int)0xFFFF0000 ),

        // WinForms only:
        /// <summary>
        /// The bitmask to extract a key code from a key value.
        /// </summary>
        [EnumMask]
        [EnumHybridMember(DataType = typeof(KeyboardKey))]
        KeyCode = 65535,
        //
        // Summary:
        //     The SHIFT modifier key.
        [EnumHidden]
        [EnumFlag]
        [DisplayNameAny("Shift", ShortName = "\u21E7")] // ⇧ https://www.fileformat.info/info/unicode/char/21e7/index.htm
        Shift = 65536,
        //
        // Summary:
        //     The CTRL modifier key.
        [EnumHidden]
        [EnumFlag]
        [DisplayNameAny("Ctrl", ShortName = "^")]
        Control = 131072,
        //
        // Summary:
        //     The ALT modifier key.
        [EnumHidden]
        [EnumFlag]
        Alt = 262144
    }


    /// <summary>
    /// Extensions methods on or relating to <see cref="KeyboardKey"/>.
    /// </summary>
    /// <seealso cref="WinForms.KeyboardKeyExtension"/>
    public static class KeyboardKeyExtension
    {
        /// <summary>
        /// Returns the modifers of this <see cref="KeyboardKey"/> as a <see cref="System.ConsoleModifiers"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ConsoleModifiers ConsoleModifiers(this KeyboardKey value)
            => (value.HasFlag(KeyboardKey.Alt) ? System.ConsoleModifiers.Alt : 0)
            | (value.HasFlag(KeyboardKey.Shift) ? System.ConsoleModifiers.Shift : 0)
            | (value.HasFlag(KeyboardKey.Control) ? System.ConsoleModifiers.Control : 0);

        /// <summary>
        /// Convert a <see cref="System.ConsoleModifiers"/> to a <see cref="KeyboardKey"/> with no base key.
        /// </summary>
        /// <param name="modifiers"></param>
        /// <returns></returns>
        public static KeyboardKey FromConsoleModifiers(System.ConsoleModifiers modifiers)
            => (modifiers.HasFlag(System.ConsoleModifiers.Alt) ? KeyboardKey.Alt : 0)
            | (modifiers.HasFlag(System.ConsoleModifiers.Shift) ? KeyboardKey.Shift : 0)
            | (modifiers.HasFlag(System.ConsoleModifiers.Control) ? KeyboardKey.Control : 0);

        /// <summary>
        /// Convert to a <see cref="System.ConsoleKey"/>, without the modifier keys.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException">If this key cannot be represented as a <see cref="System.ConsoleKey"/>.</exception>
        public static ConsoleKey ToConsoleKey(this KeyboardKey value)
        {
            ConsoleKey result = (ConsoleKey)(value & KeyboardKey.BaseKey);
            if (!result.ValidateEnumValue())
                throw new InvalidCastException("This key cannot be converted to ConsoleKey: " + value);
            return result;
        }

        /// <summary>
        /// Convert to a <see cref="KeyboardKey"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="modifiers"></param>
        /// <returns></returns>
        public static KeyboardKey ToKeyboardKey(ConsoleKey value, ConsoleModifiers modifiers = 0)
            => (KeyboardKey)value | FromConsoleModifiers(modifiers);

        /// <summary>
        /// Combines a base key with a modifier.
        /// </summary>
        /// <param name="key">The key to modify.</param>
        /// <param name="modifier">The modifier to apply to this key.
        /// This must not include a base key.
        /// This can be <see cref="KeyboardKey.None"/> or multiple modifiers.
        /// </param>
        /// <returns>The key, with the modifier applied.</returns>
        /// <example>
        /// For CTRL-ALT-DELETE: <code>KeyboardKey.Delete.AddModifier(KeyboardKey.Alt).AddModifier(KeyboardKey.Control)</code>
        /// </example>
        /// <remarks>
        /// Using this instead of bitwise OR provides some validation (that would detect a key code (e.g. ControlKey) being used in place of a modifier.
        /// </remarks>
        [Pure]
        public static KeyboardKey AddModifier(this KeyboardKey key, KeyboardKey modifier)
        {
            if ((key & KeyboardKey.KeyCode) == 0)
                throw new ArgumentException("No base key provided");
            if ((modifier & KeyboardKey.KeyCode) != 0)
                throw new ArgumentException("The given modifier is invalid - it includes bits in the range for the base key.");
            return modifier | key;
        }

        /* Options for how AddModifier could have been defined:
         * 
            public static readonly KeyboardKey A = KeyboardKey.Delete.AddModifier(KeyboardKey.Alt).AddModifier(KeyboardKey.Control);
                // Called on base key, or base key with modifier. Parameter must be modifier.
                // Chose this one.

            public static readonly KeyboardKey C = KeyboardKey.Alt.Combine(KeyboardKey.Control.Combine(KeyboardKey.P));
                // Inverse of above.

            public static readonly KeyboardKey E = KeyboardKey.Delete.Combine(KeyboardKey.Alt.Combine(KeyboardKey.Control));
                // wekaer validation: Called on base key or modifier, parameter is modifier(s).

            public static readonly KeyboardKey B = KeyboardKey.Alt.Combine(KeyboardKey.Control).Combine(KeyboardKey.Delete);
                // weaker validation: Called on modifier, but parameter can be modifier or base key.
        */

    }
}
