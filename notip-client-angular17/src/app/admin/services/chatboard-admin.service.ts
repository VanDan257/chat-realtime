import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AdminApi } from '../admin-api';
@Injectable({
  providedIn: 'root'
})
export class ChatboardAdminService {

  constructor(
    private http: HttpClient,
  ) { }

  getAllChatRoom(request: any){
    let queryParams = new HttpParams();
    for (let key in request) {
      if (request.hasOwnProperty(key)) {
        queryParams = queryParams.append(key, request[key]);
      }
    }
    return this.http.get(AdminApi.GetAllChatRoom, { params: queryParams});
  }

  getDetailChatAdmin(groupCode: any){
    return this.http.get(AdminApi.GetDetailChatRoom + "/" + groupCode);
  }

  getTrafficStatistics(request: any){
    let queryParams = new HttpParams();
    for (let key in request) {
      if (request.hasOwnProperty(key)) {
        queryParams = queryParams.append(key, request[key]);
      }
    }
    return this.http.get(AdminApi.GetTrafficStatistics, { params: queryParams});
  }
}
