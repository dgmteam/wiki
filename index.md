---
layout: default
---

## [Git](git)
{% for post in site.git %}
{% unless post.label == 'index' %}
<li>
    <a href="{{ post.url }}">{{ post.title }}</a>
</li>
{% endunless %}
{% endfor %}

## [ElasticSearch](elasticsearch)
{% for post in site.elasticsearch %}
{% unless post.label == 'index' %}
<li>
    <a href="{{ post.url }}">{{ post.title }}</a>
</li>
{% endunless %}
{% endfor %}
