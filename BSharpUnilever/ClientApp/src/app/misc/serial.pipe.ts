import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'bSerial'
})
export class SerialPipe implements PipeTransform {

  private SIZE = 5;

  transform(serial: number, prefix?: string): string {
    if (!serial) {
      return '(New)';
    }

    let s = serial + '';
    while (s.length < this.SIZE) { s = '0' + s; }

    if (!!prefix) {
      s = prefix + s;
    }

    return s;
  }
}
