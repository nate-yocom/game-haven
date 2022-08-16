using Nfw.Linux.Joystick.Xpad;
using Nfw.Linux.Joystick.Smart;

XboxGamepad joystick = new XboxGamepad("/dev/input/js0", ButtonEventTypes.All);

joystick.AxisCallback = (j, e, d) => {
    Console.WriteLine($"{j.DeviceName} => Simple Axis[{e}, {d}]");
};

joystick.ButtonCallback = (j, e, d) => {
    Console.WriteLine($"{j.DeviceName} => Simple Button[{e}, {d}]");
};

joystick.SmartButtonCallback = (j, b, e, v, d) => {
    Console.WriteLine($"{j.DeviceName} => Smart Button[{b}, {e}] => {v} [dTime: {d}]");
};

joystick.SmartAxisCallback = (j, a, v, d) => {
    Console.WriteLine($"{j.DeviceName} => Smart Axis[{a}] => {v} [dTime: {d}]");
};

joystick.XboxButtonCallback = (j, b, e, v, d) => {
    Console.WriteLine($"{j.DeviceName} => XBox Button[{b}, {e}] => {v} [dTime: {d}]");
};

joystick.XboxAxisCallback = (j, a, v, d) => {
    Console.WriteLine($"{j.DeviceName} => XBox Axis[{a}] => {v} [dTime: {d}]");
};

joystick.ConnectedCallback = (j, c) => {
    Console.WriteLine($"{j.DeviceName} => Connected[{c}]");
};

Console.Read();
