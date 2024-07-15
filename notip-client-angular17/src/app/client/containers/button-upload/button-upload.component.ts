import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { DataHelper } from '../../utils/data-helper';
import { PipeModule } from '../../pipe/pipe.module';
@Component({
  selector: 'app-button-upload',
  standalone: true,
  imports: [PipeModule],
  templateUrl: './button-upload.component.html',
  styleUrl: './button-upload.component.css'
})
export class ButtonUploadComponent implements OnInit {
  @Input()
  srcDefault: string =
    'https://pnchatstorage.blob.core.windows.net/blobcontainer/no_image.jpg';

  @Output()
  onload = new EventEmitter<string>();

  @ViewChild('inpFile', { static: true }) inpFileElement!: ElementRef;
  constructor() {}

  ngOnInit() {}

  chooseFile() {
    this.inpFileElement.nativeElement.click();
  }

  onFileChange(evt: any) {
    const target: DataTransfer = <DataTransfer>evt.target;
    if (target.files.length === 0) {
      this.srcDefault = '';
      return;
    }
    const reader: FileReader = new FileReader();
    reader.onload = (e: any) => {
      try {
        var bytes = new Uint8Array(e.target.result);
        this.srcDefault = 'data:image/png;base64,' + DataHelper.toBase64(bytes);
        this.onload.emit(this.srcDefault);
      } catch (error) {
        alert('Lỗi ảnh');
      }
    };
    reader.readAsArrayBuffer(target.files[0]);
  }
}