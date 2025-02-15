import { NgIf } from '@angular/common';
import { Component, input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule],
  templateUrl: './text-input.component.html',
  styleUrl: './text-input.component.css'
})
// reusable text input for the purpose of not repeating large amounts of code in register form
export class TextInputComponent implements ControlValueAccessor {
   label = input<string>('');
   type = input<string>('text');

   // @Self decorator to inform angular to create individual instances of NgControl (not reuse the available ones)
   constructor(@Self() public ngControl: NgControl){
      this.ngControl.valueAccessor = this;
   }


   // in these case these methods don't really need any code, they onlyneed to be defined and exist for it to work
   writeValue(obj: any): void {
   }

   registerOnChange(fn: any): void {
   }

   registerOnTouched(fn: any): void {
   }

   get control(): FormControl {
      return this.ngControl.control as FormControl
   }

}
