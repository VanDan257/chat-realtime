import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'hideAddress',
  standalone: true
})
export class HideAddressPipe implements PipeTransform {

  transform(value: any, ...args: unknown[]): unknown {
    const visiblePart = value.slice(value.length/2, value.length/2);
    const hiddenPart = '•'.repeat(value.length);
    return hiddenPart + visiblePart;
  }

}
