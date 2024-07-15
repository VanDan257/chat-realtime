import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatRoomManagementComponent } from './chat-room-management.component';

describe('ChatRoomManagementComponent', () => {
  let component: ChatRoomManagementComponent;
  let fixture: ComponentFixture<ChatRoomManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChatRoomManagementComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ChatRoomManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
