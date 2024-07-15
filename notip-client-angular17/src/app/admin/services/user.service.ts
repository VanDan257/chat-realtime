import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AdminApi } from '../admin-api';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http: HttpClient) { }

  getAllUsers(request: any){
    let queryParams = new HttpParams();
    for (let key in request) {
      if (request.hasOwnProperty(key)) {
        queryParams = queryParams.append(key, request[key]);
      }
    }
    return this.http.get(AdminApi.GetAllUsers, { params: queryParams});
  }

  getStaffs(request: any){
    let queryParams = new HttpParams();
    for (let key in request) {
      if (request.hasOwnProperty(key)) {
        queryParams = queryParams.append(key, request[key]);
      }
    }
    return this.http.get(AdminApi.GetStaffs, { params: queryParams});
  }

  createStaff(request: any){
    return this.http.post(AdminApi.CreateStaff,request);
  }
}
