import {
  Directive,
  Output,
  EventEmitter,
  HostListener,
  ElementRef,
} from '@angular/core';

@Directive({
  selector: '[appScrollTop]',
})
export class ScrollTopDirective {
  @Output() scrolledToTop = new EventEmitter<void>();

  // @HostListener('scroll', ['$event'])
  // onScroll(event: Event): void {
  //   const element = event.target as HTMLElement;
  //   console.log('element', element);
  //   if (element.scrollTop === 0) {
  //     this.scrolledToTop.emit();
  //   }
  // }

  private scrollListener: any;

  constructor(private el: ElementRef) {
    this.scrollListener = this.onScroll.bind(this);
    document.addEventListener('scroll', this.scrollListener, true);
  }

  onScroll(event: Event): void {
    const element = this.el.nativeElement;
    if (element.scrollTop === 0) {
      this.scrolledToTop.emit();
    }
  }

  ngOnDestroy(): void {
    document.removeEventListener('scroll', this.scrollListener, true);
  }
}
