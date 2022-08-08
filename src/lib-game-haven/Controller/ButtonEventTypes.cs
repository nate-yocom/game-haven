namespace GameHaven.Controller {
    [Flags]
    public enum ButtonEventTypes 
    {
            None        = 0x00,
            Press       = 0x01,
            Release     = 0x02,
            Hold        = 0x04,
            ShortClick  = 0x80,   // Press -> release
            LongClick   = 0x10,   // The result of a Press -> Release with a defined 'hold' threshold
            All         = 0xFF
    }
}