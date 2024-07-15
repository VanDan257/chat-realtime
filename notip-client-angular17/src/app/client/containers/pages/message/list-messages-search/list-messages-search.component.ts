import { User } from './../../../../models/user';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { PipeModule } from '../../../../pipe/pipe.module';
import { Group } from '../../../../models/group';
import { ChatBoardService } from '../../../../services/chat-board.service';
import { SignalRService } from '../../../../services/signalR.service';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
import { AuthenticationService } from '../../../../services/authentication.service';
import { NzSkeletonAvatarShape, NzSkeletonAvatarSize, NzSkeletonButtonShape, NzSkeletonButtonSize, NzSkeletonInputSize, NzSkeletonModule } from 'ng-zorro-antd/skeleton';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-list-messages-search',
  standalone: true,
  imports: [PipeModule, CommonModule, NzSkeletonModule],
  templateUrl: './list-messages-search.component.html',
  styleUrl: './list-messages-search.component.css',
})
export class ListMessagesSearchComponent implements OnInit {
  @Output() onClick = new EventEmitter<Group>();
  @Output() keySearch = new EventEmitter<Group>();
  
  currentUser: any = {};
  datas: Group[] = [];

  groupSelected: string = "";

  loadingSearch: boolean = true;

  constructor(
    private chatBoardService: ChatBoardService,
    private toastr: ToastrService,
    private signalRService: SignalRService,
    private authService: AuthenticationService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.currentUserValue;
  }

  accessChatRoom(item: any) {
    this.chatBoardService.accessGroup(item.Code).subscribe({
      next: (response: any) => {
        item = JSON.parse(response['data']);
        this.groupSelected = item.Code;
        this.onClick.emit(item);
      },
      error: (error) => console.log('error: ', error),
    })
  
  }

  searchGroup(search: string) {
    this.loadingSearch = true;
    this.datas = [];
    
    if(this.keySearch!= null){
      this.chatBoardService.searchGroup(search)
      .pipe(finalize(() => this.loadingSearch = false))
      .subscribe({
        next: (response: any) => {
          this.datas = JSON.parse(response['data']);
        },
        error: (error) => console.log('error: ', error),
      })
    }
  }
}
