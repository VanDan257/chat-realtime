import { Component, OnInit } from '@angular/core';
import { ChatboardAdminService } from '../../../../services/chatboard-admin.service';
import { ToastrService } from 'ngx-toastr';
import { NgxSpinnerService } from 'ngx-spinner';
import { RouterLink } from '@angular/router';
import { PipeModule } from '../../../../../client/pipe/pipe.module';
import { GroupAdmin } from '../../../../models/group-admin';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-chat-room-management',
  standalone: true,
  imports: [
    RouterLink,
    PipeModule,
    DatePipe
  ],
  templateUrl: './chat-room-management.component.html',
  styleUrl: './chat-room-management.component.css'
})
export class ChatRoomManagementComponent implements OnInit {
  chatRooms: GroupAdmin[] = [];
  pageSize: number = 0;
  pageIndex: number = 1;
  totalPage: number[] = [];

  constructor(
    private chatService: ChatboardAdminService,
    private spinner: NgxSpinnerService,
    private toastr: ToastrService,
  ) { }
  ngOnInit(): void {
    this.getAllChatRoom(1, 10);
  }

  getAllChatRoom(pageIndex: number, pageSize: number){
    this.chatService.getAllChatRoom({ pageIndex: pageIndex, pageSize: pageSize}).subscribe({
      next: (response: any) => {
        var data = JSON.parse(response['data']);
        this.chatRooms = data.Items;
      },
      error: (error) => {
        this.toastr.error('Có lỗi xảy ra!');
      },
    })
  }
}
