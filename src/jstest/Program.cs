using Nfw.Linux.Joystick.Generic;

Joystick joystick = new Joystick("/dev/input/js0");
joystick.AxisCallback = (j, e, d) => {
    Console.WriteLine($"{j.DeviceName} => Axis[{e}, {d}]");
};

joystick.ButtonCallback = (j, e, d) => {
    Console.WriteLine($"{j.DeviceName} => Button[{e}, {d}]");
};

joystick.ConnectedCallback = (j, c) => {
    Console.WriteLine($"{j.DeviceName} => Connected[{c}]");
};

Console.Read();
