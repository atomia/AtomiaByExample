# Example increment/decrement module

## Overview

In this example we are crating a new Example provisioning module (Atomia Automation Server module) that can be used to provision some arbitrary service. A new hosting control panel page (Atomia HCP plugin) is also created in order to create (and remove) this new service. The page will also be used to call service functions (increment and decrement) which will change the property of a service and which can be used to run a custom function to the provisioned service on some resource.

## Installation instructions

Example needs to be installed on default Atomia in order to be tested. Here are the installation steps:

1. build project "Atomia.Web.Plugin.Example" located in 
`ExampleWithModule/HostingControlPanel/Plugins/Atomia.Web.Plugin.Example` 
and copy binary to `C:\Program Files (x86)\Atomia\HostingControlPanel\bin` in your vagrant environment 
1. copy menu translation file `NewDefaultMenuRender.resx` from `ExampleWithModule/HostingControlPanel/Frame/App_Data` to `C:\Program Files (x86)\Atomia\HostingControlPanel\App_GlobalResources`
1. copy `ExampleWithModule/HostingControlPanel/Frame/Themes/NewDefault/Views/Example` to `C:\Program Files (x86)\Atomia\HostingControlPanel\Themes\NewDefault\Views` in your vagrant environment. 
1. copy transformation files from the folder `ExampleWithModule/HostingControlPanel/Frame/App_Data` to `C:\Program Files (x86)\Atomia\HostingControlPanel\App_Data\Transformation Files` in your vagrant environment 
1. execute `C:\Program Files (x86)\Atomia\HostingControlPanel\Recreate config files` shortcut 
1. build project "build project "Atomia.Web.Plugin.Example" located in `ExampleWithModule/AutomationServer/Common/Modules/Atomia.Provisioning.Modules.Example` 
and copy binary to `C:\Program Files (x86)\Atomia\AutomationServer\Common\Modules` in your vagrant environment. 
    
    **NOTE:** If `Atomia.Provisioning.Modules.Example.dll` already exists on your vagrant, you will not be able to override it until you first stop the following Atomia windows services: AtomiaAutomationServerCleanUpService, AtomiaAutomationServerPeriodicUpdater, AtomiaAutomationServerProvisioningEngine. And you need to stop Automation server website (automationserver.dev.atomia.com) in the IIS Manager. Don't forget to start them all again when you finish the copy.
1. copy transformation files from the folder `ExampleWithModule/AutomationServer/Common/ProvisioningDescriptions/Transformation FIles` to `C:\Program Files (x86)\Atomia\AutomationServer\Common\ProvisioningDescriptions\Transformation Files` in your vagrant environment
1. copy transformation files from the folder `ExampleWithModule/AutomationServer/Common/Transformation FIles` to `C:\Program Files (x86)\Atomia\AutomationServer\Common\Transformation Files` in your vagrant environment
1. execute `C:\Program Files (x86)\Atomia\AutomationServer\Recreate config files` shortcut 

Once all the above is done, a cusotmer needs tobe prepared in order to open the new HCP page. Check instructions in the section "Testing the customization" below.

## The customization process

The process of creating the customization will have a few different steps that we'll describe
separately:

1. Configuring the Automation Server Provisioning Description
1. Configuring the Automation Server Resource Description
1. Creating and testing the provisioning module
1. Creating HCP plugin

### Configuring the Automation Server Provisioning Description

We need to update Atomia provisioning description file with the description of our new Example service provisioning description:
```
<simpleService name="Example" friendlyName="Example of the simple service" providingModule="Atomia.Provisioning.Modules.Example.Example" xdt:Transform="Insert">
    <propertyList>
        <property name="AccountId" friendlyName="Account owner ID" />
        <property name="FirstName" friendlyName="Account owner first name" />
        <property name="LastName" friendlyName="Account owner last name" />
        <property name="Number" friendlyName="Number we are going to modify with Increment and Decrement operation" />
    </propertyList>
    <operationList>
        <operation name="Increment" />
        <operation name="Decrement" />
    </operationList>
</simpleService>
```

The provisioning description is located in `C:\Program Files (x86)\Atomia\AutomationServer\Common\ProvisioningDescriptions\ProvisioningDescription.xml` and instead of editiing the file directly we are using the [transformation file](http://learn.atomia.com/knowledge%20base/using-transformation-files/).

Our simple service is now added to the system, but without being permitted as a child of one of the existing services or packages it is not yet possible to provision it.

To make this possible we will add Example service as part service:
```
<partService name="Example" min="0" max="1" xdt:Transform="Insert">
    <propertyTransformation>
        <simpleTransformer>
            <serviceProperty name="AccountId">
                <expression>$CsBase::PackageNumber+#Account()</expression>
            </serviceProperty>
        </simpleTransformer>
    </propertyTransformation>
</partService>
```
to the `<partList>` section of the `<complexService>` provisioning description that has `name="CsBase"`. 

This `CsBase` service is part of package service `BasePackage` and `PremiumPackage` which are used for provisioning service of all "Gold" and "Platinum" package products respectively.

This means that Example service will be available in all accounts that have bought either "Gold" or "Platinum" package. Accounts with DNS package will not be able to add Example service.

### Configuring the Automation Server Resource Description

In Atomia every simple service needs to have a matching resource definition configured in the resource description file: `C:\Program Files (x86)\Atomia\AutomationServer\Common\Resources.xml`.

For our service Example, we don't really need any resource since we will not really provision anything on any resource, but we will create a simple resource for demonstration purposes.

Our simple resource description transforamation file:
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

The properties in the resource file usually hold some API URL and API username and password that you will need in order to provision the actual service on some resource (mail server, web server and so on).

### Creating and testing the provisioning module

Our Example provisioning module code is simple. All it does is extending an Atomia `Atomia.Provisioning.Modules.Common.ModuleCommandSimpleBase` and implements a couple of methods. Comments in the [code](AutomationServer/Common/Modules/Atomia.Provisioning.Modules.Example/Commands/ExampleCommand.cs) should be enough to understand how it works. The rest of the files and code is just a self-explanatory boilerplate code.

Once the code is ready and installed on Automation server and provisioning description and resource transformation files are in place (and we run the recreate shortcut), we can test the module. The simplest way is through the "Automation Server Client" which is a GUI client for Automation Server. This client is installed on the same server where Atomia Automation server is installed.

You can start it and select an account that has the `BasePackage` or `PremiumPackage` provisioned. Then you can right click on a `CsBase` service and select "Add Child Service" and then select our simple service "Example". You will then be able to specify the parameters and add the service. This way you can test your module (and actual provisioning on a resource) before you create a page in the control panel.

### Creating HCP plugin

The new HostingControlPanel page is added by creating a plugin (class library Visual Studio project) that extends `Atomia.Web.Plugin.HCP.Provisioning.Controllers.MainController`. The actual integration with the panel is done via [transformation files](HostingControlPanel/Frame/App_Data/).

There is a lot of boilerplate code here as well, but the most important is the [ExampleManager class](HostingControlPanel/Plugins/Atomia.Web.Plugin.Example/Atomia.Web.Plugin.Example/Helpers/ExampleHelper.cs) which demonstrates how to create/remove Example service and how to call it's operations.

Once everything is installed a new menu "Example" will be visible in the control panel. New link will open a new page which will allow all the actions. In case of a DNS package, the page will inform a customer that service is not available.
