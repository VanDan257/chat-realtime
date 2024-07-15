import { AuthAdminService } from './../../services/auth-admin.service';
import { Component, NgZone } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import {ReactiveFormsModule} from "@angular/forms";
import { finalize } from 'rxjs';

@Component({
  selector: 'app-login-admin',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login-admin.component.html',
  styleUrl: './login-admin.component.css'
})
export class LoginAdminComponent {
  formData!: FormGroup;
  messageErrorLogin!: string;
  submittedLogin: boolean = false;

  constructor(
    private authAdmin: AuthAdminService,
    private formBuilder: FormBuilder,
    private spinner: NgxSpinnerService,
    private toastr: ToastrService,
    private ngZone: NgZone,
    private router: Router,
  ){}
  ngOnInit() {
    this.formData = this.formBuilder.group({
      email: ['', Validators.required, Validators.email],
      password: ['', Validators.required],
    });
  }

  onSubmitLogin(){
    this.messageErrorLogin = "";
    this.submittedLogin = true;
    if (this.formData.invalid) {
      return;
    }
    this.authAdmin.loginAdmin(this.formData.getRawValue())
    .pipe(finalize(() => this.spinner.hide()))
    .subscribe({
      next: (response: any) => {
        this.toastr.success('Đăng nhập thành công', 'Thành công!', {
          timeOut: 2000,
        });
        this.navigate('/admin/trang-chu');
      },
      error: (error) => {
        console.log('error login: ', error)
        this.messageErrorLogin = error.error.message;
        this.toastr.error(error.error.message);
      },
    })
  }

  navigate(path: string): void {
    this.ngZone.run(() => this.router.navigateByUrl(path)).then();
  }

}
