import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'pipeHostFile',
})
export class PipeHostFilePipe implements PipeTransform {
  // Nếu ảnh dạng base64 => giữ nguyên base6 hiển thị
  // Ngược lại => Lấy ảnh qua server
  transform(value: any, ...args: any[]): any {
    if(value == null || value === undefined) return;
    if (value?.includes('http')) return value;
    return 'https://localhost:5001/' + value;
  }
}
