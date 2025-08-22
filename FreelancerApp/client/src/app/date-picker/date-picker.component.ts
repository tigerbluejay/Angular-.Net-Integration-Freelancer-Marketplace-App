import { NgIf } from '@angular/common';
import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';
import { BsDatepickerModule, BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-date-picker',
  standalone: true,
  imports: [BsDatepickerModule, NgIf, ReactiveFormsModule],
  templateUrl: './date-picker.component.html',
  styleUrls: ['./date-picker.component.css']
})
export class DatePickerComponent implements ControlValueAccessor {
  @Input() label: string = '';
  @Input() maxDate?: Date;

  value: Date | null = null;
  bsConfig?: Partial<BsDatepickerConfig>;

  private onChangeFn: (value: Date | null) => void = () => {};
  private onTouchedFn: () => void = () => {};

  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
    this.bsConfig = {
      containerClass: 'theme-red',
      dateInputFormat: 'DD MMMM YYYY' // ensures "12 March 1981"
    };
  }

  get control(): FormControl {
    return this.ngControl.control as FormControl;
  }

writeValue(value: Date | string | null): void {
  this.value = value ? (value instanceof Date ? value : new Date(value)) : null;
}

  registerOnChange(fn: any): void {
    this.onChangeFn = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouchedFn = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    if (isDisabled) {
      this.control?.disable();
    } else {
      this.control?.enable();
    }
  }

  onDateChange(newDate: Date | null) {
  if (!newDate) return;  // <- ignore initial null/undefined
  this.value = newDate;
  this.onChangeFn(newDate);
}

  onBlur() {
    this.onTouchedFn();
  }
}