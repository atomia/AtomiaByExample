# ECS docker container provisioning 

## Overview

In this example customization we use the SSHCommand module that can be used to 
provision arbitrary simple services by executing a command trough SSH to show
how easy it is to create a new integration with Atomia, and combine it with
other built in provisioning modules to do more complex tasks.

We do this by provisioning wordpress-containers on the Amazon ECS docker container
service, load balancing them with Amazon ELB and combining them with DNS on a local
Atomia DNS instance.

## The SSHCommand module

The SSHCommand module allows you to define an arbitrary simple service and then
mapping the add, modify and delete actions to different commands on a remote host.

More information about how it works can be found at
http://learn.atomia.com/knowledge%20base/generic-ssh-provisioning-module/

## The customization process

The process of creating the customization will be a few different steps that we'll describe
separately:

1. Creating and testing the provisioning scripts
2. Configuring the Automation Server Provisioning Description
3. Configuring the Automation Server Resource Description

### Creating and testing the provisioning scripts

To use the SSHCommand module you need to create a set of executables that do the actual
provisioning logic for the service you are integrating. The executables should take attributes
of the service as parameter and return 0 on success or any other code on errors. On errors
you get the output of STDOUT and STDERR in the Automation Server logs and exceptions.

#### Add

Since our goal is to add an Amazon ECS container, we start by installing the aws CLI and making
sure that you can use it to manage your pre-setup ECS cluster.

When logged on to the server as the user you intend to run the integration as, you should be able to:

    aws ecs list-clusters

and see your ECS cluster.

We are then ready to create the script. We create a script called add.sh looking something like:
```
#!/bin/sh

. "$(dirname $0)/config.sh.inc"

if [ -z "$1" ] || [ -z "$2" ] || [ -z "$3" ]; then
        echo "usage: $0 service_name domain task\n"
        echo "e.g:"
        echo "$0 atomia-container-test atomia-container-test.com wordpress"
        exit 1
fi

SERVICE_NAME="$1"
DOMAIN_NAME="$2"
TASK_NAME="$3"

GROUP_NAME="${SERVICE_NAME}-group"

export AWS_ACCESS_KEY_ID="$ACCESS_KEY"
export AWS_SECRET_ACCESS_KEY="$SECRET_KEY"
export AWS_DEFAULT_REGION="$REGION"

# ... commands for load balancing etc omitted for clarity

# Create the actual service
CREATE_SERVICE=`aws ecs create-service --cluster "$CLUSTER" --service-name "$SERVICE_NAME" --task-definition "$TASK_NAME" --desired-count 1 --load-balancer "targetGroupArn=$GROUP_ARN,containerName=$TASK_NAME,containerPort=80" --role "$ECS_ROLE" 2>&1`
if [ $? -ne 0 ]; then
        (>&2 echo "Failed to create new service in cluster $CLUSTER with name $SERVICE_NAME for task $TASK_NAME with load balancer group $GROUP_ARN and role $ECS_ROLE")
        (>&2 echo "$CREATE_SERVICE")
        exit 1
fi

echo "Success"
exit 0
```

The full scripts in this example are available in the [scripts](scripts) folder.

#### Delete

The delete script will in a similar way as the add script, taking a set of parameters defining the container in question and then just
doing the actual work through the AWS CLI. A shortened version of the script will look like:

```
#!/bin/sh

. "$(dirname $0)/config.sh.inc"

if [ -z "$1" ] || [ -z "$2" ]; then
        echo "usage: $0 service_name domain\n"
        echo "e.g:"
        echo "$0 atomia-container-test atomia-container-test.com wordpress"
        exit 1
fi

SERVICE_NAME="$1"
DOMAIN_NAME="$2"

GROUP_NAME="${SERVICE_NAME}-group"

export AWS_ACCESS_KEY_ID="$ACCESS_KEY"
export AWS_SECRET_ACCESS_KEY="$SECRET_KEY"
export AWS_DEFAULT_REGION="$REGION"

# Delete service
UPDATE_SERVICE_COUNT=`aws ecs update-service --service "$SERVICE_NAME" --cluster "$CLUSTER" --desired-count 0 2>&1`
if [ $? -ne 0 ]; then
        (>&2 echo "Failed to set service count to 0 of $SERVICE_NAME")
        (>&2 echo "$UPDATE_SERVICE_COUNT")
        exit 1
fi

DELETE_SERVICE=`aws ecs delete-service --service "$SERVICE_NAME" --cluster "$CLUSTER" 2>&1`
if [ $? -ne 0 ]; then
        (>&2 echo "Failed to delete service $SERVICE_NAME")
        (>&2 echo "$DELETE_SERVICE")
        exit 1
fi

# ... more cleanup omitted
``` 

#### Configuring the scripts

Since both the add and delete scripts needs to know some information about your ECS cluster we opted to store this
in a shared file called config.sh.inc.

The next step is to edit this file and set all the parameters to match your ECS cluster.


#### Testing the scripts

Since we just use regular scripts and parameters to pass the required data to them, testing is simple.

First try to add a container by executing a command like:
    ./add.sh sometest wptest.somedomain.com wordpress

You should then see a new service appearing in your ECS cluster for the task definition named 'wordpress'.

There should also be a new target group called sometest-group with a rule to use this target group for
requests where the Host header matches wptest.somedomain.com.

Once we verify that everything got added correctly in ECS we can test the delete script like:
    ./delete.sh sometest wptest.somedomain.com

And then verify that everything got removed OK from the cluster.

### Configuring the Automation Server Provisioning Description

