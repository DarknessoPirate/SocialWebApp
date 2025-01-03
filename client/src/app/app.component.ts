import { NgFor } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavComponent } from "./nav/nav.component";
import { AccountService } from './_services/account.service';
import { HomeComponent } from "./home/home.component";

@Component({
   selector: 'app-root',
   standalone: true,
   imports: [RouterOutlet, NavComponent, HomeComponent],
   templateUrl: './app.component.html',
   styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
   private _accountService = inject(AccountService)
   http = inject(HttpClient);
   title = 'client';
   users: any;

   ngOnInit(): void {
      this.getUsers();
      this.setCurrentUser();
   }

   setCurrentUser(){
      const userString = localStorage.getItem('user');
      if (!userString) return;
      
      const user = JSON.parse(userString)
      this._accountService.currentUser.set(user);
   }

   getUsers(){
      this.http.get('https://localhost:5001/api/users').subscribe({
         next: response => this.users = response,
         error: error => console.log(error),
         complete: () => console.log('request completed')
      })
   }
}
