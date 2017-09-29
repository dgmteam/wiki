---
title: Searching
description: No thing
layout: post
---


Search request

```
GET /_search?q

GET /<index1>,<index2>/<type1>,<type2>/_search?q
```

`_all` === search on all indicies

## Paging

```
GET /_search?size&from
```

You don't want to go too deep in paging. Nếu bạn có 5 shards và muốn lấy 5 kết quả từ 10000 đến 10005, để đảm bảo kết quả chính xác mỗi shard sẽ phải lấy 10005 kết quả rồi gom lại thành 50025 kết quả sau đó sort và phân trang.

