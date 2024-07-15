import {Component, OnInit, ViewEncapsulation} from '@angular/core';
import { FooterComponent } from '../shared/footer/footer.component';
import { SidebarComponent } from '../shared/sidebar/sidebar.component';
import { NavbarComponent } from '../shared/navbar/navbar.component';
import { AuthAdminService } from '../../services/auth-admin.service';
import { RouterOutlet } from '@angular/router';

// import {UserService} from "../../services/user.service";

@Component({
    selector: 'app-admin',
    templateUrl: './containers.component.html',
    standalone: true,
    imports: [
        FooterComponent,
        SidebarComponent,
        NavbarComponent,
        RouterOutlet
    ],
    encapsulation: ViewEncapsulation.None,
})

export class ContainersComponent implements OnInit{
    currentAdmin: any;
    constructor(private authAdminService: AuthAdminService) {}
  
    ngOnInit() {
      this.currentAdmin = this.authAdminService.currentAdmin;
    }
}