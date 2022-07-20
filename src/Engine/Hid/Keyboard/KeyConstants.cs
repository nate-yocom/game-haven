/**
 * USB HID Keyboard scan codes as per USB spec 1.11 
 */

namespace GameHaven.Engine.Hid.Keyboard {

    /**
    * Modifier masks - used for the first byte in the HID report.
    * NOTE: The second byte in the report is reserved, 0x00
    */
    public enum Modifiers {
        NONE            = 0x00,
        KEY_MOD_LCTRL   = 0x01,
        KEY_MOD_LSHIFT  = 0x02,
        KEY_MOD_LALT    = 0x04,
        KEY_MOD_LMETA   = 0x08,
        KEY_MOD_RCTRL   = 0x10,
        KEY_MOD_RSHIFT  = 0x20,
        KEY_MOD_RALT    = 0x40,
        KEY_MOD_RMETA   = 0x80
    }

    /**
    * Scan codes - last N slots in the HID report (usually 6).
    * 0x00 if no key pressed.
    * 
    * If more than N keys are pressed, the HID reports 
    * KEY_ERR_OVF in all slots to indicate this condition.
    */
    public enum Keys {        
        KEY_NONE = 0x00, // No key pressed
        KEY_ERR_OVF = 0x01, //  Keyboard Error Roll Over - used for all slots if too many keys are pressed ("Phantom key")
        // 0x02, //  Keyboard POST Fail
        // 0x03, //  Keyboard Error Undefined
        KEY_A = 0x04, // Keyboard a and A
        KEY_B = 0x05, // Keyboard b and B
        KEY_C = 0x06, // Keyboard c and C
        KEY_D = 0x07, // Keyboard d and D
        KEY_E = 0x08, // Keyboard e and E
        KEY_F = 0x09, // Keyboard f and F
        KEY_G = 0x0a, // Keyboard g and G
        KEY_H = 0x0b, // Keyboard h and H
        KEY_I = 0x0c, // Keyboard i and I
        KEY_J = 0x0d, // Keyboard j and J
        KEY_K = 0x0e, // Keyboard k and K
        KEY_L = 0x0f, // Keyboard l and L
        KEY_M = 0x10, // Keyboard m and M
        KEY_N = 0x11, // Keyboard n and N
        KEY_O = 0x12, // Keyboard o and O
        KEY_P = 0x13, // Keyboard p and P
        KEY_Q = 0x14, // Keyboard q and Q
        KEY_R = 0x15, // Keyboard r and R
        KEY_S = 0x16, // Keyboard s and S
        KEY_T = 0x17, // Keyboard t and T
        KEY_U = 0x18, // Keyboard u and U
        KEY_V = 0x19, // Keyboard v and V
        KEY_W = 0x1a, // Keyboard w and W
        KEY_X = 0x1b, // Keyboard x and X
        KEY_Y = 0x1c, // Keyboard y and Y
        KEY_Z = 0x1d, // Keyboard z and Z
        KEY_1 = 0x1e, // Keyboard 1 and !
        KEY_2 = 0x1f, // Keyboard 2 and @
        KEY_3 = 0x20, // Keyboard 3 and #
        KEY_4 = 0x21, // Keyboard 4 and $
        KEY_5 = 0x22, // Keyboard 5 and %
        KEY_6 = 0x23, // Keyboard 6 and ^
        KEY_7 = 0x24, // Keyboard 7 and &
        KEY_8 = 0x25, // Keyboard 8 and *
        KEY_9 = 0x26, // Keyboard 9 and (
        KEY_0 = 0x27, // Keyboard 0 and )
        KEY_ENTER = 0x28, // Keyboard Return (ENTER)
        KEY_ESC = 0x29, // Keyboard ESCAPE
        KEY_BACKSPACE = 0x2a, // Keyboard DELETE (Backspace)
        KEY_TAB = 0x2b, // Keyboard Tab
        KEY_SPACE = 0x2c, // Keyboard Spacebar
        KEY_MINUS = 0x2d, // Keyboard - and _
        KEY_EQUAL = 0x2e, // Keyboard = and +
        KEY_LEFTBRACE = 0x2f, // Keyboard [ and {
        KEY_RIGHTBRACE = 0x30, // Keyboard ] and }
        KEY_BACKSLASH = 0x31, // Keyboard \ and |
        KEY_HASHTILDE = 0x32, // Keyboard Non-US # and ~
        KEY_SEMICOLON = 0x33, // Keyboard ; and :
        KEY_APOSTROPHE = 0x34, // Keyboard ' and "
        KEY_GRAVE = 0x35, // Keyboard ` and ~
        KEY_COMMA = 0x36, // Keyboard , and <
        KEY_DOT = 0x37, // Keyboard . and >
        KEY_SLASH = 0x38, // Keyboard / and ?
        KEY_CAPSLOCK = 0x39, // Keyboard Caps Lock
        KEY_F1 = 0x3a, // Keyboard F1
        KEY_F2 = 0x3b, // Keyboard F2
        KEY_F3 = 0x3c, // Keyboard F3
        KEY_F4 = 0x3d, // Keyboard F4
        KEY_F5 = 0x3e, // Keyboard F5
        KEY_F6 = 0x3f, // Keyboard F6
        KEY_F7 = 0x40, // Keyboard F7
        KEY_F8 = 0x41, // Keyboard F8
        KEY_F9 = 0x42, // Keyboard F9
        KEY_F10 = 0x43, // Keyboard F10
        KEY_F11 = 0x44, // Keyboard F11
        KEY_F12 = 0x45, // Keyboard F12
        KEY_SYSRQ = 0x46, // Keyboard Print Screen
        KEY_SCROLLLOCK = 0x47, // Keyboard Scroll Lock
        KEY_PAUSE = 0x48, // Keyboard Pause
        KEY_INSERT = 0x49, // Keyboard Insert
        KEY_HOME = 0x4a, // Keyboard Home
        KEY_PAGEUP = 0x4b, // Keyboard Page Up
        KEY_DELETE = 0x4c, // Keyboard Delete Forward
        KEY_END = 0x4d, // Keyboard End
        KEY_PAGEDOWN = 0x4e, // Keyboard Page Down
        KEY_RIGHT = 0x4f, // Keyboard Right Arrow
        KEY_LEFT = 0x50, // Keyboard Left Arrow
        KEY_DOWN = 0x51, // Keyboard Down Arrow
        KEY_UP = 0x52, // Keyboard Up Arrow
        KEY_NUMLOCK = 0x53, // Keyboard Num Lock and Clear
        KEY_KPSLASH = 0x54, // Keypad /
        KEY_KPASTERISK = 0x55, // Keypad *
        KEY_KPMINUS = 0x56, // Keypad -
        KEY_KPPLUS = 0x57, // Keypad +
        KEY_KPENTER = 0x58, // Keypad ENTER
        KEY_KP1 = 0x59, // Keypad 1 and End
        KEY_KP2 = 0x5a, // Keypad 2 and Down Arrow
        KEY_KP3 = 0x5b, // Keypad 3 and PageDn
        KEY_KP4 = 0x5c, // Keypad 4 and Left Arrow
        KEY_KP5 = 0x5d, // Keypad 5
        KEY_KP6 = 0x5e, // Keypad 6 and Right Arrow
        KEY_KP7 = 0x5f, // Keypad 7 and Home
        KEY_KP8 = 0x60, // Keypad 8 and Up Arrow
        KEY_KP9 = 0x61, // Keypad 9 and Page Up
        KEY_KP0 = 0x62, // Keypad 0 and Insert
        KEY_KPDOT = 0x63, // Keypad . and Delete
        KEY_102ND = 0x64, // Keyboard Non-US \ and |
        KEY_COMPOSE = 0x65, // Keyboard Application
        KEY_POWER = 0x66, // Keyboard Power
        KEY_KPEQUAL = 0x67, // Keypad =
        KEY_F13 = 0x68, // Keyboard F13
        KEY_F14 = 0x69, // Keyboard F14
        KEY_F15 = 0x6a, // Keyboard F15
        KEY_F16 = 0x6b, // Keyboard F16
        KEY_F17 = 0x6c, // Keyboard F17
        KEY_F18 = 0x6d, // Keyboard F18
        KEY_F19 = 0x6e, // Keyboard F19
        KEY_F20 = 0x6f, // Keyboard F20
        KEY_F21 = 0x70, // Keyboard F21
        KEY_F22 = 0x71, // Keyboard F22
        KEY_F23 = 0x72, // Keyboard F23
        KEY_F24 = 0x73, // Keyboard F24
        KEY_OPEN = 0x74, // Keyboard Execute
        KEY_HELP = 0x75, // Keyboard Help
        KEY_PROPS = 0x76, // Keyboard Menu
        KEY_FRONT = 0x77, // Keyboard Select
        KEY_STOP = 0x78, // Keyboard Stop
        KEY_AGAIN = 0x79, // Keyboard Again
        KEY_UNDO = 0x7a, // Keyboard Undo
        KEY_CUT = 0x7b, // Keyboard Cut
        KEY_COPY = 0x7c, // Keyboard Copy
        KEY_PASTE = 0x7d, // Keyboard Paste
        KEY_FIND = 0x7e, // Keyboard Find
        KEY_MUTE = 0x7f, // Keyboard Mute
        KEY_VOLUMEUP = 0x80, // Keyboard Volume Up
        KEY_VOLUMEDOWN = 0x81, // Keyboard Volume Down
        // = 0x82  Keyboard Locking Caps Lock
        // = 0x83  Keyboard Locking Num Lock
        // = 0x84  Keyboard Locking Scroll Lock
        KEY_KPCOMMA = 0x85, // Keypad Comma
        // = 0x86  Keypad Equal Sign
        KEY_RO = 0x87, // Keyboard International1
        KEY_KATAKANAHIRAGANA = 0x88, // Keyboard International2
        KEY_YEN = 0x89, // Keyboard International3
        KEY_HENKAN = 0x8a, // Keyboard International4
        KEY_MUHENKAN = 0x8b, // Keyboard International5
        KEY_KPJPCOMMA = 0x8c, // Keyboard International6
        // = 0x8d  Keyboard International7
        // = 0x8e  Keyboard International8
        // = 0x8f  Keyboard International9
        KEY_HANGEUL = 0x90, // Keyboard LANG1
        KEY_HANJA = 0x91, // Keyboard LANG2
        KEY_KATAKANA = 0x92, // Keyboard LANG3
        KEY_HIRAGANA = 0x93, // Keyboard LANG4
        KEY_ZENKAKUHANKAKU = 0x94, // Keyboard LANG5
        // = 0x95  Keyboard LANG6
        // = 0x96  Keyboard LANG7
        // = 0x97  Keyboard LANG8
        // = 0x98  Keyboard LANG9
        // = 0x99  Keyboard Alternate Erase
        // = 0x9a  Keyboard SysReq/Attention
        // = 0x9b  Keyboard Cancel
        // = 0x9c  Keyboard Clear
        // = 0x9d  Keyboard Prior
        // = 0x9e  Keyboard Return
        // = 0x9f  Keyboard Separator
        // = 0xa0  Keyboard Out
        // = 0xa1  Keyboard Oper
        // = 0xa2  Keyboard Clear/Again
        // = 0xa3  Keyboard CrSel/Props
        // = 0xa4  Keyboard ExSel
        // = 0xb0  Keypad 00
        // = 0xb1  Keypad 000
        // = 0xb2  Thousands Separator
        // = 0xb3  Decimal Separator
        // = 0xb4  Currency Unit
        // = 0xb5  Currency Sub-unit
        KEY_KPLEFTPAREN = 0xb6, // Keypad (
        KEY_KPRIGHTPAREN = 0xb7, // Keypad )
        // = 0xb8  Keypad {
        // = 0xb9  Keypad }
        // = 0xba  Keypad Tab
        // = 0xbb  Keypad Backspace
        // = 0xbc  Keypad A
        // = 0xbd  Keypad B
        // = 0xbe  Keypad C
        // = 0xbf  Keypad D
        // = 0xc0  Keypad E
        // = 0xc1  Keypad F
        // = 0xc2  Keypad XOR
        // = 0xc3  Keypad ^
        // = 0xc4  Keypad %
        // = 0xc5  Keypad <
        // = 0xc6  Keypad >
        // = 0xc7  Keypad &
        // = 0xc8  Keypad &&
        // = 0xc9  Keypad |
        // = 0xca  Keypad ||
        // = 0xcb  Keypad :
        // = 0xcc  Keypad #
        // = 0xcd  Keypad Space
        // = 0xce  Keypad @
        // = 0xcf  Keypad !
        // = 0xd0  Keypad Memory Store
        // = 0xd1  Keypad Memory Recall
        // = 0xd2  Keypad Memory Clear
        // = 0xd3  Keypad Memory Add
        // = 0xd4  Keypad Memory Subtract
        // = 0xd5  Keypad Memory Multiply
        // = 0xd6  Keypad Memory Divide
        // = 0xd7  Keypad +/-
        // = 0xd8  Keypad Clear
        // = 0xd9  Keypad Clear Entry
        // = 0xda  Keypad Binary
        // = 0xdb  Keypad Octal
        // = 0xdc  Keypad Decimal
        // = 0xdd  Keypad Hexadecimal
        KEY_LEFTCTRL = 0xe0, // Keyboard Left Control
        KEY_LEFTSHIFT = 0xe1, // Keyboard Left Shift
        KEY_LEFTALT = 0xe2, // Keyboard Left Alt
        KEY_LEFTMETA = 0xe3, // Keyboard Left GUI
        KEY_RIGHTCTRL = 0xe4, // Keyboard Right Control
        KEY_RIGHTSHIFT = 0xe5, // Keyboard Right Shift
        KEY_RIGHTALT = 0xe6, // Keyboard Right Alt
        KEY_RIGHTMETA = 0xe7, // Keyboard Right GUI
        KEY_MEDIA_PLAYPAUSE = 0xe8,
        KEY_MEDIA_STOPCD = 0xe9,
        KEY_MEDIA_PREVIOUSSONG = 0xea,
        KEY_MEDIA_NEXTSONG = 0xeb,
        KEY_MEDIA_EJECTCD = 0xec,
        KEY_MEDIA_VOLUMEUP = 0xed,
        KEY_MEDIA_VOLUMEDOWN = 0xee,
        KEY_MEDIA_MUTE = 0xef,
        KEY_MEDIA_WWW = 0xf0,
        KEY_MEDIA_BACK = 0xf1,
        KEY_MEDIA_FORWARD = 0xf2,
        KEY_MEDIA_STOP = 0xf3,
        KEY_MEDIA_FIND = 0xf4,
        KEY_MEDIA_SCROLLUP = 0xf5,
        KEY_MEDIA_SCROLLDOWN = 0xf6,
        KEY_MEDIA_EDIT = 0xf7,
        KEY_MEDIA_SLEEP = 0xf8,
        KEY_MEDIA_COFFEE = 0xf9,
        KEY_MEDIA_REFRESH = 0xfa,
        KEY_MEDIA_CALC = 0xfb
    }

