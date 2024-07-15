import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailInfoChatRoomComponent } from './detail-info-chat-room.component';

describe('DetailInfoChatRoomComponent', () => {
  let component: DetailInfoChatRoomComponent;
  let fixture: ComponentFixture<DetailInfoChatRoomComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetailInfoChatRoomComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DetailInfoChatRoomComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
