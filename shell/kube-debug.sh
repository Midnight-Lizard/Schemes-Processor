#!/bin/bash
set -e
# echo "serching for pod"
pod=`kubectl get pods --selector=app=schemes-processor -o jsonpath='{.items[0].metadata.name}'`;
# echo "starting debugger on $pod";
kubectl exec $pod -i -- ..$1 $2;