import glob
import os

priority = {
    'woff2': 1,
    'woff': 2,
    'otf': 3,
    'ttf': 4
}

tags = []
for f in glob.glob('./Ogma3/wwwroot/fonts/*'):
    path = os.path.normpath(f)
    path_els = path.split('\\')
    ext = path_els[-1].split('.')[-1]
    tag = f"url('/fonts/{path_els[-1]}') format('{ext}'),"
    tags.append((tag, priority[ext]))

tags.sort(key=lambda x: x[1])
tags.sort(key=lambda x: x[0], reverse=True)
for t in tags:
    print(t[0])
