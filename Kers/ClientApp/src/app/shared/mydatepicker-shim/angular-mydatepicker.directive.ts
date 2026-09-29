import { Directive, ElementRef, EventEmitter, Input, OnInit, Output, Renderer2, forwardRef } from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { IAngularMyDpOptions, IMyDate, IMyDateModel } from './my-date-model';

function toIsoDate(d: IMyDate | undefined | null): string {
  if (!d) return '';
  const mm = String(d.month).padStart(2, '0');
  const dd = String(d.day).padStart(2, '0');
  return `${d.year}-${mm}-${dd}`;
}

function jsDateToIso(d: Date | null | undefined): string {
  if (!d) return '';
  const mm = String(d.getMonth() + 1).padStart(2, '0');
  const dd = String(d.getDate()).padStart(2, '0');
  return `${d.getFullYear()}-${mm}-${dd}`;
}

function isoToJsDate(iso: string): Date | null {
  if (!iso) return null;
  const [y, m, d] = iso.split('-').map(n => parseInt(n, 10));
  if (!y || !m || !d) return null;
  return new Date(y, m - 1, d);
}

function buildSingleModel(jsDate: Date): IMyDateModel {
  const date: IMyDate = { year: jsDate.getFullYear(), month: jsDate.getMonth() + 1, day: jsDate.getDate() };
  return {
    isRange: false,
    singleDate: { jsDate, date },
    dateRange: null,
    date,
    jsdate: jsDate
  };
}

function buildRangeModel(beginJsDate: Date, endJsDate: Date): IMyDateModel {
  return {
    isRange: true,
    singleDate: null,
    dateRange: {
      beginDate: { year: beginJsDate.getFullYear(), month: beginJsDate.getMonth() + 1, day: beginJsDate.getDate() },
      endDate: { year: endJsDate.getFullYear(), month: endJsDate.getMonth() + 1, day: endJsDate.getDate() },
      beginJsDate,
      endJsDate
    }
  };
}

// Drop-in replacement directive for the retired 'angular-mydatepicker' package,
// backed by native <input type="date"> elements. Preserves the original
// attribute selector, exportAs, [options]/(dateChanged) API and toggleCalendar().
@Directive({
  selector: '[angular-mydatepicker]',
  exportAs: 'angular-mydatepicker',
  standalone: false,
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => AngularMyDatePickerDirective),
    multi: true
  }]
})
export class AngularMyDatePickerDirective implements OnInit, ControlValueAccessor {

  @Input('options') options: IAngularMyDpOptions = {};
  @Output() dateChanged = new EventEmitter<IMyDateModel>();

  private onChange: (value: IMyDateModel) => void = () => { };
  private onTouched: () => void = () => { };

  private isRangeMode = false;
  private rangePanel: HTMLElement | null = null;
  private value: IMyDateModel | null = null;

  constructor(private elRef: ElementRef<HTMLInputElement>, private renderer: Renderer2) { }

  ngOnInit(): void {
    this.isRangeMode = !!(this.options && this.options.dateRange);
    const el = this.elRef.nativeElement;
    if (!this.isRangeMode) {
      el.type = 'date';
      if (this.options?.disableSince) el.max = toIsoDate(this.options.disableSince);
      if (this.options?.disableUntil) el.min = toIsoDate(this.options.disableUntil);
      this.renderer.listen(el, 'change', () => this.onSingleInputChange());
      this.renderer.listen(el, 'blur', () => this.onTouched());
      this.updateSingleDisplay();
    } else {
      el.readOnly = true;
      this.updateRangeDisplay();
    }
  }

  toggleCalendar(): void {
    if (this.isRangeMode) {
      this.rangePanel ? this.closeRangePanel() : this.openRangePanel();
    } else {
      const el = this.elRef.nativeElement as any;
      if (typeof el.showPicker === 'function') {
        el.showPicker();
      } else {
        this.elRef.nativeElement.focus();
      }
    }
  }

