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

# Create a new load balancer group for the application
CREATE_LOAD_BALANCER=`aws elbv2 create-target-group --name "$GROUP_NAME" --protocol HTTP --port 80 --matcher="HttpCode=200-305" --vpc-id "$VPC_ID" 2>&1 `
if [ $? -ne 0 ]; then
	(>&2 echo "Failed to create load balancer group $GROUP_NAME in VPC $VPC_ID")
	(>&2 echo "$CREATE_LOAD_BALANCER")
	exit 1
fi
# Parse relevant information from the command output
GROUP_ARN=`echo "$CREATE_LOAD_BALANCER" | jq [.TargetGroups][0][0].TargetGroupArn`
if [ -z "$GROUP_ARN" ];then
	(>&2 echo "Failed to parse the load balancer group arn from output")
	exit 1
fi

CUR_PRIORITY=`aws elbv2 describe-rules --listener-arn "$LISTENER_ARN" | grep Priority | grep -v default | awk '{print $2}' | sed s/\"//g | sed s/,// | sort | tail -n1 2>&1`
if ! echo "$CUR_PRIORITY" | egrep -q '^[0-9]+$'; then 
	PRIORITY=100
else
	PRIORITY=`expr "$CUR_PRIORITY" + 1`
fi

# Add a rule on the load balancer that passes traffic that matches the host header value of $DOMAIN_NAME to the $GROUP_ARN load balancer group
CREATE_LOAD_BALANCER_RULE=`aws elbv2 create-rule --listener-arn "$LISTENER_ARN" --priority "$PRIORITY" --conditions Field=host-header,Values="$DOMAIN_NAME" --actions Type=forward,TargetGroupArn="$GROUP_ARN" 2>&1`
if [ $? -ne 0 ]; then
	(>&2 echo "Failed to create load balancer rule on $LISTENER_ARN with value $DOMAIN_NAME and targe $GROUP_ARN")
	(>&2 echo "$CREATE_LOAD_BALANCER_RULE")

	exit 1
fi

# Create the actual service
CREATE_SERVICE=`aws ecs create-service --cluster "$CLUSTER" --service-name "$SERVICE_NAME" --task-definition "$TASK_NAME" --desired-count 1 --load-balancer "targetGroupArn=$GROUP_ARN,containerName=$TASK_NAME,containerPort=80" --role "$ECS_ROLE" 2>&1`
if [ $? -ne 0 ]; then
	(>&2 echo "Failed to create new service in cluster $CLUSTER with name $SERVICE_NAME for task $TASK_NAME with load balancer group $GROUP_ARN and role $ECS_ROLE")
	(>&2 echo "$CREATE_SERVICE")
	exit 1
fi

echo "Success"
exit 0
