---
title: "[Recipe] Load certificate from file"
description: "Add certificate from file"
layout: post
tags: recipe IdentityServer4 claim
---

Tạo file certificate.

```shell
openssl req -newkey rsa:2048 -nodes -keyout id_server.key -out id_server.crt

openssl req -newkey rsa:2048 -nodes -keyout id_server.key -x509 -days 365 -out id_server.crt

openssl pkcs12 -inkey id_server.key -in id_server.crt -export -out id_server.pfx
```

Thêm `winpty` vào trước openssl trên Window. [Link](https://stackoverflow.com/questions/34156938/openssl-hangs-during-pkcs12-export-with-loading-screen-into-random-state?answertab=active#tab-top)

Extension load cert file

```cs
public static class IdentityServerExtension
{
    public static IIdentityServerBuilder AddCertificateFromFile(
        this IIdentityServerBuilder builder,
        IConfigurationRoot options,
        IHostingEnvironment env)
    {
        if (env.IsDevelopment())
        {
            builder.AddDeveloperSigningCredential();
            return builder;
        }

        var keyFilePath = options.GetValue<string>(ConfigurationKeys.CertFilePath);
        var keyFilePassword = options.GetValue<string>(ConfigurationKeys.CertPassword);

        if (File.Exists(keyFilePath))
        {
            builder.AddSigningCredential(new X509Certificate2(keyFilePath, keyFilePassword));
        }
        else
        {
            Console.WriteLine($"SigninCredentialExtension cannot find key file {keyFilePath}");
        }

        return builder;
    }
}
```

Thay đổi startup

```cs
services.AddIdentityServer(/*...*/)
    .AddCertificateFromFile(Configuration, _environment);
```

Thếm cấu hình tương ứng vào file appstings.
