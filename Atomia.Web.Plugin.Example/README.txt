This is an example plugin for the Atomia HostingControlPanel.

Installation
============

* Compile the Atomia.Web.Plugin.Example project
* Copy binary to HostingControlPanel/bin in your vagrant environment
* Copy the Themes/Example folder to HostingControlPanel/Themes in your vagrant environment
* Copy the App_GlobalResources/Example folder to HostingControlPanel/App_GlobalResources in your vagrant environment
* Copy "Transformation Files" from the project to the corresponding places in HostingControlPanel in your vagrant environment
* Copy Theme/NewDefault/Content from NewDefault theme to Theme/Example to get a stylesheet
* Run "Recreate config files" in HostingControlPanel in your vagrant environment.
* Open hcp.dev.atomia.com in your browser and the new plugin should be there.


Note about managers and Automation Server connection
====================================================

The plugin is compiled with the ExampleDevManager instead of the ExampleManager.

The ExampleDevManager lets you compile and add the plugin to your vagrant environment to see how the Views and Scripts work.

The ExampleManager is a more realistic example of how communication with Automation Server works, but it requires you to have the correct entries in the provisioning description, which is not provided with this plugin example for now.
