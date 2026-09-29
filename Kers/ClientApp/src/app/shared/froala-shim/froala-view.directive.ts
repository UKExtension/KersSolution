import { Directive, ElementRef, Input, OnChanges, Renderer2 } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

// Drop-in replacement for the 'froalaView' directive from angular-froala-wysiwyg.
// Renders editor-produced HTML read-only, styled via froala's own '.fr-view' CSS class.
@Directive({
  selector: '[froalaView]',
  standalone: false
})
export class FroalaViewDirective implements OnChanges {

  @Input('froalaView') content: string = '';

  constructor(private elRef: ElementRef<HTMLElement>, private renderer: Renderer2, private sanitizer: DomSanitizer) {
    this.renderer.addClass(this.elRef.nativeElement, 'fr-view');
  }

  ngOnChanges(): void {
    const safeHtml = this.sanitizer.sanitize(1 /* SecurityContext.HTML */, this.content) || '';
    this.elRef.nativeElement.innerHTML = safeHtml;
  }
}
