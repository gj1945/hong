using System.Runtime.InteropServices;
using System.Collections.Immutable;

class netdll
{
    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_init")]
    public static extern int kmNet_init(string ip, string port, string mac);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_mouse_move")]
    public static extern int kmNet_mouse_move(short x, short y);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_mouse_left")]
    public static extern int kmNet_mouse_left(int isdown);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_mouse_right")]
    public static extern int kmNet_mouse_right(int isdown);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_mouse_middle")]
    public static extern int kmNet_mouse_middle(int isdown);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_mouse_wheel")]
    public static extern int kmNet_mouse_wheel(int wheel);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_mouse_side1")]
    public static extern int kmNet_mouse_side1(int isdown);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_mouse_side2")]
    public static extern int kmNet_mouse_side2(int isdown);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_mouse_all")]
    public static extern int kmNet_mouse_all(int button, int x, int y, int wheel);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_mouse_move_auto")]
    public static extern int kmNet_mouse_move_auto(int x, int y, int time_ms);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_mouse_move_beizer")]
    public static extern int kmNet_mouse_move_beizer(int x, int y, int ms, int x1, int y1, int x2, int y2);

    // 键盘函数
    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_keydown")]
    public static extern int kmNet_keydown(int vkey);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_keyup")]
    public static extern int kmNet_keyup(int vkey);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_keypress")]
    public static extern int kmNet_keypress(int vk_key, int ms = 10);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_enc_keyup")]
    public static extern int kmNet_enc_keyup(int vkey);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_enc_keypress")]
    public static extern int kmNet_enc_keypress(int vk_key, int ms = 10);

    // 带加密功能的函数。单机推荐使用，防止网络数据包抓包特征
    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_enc_mouse_move")]
    public static extern int kmNet_enc_mouse_move(int x, int y);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_enc_mouse_left")]
    public static extern int kmNet_enc_mouse_left(int isdown);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_enc_mouse_right")]
    public static extern int kmNet_enc_mouse_right(int isdown);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_enc_mouse_middle")]
    public static extern int kmNet_enc_mouse_middle(int isdown);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_enc_mouse_wheel")]
    public static extern int kmNet_enc_mouse_wheel(int wheel);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_enc_mouse_side1")]
    public static extern int kmNet_enc_mouse_side1(int isdown);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_enc_mouse_side2")]
    public static extern int kmNet_enc_mouse_side2(int isdown);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_enc_mouse_all")]
    public static extern int kmNet_enc_mouse_all(int button, int x, int y, int wheel);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_enc_mouse_move_auto")]
    public static extern int kmNet_enc_mouse_move_auto(int x, int y, int time_ms = 0);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_enc_mouse_move_beizer")]
    public static extern int kmNet_enc_mouse_move_beizer(int x, int y, int ms, int x1, int y1, int x2, int y2);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_enc_keydown")]
    public static extern int kmNet_enc_keydown(int vkey);

    // 监控系列
    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_monitor")]
    public static extern int kmNet_monitor(short port);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_monitor_mouse_left")]
    public static extern int kmNet_monitor_mouse_left();

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_monitor_mouse_left_code")]
    public static extern int kmNet_monitor_mouse_left_code();

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_monitor_mouse_middle")]
    public static extern int kmNet_monitor_mouse_middle();

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_monitor_mouse_middle_code")]
    public static extern int kmNet_monitor_mouse_middle_code();

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_monitor_mouse_right")]
    public static extern int kmNet_monitor_mouse_right();

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_monitor_mouse_right_code")]
    public static extern int kmNet_monitor_mouse_right_code();

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_monitor_mouse_side1")]
    public static extern int kmNet_monitor_mouse_side1();

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_monitor_mouse_side1_code")]
    public static extern int kmNet_monitor_mouse_side1_code();

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_monitor_mouse_side2")]
    public static extern int kmNet_monitor_mouse_side2();

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_monitor_mouse_side2_code")]
    public static extern int kmNet_monitor_mouse_side2_code();

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_monitor_mouse_xy")]
    public static extern int kmNet_monitor_mouse_xy(ref int x, ref int y);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_monitor_mouse_wheel")]
    public static extern int kmNet_monitor_mouse_wheel(ref int wheel);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_monitor_keyboard")]
    public static extern int kmNet_monitor_keyboard(int vk_key);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_monitor_keyboard_code")]
    public static extern int kmNet_monitor_keyboard_code();

