import { Component } from '@angular/core';
import { Routes } from '@angular/router';
import { HomeComponent } from './client/containers/home/home.component';
import { LoginComponent } from './client/containers/pages/login/login.component';
import { PageNotFoundComponent } from './client/containers/pages/page-not-found/page-not-found.component';
import { AuthGuardService } from './auth/auth-guard.service';
import { AdminComponent } from './admin/admin.component';
import { LoginAdminComponent } from './admin/pages/login-admin/login-admin.component';
import { DashboardComponent } from './admin/pages/containers/dashboard/dashboard.component';
import { ChatRoomManagementComponent } from './admin/pages/containers/chat-room/chat-room-management/chat-room-management.component';
import { ClientManagementComponent } from './admin/pages/containers/client/client-management/client-management.component';
import { DetailInfoClientComponent } from './admin/pages/containers/client/detail-info-client/detail-info-client.component';
import { DetailInfoChatRoomComponent } from './admin/pages/containers/chat-room/detail-info-chat-room/detail-info-chat-room.component';
import { StaffManagementComponent } from './admin/pages/containers/staff/staff-management/staff-management.component';
import { DetailInfoStaffComponent } from './admin/pages/containers/staff/detail-info-staff/detail-info-staff.component';
import { ContainersComponent } from './admin/pages/containers/containers.component';
import { ResetPasswordComponent } from './client/containers/pages/reset-password/reset-password.component';
import { ForGotPasswordComponent } from './client/containers/pages/for-got-password/for-got-password.component';

export const routes: Routes = [
  {
    path: '',
    title: 'NotipChat',
    component: HomeComponent,
    canActivate: [AuthGuardService],
  },
  {
    title: 'Login to NotipChat',
    path: 'dang-nhap',
    component: LoginComponent,
  },
  {
    title: 'Reset password',
    path: 'lay-lai-mat-khau',
    component: ResetPasswordComponent,
  },
  {
    title: 'Forgot password',
    path: 'quen-mat-khau',
    component: ForGotPasswordComponent,
  },
  {
    path: 'admin',
    title: 'NotipChat Admin',
    component: AdminComponent,
    canActivate: [AuthGuardService],
    children: [
      {
        title: 'Login to NotipChat Admin',
        path: 'dang-nhap',
        component: LoginAdminComponent
      },
      {
        path: '',
        component: ContainersComponent,
        children: [
          {
            path: 'trang-chu',
            title: 'Dashboard NotipChat Admin',
            component: DashboardComponent
          },
          {
            path: 'khach-hang/quan-ly-khach-hang',
            title: 'Client Management',
            component: ClientManagementComponent
          },
          {
            path: 'khach-hang/chi-tiet-khach-hang/:id',
            title: 'Client Detail',
            component: DetailInfoClientComponent
          },
          {
            path: 'phong-chat/quan-ly-phong-chat',
            title: 'ChatRoom Management',
            component: ChatRoomManagementComponent
          },
          {
            path: 'phong-chat/chi-tiet-phong-chat/:id',
            title: 'ChatRoom Detail',
            component: DetailInfoChatRoomComponent
          },
          {
            path: 'nhan-vien/quan-ly-nhan-vien',
            title: 'Staff Management',
            component: StaffManagementComponent
          },
          {
            path: 'nhan-vien/chi-tiet-nhan-vien/:id',
            title: 'Staff Detail',
            component: DetailInfoStaffComponent
          }
        ]
      }
      
    ]
  },
  {
    path: "**",
    component: PageNotFoundComponent
  },
];
