using Nfw.Linux.Joystick.Smart;
using Nfw.Linux.Joystick.Xpad;

namespace GameHaven.States {

    public abstract class BaseState {
        private StateEngine _engine;

        public BaseState(StateEngine engine) {
            _engine = engine;
        }

        public abstract void Enter();
        public abstract void Exit();        

        public void EventLoop(Button button, ButtonEventTypes buttonEvent, double duration) {
            // common handling here, i.e. 'return to main menu button from engine, i.e.
            // if button == _engine.main manu button && duration >= _engine.mainmenubutton length {
            //      _engine.SetState(main menu)
            // }
        }
    }
    
}