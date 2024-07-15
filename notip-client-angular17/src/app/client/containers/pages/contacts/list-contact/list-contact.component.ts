import { Component, EventEmitter, Output } from '@angular/core';
import { User } from '../../../../models/user';
import { UserService } from '../../../../services/user.service';
import { AuthenticationService } from '../../../../services/authentication.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-list-contact',
  standalone: true,
  imports: [CommonModule ],
  templateUrl: './list-contact.component.html',
  styleUrl: './list-contact.component.css'
})
export class ListContactComponent {
  @Output() onClick = new EventEmitter<number>();
  
  contacts: User[] = [];
  currentUser: any = {};
  itemIndexSelected: number = -1;

  constructor(
    private userService: UserService,
    private authService: AuthenticationService
  ) {}

  ngOnInit() {
    // this.getContact();
    this.currentUser = this.authService.currentUserValue;
  }

  openContact(index: number) {
    this.itemIndexSelected = index;
    this.onClick.emit(this.itemIndexSelected);
  }

  removeItem(obj: any) {
    this.contacts = this.contacts.filter((c) => c.UserName !== obj);
  }

  uniqByFilter() {
    this.contacts = this.contacts.filter(
      (value, index, array) =>
        index == array.findIndex((item) => item.UserName == value.UserName)
    );
  }
}
