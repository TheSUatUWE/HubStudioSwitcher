Hub Radio Studio Switcher
==============

Requirements
------------
 * Allen & Heath IDR4/8 matrix mixer, with Telnet enabled
 * Windows 7 and above
 * .NET 4 and above

What does it do
-----
This application is 

How does it work?
-----
Using the telnet interface, we can recall presets on the IDR mixer and check what preset is loaded. With this information, we can build a studio switching application, which is much nicer than the PL-Designer interface.

How to use
----------
 1. Extract the application 
 2. Edit the config file
  - idrServer/Port/idrPassword should reflect your IDR settings
  - windowX/Y/W/H is where you want the switcher interface to be displayed on screen. Note it will be above all windows.
  - optionXName/XCmd/XPreset - Configure up to 9 switchable presets. Name is what is displayed on screen, Cmd is the telnet command sent (only SET PRESET will work at the moment), and preset value is whatever the preset number is.
 3. Run the program! In a studio environment, you are best off putting this as a startup option.
 
Help!
-----
If you're stuck getting this working or notice something drastically wrong, you're welcome to email me. matt@mattyribbo.co.uk
