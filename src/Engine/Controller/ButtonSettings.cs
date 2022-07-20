namespace GameHaven.Engine.Controller {
    public class ButtonSettings {

        // How often should the hold event fire (at most, no garauntee of absolute rate)
        public int? HoldEventFireIntervalMilliseconds { get; set; } = null;    
        // How long must button have been held to cause a long click on release    
        public int? LongClickMinimumDurationMilliseconds { get; set; } = null;
        public int? ShortClickMinimumDurationMilliseconds { get; set; } = null;
        public int? ShortClickMaximumDurationMillseconds { get; set; } = null;
        
        public void MergeSettings(ButtonSettings? rhs) {
            if (rhs == null) return;
            if(rhs.HoldEventFireIntervalMilliseconds != null) HoldEventFireIntervalMilliseconds = rhs.HoldEventFireIntervalMilliseconds;
            if(rhs.LongClickMinimumDurationMilliseconds != null) LongClickMinimumDurationMilliseconds = rhs.LongClickMinimumDurationMilliseconds;
            if(rhs.ShortClickMinimumDurationMilliseconds != null) ShortClickMinimumDurationMilliseconds = rhs.ShortClickMinimumDurationMilliseconds;
            if(rhs.ShortClickMaximumDurationMillseconds != null) ShortClickMaximumDurationMillseconds = rhs.ShortClickMaximumDurationMillseconds;            
        }

        public int GetHoldEventFireIntervalMilliseconds() {
            return HoldEventFireIntervalMilliseconds ?? DefaultSettings?.HoldEventFireIntervalMilliseconds ?? DEFAULT_HOLD_EVENT_FIRE_INTERVAL;
        }

        public int GetLongClickMinimumDurationMilliseconds() {
            return LongClickMinimumDurationMilliseconds ?? DefaultSettings?.LongClickMinimumDurationMilliseconds ?? DEFAULT_LONG_CLICK_MINIMUM;
        }

        public int GetShortClickMinimumDurationMilliseconds() {
            return ShortClickMinimumDurationMilliseconds ?? DefaultSettings?.ShortClickMinimumDurationMilliseconds ?? DEFAULT_SHORT_CLICK_MINIMUM;
        }
        private const int DEFAULT_HOLD_EVENT_FIRE_INTERVAL = 100;
        private const int DEFAULT_LONG_CLICK_MINIMUM = 1000;
        private const int DEFAULT_SHORT_CLICK_MINIMUM = 0;
        
        public static ButtonSettings DefaultSettings { get; set; } = new ButtonSettings();        
    }    
}