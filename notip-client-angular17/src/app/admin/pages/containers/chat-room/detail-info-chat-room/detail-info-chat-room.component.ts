import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ChatboardAdminService } from '../../../../services/chatboard-admin.service';
import { PipeModule } from '../../../../../client/pipe/pipe.module';
import { EncryptionService } from '../../../../../client/services/encryption.service';

@Component({
  selector: 'app-detail-info-chat-room',
  standalone: true,
  imports: [
    PipeModule
  ],
  templateUrl: './detail-info-chat-room.component.html',
  styleUrl: './detail-info-chat-room.component.css'
})
export class DetailInfoChatRoomComponent {
  idChatRoom!: string;
  infoChat!: any;

  constructor(
    private router: Router, 
    private chatService: ChatboardAdminService, 
    private toastr: ToastrService,
    private encryption: EncryptionService
  ) {}

  ngOnInit() {
    const arrayUrl = this.router.url.split('/');
    this.idChatRoom = arrayUrl[arrayUrl.length - 1];
    this.getInfoChatRoom();
  }

  getInfoChatRoom(){
    this.chatService.getDetailChatAdmin(this.idChatRoom).subscribe({
      next: (response: any) => {
        this.infoChat = JSON.parse(response["data"]);
        this.infoChat.Messages.forEach((message: any) => {
          message.Content = this.encryption.decrypt(message.Content);
        });
        console.log(this.infoChat);
      },
      error: err => this.toastr.error('', err, {
        timeOut: 2000
      })
    })
  }

}
