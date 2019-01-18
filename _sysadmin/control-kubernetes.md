---
layout: post
title: Control Kubernetes cluster
tags:
    - sysadmin
    - ubuntu
    - deploymen
---

## Cài kubectl

https://kubernetes.io/docs/tasks/tools/install-kubectl/

## Cấu hình connect đến local cluster

```sh
kubectl config set-cluster digimed-local --server=http://192.168.1.46:8080
kubectl config set-credentials dgm-admin --username=admin

kubectl config set-context digimed-local --cluster=digimed-local --namespace=default --user=dgm-admin

kubectl config use-context digimed-local
```

## Xem các service đang chạy

```sh
kubectl get pods
```

## Xem pod đang chạy với image nào

```sh
$ kubectl describe pod <pod-name>

Name:               report-api-d754f5474-kncd5
Namespace:          default
Priority:           0
PriorityClassName:  <none>
Node:               dgmserver1/192.168.1.46
Start Time:         Fri, 18 Jan 2019 13:38:59 +0700
Labels:             app=eln
                    date=1547720615
                    eln.service=report.api
                    pod-template-hash=d754f5474
Annotations:        <none>
Status:             Running
IP:                 10.1.1.175
Controlled By:      ReplicaSet/report-api-d754f5474
Containers:
  report-api:
    Container ID:   docker://d64c180ef6ea6b8ff6c78c15c4e3c19e17fd7b9b19e9e83cc495867ef50ce93f
    Image:          docker.debugger.vn/eln/reportapi:8f31db363baf7a2218c3fa7e955796cfaa1d2093
    Image ID:       docker-pullable://docker.debugger.vn/eln/reportapi@sha256:6333e9a5e72ec8f6f699306c58725b46b490ac242684adbc039141de46cc7ee6 <-- image đây, đoạn sau là commit được build ra
    Port:           <none>
    Host Port:      <none>
    State:          Running
      Started:      Fri, 18 Jan 2019 13:39:03 +0700
    Ready:          True
    Restart Count:  0
    Environment:
      Authentication__Authority:     <set to the key 'identity-authority' of config map 'dev-server-config'>  Optional: false
      ClientCredentials__Authority:  <set to the key 'identity-authority' of config map 'dev-server-config'>  Optional: false
    Mounts:
      /aws from aws (ro)
      /var/run/secrets/kubernetes.io/serviceaccount from default-token-cm89m (ro)

```

## Xem logs

```sh
kubectl logs -f <pod-name>
```

Tham số 

* -f: follow logs
* --tail=n: in ra n dòng cuối cùng
* -t: in ra timestamp
