import { Component, inject, input, OnInit, output, } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../../_services/account.service';
import { TextInputComponent } from "../../_forms/text-input/text-input.component";
import { DatePickerComponent } from "../../_forms/date-picker/date-picker.component";
import { Router } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CommonModule } from '@angular/common';
import { animate, style, transition, trigger } from '@angular/animations';

@Component({
   selector: 'app-register',
   standalone: true,
   imports: [ReactiveFormsModule, TextInputComponent, DatePickerComponent, CommonModule],
   templateUrl: './register.component.html',
   styleUrl: './register.component.css',
   animations: [
      trigger('slideAnimation', [
         transition(':enter', [
            style({ opacity: 0, transform: '{{enterTransform}}' }),
            animate('400ms cubic-bezier(0.4, 0.0, 0.2, 1)', 
                    style({ opacity: 1, transform: 'translate3d(0, 0, 0)' }))
         ], { params: { enterTransform: 'translate3d(100%, 0, 0)' } }),
   
         transition(':leave', [
            animate('200ms ease-in', style({ opacity: 0 }))
         ])
      ])
   ]
})
export class RegisterComponent implements OnInit {
   private _accountService = inject(AccountService);
   private _formBuilder = inject(FormBuilder);
   private _router = inject(Router);

   cancelRegister = output<boolean>();
   registerForm: FormGroup = new FormGroup({});
   maxDate = new Date();
   validationErrors: string[] = [];
   step = 1; // current page in the form
   animationDirection: 'left' | 'right' = 'right'; // go back/next direction indicator in the form
   get slideAnimationParams() {
      return {
         value: '',
         params: {
            enterTransform: this.animationDirection === 'right' ? 'translate3d(100%, 0, 0)' : 'translate3d(-100%, 0, 0)'
         }
      };
   }
   
   
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

   nextStep() {
      if (this.step < 3) {
         this.animationDirection = 'right';
         this.step++;
      } 
   }

   prevStep() {
      if (this.step > 1){
         this.animationDirection = 'left';
         this.step--;
      } 
   }

   register() {
      const dob = this.getDateOnly(this.registerForm.get('dateOfBirth')?.value);
      this.registerForm.patchValue({ dateOfBirth: dob });
      console.log(this.registerForm.value);
      this._accountService.register(this.registerForm.value).subscribe({
         next: _ => this._router.navigateByUrl('/home'),
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
