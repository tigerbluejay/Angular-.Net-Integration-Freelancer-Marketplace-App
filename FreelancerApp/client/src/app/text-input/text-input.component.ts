import { NgIf } from '@angular/common';
import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule],
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})
export class TextInputComponent implements ControlValueAccessor {
  @Input() label: string = '';
  @Input() type: string = 'text';

  value: any;

  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
  }

  get control(): FormControl {
    return this.ngControl.control as FormControl;
  }

  // ===== ControlValueAccessor methods =====
  writeValue(obj: any): void {
    this.value = obj; // only update local value
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

  onInputChange(event: any) {
    this.value = event.target.value;
    this.onChangeFn(this.value);
  }

  onBlur() {
    this.onTouchedFn();
  }

  private onChangeFn: any = () => {};
  private onTouchedFn: any = () => {};
}