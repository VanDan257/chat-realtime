import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ClientApi } from '../client-api';
import { User } from '../models/user';

@Injectable({
    providedIn: 'root'
})

export class UserService {
    constructor(private http: HttpClient) { }

    getProfile() {
        return this.http.get(ClientApi.GetProfile);
    }
    
    updateProfile(user: User) {
        return this.http.put(ClientApi.UpdateProfile, user);
    }
    
    searchContact(request: any) {
        let queryParams = new HttpParams();
        for (let key in request) {
            if (request.hasOwnProperty(key)) {
              queryParams = queryParams.append(key, request[key]);
            }
          }
        return this.http.get(ClientApi.GetContact, {
          params: queryParams
        });
    }
    
    addContact(contact: User) {
        return this.http.post(ClientApi.AddContact, contact);
    }

    getContactByUserCode(userCode: string){
        return this.http.get(ClientApi.GetContact, {
            params: {
                userCode
            }
        });
    }

    updateAvatar(request: any){
        return this.http.post(ClientApi.UpdateAvatar, request);
    }
}