    // 物理键鼠屏蔽系列
    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_mask_mouse_left")]
    public static extern int kmNet_mask_mouse_left(int enable);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_mask_mouse_right")]
    public static extern int kmNet_mask_mouse_right(int enable);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_mask_mouse_middle")]
    public static extern int kmNet_mask_mouse_middle(int enable);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_mask_mouse_side1")]
    public static extern int kmNet_mask_mouse_side1(int enable);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_mask_mouse_side2")]
    public static extern int kmNet_mask_mouse_side2(int enable);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_mask_mouse_x")]
    public static extern int kmNet_mask_mouse_x(int enable);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_mask_mouse_y")]
    public static extern int kmNet_mask_mouse_y(int enable);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_mask_mouse_wheel")]
    public static extern int kmNet_mask_mouse_wheel(int enable);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_mask_keyboard")]
    public static extern int kmNet_mask_keyboard(short vkey);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_unmask_keyboard")]
    public static extern int kmNet_unmask_keyboard(short vkey);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_unmask_all")]
    public static extern int kmNet_unmask_all();

    // 配置类函数
    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_reboot")]
    public static extern int kmNet_reboot();

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_setconfig")]
    public static extern int kmNet_setconfig(string ip, ushort port);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_setvidpid")]
    public static extern int kmNet_setvidpid(ushort vid, ushort pid);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_debug")]
    public static extern int kmNet_debug(short port, char enable);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_lcd_color")]
    public static extern int kmNet_lcd_color(ushort rgb565);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_lcd_picture_bottom")]
    public static extern int kmNet_lcd_picture_bottom(byte[] buff_128_80);

    [DllImport("kmNetLib.dll", EntryPoint = "kmNet_lcd_picture")]
    public static extern int kmNet_lcd_picture(byte[] buff_128_160);
}

public enum KeyboardModifiers : byte
{
    LeftСontrol = 0x01,
    LeftShift = 0x02,
    LeftAlt = 0x04,
    LeftGui = 0x08,
    RightControl = 0x10,
    RightShift = 0x20,
    RightAlt = 0x40,
    RightGui = 0x80
}


public static class KeyboardButtonMap
{

