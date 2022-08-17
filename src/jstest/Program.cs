using Nfw.Linux.Joystick.Xpad;
using Nfw.Linux.Joystick.Smart;

XboxGamepad joystick = new XboxGamepad("/dev/input/js0", ButtonEventTypes.All);
joystick.TreatAxisAsButtons = true;
joystick.DefaultButtonSettings = new ButtonSettings() {
    LongPressMinimumDurationMilliseconds = 500,
    ShortPressMinimumDurationMilliseconds = 15
};

joystick.ButtonCallback = (j, b, e, v, d) => {
    Console.WriteLine($"{j.DeviceName} => Button[{b}, {e}] => {v} [dTime: {d}]");
};

joystick.AxisCallback = (j, a, v, d) => {
    Console.WriteLine($"{j.DeviceName} => Axis[{a}] => {v} [dTime: {d}]");
};

joystick.ConnectedCallback = (j, c) => {
    Console.WriteLine($"{j.DeviceName} => Connected[{c}]");
};

Console.Read();
