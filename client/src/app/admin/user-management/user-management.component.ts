import { Component, inject, OnInit } from '@angular/core';
import { AdminService } from '../../_services/admin.service';
import { User } from '../../_models/user';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from '../../modals/roles-modal/roles-modal.component';

@Component({
   selector: 'app-user-management',
   standalone: true,
   imports: [],
   templateUrl: './user-management.component.html',
   styleUrl: './user-management.component.css'
})
export class UserManagementComponent implements OnInit {
   private _adminService = inject(AdminService)
   private _modalService = inject(BsModalService)
   users: User[] = [];
   bsModalRef: BsModalRef<RolesModalComponent> = new BsModalRef<RolesModalComponent>();

   ngOnInit(): void {
      this.getUsersWithRoles();
   }

   openRolesModal(user: User) {
      const initialState: ModalOptions = {
         class: 'modal-lg',
         initialState: {
            title: 'User roles',
            username: user.username,
            users: this.users,
            selectedRoles: [...user.roles],
            availableRoles: ['Admin', 'Moderator', 'Member'],
            rolesUpdated: false,
         }
      }
      this.bsModalRef = this._modalService.show(RolesModalComponent, initialState);
      this.bsModalRef.onHide?.subscribe({
         next: () => {
            if (this.bsModalRef.content && this.bsModalRef.content.rolesUpdated) {
               const selectedRoles = this.bsModalRef.content.selectedRoles;
               this._adminService.updateUserRoles(user.username, selectedRoles).subscribe({
                  next: roles => user.roles = roles
               })
            }
         }
      })
   }

   getUsersWithRoles() {
      this._adminService.getUserWithRoles().subscribe({
         next: users => this.users = users
      })
   }
}
