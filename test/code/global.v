module code

import tools

const csharp_path = "../ezgal/csharp/"
const gdscript_path = "../ezgal/gdscript/"

fn get_code_file(name string) !string {
	result := tools.find_file(csharp_path, name) or {
		tools.find_file(gdscript_path, name) or {
			return error('not found ${name}')
		}
	}
	return result
}

