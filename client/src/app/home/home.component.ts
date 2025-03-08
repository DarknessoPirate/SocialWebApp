import { Component, inject, OnInit } from '@angular/core';
import { RegisterComponent } from "../auth/register/register.component";
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent  {
   httpClient = inject(HttpClient);
   registerMode = false;
   users:any;


}
