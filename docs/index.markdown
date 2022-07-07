---
# Feel free to add content and custom Front Matter to this file.
# To modify the layout, see https://jekyllrb.com/docs/themes/#overriding-theme-defaults

layout: home
---

{% for entry in site.UserManual %}
  <h2>{{ entry.title }} - {{ entry.position }}</h2>
  <p>{{ entry.content | markdownify }}</p>
{% endfor %}