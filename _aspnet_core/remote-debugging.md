# Remote debugging .NET Core process sử dụng VSCode

> Kịch bản là bạn có một ứng dụng .NET core chạy ở trên một server nào đó cần debug. Bạn có trong tay source code (tất nhiên) và khả năng access vào server. Lưu ý server ở đây bao gồm cả docker.

**Step 1**: Chuẩn bị: target machine: cài đặt vsdbg.

Với linux server:

```sh
curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l ~/vsdbg
```

Với docker, có thể mount vsdbg đã cài sẵn trên máy, lưu ý cần đảm bảo cùng nền tảng để vsdbg của bạn có thể chạy trong docker container

```yaml
# file docker-compose.override.yml
target:
  volumes:
    - <path-to-local-vsdbg-dir>:/vsdbg
```

**Step 2**: cài đặt pipe program

Với linux server: cài ssh

Với docker, bạn cần có docker client, tất nhiên là đã có.

**Step 3**: Tạo launch task cho vscode:

Mở file .vscode/launch.json, và thêm cấu hình:

```json
    {
      "name": ".NET Core Remote Attach",
      "type": "coreclr",
      "request": "attach",
      "pipeTransport": {
        "pipeCwd": "${workspaceFolder}",
        "pipeProgram": "ssh", // <1>
        "pipeArgs": [
          "account@target.machine" // <2>
          // "chỗ này có thể thêm các tham số của ssh"
        ],
        "quoteArgs": false,
        "debuggerPath": "/vsdbg/vsdbg" // <3>
      },
      "processId": "${command:pickRemoteProcess}", // <4>
      "justMyCode": false, // <5>
      "sourceFileMap": {
        "/": "${workspaceFolder}" // <6>
      }
    }
```

Các vị trí thú vị theo thứ tự xuất hiện:

* `<1>`, `<2>`: Chỗ này cấu hình pipe provider, các command remote sẽ được execute thông qua pipe program này, ví dụ ssh -e "some-command". Đối với docker container, pipe program và pipeArgs lần lượt là `docker` và `exec -i <tên container>`

*<3>: Chỗ này là path đến vsdbg bạn cài ở bước 1

*<4>: command này sẽ mở ra popup chọn process. Command này gọi đến lệnh ps nên nếu trong docker không có ps thì bạn phải cài thêm. Nếu debug entrypoint thì process id thường là 1, khỏi phải chọn.
​
*​<5>: Nếu app được build với cấu hình Release thì sẽ assembly sẽ được đánh dấu là optimized, debugger mặc định coi đó là thư viện ngoài và ignore đi. Bạn cần tắt option này mới debug được. Ngoài ra nhiều biến, dòng sẽ được bỏ đi trong quá trình optimize, ví dụ các biến local chỉ dùng 1 lần. Khi đó source code và assembly không match 1-1 với nhau, do đó nhiều chỗ set break point không có tác dụng, cần lưu ý đăt vị trí set break point hợp lý trước khi chửi người viết bài.

*<6>: Nếu app được build ở một máy khác thì source map trong assembly không giống với workspaceFolder hiện tại, cần map cho đúng thì breakpoint mới linh nghiệm. Chẳng hạn tại stage build của dockerfile bạn copy source vào folder /src thì chỗ này phải map "/src": "${workspaceFolder}"

Ví dụ 1 task hoàn chỉnh cho docker

```json
{
    "name": ".NET Core Attach",
    "type": "coreclr",
    "request": "attach",
    "processId": "1",
    "justMyCode": false,
    "pipeTransport": {
        "pipeCwd": "${workspaceFolder}",
        "pipeProgram": "docker",
        "pipeArgs": [
            "exec -i compose1_emailing-api_1"
        ],
        "quoteArgs": false,
        "debuggerPath": "/vsdbg/vsdbg"
    },
    "sourceFileMap": {
        "/": "${workspaceFolder}"
    }
}
```

docker-compose.override.yml nếu cần cài thêm utility ps để pickRemoteProcess

```yaml
emailing-api:
  # ...
  volumes:
    - ./.vscode/vsdbg:/vsdbg

  entrypoint: /bin/bash -c "/bin/bash -c \"$${@}\""
  command: |
    /bin/bash -c "
      apt update
      apt install procps -y
      dotnet Emailing.Api.dll
    "
```

Happy debugging 🤘

