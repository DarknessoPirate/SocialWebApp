import { Component } from '@angular/core';
import { LoginComponent } from "../login/login.component";
import { RegisterComponent } from "../register/register.component";
import { CommonModule, NgIf } from '@angular/common';
import { animate, style, transition, trigger } from '@angular/animations';

@Component({
  selector: 'app-auth-page',
  standalone: true,
  imports: [LoginComponent, RegisterComponent, CommonModule],
  templateUrl: './auth-page.component.html',
  styleUrl: './auth-page.component.css',
  animations: [
    trigger('loginAnimation', [
       transition(':enter', [
          style({ opacity: 0, height: '0px' }), // Start collapsed
          animate('400ms ease-out', style({ opacity: 1, height: '*' })) // Expand naturally
       ]),
       transition(':leave', [
          animate('300ms ease-in', style({ opacity: 0, height: '0px' })) // Shrink without stretching
       ])
    ])
 ]
})
export class AuthPageComponent {
  showLogin = true;

  toggleForm(){
    this.showLogin = !this.showLogin;

  }

}
