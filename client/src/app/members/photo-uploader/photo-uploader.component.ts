import { Component, ElementRef, EventEmitter, inject, Output, ViewChild } from '@angular/core';
import { NgxUploaderModule, UploadFile, UploadOutput, UploadInput, UploaderOptions } from 'ngx-uploader';
import { environment } from '../../../environments/environment';
import { AccountService } from '../../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
import { MembersService } from '../../_services/members.service';

@Component({
   selector: 'app-photo-uploader',
   standalone: true,
   imports: [NgxUploaderModule, CommonModule],
   templateUrl: './photo-uploader.component.html',
   styleUrl: './photo-uploader.component.css'
})
export class PhotoUploaderComponent {
   private toastr = inject(ToastrService);
   private accountService = inject(AccountService);
   apiUrl = environment.apiUrl + 'users/add-photo';

   @Output() uploadFinished = new EventEmitter<void>();

   // Uploader configuration
   @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;
   uploadInput = new EventEmitter<UploadInput>(); // EventEmitter for upload events
   isDragging = false;
   files: UploadFile[] = [];
   previews: { id: string; url: string }[] = [];
   uploadedCount: number = 0;

   uploadedImageUrl: string | null = null;
   options: UploaderOptions = { concurrency: 1, maxUploads: 3, allowedContentTypes: ['image/jpeg', 'image/png'] };



   // start uploading selected files
   startUpload(): void {
      if (this.files.length === 0) {
         this.toastr.error('Please add files before uploading.', "Upload Error");
         return;
      }

      this.uploadedCount = 0; // reset upload counter to 0

      // fetch saved token
      const token = this.accountService.currentUser()?.token;
      let uploadedCount = 0; // track completed uploads

      // upload only `maxUploads` files
      for (let i = 0; i < Math.min(this.files.length, this.options.maxUploads!); i++) {
         this.uploadInput.emit({
            type: 'uploadFile',
            url: this.apiUrl,
            method: 'POST',
            headers: token ? { Authorization: `Bearer ${token}` } : {},
            file: this.files[i]
         });

         this.toastr.info(`Uploading file: ${this.files[i].name}`, "Upload Status");
      }

   }

   removeFile(id: string): void {
      this.uploadInput.emit({ type: 'remove', id });
      this.files = this.files.filter(file => file.id !== id);
      this.previews = this.previews.filter(preview => preview.id !== id);
   }

   generatePreview(file: UploadFile): void {
      if (file.nativeFile) { // Ensure nativeFile is defined before using it
         const objectUrl = URL.createObjectURL(file.nativeFile);
         this.previews.push({ id: file.id, url: objectUrl });
      }
   }

   // file drop detection handler
   onUploadOutput(output: UploadOutput): void {
      //console.log('Upload Output Event:', output);
      switch (output.type) {
         case 'addedToQueue':
            if (output.file) {
               // Ensure the total count does not exceed `maxUploads`
               if (this.files.length >= this.options.maxUploads!) {
                  this.uploadInput.emit({ type: 'remove', id: output.file.id });
                  this.toastr.warning(`Max images allowed: ${this.options.maxUploads}`);
                  return;
               }

               this.files.push(output.file);
               this.generatePreview(output.file);
               this.toastr.info(`${output.file.name} added for upload`, "Upload Status");
            }
            break;

         case 'uploading':
            if (output.file) {
               const index = this.files.findIndex(file => file.id === output.file!.id);
               if (index !== -1) {
                  this.files[index] = output.file;
               }
            }
            break;

         // file was rejected (queue size or format)
         case 'rejected':
            if (output.file) {
               const isFileTypeInvalid = !this.options.allowedContentTypes!.includes(output.file.type);
               const isQueueFull = this.files.length >= this.options.maxUploads!;

               if (isFileTypeInvalid) {
                  this.toastr.error(`Invalid file type: ${output.file.name}. Only JPEG/PNG allowed.`, "Upload Error");
               } else if (isQueueFull) {
                  this.toastr.warning(`Max images allowed: ${this.options.maxUploads}`);
               } else {
                  this.toastr.error("File was rejected.", "Upload Error");
               }
            }
            break;

         // file got removed from queue
         case 'removed':
            this.toastr.info(`${output.file?.name} removed from list`)
            this.files = this.files.filter(file => file.id !== output.file?.id);
            this.previews = this.previews.filter(preview => preview.id !== output.file?.id);
            break;

         // file is hovering over the drag and drop area
         case 'dragOver':
            this.isDragging = true;
            break;

         // file stopped hovering over drag and drop area
         case 'dragOut':
            this.isDragging = false;
            break;

         // file got dropped into the area
         case 'drop':
            this.isDragging = false;
            break;

         // file finished uploading succesfully
         case 'done':
            this.toastr.success('File uploaded successfully!', 'Upload Status');
            this.uploadedCount++;
            if (this.uploadedCount >= Math.min(this.files.length, this.options.maxUploads!)) {
               //console.log("UPLOAD COMPLETED, REMOVING FILES");
               this.uploadFinished.emit(); // Notify MemberEditComponent to reload photos
               this.uploadInput.emit({ type: 'removeAll' });
               this.files = [];
               this.previews = [];
            }
            break;
            break;
      }
   }

   onDragOver(event: Event): void {
      const dragEvent = event as DragEvent;
      dragEvent.preventDefault(); // prevent browser from somehow opening file
      dragEvent.stopPropagation();
   }

   onDragLeave(event: Event): void {
      const dragEvent = event as DragEvent;
      dragEvent.preventDefault();
      dragEvent.stopPropagation();
   }

   onDrop(event: DragEvent): void {
      event.preventDefault();
      event.stopPropagation();
      this.isDragging = false;

      if (event.dataTransfer?.files.length) {
         //console.log('File dropped:', event.dataTransfer.files[0]);

      }
   }


   onFileSelected(event: Event): void {

   }

   triggerFileInput(): void {
      this.fileInput.nativeElement.click();
   }

}
