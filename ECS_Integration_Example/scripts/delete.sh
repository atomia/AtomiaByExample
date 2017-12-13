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

# Delete load balancer rule
RULE_ARN=`aws elbv2 describe-rules --listener-arn "$LISTENER_ARN" | jq '.Rules[] | select(.Conditions[].Values[0] =="'''$DOMAIN_NAME'''").RuleArn' | head -n1 | sed s/\"//g`
DELETE_RULE=`aws elbv2 delete-rule --rule-arn "$RULE_ARN"`
if [ $? -ne 0 ]; then
	(>&2 echo "Failed to delete load balancer rule $RULE_ARN")
	(>&2 echo "$DELETE_RULE")
	exit 1
fi

# Delete load balancer group
GROUP_ARN=`aws ecs describe-services --services "$SERVICE_NAME" --cluster "$CLUSTER" | jq .services[0].loadBalancers[0].targetGroupArn | sed s/\"//g`
DELETE_GROUP=`aws elbv2 delete-target-group --target-group-arn "$GROUP_ARN" 2>&1`
if [ $? -ne 0 ]; then
	(>&2 echo "Failed to delete load balancer group $GROUP_ARN")
	(>&2 echo "$DELETE_GROUP")
	exit 1
fi

echo "Success"
exit 0
