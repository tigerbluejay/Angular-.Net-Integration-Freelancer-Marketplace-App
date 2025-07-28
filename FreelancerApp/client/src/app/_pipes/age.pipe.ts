import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'age',
  standalone: true
})

export class AgePipe implements PipeTransform {
  transform(value: string | Date | undefined): number | null {
    if (!value) return null;

    const birthDate = new Date(value);
    const today = new Date();

    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();
    const dayDiff = today.getDate() - birthDate.getDate();

    if (monthDiff < 0 || (monthDiff === 0 && dayDiff < 0)) {
      age--;
    }

    return age;
  }
}