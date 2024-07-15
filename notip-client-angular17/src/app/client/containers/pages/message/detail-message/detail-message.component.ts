import { User } from './../../../../models/user';
import { Message } from './../../../../models/message';
import {
  AfterContentChecked,
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Input,
  OnInit,
  Output,
  SimpleChanges,
  TemplateRef,
  ViewChild,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { saveAs } from 'file-saver';
import { DefaultComponent } from '../../default/default.component';
import { FormsModule } from '@angular/forms';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';
import { NzPopconfirmModule } from 'ng-zorro-antd/popconfirm';
import { NzModalModule, NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
import { ChatBoardService } from '../../../../services/chat-board.service';
import { FriendService } from '../../../../services/friend.service';
import { AuthenticationService } from '../../../../services/authentication.service';
import { SignalRService } from '../../../../services/signalR.service';
import { PipeModule } from '../../../../pipe/pipe.module';
import { ToastrService } from 'ngx-toastr';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzProgressModule } from 'ng-zorro-antd/progress';
import { NzImageModule } from 'ng-zorro-antd/image';
import { UserService } from '../../../../services/user.service';
import { NzSkeletonModule } from 'ng-zorro-antd/skeleton';
import { finalize } from 'rxjs';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { HidePhonePipe } from '../../../../pipe/hide-phone.pipe';
import { HideAddressPipe } from '../../../../pipe/hide-address.pipe';
import { EncryptionService } from '../../../../services/encryption.service';

@Component({
  selector: 'app-detail-message',
  standalone: true,
  imports: [
    DefaultComponent,
    NzIconModule,
    CommonModule,
    FormsModule,
    NzDrawerModule,
    NzPopconfirmModule,
    NzModalModule,
    PipeModule,
    NzAvatarModule,
    NzInputModule,
    NzProgressModule,
    NzImageModule,
    NzSkeletonModule,
    NgxSpinnerModule,
    HidePhonePipe,
    HideAddressPipe,
  ],
  templateUrl: './detail-message.component.html',
  styleUrl: './detail-message.component.css',
})
export class DetailMessageComponent implements OnInit {
  @Input() group!: any;
  @Output() onClick = new EventEmitter<User>();

  uploadedFile: File | null = null;
  filePreview: string | ArrayBuffer | null | undefined;

  currentUser: any = {};
  contact: User | undefined;
  messages: Message[] = [];
  textMessage: string = '';
  // groupInfo: any = null;

  keySearch: string = '';
  userSearch: User[] = [];
  memberSelected: User[] = [];
  loadingSearch: boolean = false;

  modalEditGroupName: boolean = false;
  isDisabledInputNameGroup: boolean = true;

  isSendingMessage: boolean = false;
  hasBeenSentMessage: boolean = false;
  hasErrorMessage: boolean = false;

  modalContact: boolean = false;

  textTitleInfoChat: string = '';

  pageIndex: number = 1;
  pageSize: number = 10;
  isOutOfMessages: boolean = false;

  constructor(
    private nzMessageService: NzMessageService,
    private toastr: ToastrService,
    private chatBoardService: ChatBoardService,
    private authService: AuthenticationService,
    private signalRService: SignalRService,
    private userService: UserService,
    private spinner: NgxSpinnerService,
    private friendService: FriendService,
    private encryption: EncryptionService
  ) {}

  scroll = (e: any) => {
    if (e.target.scrollTop == 0) {
      if (!this.isOutOfMessages) {
        console.log('message loading...');
        this.pageIndex += 1;
        // this.getMessageByGroup(this.pageIndex, this.pageSize);
      }
    }
  };

  ngOnInit(): void {
    this.currentUser = this.authService.currentUserValue;

    // lắng nghe sự kiện ReceiveMessage
    this.signalRService.hubConnection.on('ReceiveMessage', (payload) => {
      var message = JSON.parse(payload);
      console.log('new message: ', message)
      if (this.group.Code == message.GroupCode) {
        var decryptMessage = this.encryption.decrypt(message.Content);
        message.Content = decryptMessage;
        this.messages.push(message);
      }
    });

    if (this.group?.Type == 'multi') this.textTitleInfoChat = 'Thông tin nhóm';
    else this.textTitleInfoChat = 'Thông tin liên hệ';
    this.scrollToBottom();

    var bodyChat = document.querySelector('.box-chat-body');
    if (bodyChat != null)
      bodyChat.addEventListener('scroll', this.scroll, true);
  }

  ngOnDestroy() {
    var bodyChat = document.querySelector('.box-chat-body');
    if (bodyChat != null) bodyChat.removeEventListener('scroll', this.scroll);
  }

  @ViewChild('scroll') private myScrollContainer!: ElementRef;

  ngAfterContentChecked(): void {
    //Called after every check of the component's or directive's content.
    //Add 'implements AfterContentChecked' to the class.
    this.scrollToBottom();
  }

  // bắt sự kiện scroll window đang không ở vị trí bottom window
  scrollToBottom(): void {
    try {
      this.myScrollContainer.nativeElement.scrollTop =
        this.myScrollContainer.nativeElement.scrollHeight;
    } catch (err) {}
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.group) {
      this.getMessageByGroup(this.pageIndex, this.pageSize).then(() => {
        this.scrollToBottom();
      });
    }
  }

  getChatBoardInfo() {
    if (this.group != null) {
      this.chatBoardService.getChatBoardInfo(this.group?.Code).subscribe({
        next: (response: any) => {
          this.group = JSON.parse(response['data']);
          // this.group.Users = this.groupInfo.Users;
        },
        error: (error) => console.log('error: ', error),
      });
    }
  }

  // #region message handlers

  getMessageByGroup(pageIndex: number, pageSize: number): Promise<void> {
    return new Promise((resolve, reject) => {
      this.chatBoardService
        .getMessageByGroup({
          groupCode: this.group?.Code,
          pageIndex: pageIndex,
          pageSize: pageSize,
        })
        .subscribe({
          next: (response: any) => {
            const data = JSON.parse(response['data']);
            data.Items.forEach((item: any) => {
              item.Content = this.encryption.decrypt(item.Content);
              this.messages.push(item);
            });
            this.isOutOfMessages = data.Items.length < 10; // nếu số lượng tin nhắn nhỏ hơn 10 => đã lấy hết message nhóm chat đó
            this.messages = data.Items;
            resolve(); // Giải quyết Promise khi dữ liệu đã được nhận và xử lý
          },
          error: (error) => {
            console.log('error: ', error);
            reject(error); // Từ chối Promise nếu có lỗi
          },
        });
    });
  }

  onKeydown(event: KeyboardEvent) {
    if (!event.shiftKey && event.code == 'Enter') {
      this.sendMessage();
      event.preventDefault();
    }
  }

  sendMessage() {
    if (this.textMessage == null || this.textMessage.trim() == '') return;

    if (this.messages[this.messages.length - 1]) {
      this.messages[this.messages.length - 1].IsNew = false;
    }

    this.isSendingMessage = true;
    this.hasBeenSentMessage = false;

    // this.chatBoardService
    //   .sendMessageEncrypt({encryptedMessage}).subscribe(response => {
    //     console.log('Message sent successfully');
    //   });

    var message = {
      Id: 0,
      Type: 'text',
      GroupCode: this.group == null ? '' : this.group.Code,
      Content: this.textMessage.trim(),
      Path: '',
      Created: new Date(),
      CreatedBy: this.currentUser.Id,
      UserCreatedBy: this.currentUser,
      SendTo: this.group == null ? '' : this.group.Code,
      IsNew: true,
    };

    this.messages.push(message);

    const formData = new FormData();

    formData.append(
      'data',
      JSON.stringify({
        SendTo: this.group == null ? '' : this.group.Code,
        Content: this.encryption.encrypt(this.textMessage.trim()),
        Type: 'text',
      })
    );

    this.textMessage = '';

    this.chatBoardService
      .sendMessage(this.group == null ? '' : this.group.Code, formData)
      .subscribe({
        next: (response: any) => {
          this.isSendingMessage = false;
          this.hasBeenSentMessage = true;
        },
        error: (error) => {
          this.isSendingMessage = false;
          this.hasErrorMessage = true;
        },
      });
  }

  sendFile(event: any) {
    this.isSendingMessage = true;
    this.hasBeenSentMessage = false;

    if (this.messages[this.messages.length - 1]) {
      this.messages[this.messages.length - 1].IsNew = false;
    }

    if (event.target.files && event.target.files[0]) {
      this.uploadedFile = event.target.files[0];
      const reader = new FileReader();
      reader.onload = (e) => {
        this.filePreview = e.target?.result;
      };
      if (this.uploadedFile != null) {
        reader.readAsDataURL(this.uploadedFile);
      }

      var currentDate = new Date();
      var message = {
        Id: 0,
        Type: 'attachment',
        GroupCode: this.group == null ? '' : this.group.Code,
        Content: event.target.files[0].name,
        Path: `Attachments/${this.group.Code}/${currentDate.getFullYear()}/${
          event.target.files[0].name
        }`,
        Created: new Date(),
        CreatedBy: this.currentUser.Id,
        UserCreatedBy: this.currentUser,
        SendTo: this.group == null ? '' : this.group.Code,
        IsNew: true,
      };

      this.messages.push(message);

      let filesToUpload: any[] = [];
      for (let i = 0; i < event.target.files.length; i++) {
        filesToUpload.push(event.target.files[i]);
      }
      const formData = new FormData();
      Array.from(filesToUpload).map((file, index) => {
        formData.append('files', file, file.name);
      });

      formData.append(
        'data',
        JSON.stringify({
          SendTo: this.contact == null ? '' : this.contact.Id,
          Content: this.encryption.encrypt(event.target.files[0].name),
          Type: 'attachment',
        })
      );

      this.chatBoardService
        .sendMessage(this.group == null ? '' : this.group.Code, formData)
        .subscribe({
          next: (response: any) => {
            this.isSendingMessage = false;
            this.hasBeenSentMessage = true;
          },
          error: (error) => {
            this.isSendingMessage = false;
            this.hasErrorMessage = true;
          },
        });
    } else {
      this.isSendingMessage = false;
      this.hasErrorMessage = true;
    }
  }

  sendImage(event: any) {
    this.hasBeenSentMessage = false;
    this.isSendingMessage = true;

    if (this.messages[this.messages.length - 1]) {
      this.messages[this.messages.length - 1].IsNew = false;
    }

    if (event.target.files && event.target.files[0]) {
      var currentDate = new Date();
      var message = {
        Id: 0,
        Type: 'media',
        GroupCode: this.group == null ? '' : this.group.Code,
        Content: this.textMessage.trim(),
        Path: `Attachments/${this.group.Code}/${currentDate.getFullYear()}/${
          event.target.files[0].name
        }`,
        Created: new Date(),
        CreatedBy: this.currentUser.Id,
        UserCreatedBy: this.currentUser,
        SendTo: this.group == null ? '' : this.group.Code,
        IsNew: true,
      };

      this.messages.push(message);

      this.uploadedFile = event.target.files[0];
      const reader = new FileReader();
      reader.onload = (e) => {
        this.filePreview = e.target?.result;
      };
      if (this.uploadedFile != null) {
        reader.readAsDataURL(this.uploadedFile);
      }

      let filesToUpload: any[] = [];
      for (let i = 0; i < event.target.files.length; i++) {
        filesToUpload.push(event.target.files[i]);
      }
      const formData = new FormData();
      Array.from(filesToUpload).map((file, index) => {
        formData.append('files', file, file.name);
      });

      formData.append(
        'data',
        JSON.stringify({
          SendTo: this.contact == null ? '' : this.contact.Id,
          Content: this.encryption.encrypt(event.target.files[0].name),
          Type: 'media',
        })
      );

      this.chatBoardService
        .sendMessage(this.group == null ? '' : this.group.Code, formData)
        .subscribe({
          next: (response: any) => {
            this.isSendingMessage = false;
            this.hasBeenSentMessage = true;
          },
          error: (error) => {
            this.isSendingMessage = false;
            this.hasErrorMessage = true;
          },
        });
    } else {
      this.isSendingMessage = false;
      this.hasErrorMessage = true;
    }
  }

  downloadFile(path: any, fileName: any) {
    this.chatBoardService.downloadFileAttachment(path).subscribe({
      next: (response) => saveAs(response, fileName),
      error: (error) => console.log('error: ', error),
    });
  }
  // #endregion

  // #region group handlers

  openModalOutGroup() {
    this.chatBoardService.OutGroup(this.group.Code).subscribe({
      next: () => {
        this.toastr.success('Rời nhóm thành công', '', {
          timeOut: 2000,
        });
      },
      error: (error) => {
        this.toastr.error(error.Message, 'Rời nhóm thất bại', {
          timeOut: 2000,
        });
      },
    });
  }

  updateGroupAvatar(evt: any) {
    if (evt.target.files && evt.target.files[0]) {
      let filesToUpload: any[] = [];
      for (let i = 0; i < evt.target.files.length; i++) {
        filesToUpload.push(evt.target.files[i]);
      }
      const formData = new FormData();

      formData.append('Image', filesToUpload[0]);
      formData.append('Code', this.group.Code);

      this.chatBoardService.updateGroupAvatar(formData).subscribe({
        next: (response: any) => {
          const grp = JSON.parse(response['data']);
          this.group.Avatar = grp.Avatar;
          this.toastr.success('Cập nhật ảnh nhóm thành công');
        },
        error: (error) => {
          this.toastr.error(error.error.message);
          console.log(error);
        },
      });
    } else {
      this.toastr.error('Không nhận dạng được ảnh!');
    }
  }

  updateGroupName() {
    this.chatBoardService
      .updateGroupName(this.group.Code, this.group.Name)
      .subscribe({
        next: (response) => {
          // this.group.Name = this.groupInfo.Name;
          this.toastr.success('Cập nhật tên nhóm thành công', '', {
            timeOut: 2000,
          });
          this.modalEditGroupName = false;
        },
        error: (error) => {
          this.toastr.error(error.Message, 'Cập nhật tên nhóm thất bại', {
            timeOut: 2000,
          });
        },
      });
  }

  addUserToGroup(user: User) {
    if (!this.memberSelected.some((x) => x.Id === user.Id)) {
      this.memberSelected.push(user);
    }
  }

  timeoutSearchContactId: any = null;
  searchContact() {
    if (this.keySearch != '') {
      clearTimeout(this.timeoutSearchContactId);
      this.timeoutSearchContactId = setTimeout(() => {
        if (this.userSearch.length == 0) {
          this.loadingSearch = true;
        }
        this.userService
          .searchContact({ keySearch: this.keySearch, pageSize: 8 })
          .pipe(
            finalize(() => {
              this.loadingSearch = false;
            })
          )
          .subscribe({
            next: (response: any) => {
              var data = JSON.parse(response['data']);
              this.userSearch = data.Items;
              if (this.userSearch.length > 0) {
                this.userSearch = this.userSearch.filter((x) => {
                  // Check if x.id is not present in this.userSelctedInNewGroup
                  return (
                    !this.memberSelected.some((user) => user.Id === x.Id) &&
                    !this.group.Users.some((user: User) => user.Id === x.Id)
                  );
                });
              }
            },
            error: (error) => console.log('error: ', error),
          });
      }, 300);
    }
  }

  removeMemberToGroup(member: User) {
    this.memberSelected = this.memberSelected.filter(
      (value) => value.Id != member.Id
    );
  }

  submitAddContact() {
    // console.log(contact.id, this.currentGroup.id)
    this.chatBoardService
      .addMembersToGroup(this.group.Code, this.memberSelected)
      .subscribe({
        next: (response: any) => {
          this.getChatBoardInfo();
          this.toastr.success('Thêm thành công');
          this.modalAddContact = false;
        },
        error: (error) => this.toastr.error('Thêm thất bại'),
      });
  }

  // #endregion

  //#region contact info
  menuFold: boolean = false;

  confirmLeave() {
    this.nzMessageService.info('click confirm');
  }

  cancelLeave() {
    this.nzMessageService.info('click cancel');
  }

  toggleMenuFold() {
    this.menuFold = !this.menuFold;
    if (this.menuFold) this.openContactInfoModal();
    else this.closeContactInfoModal();
  }

  openContactInfoModal(): void {
    this.menuFold = true;
  }

  closeContactInfoModal(): void {
    this.menuFold = false;
  }

  showContact(userCode: string) {
    this.spinner.show();
    // this.contact = contact;
    this.userService
      .getContactByUserCode(userCode)
      .pipe(
        finalize(() => {
          this.spinner.hide();
        })
      )
      .subscribe({
        next: (response: any) => {
          this.contact = JSON.parse(response['data']);
          this.modalContact = true;
          console.log('contact: ', this.contact);
        },
        error: (error) => console.log('error: ', error),
      });
  }

  chat() {
    this.onClick.emit(this.contact);
  }
  // # endregion

  //#region add contact
  modalAddContact: boolean = false;
  isConfirmLoading = false;

  showModalAddContact(): void {
    this.modalAddContact = true;
  }

  handleOk(): void {
    this.isConfirmLoading = true;
    setTimeout(() => {
      this.modalAddContact = false;
      this.isConfirmLoading = false;
    }, 1000);
  }

  // #endregion

  // #region hanlder friend
  sendFriendRequest(code: string | undefined) {
    if (code) {
      this.friendService.sendFriendRequest(code).subscribe({
        next: () => {
          this.toastr.success(
            'Gửi lời mời kết bạn thành công!',
            'Thành công!',
            {
              timeOut: 2000,
            }
          );
        },
        error: () => {
          this.toastr.error(
            'Gửi lời mời kết bạn không thành công!',
            'Có lỗi xảy ra!',
            {
              timeOut: 2000,
            }
          );
        },
      });
    }
  }
}
