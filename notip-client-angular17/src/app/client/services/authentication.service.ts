import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Constants } from '../utils/constants';
import { ClientApi } from '../client-api';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  constructor(private http: HttpClient) { }

  get getToken(): string | null {
      return localStorage.getItem(Constants.LOCAL_STORAGE_KEY.TOKEN)?.toString() ?? null;
  }

  get currentUserValue(): any {
      let session = localStorage.getItem(Constants.LOCAL_STORAGE_KEY.SESSION);

      if (session == null || session == undefined)
          return null;
      return JSON.parse(
          localStorage.getItem(Constants.LOCAL_STORAGE_KEY.SESSION)?.toString() ?? ""
      );
  }

  login(params: any) {
      return this.http.post(ClientApi.Login, params).pipe(
          map((response: any) => {
              localStorage.setItem(Constants.LOCAL_STORAGE_KEY.SESSION, response["data"]);
              localStorage.setItem(Constants.LOCAL_STORAGE_KEY.TOKEN, JSON.parse(response["data"])["Token"]);

              return response;
          })
      );
  }

  signUp(params: any) {
      return this.http.post(ClientApi.SignUp, params);
  }
  
  updateCurrentUser(currentUser: any){
    localStorage.setItem(Constants.LOCAL_STORAGE_KEY.SESSION, JSON.stringify(currentUser));
  }

  forgotPassword(email: any){
    return this.http.get(ClientApi.ForgotPassword + '/' + email)
  }

  resetPassword(request: any){
    return this.http.post(ClientApi.ResetPassword, request);
  }

}
