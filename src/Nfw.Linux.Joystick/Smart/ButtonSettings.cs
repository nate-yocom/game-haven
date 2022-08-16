namespace Nfw.Linux.Joystick.Smart {
    public class ButtonSettings {
        // How long must button have been held to cause a long click on release    
        public int? LongClickMinimumDurationMilliseconds { get; set; } = null;
        public int? ShortClickMinimumDurationMilliseconds { get; set; } = null;
        public int? ShortClickMaximumDurationMillseconds { get; set; } = null;
        
        public void MergeSettings(ButtonSettings? rhs) {
            if (rhs == null) return;            
            if(rhs.LongClickMinimumDurationMilliseconds != null) LongClickMinimumDurationMilliseconds = rhs.LongClickMinimumDurationMilliseconds;
            if(rhs.ShortClickMinimumDurationMilliseconds != null) ShortClickMinimumDurationMilliseconds = rhs.ShortClickMinimumDurationMilliseconds;
            if(rhs.ShortClickMaximumDurationMillseconds != null) ShortClickMaximumDurationMillseconds = rhs.ShortClickMaximumDurationMillseconds;            
        }
        
        public int GetLongClickMinimumDurationMilliseconds() {
            return LongClickMinimumDurationMilliseconds ?? DefaultSettings?.LongClickMinimumDurationMilliseconds ?? DEFAULT_LONG_CLICK_MINIMUM;
        }

        public int GetShortClickMinimumDurationMilliseconds() {
            return ShortClickMinimumDurationMilliseconds ?? DefaultSettings?.ShortClickMinimumDurationMilliseconds ?? DEFAULT_SHORT_CLICK_MINIMUM;
        }        
        private const int DEFAULT_LONG_CLICK_MINIMUM = 1000;
        private const int DEFAULT_SHORT_CLICK_MINIMUM = 0;
        
        public static ButtonSettings DefaultSettings { get; set; } = new ButtonSettings();        
    }    
}