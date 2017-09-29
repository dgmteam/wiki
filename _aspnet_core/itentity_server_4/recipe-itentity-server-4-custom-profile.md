---
title: "[Recipe] IdentityServer4 add custom profile"
description: "Add custom claims to generated token"
layout: post
tags: recipe IdentityServer4 claim
---

Tại Startup, khi thêm service IdentityServer, khai báo custom profile service

```c#
services.AddIdentityServer(o => o.IssuerUri = Configuration.GetValue<string>(ConfigurationKeys.WebHostUrl))
    .AddAspNetIdentity<Account>()
    .AddProfileService<ProfileService>();
```

`ProfileService` là một lớp sử dụng interface `IdentityServer4.Services.IProfileService`, có 2 phương thức chính:

- `Task GetProfileDataAsync(ProfileDataRequestContext context);`
- `Task IsActiveAsync(IsActiveContext context);`

Viêc quy định nội dung token được thực hiện trong `GetProfileDataAsync`

```c#
public class ProfileService : IProfileService
{
    protected UserManager<Account> _userManager;
    private readonly IUserClaimsPrincipalFactory<Account> _claimsFactory;

    public ProfileService(UserManager<Account> userManager, IUserClaimsPrincipalFactory<Account> claimsFactory)
    {
        _userManager = userManager;
        _claimsFactory = claimsFactory;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var sub = context.Subject.GetSubjectId();
        var user = await _userManager.FindByIdAsync(sub);
        var principal = await _claimsFactory.CreateAsync(user);
        var claims = principal.Claims.ToList();

        // lấy ra các claims được request
        claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();

        // thêm claim tùy thích
        claims.AddRange(new List<Claim>
        {
            new Claim(JwtClaimTypes.FamilyName, user.LastName),
            new Claim(JwtClaimTypes.GivenName, user.FirstName),
            new Claim("fullName", user.FirstName + " " + user.LastName),
        });

        context.IssuedClaims = claims;
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        //>Processing
        var user = _userManager.GetUserAsync(context.Subject).Result;

        context.IsActive = (user != null) && user.IsActive;

        //>Return
        return Task.FromResult(0);
    }
}
```
