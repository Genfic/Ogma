import glob
import os
import gzip

files = glob.glob('./wwwroot/js/dist/**/*.min.js', recursive=True)

total_size = 0

longest = len(max(files, key=len))

# Print the sizes of every file
for f in [fn for fn in files if '\\admin\\' not in fn]:
    size = os.path.getsize(f)
    total_size = total_size + size

    print(f'{f:<{longest}} {size:>6} bytes')

# Print the total size of all files
print('─'.encode('utf8').decode('cp1252') * (longest + 6 + 7))
print(f'Total: {total_size:>{longest}.2f} bytes')
print(f'{(total_size / 1024):>{longest + 7}.2f} KB')
print(f'{((total_size / 1024) / 1024):>{longest + 7}.2f} MB')

# Calculate and print gzipped size
content = ''
for f in [fn for fn in files if '\\admin\\' not in fn]:
    with open(f, mode='r', encoding='utf8') as file:
        content = content + file.read()
        file.close()

gz_size = len(gzip.compress(content.encode('utf8')))

print('─'.encode('utf8').decode('cp1252') * (longest + 6 + 7))
print(f'Gzipped: {gz_size:>{longest-2}.2f} bytes')
print(f'{(gz_size / 1024):>{longest + 7}.2f} KB')
print(f'{((gz_size / 1024) / 1024):>{longest + 7}.2f} MB')
print(f'{(gz_size / total_size) * 100:>{longest + 7}.2f} %')
