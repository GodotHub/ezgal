module code

import tools

fn set_edit() {
	fd_path := get_code_file('FlowData.cs') or { '' }
	data := tools.read_file(fd_path)
	script_bool := data.contains('List<FileData> flowdata = new List<FileData>();')
	dic_bool := data.contains('List<DicData> dicdata = new List<DicData>();')
	if script_bool && dic_bool {
		return
	}
	panic('[csharp] FlowData.cs: not set edit.')
}





