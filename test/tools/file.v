module tools

import os

pub fn read_file(path string) string {
        mut file := os.open(path) or {
                return ''
        }
        fsize := os.file_size(path).str().int()
        data := file.read_bytes(fsize).bytestr()
        file.close()
        return data
}

pub fn write_file(path string, filename string, buf []u8) {
        os.mkdir(path) or {}
                mut file := os.open_append('${path}/${filename}') or {
                return
        }
        file.write(buf) or { 0 }
                file.close()
}


