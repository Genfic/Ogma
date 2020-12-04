import glob
import os

for f in glob.glob('./Ogma3/wwwroot/fonts/*'):
    path = os.path.normpath(f)
    path_els = path.split('\\')
    tag = f'<link rel="preload" as="font" href="~/fonts/{path_els[-1]}">'
    print(tag)
