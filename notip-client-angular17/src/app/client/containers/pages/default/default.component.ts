import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-default',
  standalone: true,
  imports: [],
  templateUrl: './default.component.html',
  styleUrl: './default.component.css',
})
export class DefaultComponent implements OnInit {
  @Input() title: string = '';

  ngOnInit(): void {}
}