    public static readonly ImmutableDictionary<string, (KeyboardButton Button, KeyboardModifiers Modifiers)> CharToKeyboardButton = new Dictionary<string, (KeyboardButton Button, KeyboardModifiers Modifiers)>()
    {
        {"a", (KeyboardButton.KeyA, 0)},
        {"b", (KeyboardButton.KeyB, 0)},
        {"c", (KeyboardButton.KeyC, 0)},
        {"d", (KeyboardButton.KeyD, 0)},
        {"e", (KeyboardButton.KeyE, 0)},
        {"f", (KeyboardButton.KeyF, 0)},
        {"g", (KeyboardButton.KeyG, 0)},
        {"h", (KeyboardButton.KeyH, 0)},
        {"i", (KeyboardButton.KeyI, 0)},
        {"j", (KeyboardButton.KeyJ, 0)},
        {"k", (KeyboardButton.KeyK, 0)},
        {"l", (KeyboardButton.KeyL, 0)},
        {"m", (KeyboardButton.KeyM, 0)},
        {"n", (KeyboardButton.KeyN, 0)},
        {"o", (KeyboardButton.KeyO, 0)},
        {"p", (KeyboardButton.KeyP, 0)},
        {"q", (KeyboardButton.KeyQ, 0)},
        {"r", (KeyboardButton.KeyR, 0)},
        {"s", (KeyboardButton.KeyS, 0)},
        {"t", (KeyboardButton.KeyT, 0)},
        {"u", (KeyboardButton.KeyU, 0)},
        {"v", (KeyboardButton.KeyV, 0)},
        {"w", (KeyboardButton.KeyW, 0)},
        {"x", (KeyboardButton.KeyX, 0)},
        {"y", (KeyboardButton.KeyY, 0)},
        {"z", (KeyboardButton.KeyZ, 0)},
        {"A", (KeyboardButton.KeyA, KeyboardModifiers.LeftShift)},
        {"B", (KeyboardButton.KeyB, KeyboardModifiers.LeftShift)},
        {"C", (KeyboardButton.KeyC, KeyboardModifiers.LeftShift)},
        {"D", (KeyboardButton.KeyD, KeyboardModifiers.LeftShift)},
        {"E", (KeyboardButton.KeyE, KeyboardModifiers.LeftShift)},
        {"F", (KeyboardButton.KeyF, KeyboardModifiers.LeftShift)},
        {"G", (KeyboardButton.KeyG, KeyboardModifiers.LeftShift)},
        {"H", (KeyboardButton.KeyH, KeyboardModifiers.LeftShift)},
        {"I", (KeyboardButton.KeyI, KeyboardModifiers.LeftShift)},
        {"J", (KeyboardButton.KeyJ, KeyboardModifiers.LeftShift)},
        {"K", (KeyboardButton.KeyK, KeyboardModifiers.LeftShift)},
        {"L", (KeyboardButton.KeyL, KeyboardModifiers.LeftShift)},
        {"M", (KeyboardButton.KeyM, KeyboardModifiers.LeftShift)},
        {"N", (KeyboardButton.KeyN, KeyboardModifiers.LeftShift)},
        {"O", (KeyboardButton.KeyO, KeyboardModifiers.LeftShift)},
        {"P", (KeyboardButton.KeyP, KeyboardModifiers.LeftShift)},
        {"Q", (KeyboardButton.KeyQ, KeyboardModifiers.LeftShift)},
        {"R", (KeyboardButton.KeyR, KeyboardModifiers.LeftShift)},
        {"S", (KeyboardButton.KeyS, KeyboardModifiers.LeftShift)},
        {"T", (KeyboardButton.KeyT, KeyboardModifiers.LeftShift)},
        {"U", (KeyboardButton.KeyU, KeyboardModifiers.LeftShift)},
        {"V", (KeyboardButton.KeyV, KeyboardModifiers.LeftShift)},
        {"W", (KeyboardButton.KeyW, KeyboardModifiers.LeftShift)},
        {"X", (KeyboardButton.KeyX, KeyboardModifiers.LeftShift)},
        {"Y", (KeyboardButton.KeyY, KeyboardModifiers.LeftShift)},
        {"Z", (KeyboardButton.KeyZ, KeyboardModifiers.LeftShift)},
        {"1", (KeyboardButton.Key1ExclamationMark, 0)},
        {"2", (KeyboardButton.Key2At, 0)},
        {"3", (KeyboardButton.Key3NumberSign, 0)},
        {"4", (KeyboardButton.Key4Dollar, 0)},
        {"5", (KeyboardButton.Key5Percent, 0)},
        {"6", (KeyboardButton.Key6Caret, 0)},
        {"7", (KeyboardButton.Key7Ampersand, 0)},
        {"8", (KeyboardButton.Key8Asterisk, 0)},
        {"9", (KeyboardButton.Key9OpenParenthesis, 0)},
        {"0", (KeyboardButton.Key0ClosedParenthesis, 0)},
        {"!", (KeyboardButton.Key1ExclamationMark, KeyboardModifiers.LeftShift)},
        {"@", (KeyboardButton.Key2At, KeyboardModifiers.LeftShift)},
        {"#", (KeyboardButton.Key3NumberSign, KeyboardModifiers.LeftShift)},
        {"$", (KeyboardButton.Key4Dollar, KeyboardModifiers.LeftShift)},
        {"%", (KeyboardButton.Key5Percent, KeyboardModifiers.LeftShift)},
        {"^", (KeyboardButton.Key6Caret, KeyboardModifiers.LeftShift)},
        {"&", (KeyboardButton.Key7Ampersand, KeyboardModifiers.LeftShift)},
        {"*", (KeyboardButton.Key8Asterisk, KeyboardModifiers.LeftShift)},
        {"(", (KeyboardButton.Key9OpenParenthesis, KeyboardModifiers.LeftShift)},
        {")", (KeyboardButton.Key0ClosedParenthesis, KeyboardModifiers.LeftShift)},
        {" ", (KeyboardButton.KeySpacebar, 0)},
        {":", (KeyboardButton.KeySemicolonColon, KeyboardModifiers.LeftShift)},
        {"-", (KeyboardButton.KeyMinusUnderscore, 0)},
        {"esc", (KeyboardButton.KeyEscape, 0)},
        {"up", (KeyboardButton.KeyUparrow, 0)},
        {"down", (KeyboardButton.KeyDownarrow, 0)},
        {"left", (KeyboardButton.KeyLeftarrow, 0)},
        {"right", (KeyboardButton.KeyRightarrow, 0)},
        {"enter", (KeyboardButton.KeyEnter, 0)},
        {"backspace", (KeyboardButton.KeyBackspace, 0)},
        {"tab", (KeyboardButton.KeyTab, 0)},
        {"capslock", (KeyboardButton.KeyCapsLock, 0)},
        {"f1", (KeyboardButton.KeyF1, 0)},
        {"f2", (KeyboardButton.KeyF2, 0)},
        {"f3", (KeyboardButton.KeyF3, 0)},
        {"f4", (KeyboardButton.KeyF4, 0)},
        {"f5", (KeyboardButton.KeyF5, 0)},
        {"f6", (KeyboardButton.KeyF6, 0)},
        {"f7", (KeyboardButton.KeyF7, 0)},
        {"f8", (KeyboardButton.KeyF8, 0)},
        {"f9", (KeyboardButton.KeyF9, 0)},
        {"f10", (KeyboardButton.KeyF10, 0)},
        {"f11", (KeyboardButton.KeyF11, 0)},
        {"f12", (KeyboardButton.KeyF12, 0)},
        {"printscreen", (KeyboardButton.KeyPrintscreen, 0)},
        {"scrolllock", (KeyboardButton.KeyScrollLock, 0)},
        {"pause", (KeyboardButton.KeyPause, 0)},
        {"insert", (KeyboardButton.KeyInsert, 0)},
        {"home", (KeyboardButton.KeyHome, 0)},
        {"pageup", (KeyboardButton.KeyPageup, 0)},
        {"delete", (KeyboardButton.KeyDelete, 0)},
        {"end", (KeyboardButton.KeyEnd1, 0)},
        {"pagedown", (KeyboardButton.KeyPagedown, 0)},
        {"numlock", (KeyboardButton.KeyKeypadNumLockAndClear, 0)}
        // Add yours if neccessary
    }.ToImmutableDictionary();
}


