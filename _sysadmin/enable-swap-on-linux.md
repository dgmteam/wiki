---
layout: post
title: Enable swap space on Ubuntu
tags:
    - sysadmin
    - ubuntu
    - swap
---

Thêm swap cho ubuntu server tránh việc một số tiến trình sử dụng nhiều RAM dẫn đến việc tự động kill

Kiểm tra xem swap đã được bật chưa

```sh
$ swapon --show

#Output
no thing shown, mean no swap
```

Kiểm tra dung lượng còn trống

```sh
$ sudo df -h

#Output
Filesystem      Size  Used Avail Use% Mounted on
/dev/xvda1      7.8G  3.3G  4.4G  43% /
devtmpfs        3.9G  124K  3.9G   1% /dev
tmpfs           3.9G     0  3.9G   0% /dev/shm
```

Allocate một vùng dung lượng vừa đủ, thường đặt bằng 1.5 lần dung lượng RAM. Lưu ý nếu đặt lớn hơn dung lượng đĩa còn trống thì toàn bộ phần trống sẽ bị chiếm hết.

```sh
$ sudo fallocate -l 2G /swapfile
```

Kiểm tra lại kết quả allocate

```sh
$ ls -lh /swapfile

# Output
-rw-r--r-- 1 root root 1.0G Apr 25 11:14 /swapfile
```

Set swapfile làm phân vùng swap

```sh
$ sudo chmod 600 /swapfile # chỉ cho phép
$ sudo mkswap /swapfile

#Output
Setting up swapspace version 1, size = 1024 MiB (1073737728 bytes)
no label, UUID=6e965805-2ab9-450f-aed6-577e74089dbf

$ sudo swapon /swapfile
```

Sửa file fstab để file swap tự động được sử dụng khi khởi động

```sh
$ echo '/swapfile none swap sw 0 0' | sudo tee -a /etc/fstab
```
