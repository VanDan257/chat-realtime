import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../../services/user.service';
import { UserAdmin } from '../../../../models/user-admin';
import { PipeModule } from '../../../../../client/pipe/pipe.module';
import { DatePipe } from '@angular/common';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';

@Component({
  selector: 'app-client-management',
  standalone: true,
  imports: [
    PipeModule,
    DatePipe,
    NzLayoutModule,
    NzButtonModule,
    NzIconModule,
  ],
  templateUrl: './client-management.component.html',
  styleUrl: './client-management.component.css'
})
export class ClientManagementComponent implements OnInit {
  clients: UserAdmin[]=[];
  pageIndex: number = 1;
  pageSize: number = 10;
  totalPage: number[] = [1];
  
  constructor(
    private userService: UserService,

  ) {}
  ngOnInit(): void {
    this.getTopUser(1);
  }

  getTopUser(pageIndex: number){
    this.userService.getAllUsers({pageIndex: pageIndex, pageSize: this.pageSize}).subscribe({
      next: (response: any) => {
        var data = JSON.parse(response['data']);
        this.clients = data.Items;
        console.log(this.clients);
        this.totalPage = [];
        for (let i = 1; i <= data.TotalPage; i++) {
          if (!this.totalPage.includes(i)) {
            this.totalPage.push(i);
          }
        }
      },
      error: (error: any) => {
        console.log(error);
      },
    })
  }
}
