import { Component, Input } from '@angular/core';
import { PipeModule } from '../../../../client/pipe/pipe.module';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [PipeModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
  @Input() admin: any;
}
