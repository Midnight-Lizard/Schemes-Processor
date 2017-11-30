#!/bin/sh
set -e
TAG=1
PROJ=schemes-processor
REGISTRY=localhost:5000
IMAGE=$REGISTRY/$PROJ:$TAG
eval $(docker-machine env default --shell bash)
docker build -t $IMAGE \
    --build-arg DOTNET_CONFIG=Build \
    --build-arg INSTALL_CLRDBG="apt-get update \
        && apt-get install -y --no-install-recommends unzip \
        && rm -rf /var/lib/apt/lists/* \
        && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg" \
    ../app
kubectl config set-context minikube
docker push $IMAGE
./helm-deploy.sh -i $IMAGE -r $PROJ -c ../kube/$PROJ -s env.ASPNETCORE_ENVIRONMENT=Development
