#!/bin/sh

# Configuration variables
VPC_ID="vpc-someid"
LISTENER_ARN="arn:aws:elasticloadbalancing:eu-west-1:arn-to-your-loadbalancer-listener"
CLUSTER="your-cluster-name"
ECS_ROLE="ecsInstanceRole"
ACCESS_KEY=`cat ~/.aws/credentials | grep aws_access_key_id | awk '{ print $NF }'`
SECRET_KEY=`cat ~/.aws/credentials | grep aws_secret_access_key | awk '{ print $NF }'`
REGION="eu-west-1"
