import { Injectable, NgZone } from "@angular/core";
import { Router, CanActivate } from '@angular/router';
import { AuthenticationService } from "../client/services/authentication.service";
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
    providedIn: 'root'
})

export class AuthGuardService implements CanActivate {
    constructor(
        private ngZone: NgZone,
        private router: Router,
        private authService: AuthenticationService,
        private jwtHelper: JwtHelperService
    ) { }

    canActivate() {
        const token = this.authService.getToken;
        if (token == null && !this.isTokenExpired(token)) {
            this.navigate("/dang-nhap");
            return false;
        }
        return true;
    }

    isTokenExpired(token: string | null): boolean {
        return this.jwtHelper.isTokenExpired(token);
    }

    public navigate(path: string) : void {
        this.ngZone.run(() => this.router.navigateByUrl(path)).then();
    }
}