  writeValue(value: IMyDateModel): void {
    this.value = value;
    if (this.isRangeMode) {
      this.updateRangeDisplay();
    } else {
      this.updateSingleDisplay();
    }
  }

  registerOnChange(fn: (value: IMyDateModel) => void): void { this.onChange = fn; }
  registerOnTouched(fn: () => void): void { this.onTouched = fn; }
  setDisabledState(isDisabled: boolean): void { this.elRef.nativeElement.disabled = isDisabled; }

  private extractSingleJsDate(value: any): Date | null {
    if (!value) return null;
    if (value.singleDate?.jsDate) return value.singleDate.jsDate;
    if (value.jsdate) return value.jsdate;
    if (value.date) return new Date(value.date.year, value.date.month - 1, value.date.day);
    return null;
  }

  private updateSingleDisplay(): void {
    const jsDate = this.extractSingleJsDate(this.value);
    this.elRef.nativeElement.value = jsDateToIso(jsDate);
  }

  private onSingleInputChange(): void {
    const iso = this.elRef.nativeElement.value;
    const jsDate = isoToJsDate(iso);
    if (!jsDate) return;
    const model = buildSingleModel(jsDate);
    this.value = model;
    this.onChange(model);
    this.dateChanged.emit(model);
  }

  private updateRangeDisplay(): void {
    const range = this.value?.dateRange;
    if (range?.beginJsDate && range?.endJsDate) {
      const fmt = (d: Date) => d.toLocaleDateString();
      this.elRef.nativeElement.value = `${fmt(range.beginJsDate)} - ${fmt(range.endJsDate)}`;
    }
  }

  private openRangePanel(): void {
    const host = this.elRef.nativeElement;
    const parent = host.parentElement;
    if (!parent) return;

    const panel = this.renderer.createElement('div');
    this.renderer.setStyle(panel, 'position', 'absolute');
    this.renderer.setStyle(panel, 'zIndex', '1000');
    this.renderer.setStyle(panel, 'background', '#fff');
    this.renderer.setStyle(panel, 'border', '1px solid #ccc');
    this.renderer.setStyle(panel, 'borderRadius', '4px');
    this.renderer.setStyle(panel, 'padding', '8px');
    this.renderer.setStyle(panel, 'boxShadow', '0 2px 8px rgba(0,0,0,0.15)');

    const range = this.value?.dateRange;

    const beginInput = this.renderer.createElement('input');
    beginInput.type = 'date';
    if (range?.beginJsDate) beginInput.value = jsDateToIso(range.beginJsDate);

    const endInput = this.renderer.createElement('input');
    endInput.type = 'date';
    if (range?.endJsDate) endInput.value = jsDateToIso(range.endJsDate);

    const applyBtn = this.renderer.createElement('button');
    applyBtn.type = 'button';
    this.renderer.setProperty(applyBtn, 'innerText', 'Apply');
    this.renderer.setStyle(applyBtn, 'marginLeft', '6px');
    this.renderer.listen(applyBtn, 'click', () => {
      const beginJsDate = isoToJsDate(beginInput.value);
      const endJsDate = isoToJsDate(endInput.value);
      if (beginJsDate && endJsDate) {
        const model = buildRangeModel(beginJsDate, endJsDate);
        this.value = model;
        this.updateRangeDisplay();
        this.onChange(model);
        this.dateChanged.emit(model);
      }
      this.closeRangePanel();
    });

    this.renderer.appendChild(panel, beginInput);
    this.renderer.appendChild(panel, endInput);
    this.renderer.appendChild(panel, applyBtn);
    this.renderer.appendChild(parent, panel);
    this.rangePanel = panel;
  }

  private closeRangePanel(): void {
    if (this.rangePanel && this.rangePanel.parentElement) {
      this.rangePanel.parentElement.removeChild(this.rangePanel);
    }
    this.rangePanel = null;
  }
}
