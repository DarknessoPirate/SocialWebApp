import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailsComponent } from './members/member-details/member-details.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { authGuard } from './_guards/auth.guard';
import { TestErrorsComponent } from './errors/test-errors/test-errors.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';

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
   {path: 'errors', component: TestErrorsComponent},
   {path: 'not-found', component: NotFoundComponent},
   {path: 'server-error', component: ServerErrorComponent},
   { path: '**', component: HomeComponent, pathMatch: 'full' },
];
