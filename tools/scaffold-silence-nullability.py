import glob

files = glob.glob('Ogma3/Areas/Identity/**/*.cshtml.cs', recursive=True)

for file in files:
	print(file)
	with open(file, 'r+', encoding='utf8') as f:
		old = f.read()
		f.seek(0)
		f.write(f'#nullable disable\n\n{old}')
	print('Nullability disabled')
