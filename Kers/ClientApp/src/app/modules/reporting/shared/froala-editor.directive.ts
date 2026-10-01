import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Directive, ElementRef, EventEmitter, forwardRef, Input, NgZone, OnDestroy, OnInit, Output } from '@angular/core';

declare const $: any;

@Directive({
    selector: '[froalaEditor]',
    exportAs: 'froalaEditor',
    providers: [{
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => FroalaEditorDirective),
            multi: true
        }],
    standalone: false
})
export class FroalaEditorDirective implements ControlValueAccessor, OnInit, OnDestroy {
  private readonly element: any;
  private readonly specialTags = ['img', 'button', 'input', 'a'];
  private readonly listeningEvents: string[] = [];
  private readonly hasSpecialTag: boolean;
  private options: any = {
    immediateAngularModelUpdate: false,
    angularIgnoreAttrs: null
  };
  private editor: any;
  private editorInitialized = false;
  private model: any;
  private oldModel: any = null;

  onChange: (value: any) => void = () => { };
  onTouched: () => void = () => { };

  @Output() froalaModelChange = new EventEmitter<any>();
  @Output() froalaInit = new EventEmitter<any>();

  constructor(element: ElementRef, private readonly zone: NgZone) {
    this.element = $(element.nativeElement);
    this.hasSpecialTag = this.specialTags.includes(element.nativeElement.tagName.toLowerCase());
  }

  @Input()
  set froalaEditor(options: any) {
    this.options = options || this.options;
  }

  @Input()
  set froalaModel(content: any) {
    this.updateEditor(content);
  }

  writeValue(content: any): void {
    this.updateEditor(content);
  }

  registerOnChange(fn: (value: any) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  ngOnInit(): void {
    if (this.froalaInit.observers.length === 0) {
      this.createEditor();
      return;
    }

    this.froalaInit.emit({
      initialize: this.createEditor.bind(this),
      destroy: this.destroyEditor.bind(this),
      getEditor: this.getEditor.bind(this)
    });
  }

  ngOnDestroy(): void {
    this.destroyEditor();
  }

  private updateEditor(content: any): void {
    if (JSON.stringify(this.oldModel) === JSON.stringify(content)) {
      return;
    }

    if (this.hasSpecialTag) {
      this.model = content;
    } else {
      this.oldModel = content;
    }

    if (this.editorInitialized) {
      if (this.hasSpecialTag) {
        this.setContent();
      } else {
        this.element.froalaEditor('html.set', content);
      }
    } else if (this.hasSpecialTag) {
      this.setContent();
    } else {
      this.element.html(content);
    }
  }

  private updateModel(): void {
    this.zone.run(() => {
      let content: any = null;

      if (this.hasSpecialTag) {
        const attributes = this.element[0].attributes;
        const model: any = {};
        for (let index = 0; index < attributes.length; index++) {
          const name = attributes[index].name;
          if (!this.options.angularIgnoreAttrs || !this.options.angularIgnoreAttrs.includes(name)) {
            model[name] = attributes[index].value;
          }
        }
        if (this.element[0].innerHTML) {
          model.innerHTML = this.element[0].innerHTML;
        }
        content = model;
      } else {
        const html = this.element.froalaEditor('html.get');
        if (typeof html === 'string') {
          content = html;
        }
      }

      if (this.oldModel !== content) {
        this.oldModel = content;
        this.froalaModelChange.emit(content);
        this.onChange(content);
      }
    });
  }

  private registerEvent(eventName: string, callback: (...args: any[]) => void): void {
    if (!eventName || !callback) {
      return;
    }
    this.listeningEvents.push(eventName);
    this.element.on(eventName, callback);
  }

  private registerFroalaEvents(): void {
    const events = this.options.events;
    if (!events) {
      return;
    }
    for (const eventName in events) {
      if (Object.prototype.hasOwnProperty.call(events, eventName)) {
        this.registerEvent(eventName, events[eventName]);
      }
    }
  }

  private createEditor(): void {
    if (this.editorInitialized) {
      return;
    }

    this.setContent(true);
    this.registerFroalaEvents();
    this.registerEvent('froalaEditor.contentChanged', () => setTimeout(() => this.updateModel(), 0));
    this.registerEvent('froalaEditor.mousedown', () => setTimeout(() => this.onTouched(), 0));
    if (this.options.immediateAngularModelUpdate) {
      this.registerEvent('keyup', () => setTimeout(() => this.updateModel(), 0));
    }

    this.zone.runOutsideAngular(() => {
      this.registerEvent('froalaEditor.initialized', () => {
        this.editorInitialized = true;
      });
      this.editor = this.element.froalaEditor(this.options).data('froala.editor').$el;
    });
  }

  private setContent(firstTime = false): void {
    if (this.model === null || this.model === undefined) {
      return;
    }

    this.oldModel = this.model;
    if (this.hasSpecialTag) {
      for (const name in this.model) {
        if (Object.prototype.hasOwnProperty.call(this.model, name) && name !== 'innerHTML') {
          this.element.attr(name, this.model[name]);
        }
      }
      if (Object.prototype.hasOwnProperty.call(this.model, 'innerHTML')) {
        this.element[0].innerHTML = this.model.innerHTML;
      }
      return;
    }

    if (firstTime) {
      this.registerEvent('froalaEditor.initialized', () => this.setHtml());
    } else {
      this.setHtml();
    }
  }

  private setHtml(): void {
    this.element.froalaEditor('html.set', this.model || '', true);
    this.element.froalaEditor('undo.reset');
    this.element.froalaEditor('undo.saveStep');
  }

  private destroyEditor(): void {
    if (!this.editorInitialized) {
      return;
    }
    this.element.off(this.listeningEvents.join(' '));
    this.editor.off('keyup');
    this.element.froalaEditor('destroy');
    this.listeningEvents.length = 0;
    this.editorInitialized = false;
  }

  private getEditor(): any {
    return this.element.froalaEditor.bind(this.element);
  }
}