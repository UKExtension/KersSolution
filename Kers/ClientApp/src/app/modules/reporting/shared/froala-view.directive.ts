import { Directive, ElementRef, Input, OnInit, Renderer2 } from '@angular/core';

@Directive({
    selector: '[froalaView]',
    standalone: false
})
export class FroalaViewDirective implements OnInit {
  constructor(private readonly renderer: Renderer2, private readonly element: ElementRef) { }

  @Input()
  set froalaView(content: string) {
    this.element.nativeElement.innerHTML = content;
  }

  ngOnInit(): void {
    this.renderer.addClass(this.element.nativeElement, 'fr-view');
  }
}