public enum KeyboardButton : byte
{
    KeyNone = 0x00,
    KeyErrorrollover = 0x01,
    KeyPostfail = 0x02,
    KeyErrorundefined = 0x03,
    KeyA = 0x04,
    KeyB = 0x05,
    KeyC = 0x06,
    KeyD = 0x07,
    KeyE = 0x08,
    KeyF = 0x09,
    KeyG = 0x0A,
    KeyH = 0x0B,
    KeyI = 0x0C,
    KeyJ = 0x0D,
    KeyK = 0x0E,
    KeyL = 0x0F,
    KeyM = 0x10,
    KeyN = 0x11,
    KeyO = 0x12,
    KeyP = 0x13,
    KeyQ = 0x14,
    KeyR = 0x15,
    KeyS = 0x16,
    KeyT = 0x17,
    KeyU = 0x18,
    KeyV = 0x19,
    KeyW = 0x1A,
    KeyX = 0x1B,
    KeyY = 0x1C,
    KeyZ = 0x1D,
    Key1ExclamationMark = 0x1E,
    Key2At = 0x1F,
    Key3NumberSign = 0x20,
    Key4Dollar = 0x21,
    Key5Percent = 0x22,
    Key6Caret = 0x23,
    Key7Ampersand = 0x24,
    Key8Asterisk = 0x25,
    Key9OpenParenthesis = 0x26,
    Key0ClosedParenthesis = 0x27,
    KeyEnter = 0x28,
    KeyEscape = 0x29,
    KeyBackspace = 0x2A,
    KeyTab = 0x2B,
    KeySpacebar = 0x2C,
    KeyMinusUnderscore = 0x2D,
    KeyEqualPlus = 0x2E,
    KeyObracketAndObrace = 0x2F,
    KeyCbracketAndCbrace = 0x30,
    KeyBackslashVerticalBar = 0x31,
    KeyNonusNumberSignTilde = 0x32,
    KeySemicolonColon = 0x33,
    KeySingleAndDoubleQuote = 0x34,
    KeyGraveAccentAndTilde = 0x35,
    KeyCommaAndLess = 0x36,
    KeyDotGreater = 0x37,
    KeySlashQuestion = 0x38,
    KeyCapsLock = 0x39,
    KeyF1 = 0x3A,
    KeyF2 = 0x3B,
    KeyF3 = 0x3C,
    KeyF4 = 0x3D,
    KeyF5 = 0x3E,
    KeyF6 = 0x3F,
    KeyF7 = 0x40,
    KeyF8 = 0x41,
    KeyF9 = 0x42,
    KeyF10 = 0x43,
    KeyF11 = 0x44,
    KeyF12 = 0x45,
    KeyPrintscreen = 0x46,
    KeyScrollLock = 0x47,
    KeyPause = 0x48,
    KeyInsert = 0x49,
    KeyHome = 0x4A,
    KeyPageup = 0x4B,
    KeyDelete = 0x4C,
    KeyEnd1 = 0x4D,
    KeyPagedown = 0x4E,
    KeyRightarrow = 0x4F,
    KeyLeftarrow = 0x50,
    KeyDownarrow = 0x51,
    KeyUparrow = 0x52,
    KeyKeypadNumLockAndClear = 0x53,
    KeyKeypadSlash = 0x54,
    KeyKeypadAsteriks = 0x55,
    KeyKeypadMinus = 0x56,
    KeyKeypadPlus = 0x57,
    KeyKeypadEnter = 0x58,
    KeyKeypad1End = 0x59,
    KeyKeypad2DownArrow = 0x5A,
    KeyKeypad3Pagedn = 0x5B,
    KeyKeypad4LeftArrow = 0x5C,
    KeyKeypad5 = 0x5D,
    KeyKeypad6RightArrow = 0x5E,
    KeyKeypad7Home = 0x5F,
    KeyKeypad8UpArrow = 0x60,
    KeyKeypad9Pageup = 0x61,
    KeyKeypad0Insert = 0x62,
    KeyKeypadDecimalSeparatorDelete = 0x63,
    KeyNonusBackSlashVerticalBar = 0x64,
    KeyApplication = 0x65,
    KeyPower = 0x66,
    KeyKeypadEqual = 0x67,
    KeyF13 = 0x68,
    KeyF14 = 0x69,
    KeyF15 = 0x6A,
    KeyF16 = 0x6B,
    KeyF17 = 0x6C,
    KeyF18 = 0x6D,
    KeyF19 = 0x6E,
    KeyF20 = 0x6F,
    KeyF21 = 0x70,
    KeyF22 = 0x71,
    KeyF23 = 0x72,
    KeyF24 = 0x73,
    KeyExecute = 0x74,
    KeyHelp = 0x75,
    KeyMenu = 0x76,
    KeySelect = 0x77,
    KeyStop = 0x78,
    KeyAgain = 0x79,
    KeyUndo = 0x7A,
    KeyCut = 0x7B,
    KeyCopy = 0x7C,
    KeyPaste = 0x7D,
    KeyFind = 0x7E,
    KeyMute = 0x7F,
    KeyVolumeUp = 0x80,
    KeyVolumeDown = 0x81,
    KeyLockingCapsLock = 0x82,
    KeyLockingNumLock = 0x83,
    KeyLockingScrollLock = 0x84,
    KeyKeypadComma = 0x85,
    KeyKeypadEqualSign = 0x86,
    KeyInternational1 = 0x87,
    KeyInternational2 = 0x88,
    KeyInternational3 = 0x89,
    KeyInternational4 = 0x8A,
    KeyInternational5 = 0x8B,
    KeyInternational6 = 0x8C,
    KeyInternational7 = 0x8D,
    KeyInternational8 = 0x8E,
    KeyInternational9 = 0x8F,
    KeyLang1 = 0x90,
    KeyLang2 = 0x91,
    KeyLang3 = 0x92,
    KeyLang4 = 0x93,
    KeyLang5 = 0x94,
    KeyLang6 = 0x95,
    KeyLang7 = 0x96,
    KeyLang8 = 0x97,
    KeyLang9 = 0x98,
    KeyAlternateErase = 0x99,
    KeySysreq = 0x9A,
    KeyCancel = 0x9B,
    KeyClear = 0x9C,
    KeyPrior = 0x9D,
    KeyReturn = 0x9E,
    KeySeparator = 0x9F,
    KeyOut = 0xA0,
    KeyOper = 0xA1,
    KeyClearAgain = 0xA2,
    KeyCrsel = 0xA3,
    KeyExsel = 0xA4,
    KeyKeypad00 = 0xB0,
    KeyKeypad000 = 0xB1,
    KeyThousandsSeparator = 0xB2,
    KeyDecimalSeparator = 0xB3,
    KeyCurrencyUnit = 0xB4,
    KeyCurrencySubUnit = 0xB5,
    KeyKeypadOparenthesis = 0xB6,
    KeyKeypadCparenthesis = 0xB7,
    KeyKeypadObrace = 0xB8,
    KeyKeypadCbrace = 0xB9,
    KeyKeypadTab = 0xBA,
    KeyKeypadBackspace = 0xBB,
    KeyKeypadA = 0xBC,
    KeyKeypadB = 0xBD,
    KeyKeypadC = 0xBE,
    KeyKeypadD = 0xBF,
    KeyKeypadE = 0xC0,
    KeyKeypadF = 0xC1,
    KeyKeypadXor = 0xC2,
    KeyKeypadCaret = 0xC3,
    KeyKeypadPercent = 0xC4,
    KeyKeypadLess = 0xC5,
    KeyKeypadGreater = 0xC6,
    KeyKeypadAmpersand = 0xC7,
    KeyKeypadLogicalAnd = 0xC8,
    KeyKeypadVerticalBar = 0xC9,
    KeyKeypadLogiaclOr = 0xCA,
    KeyKeypadColon = 0xCB,
    KeyKeypadNumberSign = 0xCC,
    KeyKeypadSpace = 0xCD,
    KeyKeypadAt = 0xCE,
    KeyKeypadExclamationMark = 0xCF,
    KeyKeypadMemoryStore = 0xD0,
    KeyKeypadMemoryRecall = 0xD1,
    KeyKeypadMemoryClear = 0xD2,
    KeyKeypadMemoryAdd = 0xD3,
    KeyKeypadMemorySubtract = 0xD4,
    KeyKeypadMemoryMultiply = 0xD5,
    KeyKeypadMemoryDivide = 0xD6,
    KeyKeypadPlusminus = 0xD7,
    KeyKeypadClear = 0xD8,
    KeyKeypadClearEntry = 0xD9,
    KeyKeypadBinary = 0xDA,
    KeyKeypadOctal = 0xDB,
    KeyKeypadDecimal = 0xDC,
    KeyKeypadHexadecimal = 0xDD,
    KeyLeftcontrol = 0xE0,
    KeyLeftshift = 0xE1,
    KeyLeftalt = 0xE2,
    KeyLeftGui = 0xE3,
    KeyRightcontrol = 0xE4,
    KeyRightshift = 0xE5,
    KeyRightalt = 0xE6,
    KeyRightGui = 0xE7
}
