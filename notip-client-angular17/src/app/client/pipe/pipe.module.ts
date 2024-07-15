import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PipeHostFilePipe } from './pipe-host-file.pipe';
import { ChatDatePipe } from './chat-date.pipe';

@NgModule({
  declarations: [PipeHostFilePipe, ChatDatePipe],
  imports: [CommonModule],
  exports: [PipeHostFilePipe, ChatDatePipe],
})
export class PipeModule {}
