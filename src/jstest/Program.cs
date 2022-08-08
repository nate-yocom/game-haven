using GameHaven.Controller;

GenericJoystick joystick = new GenericJoystick("/dev/input/js0");
joystick.AxisChanged += (e, d) => {
    Console.WriteLine($"{joystick.Identifier} [{joystick.Device}] => {e}: Axis[{d.Axis}, {d.Value}]");
};

joystick.ButtonChanged += (e, d) => {
    Console.WriteLine($"{joystick.Identifier} [{joystick.Device}] => {e}: Button[{d.Button}, {d.Pressed}]");
};

Console.Read();