    public struct KeyAndModifier {
        public Keys KeyScanCode { get; set; }
        public Modifiers ModifierScanCode { get; set; }        
    }

    public static class KeyboardHelper { 
        private static Dictionary<char, KeyAndModifier> s_charToScanCode = new Dictionary<char, KeyAndModifier>() {
            { 'a', new KeyAndModifier() { KeyScanCode = Keys.KEY_A, ModifierScanCode = Modifiers.NONE } },
            { 'b', new KeyAndModifier() { KeyScanCode = Keys.KEY_B, ModifierScanCode = Modifiers.NONE } },
            { 'c', new KeyAndModifier() { KeyScanCode = Keys.KEY_C, ModifierScanCode = Modifiers.NONE } },
            { 'd', new KeyAndModifier() { KeyScanCode = Keys.KEY_D, ModifierScanCode = Modifiers.NONE } },
            { 'e', new KeyAndModifier() { KeyScanCode = Keys.KEY_E, ModifierScanCode = Modifiers.NONE } },
            { 'f', new KeyAndModifier() { KeyScanCode = Keys.KEY_F, ModifierScanCode = Modifiers.NONE } },
            { 'g', new KeyAndModifier() { KeyScanCode = Keys.KEY_G, ModifierScanCode = Modifiers.NONE } },
            { 'h', new KeyAndModifier() { KeyScanCode = Keys.KEY_H, ModifierScanCode = Modifiers.NONE } },
            { 'i', new KeyAndModifier() { KeyScanCode = Keys.KEY_I, ModifierScanCode = Modifiers.NONE } },
            { 'j', new KeyAndModifier() { KeyScanCode = Keys.KEY_J, ModifierScanCode = Modifiers.NONE } },
            { 'k', new KeyAndModifier() { KeyScanCode = Keys.KEY_K, ModifierScanCode = Modifiers.NONE } },
            { 'l', new KeyAndModifier() { KeyScanCode = Keys.KEY_L, ModifierScanCode = Modifiers.NONE } },
            { 'm', new KeyAndModifier() { KeyScanCode = Keys.KEY_M, ModifierScanCode = Modifiers.NONE } },
            { 'n', new KeyAndModifier() { KeyScanCode = Keys.KEY_N, ModifierScanCode = Modifiers.NONE } },
            { 'o', new KeyAndModifier() { KeyScanCode = Keys.KEY_O, ModifierScanCode = Modifiers.NONE } },
            { 'p', new KeyAndModifier() { KeyScanCode = Keys.KEY_P, ModifierScanCode = Modifiers.NONE } },
            { 'q', new KeyAndModifier() { KeyScanCode = Keys.KEY_Q, ModifierScanCode = Modifiers.NONE } },
            { 'r', new KeyAndModifier() { KeyScanCode = Keys.KEY_R, ModifierScanCode = Modifiers.NONE } },
            { 's', new KeyAndModifier() { KeyScanCode = Keys.KEY_S, ModifierScanCode = Modifiers.NONE } },
            { 't', new KeyAndModifier() { KeyScanCode = Keys.KEY_T, ModifierScanCode = Modifiers.NONE } },
            { 'u', new KeyAndModifier() { KeyScanCode = Keys.KEY_U, ModifierScanCode = Modifiers.NONE } },
            { 'v', new KeyAndModifier() { KeyScanCode = Keys.KEY_V, ModifierScanCode = Modifiers.NONE } },
            { 'w', new KeyAndModifier() { KeyScanCode = Keys.KEY_W, ModifierScanCode = Modifiers.NONE } },
            { 'x', new KeyAndModifier() { KeyScanCode = Keys.KEY_X, ModifierScanCode = Modifiers.NONE } },
            { 'y', new KeyAndModifier() { KeyScanCode = Keys.KEY_Y, ModifierScanCode = Modifiers.NONE } },
            { 'z', new KeyAndModifier() { KeyScanCode = Keys.KEY_Z, ModifierScanCode = Modifiers.NONE } },
            { 'A', new KeyAndModifier() { KeyScanCode = Keys.KEY_A, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'B', new KeyAndModifier() { KeyScanCode = Keys.KEY_B, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'C', new KeyAndModifier() { KeyScanCode = Keys.KEY_C, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'D', new KeyAndModifier() { KeyScanCode = Keys.KEY_D, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'E', new KeyAndModifier() { KeyScanCode = Keys.KEY_E, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'F', new KeyAndModifier() { KeyScanCode = Keys.KEY_F, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'G', new KeyAndModifier() { KeyScanCode = Keys.KEY_G, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'H', new KeyAndModifier() { KeyScanCode = Keys.KEY_H, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'I', new KeyAndModifier() { KeyScanCode = Keys.KEY_I, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'J', new KeyAndModifier() { KeyScanCode = Keys.KEY_J, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'K', new KeyAndModifier() { KeyScanCode = Keys.KEY_K, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'L', new KeyAndModifier() { KeyScanCode = Keys.KEY_L, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'M', new KeyAndModifier() { KeyScanCode = Keys.KEY_M, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'N', new KeyAndModifier() { KeyScanCode = Keys.KEY_N, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'O', new KeyAndModifier() { KeyScanCode = Keys.KEY_O, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'P', new KeyAndModifier() { KeyScanCode = Keys.KEY_P, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'Q', new KeyAndModifier() { KeyScanCode = Keys.KEY_Q, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'R', new KeyAndModifier() { KeyScanCode = Keys.KEY_R, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'S', new KeyAndModifier() { KeyScanCode = Keys.KEY_S, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'T', new KeyAndModifier() { KeyScanCode = Keys.KEY_T, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'U', new KeyAndModifier() { KeyScanCode = Keys.KEY_U, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'V', new KeyAndModifier() { KeyScanCode = Keys.KEY_V, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'W', new KeyAndModifier() { KeyScanCode = Keys.KEY_W, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'X', new KeyAndModifier() { KeyScanCode = Keys.KEY_X, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'Y', new KeyAndModifier() { KeyScanCode = Keys.KEY_Y, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { 'Z', new KeyAndModifier() { KeyScanCode = Keys.KEY_Z, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { ' ', new KeyAndModifier() { KeyScanCode = Keys.KEY_SPACE, ModifierScanCode = Modifiers.NONE } },        
            { '1', new KeyAndModifier() { KeyScanCode = Keys.KEY_1, ModifierScanCode = Modifiers.NONE } },
            { '2', new KeyAndModifier() { KeyScanCode = Keys.KEY_2, ModifierScanCode = Modifiers.NONE } },
            { '3', new KeyAndModifier() { KeyScanCode = Keys.KEY_3, ModifierScanCode = Modifiers.NONE } },
            { '4', new KeyAndModifier() { KeyScanCode = Keys.KEY_4, ModifierScanCode = Modifiers.NONE } },
            { '5', new KeyAndModifier() { KeyScanCode = Keys.KEY_5, ModifierScanCode = Modifiers.NONE } },
            { '6', new KeyAndModifier() { KeyScanCode = Keys.KEY_6, ModifierScanCode = Modifiers.NONE } },
            { '7', new KeyAndModifier() { KeyScanCode = Keys.KEY_7, ModifierScanCode = Modifiers.NONE } },
            { '8', new KeyAndModifier() { KeyScanCode = Keys.KEY_8, ModifierScanCode = Modifiers.NONE } },
            { '9', new KeyAndModifier() { KeyScanCode = Keys.KEY_9, ModifierScanCode = Modifiers.NONE } },
            { '0', new KeyAndModifier() { KeyScanCode = Keys.KEY_0, ModifierScanCode = Modifiers.NONE } },
            { '!', new KeyAndModifier() { KeyScanCode = Keys.KEY_1, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { '@', new KeyAndModifier() { KeyScanCode = Keys.KEY_2, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { '#', new KeyAndModifier() { KeyScanCode = Keys.KEY_3, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { '$', new KeyAndModifier() { KeyScanCode = Keys.KEY_4, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { '%', new KeyAndModifier() { KeyScanCode = Keys.KEY_5, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { '^', new KeyAndModifier() { KeyScanCode = Keys.KEY_6, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { '&', new KeyAndModifier() { KeyScanCode = Keys.KEY_7, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { '*', new KeyAndModifier() { KeyScanCode = Keys.KEY_8, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { '(', new KeyAndModifier() { KeyScanCode = Keys.KEY_9, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },
            { ')', new KeyAndModifier() { KeyScanCode = Keys.KEY_0, ModifierScanCode = Modifiers.KEY_MOD_LSHIFT } },        
        };

        private static KeyAndModifier s_defaultUnknownChar = new KeyAndModifier() { KeyScanCode = Keys.KEY_SPACE, ModifierScanCode = Modifiers.NONE };

        public static KeyAndModifier KeyFromChar(char input) {
            return KeyFromChar(input, ' ');
        }
        public static KeyAndModifier KeyFromChar(char input, char def) {
            return s_charToScanCode.ContainsKey(input) ? s_charToScanCode[input] : s_charToScanCode.ContainsKey(def) ? s_charToScanCode[def] : s_defaultUnknownChar;
        }
    }    
}