import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from "@angular/common/http";
import { Injectable, NgZone } from "@angular/core";
import { Router } from "@angular/router";
import { Observable, throwError } from "rxjs";
import { catchError } from "rxjs/operators";

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private ngZone: NgZone, private router: Router) { }

  intercept(
      request: HttpRequest<any>,
      next: HttpHandler
  ): Observable<HttpEvent<any>> {
      return next.handle(request).pipe(
          catchError((err) => {
              if (err.status === 401) {
                  this.navigate("/dang-nhap");
              } else if (err.status === 406) {
                  this.navigate("/dang-xuat");
              }
              else if (err.status === 302) {
                  this.navigate("/");
              }
              return throwError(() => err);
          })
      );
  }

  public navigate(path: string): void {
      this.ngZone.run(() => this.router.navigateByUrl(path)).then();
  }
}
