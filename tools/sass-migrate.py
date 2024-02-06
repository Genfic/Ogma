import os
import re

base = '../Ogma3/wwwroot/css/src/'

""" Adds semicolons where needed
in:  `background-color: red`
out: `background-color: red;`
"""
semicolon_regex = re.compile(r'([a-z-]+): (.+)\n')

""" Migrates mixin references from short syntax
in:  `+foo.bar`
out: `@include foo.bar;`
"""
mixin_regex = re.compile(r'\+\s?([a-z]+\.[a-z\-]+)\n')

""" Adds semicolons specifically to imports
in:  `@use '../foo' as bar`
out: `@use '../foo' as bar;`
"""
imports_regex = re.compile(r'(@use ["\'].+["\'] as [a-z\-]+)\n')

folder = input('Enter source folder: ').strip()

print('Leave file name empty to exit')
while True:
    file = input('Enter file: ').strip()

    if len(file) <= 0:
        break

    if not file.endswith('.sass'):
        file = f'{file}.sass'

    path = os.path.join(base, folder, file)

    print(f'Migrating {path}')

    with open(path, 'r+', encoding='utf8') as f:
        old = f.read()
        new = old + '\n'
        new = semicolon_regex.sub(r'\1: \2;\n', new)
        new = mixin_regex.sub(r'@include \1;\n', new)
        new = imports_regex.sub(r'\1;\n', new)
        f.seek(0)
        f.write(new)
        print('Done!')

print('Exiting')