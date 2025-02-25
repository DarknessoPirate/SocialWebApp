import { Component, inject, input, OnInit, output, } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { TextInputComponent } from "../_forms/text-input/text-input.component";
import { DatePickerComponent } from "../_forms/date-picker/date-picker.component";
import { Router } from '@angular/router';

@Component({
   selector: 'app-register',
   standalone: true,
   imports: [ReactiveFormsModule, TextInputComponent, DatePickerComponent],
   templateUrl: './register.component.html',
   styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
   private _accountService = inject(AccountService);
   private _formBuilder = inject(FormBuilder);
   private _router = inject(Router);

   cancelRegister = output<boolean>();
   registerForm: FormGroup = new FormGroup({});
   maxDate = new Date();
   validationErrors: string[] = [];

   ngOnInit(): void {
      this.initializeForm();
      this.maxDate.setFullYear(this.maxDate.getFullYear() - 18)
   }

   initializeForm() {
      this.registerForm = this._formBuilder.group({
         gender: ['male'],
         username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(10)]],
         knownAs: ['', Validators.required],
         dateOfBirth: ['', Validators.required],
         city: ['', Validators.required],
         country: ['', Validators.required],
         password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(10)]],
         confirmPassword: ['', [Validators.required, this.matchValues('password')]],
      })
      // subscribe to password field and update 'confirm password' validity on 'password' field changes (otherwise validity updates only on 'confirm password' field change and only then)
      this.registerForm.controls['password'].valueChanges.subscribe({
         next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
      })
   }

   matchValues(matchTo: string): ValidatorFn {
      return (control: AbstractControl) => {
         return control.value === control.parent?.get(matchTo)?.value ? null : { isMatching: true };
      }
   }


   register() {
      const dob = this.getDateOnly(this.registerForm.get('dateOfBirth')?.value);
      this.registerForm.patchValue({ dateOfBirth: dob });
      console.log(this.registerForm.value);
      this._accountService.register(this.registerForm.value).subscribe({
         next: _ => this._router.navigateByUrl('/members'),
         error: error => this.validationErrors = error
      })
   }

   cancel() {
      this.cancelRegister.emit(false);
   }

   private getDateOnly(date: string | undefined) {
      if (!date)
         return;

      return new Date(date).toISOString().slice(0, 10);
   }
}
