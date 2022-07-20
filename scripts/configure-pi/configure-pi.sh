#!/bin/bash

# Installs /opt/usb-gadget/ scripts and systemd service

# Exit on first error.
set -e

# Echo commands to stdout.
set -x

# TBD:
#   - Add a lot of env checking, do we have raspi-config, ... etc?
#   - Are we running as root? (MUST)
#   - apt-get update / upgrade?

BUILTIN_USER=nate
USE_SPI_DISPLAY=0

#apt-get update -y
#apt-get upgrade -y
#apt-get install -y git

reboot_needed=0

# Configure DWC in modules and boot config
if grep -Fxq "dtoverlay=dwc2" /boot/config.txt
then
    echo "dwc2 already in /boot/config.txt"
else
    echo "dtoverlay=dwc2" >> /boot/config.txt
    reboot_needed=1
fi

if grep -Fxq "dwc2" /etc/modules    
then
    echo "dwc2 already in /modules"
else
    echo "dwc2" >> /etc/modules
    reboot_needed=1
fi

if grep -Fq "LCD-INSTALLED" /boot/config.txt
then
    echo "LCD already configured"
else    
    echo "Updating vc4-kms to vc4-fkms"
    sed -i 's/vc4-kms/vc4-fkms/g' /boot/config.txt    
    echo "# Automation Marker: [LCD-INSTALLED]" >> /boot/config.txt

    echo "Updating cmdline.txt"
    sed -i 's/rootwait/rootwait fbcon=map:2/g' /boot/cmdline.txt

    reboot_needed=1
fi

if [ "$reboot_needed" = "1" ]; then
    echo "Pausing for a few seconds, then rebooting..."
    sleep 5
    reboot
    exit 0
fi
 
# Install usb-hid gadget
echo "Installing USB HID gadget..."
./usb-gadget/init-usb-gadget.sh
./usb-gadget/install-usb-gadget.sh

echo "----------------------------------------------"
echo "   COMPLETE! Your Pi is now a ghaven host!"
echo "----------------------------------------------"