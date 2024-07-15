import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../../services/user.service';
import { User } from '../../../../../client/models/user';
import { PipeModule } from '../../../../../client/pipe/pipe.module';
import { DatePipe } from '@angular/common';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { NgxSpinnerService } from 'ngx-spinner';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';

@Component({
  selector: 'app-staff-management',
  standalone: true,
  imports: [
    PipeModule,
    DatePipe,
    NzModalModule,
    FormsModule,
    ReactiveFormsModule,
    NzButtonModule,
    NzIconModule,
    NzDatePickerModule
  ],
  templateUrl: './staff-management.component.html',
  styleUrl: './staff-management.component.css'
})
export class StaffManagementComponent implements OnInit {
  staffs: User[] = [];
  pageIndex: number = 1;
  pageSize: number = 10;
  totalPage: number[] = [1];

  isVisible = false;
  isConfirmLoading = false;

  createStaffForm: FormGroup = new FormGroup({});
  maxDate: Date = new Date();
  user = new FormData();
  staffDelete: any | undefined;

  dateFormat = 'dd/MM/yyyy';

  constructor(
    private userSerice: UserService,
    private toastr: ToastrService,
    private fb: FormBuilder,
    private spinner: NgxSpinnerService
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.getStaff(1);
  }

  initializeForm(): void {
    this.createStaffForm = this.fb.group({
      UserName: ['', Validators.required],
      Email: ['', [Validators.required, Validators.email]],
      Gender: [''],
      PhoneNumber: [''],
      Dob: [''],
      Avatar: {},
      Role: [''],
      Address: [''],
      Password: [
        '',
        [
          Validators.required,
        ],
      ],
    });
  }

  getStaff(pageIndex: number){
    this.userSerice.getStaffs({pageIndex: pageIndex}).subscribe((data: any) => {
      var data = JSON.parse(data['data']);
      this.staffs = data.Items;
    });
  }
  
  showModal(): void {
    this.isVisible = true;
  }

  handleOk(): void {
    this.isConfirmLoading = true;
    setTimeout(() => {
      this.isVisible = false;
      this.isConfirmLoading = false;
    }, 1000);
  }

  handleCancel(): void {
    this.isVisible = false;
  }

  uploadFile(event: any) {
    if (event.target.files && event.target.files[0]) {
      let filesToUpload: any[] = [];
      for (let i = 0; i < event.target.files.length; i++) {
        filesToUpload.push(event.target.files[i]);
      }
      this.user.append('Avatar', filesToUpload[0]);
    }
  }
  createStaff(){
    this.spinner.show();
    this.user.append('UserName', this.createStaffForm.value.UserName)
    this.user.append('Gender', this.createStaffForm.value.Gender)
    this.user.append('Role', this.createStaffForm.value.Role)
    this.user.append('Email', this.createStaffForm.value.email)
    this.user.append('PhoneNumber', this.createStaffForm.value.PhoneNumber)
    this.user.append('Address', this.createStaffForm.value.Address)
    this.user.append('Password', this.createStaffForm.value.Password)
    this.user.append('Dob', this.createStaffForm.value.Dob)
    console.log('form: ', this.user);

    this.userSerice.createStaff(this.user).subscribe({
      next: (response: any) =>{
        this.toastr.success('Thêm nhân viên thành công');
        this.getStaff(this.pageIndex);
      },
      error: (error) => {
        this.toastr.error('Thêm nhân viên thất bại');
      }
    })
  }
}
