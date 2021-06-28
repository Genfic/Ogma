import glob
import os

files = glob.glob('./wwwroot/js/dist/**/*.min.js', recursive=True)
total = 0

for file in files:
    size = os.path.getsize(file)
    total += size
    print(f'{file} ({size} bytes)')

print(f'Total size: {total} bytes')
