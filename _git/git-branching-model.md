---
layout: post
title: Git branching model
description: Quy tắc sử dụng branch
tags:
    - git

---

# 1. Git branches
Thống nhất sử dụng các branches như sau, với tác dụng như mô tả:

### master
* Đây là branch chính của tất cả các repository → code ở trên branch này là code đã ở trạng thái Production-ready.
* Các developers không thể tự ý push code lên branch này, mà buộc phải thông qua Pull request từ cách branches khác.
* Branch sẽ được tự động build và test qua hệ thống Travis-CI → nếu build và test failed thì developers cần có trách nhiệm khắc phục càng sớm càng tốt.
* Code trên branch master sẽ được release lên các server sit, uat, production qua cách đánh Tag tương ứng.

### develop
* Đây là branch cho công việc phát triển code → code tại đây sẽ là xuất phát điểm cho tất cả các branch phát triển feature.
* Code trên branch develop cũng sẽ được tự động build và test.
* Các developers cũng nên hạn chế code trực tiếp trên branch develop nếu không cần thiết

### feature
* Feature branch là branch dành cho việc phát triển code cho cách tính năng / nhiệm vụ riêng biệt.
* Branch cần được base từ Develop và merge lại vào Develop (tất cả các pull request để merge thẳng từ feature vào master sẽ bị reject).
* Branch này sẽ không được tự động build và test, tuy nhiên developer khi pull request thì hệ thống sẽ build → nếu failed thì developer cần có nghĩa vụ phải fixed trước khi merge vào develop branch.
* Cách đặt tên branch feature sẽ mặc định là: feature/ten_tinh_nang , feature/ten_nhiem_vu

### release
* Branch release là branch có thời gian tồn tại ngắn, được tách ra từ nhánh develop và để chuẩn bị cho việc release (ví dụ: thay version number).
* Sau khi hoàn thành các thay đổi, branch sẽ được merge lại vào develop và master để tiến hành release (= cách đánh tag trên master). Branch này sau đó sẽ được remove ngay lập tức.
* Tên branch release nên được đặt theo tên version, ví dụ: release-v1.0.0-beta1

### hotfix
* Khi một version đã được release lên môi trường sit / uat / production. Để fix các tính năng hoặc lỗi nhưng không muốn tiến hành một đợt release như workflow ở trên thì ta có thể mở ra những nhánh hotfix (có thể branch bắt đầu từ master). Nhánh này sau khi apply trên master sẽ bắt buộc cần phải merge vào nhánh develop.

# 2. Versioning
Cách đánh version trong các sản phẩm của DigiMed sẽ theo cách sau:
* Alpha version: áp dụng khi đang phát triển nội bộ (trong sprint, sit, uat). Ví dụ: version 1.0.0-alpha15
* Beta version: áp dụng khi sản phẩm đã được đưa ra production nhưng vẫn được bên khách hàng dùng thử nghiệm
* Release version: áp dụng khi sản phẩm đã được đưa ra market (nhiều ng biết đến, hoặc là sản phẩm thương mại)
Version sẽ được đánh đồng nhất trên tất cả các platform (backend, frontend, ios, android). Version được release và version tên JIRA cần được map chính xác như nhau
Khi apply hotfixes cho các version, chúng ta sẽ không bump version mà là đánh thêm số, ví dụ đối với version 1.0.0-alpha15, khi apply hotfixes sẽ được tags dưới dạng 1.0.0-alpha15.1 , 1.0.0-alpha15.2