The Automation Server Provisioning Description configuration for our example consists of three parts; a simple
service called AmazonECSContainerService that is handled by our scripts, a complex service called
CsAmazonECSContainerService that bundles our containers with locally hosted DNS and amending one of the configured
hosting packages to allow for customers to provision the complex service.

#### The AmazonECSContainerService simple service

Since our scripts took three parameters we define the simple service mapping with properties matching the script parameters:
```
<simpleService 
	name="AmazonECSContainerService" friendlyName="Simple service for creating and managing a container on ECS" 
	providingModule="Atomia.Provisioning.Modules.SSHCommand.SSHCommand">
	<propertyList>
		<property name="ContainerName" friendlyName="The service name to create in ECS" required="true" />
		<property name="Hostname" friendlyName="The hostname to connect to the container through an ELB rule" required="true" />
		<property name="Type" friendlyName="The container type" required="true" />
	</propertyList>
</simpleService>
```

#### The CsAmazonECSContainerService complex service

For the complex service we wan to achieve three things:
1. We want to incorporate the customer account name in the ContainerName to make them unique and also easier to trace to a specific customer
2. To create a DNS zone and a DNS record at the same time as the container

We thus define the complex service as:
```
<complexService name="CsAmazonECSContainerService"
	description="Complex service for an ECS container and DNS through Atomia DNS">
	<complexServicePropertyList>
		<complexServiceProperty name="ContainerName" description="Container name" />
		<complexServiceProperty name="Hostname" description="The hostname to connect to the container through an ELB rule" />
		<complexServiceProperty name="Type" friendlyName="The container type" defaultValue="wordpress" />
	</complexServicePropertyList>
	<partList>
		<partService name="AmazonECSContainerService" min="1" max="1">
			<propertyTransformation>
				<simpleTransformer>
					<serviceProperty name="ContainerName">
						<expression>#Account()+-+$CsAmazonECSContainerService::ContainerName</expression>
					</serviceProperty>
					<serviceProperty name="Hostname">
						<expression>$CsAmazonECSContainerService::Hostname</expression>
					</serviceProperty>
					<serviceProperty name="Type">
						<expression>$CsAmazonECSContainerService::Type</expression>
					</serviceProperty>
				</simpleTransformer>
			</propertyTransformation>
		</partService>
		<partService name="DnsZone" />
	</partList>
	<init>
		<add partName="AmazonECSContainerService" instanceName="AmazonECSContainerServiceInstance" />
		<add partName="DnsZone">
			<initPropertyList>
			  <propertyTransformation>
				<simpleTransformer>
				  <serviceProperty name="Zonename">
					<expression>$CsAmazonECSContainerService::Hostname</expression>
				  </serviceProperty>
				</simpleTransformer>
			  </propertyTransformation>
			</initPropertyList>
			<add partName="DnsZoneRecord">
			  <initPropertyList>
				<initProperty name="Id" value="" />
				<initProperty name="DnsType" value="CNAME" />
				<initProperty name="Label" value="www" />
				<propertyTransformation>
				  <simpleTransformer>
					<serviceProperty name="Data">
					  <expression>#Instance(AmazonECSContainerServiceInstance).Resource.LoadbalancerCNAME</expression>
					</serviceProperty>
				  </simpleTransformer>
				</propertyTransformation>
			  </initPropertyList>
			</add>
		</add>
	</init>
</complexService>
```

#### Amending a hosting package

Our complex and simple service is now up and running, but without beeing permitted as a child of one of the existing services or packages it is
not yet possible to provision them.

To make this possible we choose to add:
    <service name="CsAmazonECSContainerService" />

to the <serviceList> of the <package> with name="BasePackage".

In a real world scenario the containers could be sold as extra services within a package, sold as a package allowing a limited number of containers to
be provisioned, sold usage based, etc.

### Configuring the Automation Server Resource Description

In Atomia every simple service needs to have a matching resource definition configured in the resource description configuration.

For our service AmazonECSContainerService, since we are using the SSHCommand module, this is where we specify which host we will provision
the containers from, the private key to connect with, the name of the scripts, etc.

The resource description will look like:
```
  <bindings>
    <moduleList>
      <module name="Atomia.Provisioning.Modules.SSHCommand.SSHCommand" resourceAsignmentPolicy="MatchingServiceNameLimitPerResource" />
    </moduleList>
    <resourceList>
      <resource name="ECSResource">
        <property name="ServiceName">AmazonECSContainerService</property>
        <property name="Hostname">yourserver.example.com</property>
        <property name="Username">youruser</property>
        <property name="PrivateKey"><![CDATA[-----BEGIN RSA PRIVATE KEY-----
...snip...
-----END RSA PRIVATE KEY-----]]></property>
        <property name="LoadbalancerCNAME">ecsloadbalancer-somehostname.eu-west-1.elb.amazonaws.com.</property>
        <property name="AmazonECSContainerService_Add">ecsdemo/add.sh {{containername}} {{hostname}} {{type}}</property>
        <property name="AmazonECSContainerService_Remove">ecsdemo/delete.sh {{containername}} {{hostname}}</property>
      </resource>
    </resourceList>
  </bindings>
```

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

Once open you should right click and select "Add Child Service" and then select our complex service "CsAmazonECSContainerService".

You will then be able to specify the parameters and add the service.

The end result should be identical to when you tested with the provisioning script manually, with the exception that you now also
got a DNS zone and a CNAME record provisioned at the same time.

After verifying that everything looks good in ECS you can then delete your container by right clicking the new service and selecting
"Delete service".
