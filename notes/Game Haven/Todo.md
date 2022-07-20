# TBD
- [ ] Minimal rpi engine and async renderer - launches, displays logo
- [ ] Minimal 'app only' engine - is there a x-platform render mech that would let us do bmp->window for compat with fb?
- [ ] Detect missing controller and show image
- [ ] Add dedicated game-loop, pull controller events off a queue (have controller loop push to same)
- [ ] Add non-infringing game images
- [ ] Adaptive keyboard mode
	- [ ] Show quick-key/shortcuts (TBD) on LCD
- [ ] Configuration:
	- [ ] Calibration: Allow for hold-tapping a button N times, average, this becomes long-click value?
	- [ ] Flip screen orientation
	- [ ] Add entire new layer?
- [ ] Other button events/handling:
	- [ ] Add a 'duration' to data, i.e. only if said event happens with provided duration
	- [ ] Allow for use of duration in emit side - i.e. duration of press == number of pixels to move mouse (or other arbitrary functions)
		- [ ] inline c#? lua? other minor scripting? 
- [ ] Multiple button inputs in data - i.e. "When A + B for some duration do thing..."
- [ ] Output joystick? 
	- [ ] Xbox, ps4, switch, ...?
- [ ] Other mouse movement options, move while holding - with min click travel?
- [ ] Configure pi script:
	- [ ] Add a lot of env checking, verify distribution, current user is root, etc etc
	- [ ] Or maybe run as current user (so we can know who to chown to) and expect sudo to work?
		
# Refactor
- [ ] ScaledRelativeMouse class - less Point/PointF construction, just pass around int/floats might be nicer on the stack?

# Before Any 'Production'
- [ ] All above TBDs
- [ ] Remove any ssh keys
- [ ] Generic default user (instead of 'nate')
- [ ] Auto-update - or update via http, pull from github? other?
- [ ] No builtin wifi config 
	- [ ] How to configure networking? must use eth0 first, then provide web iface? 
	- [ ] UI code show / and allow for select/store?
- [ ] Better configuration / data manip
	- [ ] In the device UI?
	- [ ] Webpage (hosted via app/device)?
	- [ ] Remote service (pull down from cloud)?
- [ ] How to streamline bootstrap/build of new device?