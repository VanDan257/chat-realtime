import { Message } from './../../../models/message';
import { Component } from '@angular/core';
import { FormGroup, FormsModule } from '@angular/forms';
import { AuthenticationService } from '../../../services/authentication.service';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-for-got-password',
  standalone: true,
  imports: [FormsModule, ToastrModule, RouterLink],
  templateUrl: './for-got-password.component.html',
  styleUrl: './for-got-password.component.css'
})
export class ForGotPasswordComponent {
  email:string = "";
  checkEmail: boolean = true;
  constructor(
    private authService: AuthenticationService,
    private toastr: ToastrService,
  ) {}

  forgotPassword(){
    if(this.email == ""){
      this.checkEmail = false;
      return;
    }
    this.authService.forgotPassword(this.email).subscribe({
      next: (response: any) => {
        this.toastr.success("Hãy kiểm tra email của bạn!", "Đã gửi yêu cầu lấy lại mật khẩu thành công!", {
          timeOut: 2000
        });
      },
      error: (error: any) => {
        this.toastr.error(error.error.Message);
      }
    })
  }
}
