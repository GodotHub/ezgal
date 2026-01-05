module tools

import os

pub fn find_file(path string, name string) !string {
	files := walk_files(path)
	for file in files {
		if file[file.len-name.len..] == name {
			return file
		}
	}
	return error('not found ${name}')
}

pub fn walk_files(path string) []string {
	entries := os.ls(path) or { []string{} }
	mut files := []string{}
	for entry in entries {
		full_path := os.join_path(path, entry)
		if os.is_dir(full_path) {
			add_files := walk_files(full_path)
			for file in add_files {
				files << file
			}
		} else {
			files << full_path
		}
	}
	return files
}
