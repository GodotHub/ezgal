import os
import re

# 修改文件内容
def replace_in_file(file_path, old, new):
    with open(file_path, 'r+', encoding='utf-8') as f:
        content = f.read()
        content = content.replace(old, new)
        f.seek(0)
        f.write(content)
        f.truncate()

# 修改文件名
def replace_file_name(file_path, old, new):
    pass

# 遍历文件
def process_dir(dir_path, old, new):
    for root, _, files in os.walk(dir_path):
        for file in files:
            if file.endswith('.cs'):
                replace_in_file(os.path.join(root, file), old, new)

replace_data = [
    # Dictionary ( Godot.Dictionary ) -> Technical.
    #("dictionary", "technical"),
    #("Dictionary", "Technical"),
    #("dicdata", "Techdata"),
    #("Dicdata", "Techdata"),
    #("dicData", "techData"),
    #("DicData", "TechData"),
    #("SetBracesFunc2", "SetOptionBracesFunc"),
    #("SetBracesFunc1", "SetBracesFunc"),
        ]

for i in replace_data:
    process_dir(os.getcwd(), i[0], i[1])
    process_dir("../../make/", i[0], i[1])
