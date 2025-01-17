import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailsComponent } from './members/member-details/member-details.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { authGuard } from './_guards/auth.guard';

export const routes: Routes = [
   { path: '', component: HomeComponent },
   // make the paths children of a main root path to make the guards run on all of them, instead of individually specify guards for each path
   {
      path: '',
      runGuardsAndResolvers: 'always',
      canActivate: [authGuard],
      children: [
         { path: 'members', component: MemberListComponent},
         { path: 'members/:id', component: MemberDetailsComponent },
         { path: 'lists', component: ListsComponent },
         { path: 'messages', component: MessagesComponent },
      ]
   },

   { path: '**', component: HomeComponent, pathMatch: 'full' },
];
