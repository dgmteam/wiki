---
layout: default
title: Server management tips and tricks
label: index
---

## [Server management tips and tricks](sysadmin)
{% for post in site.sysadmin %}
{% unless post.label == 'index' %}
<li>
    <a href="{{ post.url }}">{{ post.title }}</a>
</li>
{% endunless %}
{% endfor %}
