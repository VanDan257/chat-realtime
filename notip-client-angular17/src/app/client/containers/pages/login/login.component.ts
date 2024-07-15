import { Component, ElementRef, NgZone, OnInit, Renderer2 } from '@angular/core';
import $ from 'jquery';
import { NgxSpinnerModule, NgxSpinnerService } from "ngx-spinner";
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { AuthenticationService } from '../../../services/authentication.service';
import { finalize } from 'rxjs';
import { Router, RouterLink } from '@angular/router';
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, NgxSpinnerModule, ToastrModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent implements OnInit {
  formData!: FormGroup;
  formDataSignup!: FormGroup;
  submitted: boolean = false;
  submittedSiginup: boolean = false;
  signUp!: HTMLElement;
  signIn!: HTMLElement;
  container!: HTMLElement;

  messageErrorLogin: string = "";
  messageErrorSignup: string = "";

  constructor(
    private authService: AuthenticationService,
    private formBuilder: FormBuilder, // login
    private formBuilder2: FormBuilder, // sign up
    private elment: ElementRef,
    private spinner: NgxSpinnerService,
    private toastr: ToastrService,
    private ngZone: NgZone,
    private router: Router,
    private renderer: Renderer2
  ) {}
  ngOnInit(): void {
    $('.btn-login').on('click', () => {
      $('.container').addClass('sign-up-mode');
    });
    $('.btn-signup').on('click', () => {
      $('.container').removeClass('sign-up-mode');
    });
    this.formData = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    });

    this.formDataSignup = this.formBuilder2.group({
      userName: ['', Validators.required],
      phone: [''],
      email: ['', [Validators.required, , Validators.email]],
      password: ['', Validators.required],
    });
    this.signUp = this.elment.nativeElement.querySelector('#sign-up-btn');
    this.signIn = this.elment.nativeElement.querySelector('#sign-in-btn');
    this.container = this.elment.nativeElement.querySelector('.container');
  }

  onSubmitLogin() {
    this.messageErrorLogin = "";
    this.submitted = true;
    if (this.formData.invalid) {
      return;
    }

    this.spinner.show();
    this.authService
      .login(this.formData.getRawValue())
      .pipe(finalize(() => this.spinner.hide()))
      .subscribe({
        next: (response: any) => {
          this.toastr.success('Đăng nhập thành công', 'Thành công!', {
            timeOut: 2000,
          });
          this.navigate('/');
        },
        error: (error) => {
          console.log('error login: ', error)
          this.messageErrorLogin = error.error.message;
        },
      });
  }

  
  onSubmitSignup() {
    this.messageErrorSignup = "";
    this.submittedSiginup = true;
    if (this.formDataSignup.invalid) {
      console.log(this.formDataSignup.value);
      return;
    }
    this.spinner.show();
    this.authService
      .signUp(this.formDataSignup.getRawValue())
      .pipe(finalize(() => this.spinner.hide()))
      .subscribe({
        next: (response: any) => {
          $('.container').removeClass('sign-up-mode');
          this.toastr.success('Đăng ký tài khoản thành công', 'Thành công', {
            timeOut: 2000,
          });
        },
        error: (error) => {
          console.log('error: ', error);
          this.messageErrorSignup = error.error.message;
        }
      });
  }
  
  navigate(path: string): void {
    this.ngZone.run(() => this.router.navigateByUrl(path)).then();
  }

  callFunSignup() {
    //console.log(this.signUp);
    this.renderer.addClass(this.container, 'sign-up-mode');
  }

  callFunLogin() {
    //console.log(this.signIn);
    this.renderer.removeClass(this.container, 'sign-up-mode');
  }
}
