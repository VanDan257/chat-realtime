import { Pipe, PipeTransform } from '@angular/core';
import moment from 'moment';
import 'moment/locale/vi';
@Pipe({
  name: 'chatDate',
})
export class ChatDatePipe implements PipeTransform {
  // Nếu cùng ngày => hiển thị giờ
  // Khác ngày => Hiển thị ngày giờ
  transform(value: any, ...args: any[]): any {
    const currentDate = new Date();
    const dateCompare = moment(value).toDate();
    const isSameDay = currentDate.toDateString() === dateCompare.toDateString();

    const formatString = isSameDay ? 'hh:mm ' : 'DD/MM hh:mm a';
    return moment(value).locale('vi').format(formatString);
  }
}
