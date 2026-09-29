import { Directive, ElementRef, Input, OnDestroy, OnInit, Output, EventEmitter, forwardRef } from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';

declare const jQuery: any;

// Drop-in replacement for the 'froalaEditor' directive from angular-froala-wysiwyg,
// wired directly to the froala-editor v2 (jQuery) core API loaded globally via angular.json scripts.
// The 'froalaEditor.xxx' event keys used across the app match that wrapper's convention
// of prefixing the underlying editor's native event names (e.g. 'contentChanged', 'initialized').
@Directive({
  selector: '[froalaEditor]',
  standalone: false,
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => FroalaEditorDirective),
    multi: true
  }]
})
export class FroalaEditorDirective implements OnInit, OnDestroy, ControlValueAccessor {

  @Input('froalaEditor') options: any = {};

  @Input() froalaModel: any;
  @Output() froalaModelChange = new EventEmitter<any>();

  private editor: any;
  private pendingValue: any = null;
  private onChange: (value: any) => void = () => { };
  private onTouched: () => void = () => { };

  constructor(private elRef: ElementRef<HTMLElement>) { }

  ngOnInit(): void {
    const opts = { ...(this.options || {}) };
    const userEvents = { ...(opts.events || {}) };
    const isImage = this.elRef.nativeElement.tagName.toLowerCase() === 'img';

    const events: any = {};
    for (const key of Object.keys(userEvents)) {
      const nativeKey = key.startsWith('froalaEditor.') ? key.substring('froalaEditor.'.length) : key;
      events[nativeKey] = userEvents[key];
    }

    const emitChange = () => {
      const value = isImage ? this.editor.opts.imageDefaultDisplay : this.editor.html.get();
      this.onChange(value);
      this.froalaModelChange.emit(value);
    };

    const userInitialized = events['initialized'];
    events['initialized'] = function (...args: any[]) {
      if (typeof userInitialized === 'function') userInitialized.apply(this, args);
    };

    const userContentChanged = events['contentChanged'];
    events['contentChanged'] = (...args: any[]) => {
      if (typeof userContentChanged === 'function') userContentChanged.apply(this.editor, args);
      emitChange();
    };

    opts.events = events;

    const $ = (window as any).jQuery || jQuery;
    $(this.elRef.nativeElement).froalaEditor(opts);
    this.editor = $(this.elRef.nativeElement).data('froala.editor');

    const initialValue = this.pendingValue !== null ? this.pendingValue : this.froalaModel;
    if (initialValue != null) {
      this.applyValue(initialValue);
    }
  }

  ngOnDestroy(): void {
    const $ = (window as any).jQuery || jQuery;
    if (this.editor) {
      $(this.elRef.nativeElement).froalaEditor('destroy');
    }
  }

  writeValue(value: any): void {
    if (!this.editor) {
      this.pendingValue = value;
      return;
    }
    this.applyValue(value);
  }

  registerOnChange(fn: (value: any) => void): void { this.onChange = fn; }
  registerOnTouched(fn: () => void): void { this.onTouched = fn; }
  setDisabledState(isDisabled: boolean): void {
    if (this.editor) {
      isDisabled ? this.editor.edit.off() : this.editor.edit.on();
    }
  }

  private applyValue(value: any): void {
    if (this.elRef.nativeElement.tagName.toLowerCase() === 'img') {
      if (value) this.elRef.nativeElement.setAttribute('src', value);
    } else if (this.editor?.html) {
      this.editor.html.set(value || '');
    }
  }
}
