---
title: Home
layout: home
nav_order: 100
---
This is the online handbook for Dylan Beattie's  workshop "Modern Web Development with C# and .NET 8"

<ul id="index-nav">
{% assign contents = site.pages | where_exp:"item", "item.summary != nil" %}
{% for page in contents %}
    <li>
        <a href="{{ page.url | relative_url }}">{{ page.title }}</a>
        <p>{{ page.summary }}</p>
</li>
{% endfor %}
</ul>
