import { Message } from './../../models/message';
import { ContactDetailComponent } from './../pages/contacts/contact-detail/contact-detail.component';
import { CommonModule } from '@angular/common';
import { Component, ElementRef, NgZone, OnInit, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ListMessagesComponent } from '../pages/message/list-messages/list-messages.component';
import { DetailMessageComponent } from '../pages/message/detail-message/detail-message.component';
import { DefaultComponent } from '../pages/default/default.component';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { AuthenticationService } from '../../services/authentication.service';
import { CallService } from '../../services/call.service';
import { ChatBoardService } from '../../services/chat-board.service';
import { UserService } from '../../services/user.service';
import { SignalRService } from '../../services/signalR.service';
import { Router } from '@angular/router';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { NzImageModule } from 'ng-zorro-antd/image';
import { User } from '../../models/user';
import { finalize } from 'rxjs';
import { Constants } from '../../utils/constants';
import { ButtonUploadComponent } from '../button-upload/button-upload.component';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { ListMessagesSearchComponent } from '../pages/message/list-messages-search/list-messages-search.component';
import { PipeModule } from '../../pipe/pipe.module';
import { NzSkeletonModule } from 'ng-zorro-antd/skeleton';
import { ListContactComponent } from '../pages/contacts/list-contact/list-contact.component';
import { AuthGuardService } from '../../../auth/auth-guard.service';
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    FormsModule,
    CommonModule,
    ListMessagesComponent,
    ListContactComponent,
    DetailMessageComponent,
    ButtonUploadComponent,
    DefaultComponent,
    NzLayoutModule,
    NzIconModule,
    NzAvatarModule,
    NzInputModule,
    NzModalModule,
    PipeModule,
    NgxSpinnerModule,
    NzDatePickerModule,
    NzSelectModule,
    ListMessagesSearchComponent,
    NzSkeletonModule,
    ContactDetailComponent,
    NzImageModule,
  ],
  providers: [AuthGuardService],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit {
  @ViewChild('listMessage', { static: true })
  listMessage!: ListMessagesComponent;
  @ViewChild('listMessageSearch', { static: true })
  listMessageSearch!: ListMessagesSearchComponent;
  // @ViewChild('listCall', { static: true }) listCall!: ListCallComponent;
  @ViewChild('listContact', { static: true })
  listContact!: ListContactComponent;
  searchUsers = '';
  tabIndexSelected: number = 0;
  tabControls: any[] = [
    {
      index: 0,
      title: 'Tin nhắn',
      iconClass: 'fa fa-solid fa-comments',
    },
    {
      index: 1,
      title: 'Cuộc gọi',
      iconClass: 'fa fa-solid fa-phone',
    },
    {
      index: 2,
      title: 'Danh bạ',
      iconClass: 'fa fa-solid fa-address-book',
    },
    {
      index: 3,
      title: 'Thông báo',
      iconClass: 'fa fa-solid fa-bell',
    },
  ];

  keySearch: string = '';

  filter = {
    keySearch: '',
    groupName: '',
    group: null,
    contact: null,
    groupCall: null,
  };

  @ViewChild('inpFile', { static: true }) inpFileElement!: ElementRef;

  memberSelected: User[] = [];
  userSearch: User[] = [];
  keySearchUser: string = '';
  loadingSearch: boolean = false;

  dateFormat = 'dd/MM/yyyy';

  currentUser: any = {};
  userProfile: User | any = {};

  constructor(
    private authService: AuthenticationService,
    private callService: CallService,
    private chatBoardService: ChatBoardService,
    private userService: UserService,
    private signalRService: SignalRService,
    private ngZone: NgZone,
    private router: Router,
    private spinner: NgxSpinnerService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.currentUser = this.authService.currentUserValue;
    this.signalRService.startConnection(this.currentUser.Id);

    // sự kiện được gọi mỗi khi thoát page hoặc load lại
    window.addEventListener('beforeunload', (event) => {
      this.signalRService.disconnected();
    });

    // lắng nghe cuôc gọi đến => xử lý
    this.signalRService.hubConnection.on('callHubListener', (data) => {
      console.log('callHubListener');
      // this.openModalCall(data);
    });

    // search group chat or user to chat
    // debounce pattern
    let timeoutSearchId: any;
    $('#search-contact').on('input', () => {
      this.tabIndexSelected = 4;
      if (this.keySearch != '') {
        clearTimeout(timeoutSearchId);
        timeoutSearchId = setTimeout(() => {
          this.listMessageSearch.searchGroup(this.keySearch);
        }, 300);
      } else {
        clearTimeout(timeoutSearchId);
      }
    });

    $('#search-contact').on('blur', () => {
      if (this.keySearch == '') {
        this.tabIndexSelected = 0;
      }
    });
  }

  timeoutSearchContactId: any;
  searchUserAddGroup() {
    if (this.keySearchUser != '') {
      clearTimeout(this.timeoutSearchContactId);
      this.timeoutSearchContactId = setTimeout(() => {
        if (this.userSearch.length == 0) {
          this.loadingSearch = true;
        }
    
        this.userService
          .searchContact({ keySearch: this.keySearchUser, pageSize: 8 })
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
                  return !this.memberSelected.some((user) => user.Id === x.Id);
                });
              }
            },
            error: (error: any) => {
              console.log(error);
            },
          });
      }, 300);
    } else {
      clearTimeout(this.timeoutSearchContactId);
    }
    
  }

  clickTab(index: any) {
    this.tabIndexSelected = index;
  }

  onClickMessage(group: any) {
    console.log('group: ', group);
    this.filter.group = group;
  }

  onClickCall(groupCall: any) {
    this.filter.groupCall = groupCall;
  }

  onClickContact(contact: any) {
    this.filter.contact = contact;
  }

  // Create group

  createGroupModal = false;
  isConfirmLoading = false;
  showModalCreateGroup() {
    this.createGroupModal = true;
  }

  addUserToGroup(user: User) {
    if (!this.memberSelected.some((x) => x.Id === user.Id)) {
      this.memberSelected.push(user);
    }
  }

  removeMemberToGroup(member: User) {
    this.memberSelected = this.memberSelected.filter(
      (value) => value.Id != member.Id
    );
  }

  createGroup() {
    this.isConfirmLoading = true;
    if (this.filter.groupName == null || this.filter.groupName.trim() == '') {
      this.toastr.error('Tên nhóm không được để trống', 'Tạo nhóm', {
        timeOut: 2000,
      });
      return;
    }

    if (this.memberSelected.length == 0) {
      this.toastr.error(
        'Danh sách thành viên không được để trống',
        'Thành viên nhóm',
        {
          timeOut: 2000,
        }
      );
      return;
    }

    this.chatBoardService
      .addGroup({
        Name: this.filter.groupName,
        Users: this.memberSelected,
      })
      .pipe(
        finalize(() => {
          this.createGroupModal = false;
          this.isConfirmLoading = false;
        })
      )
      .subscribe({
        next: (response: any) => {
          this.toastr.success('Tạo mới nhóm thành công!', 'Thành công', {
            timeOut: 2000,
          });
          this.listMessage.getData();
        },
        error: (error) =>
          this.toastr.error('Tạo mới nhóm thất bại!', 'Thất bại', {
            timeOut: 2000,
          }),
      });
  }

  //#region thêm mới liên hệ
  contactSearchs: User[] = [];
  openModalAddContact() {
    this.filter.keySearch = '';
    this.contactSearchs = [];
  }

  searchContact() {
    this.userService.searchContact(this.filter.keySearch).subscribe({
      next: (response: any) => {
        this.contactSearchs = JSON.parse(response['data']);
      },
      error: (error) => console.log('error: ', error),
    });
  }

  submitAddContact(contact: any) {
    this.userService.addContact(contact).subscribe({
      next: (response: any) => {
        this.toastr.success('Thêm thành công');
        // this.listContact.getContact();
      },
      error: (error) => console.log('error: ', error),
    });
  }

  //#endregion

  //#region Personal information
  personalInformation = false;

  showModalPersonalInformation() {
    this.spinner.show();
    this.userService
      .getProfile()
      .pipe(
        finalize(() => {
          this.spinner.hide();
        })
      )
      .subscribe({
        next: (response: any) => {
          this.userProfile = JSON.parse(response['data']);
          console.log('userProfile: ', this.userProfile);
          this.personalInformation = true;
        },
        error: (error) => console.log('error: ', error),
      });
  }

  handleCancelPersonal() {
    this.personalInformation = false;
  }

  chooseFile() {
    this.inpFileElement.nativeElement.click();
  }

  onloadAvatar(evt: any) {
    this.spinner.show();
    if (evt.target.files && evt.target.files[0]) {
      let filesToUpload: any[] = [];
      for (let i = 0; i < evt.target.files.length; i++) {
        filesToUpload.push(evt.target.files[i]);
      }
      const file = new FormData();

      file.append('Image', filesToUpload[0]);

      this.userService.updateAvatar(file).pipe(
          finalize(() => {
            this.spinner.hide();
          })
      ).subscribe({
        next: (response: any) => {
          let user: any = JSON.parse(response['data']);
          this.currentUser.Avatar = user.Avatar;
          this.authService.updateCurrentUser(this.currentUser);
          this.toastr.success('Cập nhật ảnh đại diện thành công', 'Thông tin cá nhân', {
            timeOut: 2000,
          });
        },
        error: (error) => {
          this.toastr.error(error.error.Message);
        },
      });
    }
  }

  updateProfile() {
    this.spinner.show();
    this.userService
      .updateProfile(this.userProfile)
      .pipe(
        finalize(() => {
          this.spinner.hide();
        })
      )
      .subscribe({
        next: (response: any) => {
          this.userProfile = JSON.parse(response['data']);
          this.toastr.success('Cập nhật thành công', 'Thông tin cá nhân', {
            timeOut: 2000,
          });
          this.currentUser.Avatar = this.userProfile.Avatar;
          this.currentUser.UserName = this.userProfile.UserName;
          localStorage.setItem(
            Constants.LOCAL_STORAGE_KEY.SESSION,
            JSON.stringify(this.currentUser)
          );
          this.handleCancelPersonal();
        },
        error: (error) =>
          this.toastr.error('Cập nhật thất bại', 'Thông tin cá nhân', {
            timeOut: 2000,
          }),
      });
  }
  //#endregion

  signOut() {
    this.navigate('/dang-nhap');
    localStorage.clear();
  }

  navigate(path: string): void {
    this.ngZone.run(() => this.router.navigateByUrl(path)).then();
  }
}
