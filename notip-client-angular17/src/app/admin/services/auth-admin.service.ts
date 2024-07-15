import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AdminApi } from '../admin-api';
import { Constants } from '../../client/utils/constants';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthAdminService {

  constructor(private http: HttpClient) { }

  loginAdmin(request: any){
    return this.http.post(AdminApi.LoginAdmin, request).pipe(
      map((response: any) => {
          localStorage.setItem(Constants.LOCAL_STORAGE_KEY.SESSION, response["data"]);
          localStorage.setItem(Constants.LOCAL_STORAGE_KEY.TOKEN, JSON.parse(response["data"])["Token"]);

          return response;
      })
    );
  }
  
  get currentAdmin(): any {
    let session = localStorage.getItem(Constants.LOCAL_STORAGE_KEY.SESSION);

    if (session == null || session == undefined) return null;
    return JSON.parse(
      localStorage.getItem(Constants.LOCAL_STORAGE_KEY.SESSION)?.toString() ??
      ''
    );
  }

}
