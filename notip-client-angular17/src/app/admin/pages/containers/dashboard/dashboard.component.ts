import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ChatboardAdminService } from '../../../services/chatboard-admin.service';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { UserService } from '../../../services/user.service';
import { PipeModule } from '../../../../client/pipe/pipe.module';
import { User } from '../../../../client/models/user';
import { UserAdmin } from '../../../models/user-admin';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    ToastrModule,
    NgxChartsModule,
    NzIconModule,
    PipeModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  chats: any[]=[];
  showButtonBack: boolean = false; 
  clients: UserAdmin[]=[];
  trafficStatistics: any | undefined;

  pageIndexChatRoom: number = 1;
  pageSizeChatRoom: number = 10;
  totalPageChatRoom: number[] = [1];
  pageIndexUsers: number = 1;
  pageSizeUsers: number = 10;
  totalPageUser: number[] = [1];

  currentTime = new Date();

  rateOfNewUser: string | undefined;

  @ViewChild('myChart') myChart!: ElementRef;

  //config chart;
  view: [number, number] = [750, 400];

  // options
  legend: boolean = true;
  showLabels: boolean = true;
  animations: boolean = true;
  xAxis: boolean = true;
  yAxis: boolean = true;
  showYAxisLabel: boolean = true;
  showXAxisLabel: boolean = true;
  xAxisLabel: string = 'Tháng';
  yAxisLabel: string = 'Số lượng';
  timeline: boolean = true;

  // colorScheme = {
  //   domain: ['#5AA454', '#E44D25', '#CFC0BB', '#7aa3e5', '#a8385d', '#aae3f5']
  // };

  statistics: any[] = [ 
    { name: 'Tin nhắn', series: [] }, 
    { name: 'Truy cập người dùng', series: [] }
  ];

  constructor(
    private chatboardService: ChatboardAdminService,
    private userService: UserService,
    private toastr: ToastrService
  ){}

  onSelect(data: any): void {
    const timeClick = data.name.split('-');
    this.getTrafficStatistics({statisticByYear: timeClick[1], statisticByMonth: timeClick[0]});
  }

  ngOnInit(): void {
    this.getTrafficStatistics({statisticByYear: this.currentTime.getFullYear(), statisticByMonth: 0})
    // this.getTopUser();
    this.getAllUser();
  }

  getAllChatRoomAdmin(){
    this.chatboardService.getAllChatRoom({pageIndex: this.pageIndexChatRoom, pageSize: this.pageSizeChatRoom}).subscribe({
      next: (response: any) => {
        var data = JSON.parse(response['data']);
        this.chats = data.Items;
      }
    })
  }

  getTrafficStatistics(request: any){
    if(request.statisticByMonth != 0) this.showButtonBack = true;
    else this.showButtonBack = false;
    this.chatboardService.getTrafficStatistics(request).subscribe({
      next: (response: any) => {
        this.trafficStatistics = JSON.parse(response['data']);
        this.statistics[0].series = [];
        this.statistics[1].series = [];

        for (let i = 0; i < this.trafficStatistics.length; i++) {
          let timeStatictis = "";
          if(this.trafficStatistics[i].Day != null && this.trafficStatistics[i].Day != ""){
            timeStatictis = `${this.trafficStatistics[i].Day}-${this.trafficStatistics[i].Month}`;
          }
          else{
            timeStatictis = `${this.trafficStatistics[i].Month}-${this.trafficStatistics[i].Year}`;
          }
          this.statistics[0].series.push({
            value: this.trafficStatistics[i].MessageCount,
            name: timeStatictis
          })
          this.statistics[1].series.push({
            value: this.trafficStatistics[i].LoginCount,
            name: timeStatictis
          })
        }

        const processedData = this.statistics.map(item => ({
          name: item.name,
          series: item.series.map((seriesItem: any) => ({
              name: seriesItem.name,
              value: seriesItem.value ? seriesItem.value : 0  // Đảm bảo giá trị value là số hợp lệ
          }))
      }));
      
      this.statistics = processedData;
      }
    })
  }

  getAllUser(){
    this.userService.getAllUsers({}).subscribe({
      next: (response: any) => {
        var data = JSON.parse(response['data']);
        var users = data.Items;
        let numberNewUserPeriodMonth = 0;
        let numberNewUserCurrentMonth = 0;

        const currentMonth = this.currentTime.getMonth();
        const currentYear = this.currentTime.getFullYear();
        for(let i = 0; i < users.length; i++){
          if(users[i].User.Created){
            const dateCreated = new Date(users[i].User.Created);
            if(currentYear == dateCreated.getFullYear()){
              if(currentMonth == dateCreated.getMonth() + 1){
                numberNewUserPeriodMonth++;
              }
              else if(currentMonth == dateCreated.getMonth()){
                numberNewUserCurrentMonth++;
              }
            }
          }
        }

        this.clients = users.slice(0, 10)

        if(numberNewUserPeriodMonth != 0){
          this.rateOfNewUser = (((numberNewUserCurrentMonth-numberNewUserPeriodMonth)/numberNewUserPeriodMonth) * 100).toFixed(2);;
        }
      }
    })
  }
}
