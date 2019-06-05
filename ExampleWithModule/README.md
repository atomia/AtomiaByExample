# Example increment/decrement module

## Overview

In this example customization we use the Example module that can be used to 
provision arbitrary simple services by executing a command trough HCP to show
how easy it is to create a new integration with Atomia, and combine it with
other built in provisioning modules to do more complex tasks.

We do this by provisioning Example module through Automation Server Client.

## The Example module

The Example module allows you to define an arbitrary simple service and then
calling the Increment, Decrement, GetFullName and GetFirstName operations via automation server core api.

## The customization process

The process of creating the customization will have a few different steps that we'll describe
separately:

1. Creating and testing the provisioning scripts
2. Configuring the Automation Server Provisioning Description
3. Configuring the Automation Server Resource Description

### Build and testing the HCP plugin

To use the Example module you need to build project Atomia.Web.Plugin.Example located in ExampleWithModule/HostingControlPanel/Plugins/Atomia.Web.Plugin.Example and copy binary to HostingControlPanel/bin in your vagrant environment. Next step is to copy Frame/Themes/NewDefault to HostingControlPanel/Themes in your vagrant environment. Then copy "Transformation Files" from the folder Frame to the corresponding places in HostingControlPanel in your vagrant environment and then use the "Recreate config files" shortcut.

#### Increment

Since our goal is to show communication between atomia components, increment operation will increment value on the automation server through the hosting control panel.

#### Decrement

The decrement operation will in a similar way as the increment operation, modify value on the automation server.

#### GetFullName

This operation will retun combination of values fetched from the properties of the simple service.

### Configuring the Automation Server Modules

Also, you need to build project Atomia.Provisioning.Modules.Example located in AutomationServer/Common/Modules/Atomia.Provisioning.Modules.Example and copy binary to AutomationServer/Common/Modules. To do this, you need to stop several services:
- AtomiaAutomationServerCleanUpService
- AtomiaAutomationServerPeriodicUpdater
- AtomiaAutomationServerProvisioningEngine

Then copy "Transformation Files" to the corresponding places in AutomationServer in your vagrant environment.
### Configuring the Automation Server Provisioning Description

The Automation Server Provisioning Description configuration for our example consists of two parts; a simple
service called Example that is handled amending one of the configured
hosting packages to allow for customers to provision the simple service.

#### The Example simple service

Since our scripts took three parameters we define the simple service mapping with properties relavant to our :
```
<simpleService name="Example" friendlyName="Example of the simple service"  providingModule="Atomia.Provisioning.Modules.Example.Example">
	<propertyList>
		<property name="AccountId" friendlyName="Account owner ID" />
		<property name="FirstName" friendlyName="Account owner first name" />
		<property name="LastName" friendlyName="Account owner last name" />
	</propertyList>
	<operationList>
		<operation name="GetFirstName" />
		<operation name="GetFullName" />
		<operation name="Increment" />
		<operation name="Decrement" />
	</operationList>
</simpleService>
```

#### Amending a hosting package

Our simple service is now up and running, but without beeing permitted as a child of one of the existing services or packages it is
not yet possible to provision them.

To make this possible we choose to add:
    <group maxCount="2">
        <service name="Example" />
    </group>
to the <globalLimitaitons>
to the <serviceLimitationList> of the <package> with name="BasePackage".

In a real world scenario the service could be sold as extra services within a package, sold as a package allowing a limited number of services to
be provisioned, sold usage based, etc.

### Configuring the Automation Server Resource Description

In Atomia every simple service needs to have a matching resource definition configured in the resource description configuration.

For our service Example, it's not relavante to use resource properties, but we should have it for provisioning service.

The resource description will look like:
```
  <bindings>
    <moduleList>
      <module name="Atomia.Provisioning.Modules.Example.Example" resourceAsignmentPolicy="RoundRobin" />
    </moduleList>
    <resourceList>
      <resource name="Example">
        <property name="Test">Atomia</property>
      </resource>
    </resourceList>
  </bindings>
```

Also, you can do this configuration resource description via Tranformation files which can be found in [transformations](transformations) folder.

To use it you download it and put it in the
    C:\Program Files (x86)\Atomia\AutomationServer\Common\Transformation Files

folder, and then use the "Recreate config files" shortcut.


### Creating a transformation file

The supported way of changing the provisioning description and other configuration in Atomia is through a transformation file.

You can find more information about transformation files at http://learn.atomia.com/knowledge%20base/using-transformation-files/

A transformation file for our provisioning description changes can be found in the [transformations](transformations) folder.

To use it you download it and put it in the
    C:\Program Files (x86)\Atomia\AutomationServer\Common\ProvisioningDescriptions\Transformation Files

folder, and then use the "Recreate config files" shortcut.

## Testing the customization

Once everything is in place and we want to test it, the simplest way is through the "Automation Server Client" which is a GUI
client for Automation Server.

You should start it and select an account that has the BasePackage provisioned.

Once open you should right click and select "Add Child Service" and then select our simple service "Example".

You will then be able to specify the parameters and add the service.

The end result (first name) in HCP (hcp.dev.atomia.com/accountNumber/Example) should be identical to when you added service in to Automation Server Client.
