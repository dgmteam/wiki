---
layout: default
---

## [Git](git)
{% for post in site.git %}
{% unless post.label == 'index' %}
<li>
    <a href="{{ post.url | remove_first:'/' }}">{{ post.title }}</a>
</li>
{% endunless %}
{% endfor %}

## [Test driven development](tdd)
{% for post in site.tdd %}
{% unless post.label == 'index' %}
<li>
    <a href="{{ post.url | remove_first:'/' }}">{{ post.title }}</a>
</li>
{% endunless %}
{% endfor %}

## [ElasticSearch](elasticsearch)
{% for post in site.elasticsearch %}
{% unless post.label == 'index' %}
<li>
    <a href="{{ post.url | remove_first:'/' }}">{{ post.title }}</a>
</li>
{% endunless %}
{% endfor %}

## [ASP .NET Core](aspnet_core)
{% for post in site.aspnet_core %}
{% unless post.label == 'index' %}
<li>
    <a href="{{ post.url | remove_first:'/' }}">{{ post.title }}</a>
</li>
{% endunless %}
{% endfor %}

## [Server management tips and tricks](sysadmin)
{% for post in site.sysadmin %}
{% unless post.label == 'index' %}
<li>
    <a href="{{ post.url | remove_first:'/' }}">{{ post.title }}</a>
</li>
{% endunless %}
{% endfor %}
