---
layout: post
title: Linux server deployment
tags:
    - sysadmin
    - ubuntu
    - deploymen
---

## Trong bảng điều khiển của domain, tạo 1 record A để trỏ về ip của server S1

## Truy cập server sử dụng SSH, tạm gọi là server S1

Dùng putty hoặc git bash, cygwin...

## Cấu hình service để tự động chạy app lúc khởi động

### Ubuntu 14.04: Sử dụng upstart

#### Cấu hình
Thêm cấu hình bằng cách thêm file *.conf vào thư mục /etc/init. Ví dụ 

```
description "iagency api"
author "mashix"
start on runlevel [2345]
stop on runlevel [06]
respawn
script
    su -c "export ASPNETCORE_ENVIRONMENT=Production && cd /var/www/iagency/backend && /usr/bin/dotnet Ia.Web.dll" ubuntu
end script
```

Link tham khảo https://www.digitalocean.com/community/tutorials/the-upstart-event-system-what-it-is-and-how-to-use-it

#### Xem log

```
sudo tail -f /var/log/upstart/<service_name>.log
```

### Ubunut 16.04: Sử dụng systemd 

#### Cấu hình
File cấu hình có đuôi .service nằm trong thư mục /etc/systemd/system

```
[Service]
WorkingDirectory=/media/storage/sites/eln-dev/backend
ExecStart=/usr/bin/dotnet Eln.Application.Api.dll
Restart=always
RestartSec=10 
SyslogIdentifier=eln-app-dev
User=jenkins
Environment=ASPNETCORE_ENVIRONMENT=DevServer
Environment=ASPNETCORE_URLS=http://localhost:9002
Environment='ConnectionStrings__DefaultConnection=User ID=digimed;Password=DigiMed123;Host=localhost;Port=5432;Database=eln_app_dev2'

[Install]
WantedBy=multi-user.target
```

Sau khi tạo file service cần enable bằng lệnh

```
sudo systemctl enable <file_name>
```

#### Xem log

```
sudo journalctl -f -u <unit_name>
```

## Cấu hình nginx để map các request đến domain `abc.com` vào cổng mà app đang listen

Các file cấu hình nginx sẽ nằm trong thư mục /etc/nginx/sites-available/

```
# file name site-abc
server {
    server_name abc.com;

    location / {
        proxy_pass http://localhost:9999;
        proxy_set_header Host $host;    # Copy header của request hoặc thêm các header mới nếu cần
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "";
        proxy_http_version 1.1;
        proxy_set_header X-Forwarded-Proto https;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $remote_addr;
        proxy_set_header X-Forwarded-Host $remote_addr;
    }
}

server {
    server_name admin.abc.com;
    root /angular/...;
    index index.html default.html index.htm;

    try_files $uri $uri/ /index.html =404;
}
```

Sau khi tạo file này cần enable nó lên bằng cách tạo liên kết sang thư mục /etc/nginx/sites-enable/

```sh
sudo ln -s /etc/nginx/sites-available/site-abc /etc/nginx/sites-enabled/site-abc
```

## Tạo certificate để có https nếu cần, sử dụng certbot

```
sudo certbot --nginx
```

## Cấu hình jenkins để build app, đẩy lên server S1

