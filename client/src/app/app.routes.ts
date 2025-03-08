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
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { preventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { memberDetailedResolver } from './_resolvers/member-detailed.resolver';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { adminGuard } from './_guards/admin.guard';
import { guestGuard } from './_guards/guest.guard';
import { RegisterComponent } from './auth/register/register.component';
import { AuthPageComponent } from './auth/auth-page/auth-page.component';

export const routes: Routes = [
   
   { path: 'auth', component: AuthPageComponent, canActivate: [guestGuard] },
   // make the paths children of a main root path to make the guards run on all of them, instead of individually specify guards for each path
   {
      path: '',
      runGuardsAndResolvers: 'always',
      canActivate: [authGuard],
      children: [
         { path: 'home', component: HomeComponent },
         { path: 'members', component: MemberListComponent },
         { path: 'member/edit', component: MemberEditComponent, canDeactivate: [preventUnsavedChangesGuard] },
         { path: 'members/:username', component: MemberDetailsComponent, resolve: { member: memberDetailedResolver } },
         { path: 'lists', component: ListsComponent },
         { path: 'messages', component: MessagesComponent },
         { path: 'admin', component: AdminPanelComponent, canActivate: [adminGuard] }
      ]
   },
   { path: 'errors', component: TestErrorsComponent },
   { path: 'not-found', component: NotFoundComponent },
   { path: 'server-error', component: ServerErrorComponent },

   // redirect to home
   { path: '', redirectTo: 'home', pathMatch: 'full' },
   { path: '**', redirectTo: 'home', pathMatch: 'full' }
];
