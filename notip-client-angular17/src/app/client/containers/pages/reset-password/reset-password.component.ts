import { Message } from './../../../models/message';
import { Component, NgZone, OnInit, Renderer2 } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthenticationService } from '../../../services/authentication.service';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { NgxSpinnerModule } from 'ngx-spinner';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule, 
    NgxSpinnerModule, 
    ToastrModule
  ],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent implements OnInit {

  newPassword: string = "";
  invalidPassword: boolean = false;

  constructor(
    private authService: AuthenticationService,
    private toastr: ToastrService,
    private ngZone: NgZone,
    private router: Router,
  ) {}
  ngOnInit(): void {
    

    // this.formData = this.formBuilder.group({
    //   Email: ['', Validators.required],
    //   NewPassword: ['', Validators.required],
    // });
  }

  onSubmitResetPassword(){
    if(this.newPassword == ""){
      this.invalidPassword = true;
      return;
    }
    const currentUrl = this.router.url;
    // Tìm vị trí của "email="
    const emailIndex = currentUrl.indexOf("email=");
    // Tìm vị trí của "token"
    const tokenIndex = currentUrl.indexOf("token=");
    var email = "";
    var token = ""
    if (emailIndex !== -1 && tokenIndex !== -1 && tokenIndex > emailIndex){
      const emailSubstring = currentUrl.substring(emailIndex, tokenIndex);
      email = emailSubstring.replace("email=", "").replace("&", "");
      token = currentUrl.substring(tokenIndex + "token=".length);
    }
      
    this.authService.resetPassword({Email: email, ResetCode: token, NewPassword: this.newPassword}).subscribe({
      next: (response: any) => {
        this.ngZone.run(() => {
          this.toastr.success('Lấy lại mật khẩu thành công');
          this.router.navigate(['/dang-nhap']);
        });
      },
      error: (error: any) => {
        this.toastr.error('Lấy lại mật khẩu không thành công');
      }
    })
  }
}
