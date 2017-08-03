---
layout: post
title: Rules, Conventions of writing Unit tests and samples
tags:
    - tdd
    - unit test
---

Guideline này nhằm chỉ ra:

* Rules
* Cách tiếp cận để bắt đầu viết 1 UT
* Mẫu các UTs thường viết

## 1. Rules

Chỉ test Public method, đối với private method, protected methods chỉ cần test output (được sử dụng trong public methods).

Chỉ dùng 1 object thật là đối tượng đang test, ngoài ra cần phải tạo Mock object cho toàn bộ các Object được sử dụng khác. Nếu có lý do nào mà developer không thể sử dụng mock object có nghĩa là cách code hiện tại đang không phù hợp cho việc viết UTs (không sử dụng DI), cần phải xem lại.
Khi verify Object: Cần verify tất cả các fields đang có của Object
Khi verify Method đã được gọi: Cần specify số lần gọi và verify argument được gọi.

## 2. Cách tiếp cận khi viết UTs

Khi viết UTs, cần quan tâm đến những gì mà component / service mà mình đang viết cần làm những gì chứ không nên tập trung vào viết UTs để cover các dòng code đã viết.

Chính vì vậy, trước khi bắt tay vào viết UTs, cần phải gạch đầu dòng ra những chức năng chính của đối tượng cần viết. Đồng thời plan trước cần phải test những chức năng đó như thế nào.

Ví dụ: Khi viết UTs cho HeaderComponent, chỉ cần tập trung vào các tính năng:

* Hiển thị Logo
* Hiển thị Profile Image / Avatar
* Hiển thị Link to edit profile
* Hiển thị Link to Logout

## 3. Mẫu của các UTs thường viết cho Frontend

### 3.1 Unit tests cho Angular component:

https://github.com/dgmteam/wiki/tree/master/unit-test-samples/angular/component

### 3.2 Unit tests cho Angular service:

https://github.com/dgmteam/wiki/tree/master/unit-test-samples/angular/service

### 3.3 Unit tests cho Angular API service:

https://github.com/dgmteam/wiki/tree/master/unit-test-samples/angular/api

### 3.4 Unit tests cho Angular directive:

(chưa ai viết)

### 3.5 Unit tests cho Angular pipe:

(chưa ai viết)

## 4. Mẫu của các UTs thường viết cho Backend

### 4.1 Unit tests cho API controller:

https://github.com/dgmteam/wiki/tree/master/unit-test-samples/dotnet/api

### 4.2 Unit tests cho API service:
(chờ cập nhật)

## 5. Mẫu của các UTs sử dụng trên React Native

(chờ cập nhật)
