import { Component, EventEmitter, Output } from '@angular/core';
import { PipeModule } from '../../../../pipe/pipe.module';
import { FormsModule } from '@angular/forms';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { Group } from '../../../../models/group';
import { ChatBoardService } from '../../../../services/chat-board.service';
import { CommonModule } from '@angular/common';
import {
  NzSkeletonAvatarShape,
  NzSkeletonAvatarSize,
  NzSkeletonButtonShape,
  NzSkeletonButtonSize,
  NzSkeletonInputSize,
  NzSkeletonModule,
} from 'ng-zorro-antd/skeleton';
import { SignalRService } from '../../../../services/signalR.service';
import { group } from '@angular/animations';
import { EncryptionService } from '../../../../services/encryption.service';

@Component({
  selector: 'app-list-messages',
  standalone: true,
  imports: [
    PipeModule,
    FormsModule,
    NzAvatarModule,
    CommonModule,
    NzSkeletonModule,
  ],
  templateUrl: './list-messages.component.html',
  styleUrl: './list-messages.component.css',
})
export class ListMessagesComponent {
  buttonActive = true;
  avatarActive = true;
  inputActive = true;
  imageActive = true;
  buttonSize: NzSkeletonButtonSize = 'default';
  avatarSize: NzSkeletonAvatarSize = 'default';
  inputSize: NzSkeletonInputSize = 'default';
  elementActive = true;
  buttonShape: NzSkeletonButtonShape = 'default';
  avatarShape: NzSkeletonAvatarShape = 'circle';
  elementSize: NzSkeletonInputSize = 'default';

  @Output() onClick = new EventEmitter<Group>();

  groups: Group[] = [];
  groupSelected!: string;
  constructor(
    private chatBoardService: ChatBoardService,
    private signalRService: SignalRService,
    private encryption: EncryptionService
  ) {}

  ngOnInit() {
    this.signalRService.hubConnection.on('ReceiveMessage', (payload) => {
      var message = JSON.parse(payload);
      var decryptMessage = this.encryption.decrypt(message.Content);
      message.Content = decryptMessage;
      let isNewGroup = true;

      for (let i = 0; i < this.groups.length; i++) {
        if (this.groups[i].Code == message.GroupCode) {
          isNewGroup = false;

          this.groups[i].LastMessage = message;
          this.groups[i].LastActive = message.Created;

          // thêm group có tin nhắn mới vào đầu mảng
          this.groups.unshift(this.groups[i]);

          this.groups.splice(i + 1, 1);
          break;
        }
      }
      if (isNewGroup) {
        this.getData();
      }
    });

    this.getData();
  }

  getData() {
    this.chatBoardService.getHistory().subscribe({
      next: (respone: any) => {
        this.groups = JSON.parse(respone['data']);
        this.groups.forEach((group) => {
          if (group.LastMessage != null) {
            group.LastMessage.Content = this.encryption.decrypt(
              group.LastMessage.Content
            );
          }
        });
      },
      error: (error) => console.log('error: ', error),
    });
  }

  accessChatRoom(item: any) {
    if (item.Code != this.groupSelected) {
      this.chatBoardService.accessGroup(item.Code).subscribe({
        next: (response: any) => {
          item = JSON.parse(response['data']);
          this.groupSelected = item.Code;
          this.onClick.emit(item);
        },
        error: (error) => console.log('error: ', error),
      });
    }
  }
}
