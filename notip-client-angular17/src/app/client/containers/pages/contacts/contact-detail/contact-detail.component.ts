import { FriendService } from './../../../../services/friend.service';
import {
  Component,
  Input,
  OnChanges,
  OnInit,
  SimpleChanges,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DetailMessageComponent } from '../../message/detail-message/detail-message.component';
import { DefaultComponent } from '../../default/default.component';
import { PipeModule } from '../../../../pipe/pipe.module';
import { CommonModule } from '@angular/common';
import { User } from '../../../../models/user';
import { ToastrService } from 'ngx-toastr';
import { AuthenticationService } from '../../../../services/authentication.service';
import { UserService } from '../../../../services/user.service';
import { finalize } from 'rxjs';
import { Friend } from '../../../../models/friend';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NgxSpinnerModule, NgxSpinnerService } from 'ngx-spinner';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzButtonModule } from 'ng-zorro-antd/button';

@Component({
  selector: 'app-contact-detail',
  standalone: true,
  imports: [
    DetailMessageComponent,
    DefaultComponent,
    PipeModule,
    CommonModule,
    FormsModule,
    NzInputModule,
    NgxSpinnerModule,
    NzIconModule,
    NzButtonModule,
  ],
  templateUrl: './contact-detail.component.html',
  styleUrl: './contact-detail.component.css',
})
export class ContactDetailComponent implements OnInit, OnChanges {
  @Input() contact!: any;

  contacts: Friend[] = [];
  keySearchFriend!: string;
  keySearchContact!: string;
  keySearchContactFriend!: string;
  title: string = 'Lời mời kết bạn';
  chatId!: string;
  toggleTabChat: boolean = false;
  currentUser!: User;

  pageIndex: number = 1;
  pageSize: number = 8;
  totalPage: number[] = [1];

  constructor(
    private toastr: ToastrService,
    private authService: AuthenticationService,
    private friendService: FriendService,
    private userService: UserService,
    private spinner: NgxSpinnerService // private friendService: FriendService
  ) {}

  ngOnInit() {
    if (this.contact === null || this.contact === undefined) {
      this.contact = 1;
    }
    this.getListFriendInvite(1);
    this.currentUser = this.authService.currentUserValue;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.contact == 1) {
      this.getListFriendInvite(1);
      this.title = 'Lời mời kết bạn';
    } else if (this.contact == 2) {
      this.getListFriends(1);
      this.title = 'Danh sách bạn bè';
    } else if (this.contact == 3) {
      // this.getListFriends();
      this.title = 'Liên hệ đã chặn';
    }
  }

  getListFriends(pageIndex: number){
    this.friendService
      .getListFriend({ PageIndex: pageIndex, PageSize: 8 })
      .subscribe({
        next: (response: any) => {
          this.totalPage.splice(1, this.totalPage.length-1);
          var data = JSON.parse(response['data']);
          console.log(data);
          this.contacts = data.Items;
        },
      });
  }

  getListFriendInvite(pageIndex: number) {
    this.friendService
      .getListFriendInvite({ PageIndex: pageIndex, PageSize: 8 })
      .subscribe({
        next: (response: any) => {
          this.totalPage.splice(1, this.totalPage.length-1);

          var data = JSON.parse(response['data']);
          console.log(data);
          this.contacts = data.Items;
        },
      });
  }

  timeoutSearchContactFriendId: any;
  searchFriendContact(pageIndex: number){

  }

  timeoutSearchContactId: any;
  searchContact(pageIndex: number) {
    if (this.keySearchContact != '' && pageIndex != this.pageIndex) {
      if (pageIndex == 0) pageIndex = 1;
      clearTimeout(this.timeoutSearchContactId);
      this.timeoutSearchContactId = setTimeout(() => {
        this.userService
          .searchContact({
            keySearch: this.keySearchContact,
            pageSize: 8,
            pageIndex: pageIndex,
          })
          .pipe(
            finalize(() => {
              // this.spinner.hide();
            })
          )
          .subscribe({
            next: (response: any) => {
              this.totalPage.splice(1, this.totalPage.length-1);
              
              var data = JSON.parse(response['data']);
              this.contacts = data.Items;
              this.pageIndex = data.PageIndex;
              this.pageSize = data.PageSize;
              for (let i = 2; i <= data.TotalPage; i++) {
                if (!this.totalPage.includes(i)) {
                  this.totalPage.push(i);
                }
              }
            },
            error: (error: any) => {
              console.log(error);
            },
          });
      }, 300);
    } else {
      clearTimeout(this.timeoutSearchContactId); // Xóa timeout nếu không có giá trị hợp lệ
    }
  }

  acceptInviteFriend(userCode: string) {
    var user = this.contacts.find((x) => x.Id == userCode);
    if (user != null) {
      
      this.friendService.acceptFriendRequest(userCode).subscribe({
        next: () => {
          if(user){
            user.IsFriend = true;
            user.IsBeenSentFriend = false;
          }
          this.toastr.success('Chấp nhận kết bạn thành công!');
        },
        error: (error: any) => {
          if (user) {
            user.IsBeenSentFriend = true;
            user.IsFriend = false;
          }
          this.toastr.error('Hãy thử lại!', 'Có lỗi xảy ra!', {
            timeOut: 2000,
          });
        },
      });
    }
  }

  sendInviteFriend(userCode: string) {
    var user = this.contacts.find((x) => x.Id == userCode);
    if (user != null) {
      this.friendService.sendFriendRequest(userCode).subscribe({
        next: () => {
          if(user){
            user.IsSentFriend = true;

          }
          this.toastr.success('Gửi lời mời kết bạn thành công!');
        },
        error: (error: any) => {
          if (user) user.IsSentFriend = false;
          this.toastr.error('Hãy gửi lại lời mời!', 'Có lỗi xảy ra!', {
            timeOut: 2000,
          });
        },
      });
    }
  }

  cancelInviteFriend(userCode: string) {
    var user = this.contacts.find((x) => x.Id == userCode);
    if (user != null) {
      this.friendService.cancelFriendRequest(userCode).subscribe({
        next: () => {
          if(user)
            user.IsSentFriend = false;
          this.toastr.success('Hủy gửi lời mời kết bạn thành công!');
        },
        error: (error: any) => {
          if (user) {
            user.IsSentFriend = true;
          }
          this.toastr.error('Hãy thử lại!', 'Có lỗi xảy ra!', {
            timeOut: 2000,
          });
        },
      });
    }
  }

  removeFriend(userCode: string) {
    var user = this.contacts.find((x) => x.Id == userCode);
    if (user != null) {
      user.IsFriend = false;
      this.friendService.cancelFriendRequest(userCode).subscribe({
        next: () => {
          this.toastr.success('Hủy kết bạn thành công!');
        },
        error: (error: any) => {
          if (user) {
            user.IsFriend = true;
          }
          this.toastr.error('Hãy thử lại!', 'Có lỗi xảy ra!', {
            timeOut: 2000,
          });
        },
      });
    }
  }

  unblockFriend(userCode: string) {
    var user = this.contacts.find((x) => x.Id == userCode);
    if (user != null) {
      user.IsBlocked = false;
      this.friendService.cancelFriendRequest(userCode).subscribe({
        next: () => {},
        error: (error: any) => {
          if (user) {
            user.IsBlocked = true;
          }
          this.toastr.error('Hãy thử lại!', 'Có lỗi xảy ra!', {
            timeOut: 2000,
          });
        },
      });
    }
  }
}
