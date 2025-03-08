import { Component, inject } from '@angular/core';
import { AccountService } from '../../_services/account.service';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TextInputComponent } from '../../_forms/text-input/text-input.component';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, TextInputComponent],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private _accountService = inject(AccountService);
  private _formBuilder = inject(FormBuilder);
  private _router = inject(Router);

  loginForm: FormGroup;

  constructor() {
    this.loginForm = this._formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  login() {
    if (this.loginForm.valid) {
      this._accountService.login(this.loginForm.value).subscribe({
        next: _ => this._router.navigate(['/home']),
        error: error => console.error(error)
      });
    }
  }
}
