import { Injectable } from "@angular/core";
import { HttpEvent, HttpHandler, HttpInterceptor, HttpInterceptorFn, HttpRequest } from "@angular/common/http";
import { Observable } from "rxjs";
import { AuthenticationService } from "../client/services/authentication.service";

// export const JwtInterceptor: HttpInterceptorFn = (req, next) => {
//     return next(req);
// }

@Injectable()

export class JwtInterceptor implements HttpInterceptor {
    constructor(private authService: AuthenticationService) { }

    intercept(
        request: HttpRequest<any>,
        next: HttpHandler
    ): Observable<HttpEvent<any>> {
        let token = this.authService.getToken;
        let header: any = {};

        if (token != null) {
            header["Authorization"] = `Bearer ${token}`;
        }
        request = request.clone({
            setHeaders: header,
        });
        return next.handle(request);
    }
}