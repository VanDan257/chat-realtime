import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'hidePhone',
  standalone: true
})
export class HidePhonePipe implements PipeTransform {

  transform(value: any, ...args: unknown[]): unknown {

    const visiblePart = value.slice(0, length - 6);
    const hiddenPart = '•'.repeat(6);
    return visiblePart + hiddenPart;

  }

}
